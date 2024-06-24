using Amdaris.Domain.Paging;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;
using NHibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
    public class DocumentsBll : IDocumentsBll
    {
        private readonly ISaiseRepository _saiseRepository;
        private readonly ISessionFactory _sessionFactory;

        public DocumentsBll(ISaiseRepository saiseRepository, ISessionFactory sessionFactory)
        {
            _saiseRepository = saiseRepository;
            _sessionFactory = sessionFactory;
        }

        public async Task<List<ReportParameterDto>> ListTemplateParameters(long templateNameId)
        {
            var template = await _saiseRepository.QueryAsync<Template>(z =>
                    z.FirstOrDefault(x => x.TemplateName.Id == templateNameId));

            if (template == null || template.ReportParameters == null)
            {
                return new List<ReportParameterDto>();
            }

            var dtos = template.ReportParameters.Select(rp => new ReportParameterDto
            {
                ParameterId = rp.Id,
                ParameterName = rp.ParameterName,
                ParameterDescription = rp.ParameterDescription,
                IsLookup = rp.IsLookup,
                ParameterCode = rp.ParameterCode

            }).ToList();

            return dtos;
        }

        public async Task<List<ReportParameterValueDto>> ListDocumentParameters(long documentId)
        {
            var document = await _saiseRepository.QueryAsync<Document>(z =>
                z.FirstOrDefault(x => x.Id == documentId));

            var dtos = document.ReportParameterValues.Select(rp => new ReportParameterValueDto
            {
                ParameterValueId = rp.Id,
                DocumentId = document.Id,
                ReportParameterId = rp.ReportParameter.Id,
                ReportParameterName = rp.ReportParameter.ParameterName,
                ValueContent = rp.ValueContent,
                ParameterCode = rp.ReportParameter.ParameterCode
            }).ToList();

            return dtos;
        }
        private async Task<List<ReportParamValueResultsDto>> ListDocumentParametersByBallotPaperId(long ballotPaperId)
        {
            var document = await _saiseRepository.QueryAsync<Document>(z =>
                z.FirstOrDefault(x => x.BallotPaper.Id == ballotPaperId));

            var dtos = document.ReportParameterValues.Select(rp => new ReportParamValueResultsDto
            {
                ParameterValueId = rp.Id,
                DocumentId = document.Id,
                ReportParameterId = rp.ReportParameter.Id,
                ReportParameterName = rp.ReportParameter.ParameterName,
                ValueContent = rp.ValueContent,
                ElectionCompetitorId = rp.ElectionCompetitor.Id,
                ElectionCompetitorMemberId = rp.ElectionCompetitorMember?.Id,
                ElectionCompetitorName = rp.ElectionCompetitor.NameRo,
                ElectionCompetitorMemberName = rp.ElectionCompetitorMember != null ? string.Format("{0} {1}", rp.ElectionCompetitorMember.NameRo, rp.ElectionCompetitorMember.LastNameRo) : null,
                Order = rp.ElectionCompetitorMember != null ? rp.ElectionCompetitorMember.CompetitorMemberOrder : rp.ElectionCompetitor.BallotOrder,
                BallotCount = rp.BallotCount,
            }).ToList();

            return dtos;
        }
        public async Task<DocumentDto> GetDocumentAsync(long electionId, long pollingStationId, long templateNameId)
        {
            try
            {
                var document = await _saiseRepository.QueryAsync<Document>(z =>
                     z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId
                     && x.PollingStation.Id == pollingStationId
                     && x.Template.TemplateName.Id == templateNameId));
                var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                    z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.PollingStation.Id == pollingStationId));

                if (document == null)
                {
                    var result = new DocumentDto();
                    result.AlreadySent = false;

                    var parametersList = await ListTemplateParameters(templateNameId);
                    var parameterValueList = parametersList.Select(dto => new ReportParameterValueDto
                    {
                        ReportParameterId = dto.ParameterId,
                        ReportParameterName = dto.ParameterName,
                        ParameterCode = dto.ParameterCode,

                    }).ToList();
                    result.ReportParameterValues = parameterValueList;
                    return result;
                }

                var reportParamValues = await _saiseRepository.QueryAsync<ReportParameterValue>(x => x.Where(rp => rp.Document.Id == document.Id).ToList());
                return MapPollingStationDocumentAsync(document, reportParamValues, assignedPollingStation);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DocumentDto> GetCircumscriptionDocumentAsync(long electionId, long circumscriptionId, long templateNameId)
        {
            var document = await GetCECEDocumentAsync(electionId, circumscriptionId, templateNameId);
            var template = await GetTemplate(templateNameId);
            var electionRound = await GetElectionRoundByElection(electionId);
            var assignedCircumscription = await _saiseRepository.QueryAsync<AssignedCircumscription>(z =>
                        z.FirstOrDefault(x => x.Id == circumscriptionId));
            if (document == null)
            {
                var result = new DocumentDto();
                result.TemplateId = template.Id;
                result.AssignedCircumscriptionId = assignedCircumscription.Id;
                result.ElectionRoundId = electionRound.Id;
                result.DocumentName = template.TemplateName + assignedCircumscription.NameRo;
                result.AlreadySent = false;

                var parametersList = await ListTemplateParameters(templateNameId);
                var parameterValueList = parametersList.Select(dto => new ReportParameterValueDto
                {
                    ReportParameterId = dto.ParameterId,
                    ReportParameterName = dto.ParameterName,
                    ParameterCode = dto.ParameterCode,

                }).ToList();
                result.ReportParameterValues = parameterValueList;
                return result;
            }

            var reportParamValues = await _saiseRepository.QueryAsync<ReportParameterValue>(x => x.Where(rp => rp.Document.Id == document.Id).ToList());
            return MapCircumscriptionDocumentAsync(document, reportParamValues, assignedCircumscription);
        }


        public async Task<SaveUpdateResult> SaveUpdateDocument(DocumentDto documentDto, long templateNameId, DocumentStatus bpStatusToBeSet)
        {
            try
            {
                var electionRound = await GetElectionRound(documentDto.ElectionRoundId);
                if (electionRound == null)
                {
                    return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.InvalidStatus };
                }
                var document = await GetOrCreateDocument(documentDto, bpStatusToBeSet);

                var validationResult = ValidateDocument(document, bpStatusToBeSet);
                if (validationResult != BallotPaperValidationStatus.IsValid)
                {
                    return new SaveUpdateResult { Success = false, ValidationStatus = validationResult };
                }

                if (document.IsResultsConfirmed && !SecurityHelper.LoggedUserIsInRole("Administrator"))
                {
                    return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.MultipleConfirmationsProhibited };
                }

                await UpdateDocumentProperties(documentDto, document, bpStatusToBeSet, templateNameId);
                await _saiseRepository.SaveOrUpdateAsync(document);

                await MapAndSaveParameterValues(documentDto, document);
                //Need to divide Parameters Map and saving
                //Add Table header ElectionName, CircumscriptionName, PollingStationAddress+Name, ReferendumQuestion
                var userId = SecurityHelper.GetLoggedUserId();
                var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);

                return new SaveUpdateResult { Success = true, ValidationStatus = BallotPaperValidationStatus.IsValid, UpdatedAt = DateTime.Now, UserName = userProxy.UserName };
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.InvalidStatus };
            }
        }

        private async Task<Document> GetOrCreateDocument(DocumentDto documentDto, DocumentStatus bpStatusToBeSet)
        {
            var document = await _saiseRepository.QueryAsync<Document>(z => z.FirstOrDefault(x => x.Id == documentDto.DocumentId));

            if (document != null)
            {
                return document;
            }

            if (documentDto.AssignedCircumscriptionId == null && documentDto.PollingStationId == null)
            {
                return null;
            }

            return new Document();
        }

        private async Task UpdateDocumentProperties(DocumentDto documentDto, Document document, DocumentStatus bpStatusToBeSet, long templateNameId)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);

            document.AssignedCircumscription = await GetAssignedCircumscription(documentDto.AssignedCircumscriptionId);
            document.PollingStation = await GetPollingStation(documentDto.PollingStationId);
            document.DocumentName = documentDto.DocumentName;
            document.StatusId = (int)bpStatusToBeSet;
            document.EditUser = userProxy;
            document.EditDate = DateTime.Now;
            document.ElectionRound = await GetElectionRound(documentDto.ElectionRoundId);
            document.Template = await GetTemplate(templateNameId);

            if (bpStatusToBeSet == DocumentStatus.WaitingForApproval)
            {
                document.EditDate = DateTime.Now;
            }

            if (bpStatusToBeSet == DocumentStatus.Approved)
            {
                document.IsResultsConfirmed = true;
                document.ConfirmationUserId = userId;
                document.ConfirmationDate = DateTime.Now;
                document.StatusId = (int)bpStatusToBeSet;
            }
        }


        public async Task<SaveUpdateResult> SaveUpdateDocumentElectionResults(BallotPaperDataDto ballotPaperDto, long templateNameId, DocumentStatus bpStatusToBeSet)
        {
            try
            {
                var validationResult = BallotPaperValidationStatus.InvalidStatus;
                var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                            z.FirstOrDefault(x => x.Id == ballotPaperDto.BallotPaperId));

                //Section Saving Document Data
                var documentEntity = await _saiseRepository.QueryAsync<Document>(z =>
                        z.FirstOrDefault(x => x.BallotPaper.Id == ballotPaper.Id));
                var template = await GetTemplate(templateNameId);
                Dictionary<string, long> dictionary = MapBallotPaperDtoToDictionary(ballotPaperDto);

                var userIsAdmin = SecurityHelper.LoggedUserIsInRole("Administrator");

                if (documentEntity != null)
                {
                    if (documentEntity.IsResultsConfirmed && !userIsAdmin)
                        return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.MultipleConfirmationsProhibited };
                }

                var reportParameterValueDtos = new List<ReportParamValueResultsDto>();

                if (documentEntity == null)
                {
                    validationResult = BallotPaperValidationStatus.IsValid;
                    documentEntity = new Document();
                    var parametersList = await ListTemplateParameters(templateNameId);

                    reportParameterValueDtos = MapToReportParameterValueDtoList(parametersList, null);

                    //Assign value to ValueContent for each PropertyName form BallotPaper matching ReportParameterName value from ReportParameterValueDto
                    foreach (var reportParameterValueDto in reportParameterValueDtos)
                    {
                        if (dictionary.TryGetValue(reportParameterValueDto.ReportParameterName, out long value))
                        {
                            reportParameterValueDto.ValueContent = value.ToString();
                        }
                    }
                    var existingDto = reportParameterValueDtos.FirstOrDefault(dto => dto.ReportParameterName == "DynamicParameter");

                    if (existingDto != null)
                    {
                        foreach (var electionResult in ballotPaper.ElectionResults)
                        {
                            // Create a new ReportParamValueResultsDto for each CompetitorResultDto and add it to the list.
                            ReportParamValueResultsDto newDto = new ReportParamValueResultsDto
                            {
                                ReportParameterId = existingDto.ReportParameterId,
                                ReportParameterName = existingDto.ReportParameterName,
                                ElectionCompetitorId = electionResult.PoliticalParty.Id,
                                ElectionCompetitorMemberId = electionResult.Candidate?.Id,
                                ElectionCompetitorName = electionResult.PoliticalParty.NameRo,
                                ElectionCompetitorMemberName = string.Concat((electionResult.Candidate != null && electionResult.Candidate.Id > 0)
                                ? string.Format("{0} {1}", electionResult.Candidate.LastNameRo, electionResult.Candidate.NameRo)
                                : ""),
                                Order = electionResult.Candidate != null ? electionResult.Candidate.CompetitorMemberOrder : electionResult.BallotOrder,
                                ValueContent = electionResult.BallotCount.ToString(),
                                BallotCount = electionResult.BallotCount
                            };

                            // Add newDto to reportParamValueResults list.
                            reportParameterValueDtos.Add(newDto);
                        }

                        // Remove the initial object which has ReportParameterName == "DynamicParameter"
                        reportParameterValueDtos.Remove(existingDto);
                    }
                    MapDocumentElectionResults(documentEntity, ballotPaper, bpStatusToBeSet, template);

                }
                else
                {
                    validationResult = ValidateDocument(documentEntity, bpStatusToBeSet);
                    reportParameterValueDtos = await ListDocumentParametersByBallotPaperId(ballotPaper.Id);

                    MapDocumentElectionResults(documentEntity, ballotPaper, bpStatusToBeSet, template);
                }
                if (validationResult != BallotPaperValidationStatus.IsValid)
                {
                    return new SaveUpdateResult { Success = false, ValidationStatus = validationResult };
                }

                await _saiseRepository.SaveOrUpdateAsync(documentEntity);
                // save documentParameterValues

                var result = await CreateAndSaveReportParameterValues(reportParameterValueDtos, documentEntity);

                return result;
            }
            catch (Exception ex)
            {
                return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.InvalidStatus };
            }
        }

        private async Task<bool> MapAndSaveParameterValues(DocumentDto documentDto, Document document)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);
            var electionRound = await GetElectionRound(documentDto.ElectionRoundId);
            var assignedCircumscription = await GetAssignedCircumscription(documentDto.AssignedCircumscriptionId);
            var pollingStation = await GetPollingStation(documentDto.PollingStationId);

            foreach (var reportParameter in documentDto.ReportParameterValues)
            {
                var reportParameterValue = new ReportParameterValue()
                {
                    Document = document,
                    ReportParameter = await _saiseRepository.QueryAsync<ReportParameter>(z =>
                       z.FirstOrDefault(x => x.Id == reportParameter.ReportParameterId)),
                    ValueContent = reportParameter.ValueContent,
                    EditDate = DateTime.Now,
                    EditUser = userProxy
                };
                //reportParameterValue = await AddDocumentHeaderData(documentDto.PollingStationId, reportParameterValue, electionRound, assignedCircumscription, pollingStation);

                await _saiseRepository.SaveOrUpdateAsync(reportParameterValue);
            }
            return true;
        }

        private async Task<ReportParameterValue> AddDocumentHeaderData(long? pollingstationId, ReportParameterValue reportParameterValue,
            ElectionRound electionRound, AssignedCircumscription assignedCircumscription, PollingStation pollingStation)
        {
            switch (reportParameterValue.ReportParameter.ParameterName.Trim())
            {
                case "ElectionName":
                    HandleElectionName(reportParameterValue, electionRound);
                    break;
                case "ElectionDate":
                    HandleElectionDate(reportParameterValue, electionRound);
                    break;
                case "CircumscriptionName":
                    await HandleCircumscriptionNameAsync(reportParameterValue, pollingstationId, assignedCircumscription);
                    break;
                case "PollingStationAddress":
                    HandlePollingStationAddress(reportParameterValue, pollingStation);
                    break;
                case "ReferendumQuestion":
                    await HandleReferendumQuestionAsync(reportParameterValue);
                    break;
                default:
                    // Handle the default case if needed
                    break;
            }

            return reportParameterValue;
        }

        private void HandleElectionName(ReportParameterValue reportParameterValue, ElectionRound electionRound)
        {
            reportParameterValue.ValueContent = electionRound.Election.GetFullName();
        }

        private void HandleElectionDate(ReportParameterValue reportParameterValue, ElectionRound electionRound)
        {
            reportParameterValue.ValueContent = electionRound.Election.DateOfElection.ToString();
        }

        private async Task HandleCircumscriptionNameAsync(ReportParameterValue reportParameterValue, long? pollingstationId, AssignedCircumscription assignedCircumscription)
        {
            var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.FirstOrDefault(x => x.PollingStation.Id == pollingstationId));
            reportParameterValue.ValueContent = string.IsNullOrEmpty(assignedCircumscription.GetFullName())
                ? assignedPollingStation.AssignedCircumscription.GetFullName()
                : assignedCircumscription.GetFullName();
        }

        private void HandlePollingStationAddress(ReportParameterValue reportParameterValue, PollingStation pollingStation)
        {
            reportParameterValue.ValueContent = pollingStation.GetFullName();
        }

        private async Task HandleReferendumQuestionAsync(ReportParameterValue reportParameterValue)
        {
            var referendumQuestion = await _saiseRepository.QueryAsync<ElectionCompetitor>(z =>
                z.FirstOrDefault());
            reportParameterValue.ValueContent = referendumQuestion.NameRo;
        }

        private DocumentDto MapPollingStationDocumentAsync(Document document, IList<ReportParameterValue> reportParameterValues, AssignedPollingStation assignedPollingStation)
        {
            try
            {
                string pollingStationName;

                if (document.PollingStation == null)
                {
                    pollingStationName = string.Empty;
                }
                else
                {
                    pollingStationName = document.PollingStation.NameRo;
                }
                var result = new DocumentDto
                {
                    DocumentId = document.Id,
                    TemplateId = document.Template.Id,
                    PollingStationId = document.PollingStation?.Id,
                    ElectionRoundId = document.ElectionRound.Id,
                    DocumentName = document.Template.TemplateName + pollingStationName,
                    EditDate = document.EditDate,
                    EditUser = document.EditUser.UserName,
                    IsResultsConfirmed = document.IsResultsConfirmed,
                    ConfirmationUserId = document.ConfirmationUserId,
                    ConfirmationDate = document.ConfirmationDate,
                    Status = document.StatusId,
                    AlreadySent = true,
                    ReportParameterValues = reportParameterValues.Select(rpValue => new ReportParameterValueDto
                    {
                        ParameterValueId = rpValue.Id,
                        DocumentId = document.Id,
                        ReportParameterId = rpValue.ReportParameter.Id,
                        ValueContent = rpValue.ValueContent,
                        ReportParameterName = rpValue.ReportParameter.ParameterName,
                        ParameterCode = rpValue.ReportParameter.ParameterCode
                    }).ToList()
                };
                // TO DO SetPermissionsForCurrentUser(result, assignedPollingStation.ImplementsEVR);
                return result;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private DocumentDto MapCircumscriptionDocumentAsync(Document document, IList<ReportParameterValue> reportParameterValues, AssignedCircumscription assignedCircumscription)
        {
            string circumscriptionName;

            if (document.AssignedCircumscription == null)
            {
                circumscriptionName = string.Empty;
            }
            else
            {
                circumscriptionName = document.AssignedCircumscription.NameRo;
            }
            var result = new DocumentDto
            {
                DocumentId = document.Id,
                TemplateId = document.Template.Id,
                PollingStationId = document.PollingStation?.Id,
                AssignedCircumscriptionId = document.AssignedCircumscription.Id,
                ElectionRoundId = document.ElectionRound.Id,
                DocumentName = document.Template.TemplateName + circumscriptionName,
                EditDate = document.EditDate,
                EditUser = document.EditUser.UserName,
                IsResultsConfirmed = document.IsResultsConfirmed,
                ConfirmationUserId = document.ConfirmationUserId,
                ConfirmationDate = document.ConfirmationDate,
                Status = document.StatusId,
                AlreadySent = true,
                ReportParameterValues = reportParameterValues.Select(rpValue => new ReportParameterValueDto
                {
                    ParameterValueId = rpValue.Id,
                    DocumentId = document.Id,
                    ReportParameterId = rpValue.ReportParameter.Id,
                    ValueContent = rpValue.ValueContent,
                    ReportParameterName = rpValue.ReportParameter.ParameterName,
                    ParameterCode = rpValue.ReportParameter.ParameterCode
                }).ToList()
            };
            // TO DO SetPermissionsForCurrentUser(result, assignedPollingStation.ImplementsEVR);
            return result;
        }
        private BallotPaperValidationStatus ValidateDocument(Document document, DocumentStatus documentStatusToBeSet)
        {

            if (documentStatusToBeSet == DocumentStatus.WaitingForApproval && document.StatusId != (int)DocumentStatus.New)
            {
                return BallotPaperValidationStatus.InvalidStatus;
            }
            if (documentStatusToBeSet == DocumentStatus.Approved && document.StatusId.Equals(0))
            {
                return BallotPaperValidationStatus.IsValid;
            }

            if (documentStatusToBeSet == DocumentStatus.Approved &&
                document.StatusId != (int)DocumentStatus.WaitingForApproval)
            {
                return BallotPaperValidationStatus.InvalidStatus;
            }


            return BallotPaperValidationStatus.IsValid;
        }
        private BallotPaperValidationStatus ValidateConsolidationDocument(Document document)
        {

            if (document.StatusId == (int)DocumentStatus.Approved)
            {
                return BallotPaperValidationStatus.InvalidStatus;
            }
            return BallotPaperValidationStatus.IsValid;
        }

        static Dictionary<string, long> MapBallotPaperDtoToDictionary(BallotPaperDataDto ballotPaperDataDto)
        {
            Dictionary<string, long> result = new Dictionary<string, long>();
            Type type = typeof(BallotPaperDataDto);

            foreach (PropertyInfo property in type.GetProperties())
            {
                // Exclude properties that are not to be included
                if (property.Name == "BallotPaperId" || property.Name == "CompetitorResults" || property.Name == "OpeningVotersCount")
                    continue;

                // Map the property name and value to the dictionary
                result[property.Name] = (long)(property.GetValue(ballotPaperDataDto) ?? 0);
            }

            return result;
        }
        static List<ReportParamValueResultsDto> MapToReportParameterValueDtoList(List<ReportParameterDto> reportParameterDtos, long? documentId)
        {
            List<ReportParamValueResultsDto> resultList = new List<ReportParamValueResultsDto>();
            foreach (var reportParameterDto in reportParameterDtos)
            {
                resultList.Add(new ReportParamValueResultsDto
                {
                    DocumentId = documentId,
                    ReportParameterId = reportParameterDto.ParameterId,
                    ReportParameterName = reportParameterDto.ParameterName,
                });
            }
            return resultList;
        }
        private void MapDocumentElectionResults(Document document, BallotPaper ballotPaper, DocumentStatus bpStatusToBeSet, Template template)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);


            document.BallotPaper = ballotPaper;
            document.ElectionRound = ballotPaper.ElectionRound;
            document.PollingStation = ballotPaper.PollingStation;
            document.StatusId = (int)bpStatusToBeSet;
            document.EditUser = userProxy;
            document.EditDate = DateTime.Now;
            document.Template = template;

            if (bpStatusToBeSet == DocumentStatus.WaitingForApproval)
            {
                document.EditDate = DateTime.Now;
            }

            if (bpStatusToBeSet == DocumentStatus.Approved)
            {
                document.IsResultsConfirmed = true;
                document.ConfirmationUserId = userId;
                document.ConfirmationDate = DateTime.Now;
            }
        }

        private async Task<SaveUpdateResult> CreateAndSaveReportParameterValues(List<ReportParamValueResultsDto> reportParameterValueDtos, Document documentEntity)
        {
            try
            {
                var userId = SecurityHelper.GetLoggedUserId();
                var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);
                var electionRound = await GetElectionRound(documentEntity.ElectionRound.Id);
                var assignedCircumscription = await GetAssignedCircumscription(documentEntity.AssignedCircumscription?.Id);

                var reportParameterValues = reportParameterValueDtos.Select(x => new ReportParameterValue
                {
                    Document = documentEntity,
                    ValueContent = x.ValueContent,
                    ElectionCompetitor = GetElectionCompetitor(x.ElectionCompetitorId),
                    ElectionCompetitorMember = GetElectionCompetitorMember(x.ElectionCompetitorMemberId),
                    //ElectionCompetitorName = x.ElectionCompetitorName,
                    //ElectionCompetitorMemberName = x.ElectionCompetitorMemberName,
                    BallotCount = x.BallotCount,
                    ReportParameter = _saiseRepository.Get<ReportParameter>(x.ReportParameterId),
                    EditDate = DateTime.Now,
                    EditUser = userProxy
                }).ToList();
                //reportParameterValues = reportParameterValues.ToList();

                foreach (var reportParameterValue in reportParameterValues)
                {
                    //var reportParamValue = await AddDocumentHeaderData(documentEntity.PollingStation.Id, reportParameterValue, electionRound, assignedCircumscription, pollingStation);
                    await _saiseRepository.SaveOrUpdateAsync(reportParameterValue);
                }

                return new SaveUpdateResult { Success = true, ValidationStatus = BallotPaperValidationStatus.IsValid, UpdatedAt = DateTime.Now, UserName = userProxy.UserName };
            }
            catch (Exception ex)
            {
                return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.InvalidStatus };
            }
        }
        #region Circumscritpion Consolidation Results
        public async Task<SaveUpdateResult> SaveConsolidatedResultsDocument(BallotPaperConsolidationDataDto ballotPaperDto, long templateNameId)
        {
            try
            {
                var validationResult = BallotPaperValidationStatus.InvalidStatus;
                //Section Saving Document Data

                var template = await GetTemplate(templateNameId);
                var documentEntity = await _saiseRepository.QueryAsync<Document>(z =>
                     z.FirstOrDefault(x => x.AssignedCircumscription.Id == ballotPaperDto.AssignedCircumscriptionId && x.Template.Id == template.Id));

                Dictionary<string, long> dictionary = MapConsolidationDtoToDictionary(ballotPaperDto);
                var reportParameterValueDtos = new List<ReportParamValueResultsDto>();

                if (documentEntity == null)
                {
                    validationResult = BallotPaperValidationStatus.IsValid;
                    documentEntity = new Document();
                    var parametersList = await ListTemplateParameters(templateNameId);

                    reportParameterValueDtos = MapToReportParameterValueDtoList(parametersList, null);

                    //Assign value to ValueContent for each PropertyName form BallotPaper matching ReportParameterName value from ReportParameterValueDto
                    foreach (var reportParameterValueDto in reportParameterValueDtos)
                    {
                        if (dictionary.TryGetValue(reportParameterValueDto.ReportParameterName, out long value))
                        {
                            reportParameterValueDto.ValueContent = value.ToString();
                        }
                    }
                    var existingDynamicParameter = reportParameterValueDtos.FirstOrDefault(dto => dto.ReportParameterName == "DynamicParameter");

                    if (existingDynamicParameter != null)
                    {
                        foreach (var electionResult in ballotPaperDto.CompetitorResults)
                        {
                            // Create a new ReportParamValueResultsDto for each CompetitorResultDto and add it to the list.
                            ReportParamValueResultsDto newDto = new ReportParamValueResultsDto
                            {
                                ReportParameterId = existingDynamicParameter.ReportParameterId,
                                ReportParameterName = existingDynamicParameter.ReportParameterName,
                                ElectionCompetitorId = electionResult.PoliticalPartyId,
                                ElectionCompetitorMemberId = electionResult.CandidateId,
                                ElectionCompetitorName = electionResult.PoliticalPartyName,
                                ElectionCompetitorMemberName = electionResult.CandidateName,
                                Order = electionResult.BallotOrder,
                                ValueContent = electionResult.BallotCount.ToString(),
                                BallotCount = electionResult.BallotCount
                            };

                            // Add newDto to reportParamValueResults list.
                            reportParameterValueDtos.Add(newDto);
                        }

                        // Remove the initial object which has ReportParameterName == "DynamicParameter"
                        reportParameterValueDtos.Remove(existingDynamicParameter);
                    }
                    MapDocumentElectionResultsConsolidated(documentEntity, ballotPaperDto, DocumentStatus.Approved, template);

                }
                else
                {
                    validationResult = ValidateConsolidationDocument(documentEntity);
                    reportParameterValueDtos = await ListDocumentParametersByCircumscription(ballotPaperDto.AssignedCircumscriptionId);

                    MapDocumentElectionResultsConsolidated(documentEntity, ballotPaperDto, DocumentStatus.Approved, template);
                }
                if (validationResult != BallotPaperValidationStatus.IsValid)
                {
                    return new SaveUpdateResult { Success = false, ValidationStatus = validationResult };
                }

                await _saiseRepository.SaveOrUpdateAsync(documentEntity);
                // save documentParameterValues

                var result = await CreateAndSaveReportParameterValues(reportParameterValueDtos, documentEntity);

                return result;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        static Dictionary<string, long> MapConsolidationDtoToDictionary(BallotPaperConsolidationDataDto ballotPaperDto)
        {
            Dictionary<string, long> result = new Dictionary<string, long>();
            Type type = typeof(BallotPaperConsolidationDataDto);

            foreach (PropertyInfo property in type.GetProperties())
            {
                // Exclude properties that are not to be included
                if (property.Name == "BallotPaperId" || property.Name == "CompetitorResults" || property.Name == "OpeningVotersCount")
                    continue;

                // Map the property name and value to the dictionary
                result[property.Name] = (long)(property.GetValue(ballotPaperDto) ?? 0);
            }

            return result;
        }

        private void MapDocumentElectionResultsConsolidated(Document document, BallotPaperConsolidationDataDto ballotPaperConsolidated, DocumentStatus bpStatusToBeSet, Template template)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);
            var assignedCircumscription = _saiseRepository.Get<AssignedCircumscription>(ballotPaperConsolidated.AssignedCircumscriptionId);

            document.AssignedCircumscription = assignedCircumscription;
            document.ElectionRound = assignedCircumscription.ElectionRound;
            document.StatusId = (int)bpStatusToBeSet;
            document.EditUser = userProxy;
            document.EditDate = DateTime.Now;
            document.Template = template;

            if (bpStatusToBeSet == DocumentStatus.WaitingForApproval)
            {
                document.EditDate = DateTime.Now;
            }

            if (bpStatusToBeSet == DocumentStatus.Approved)
            {
                document.IsResultsConfirmed = true;
                document.ConfirmationUserId = userId;
                document.ConfirmationDate = DateTime.Now;
            }
        }

        //private async Task<List<ReportParamValueResultsDto>> BallotPaperConsolidationDataDto(long assignedCircumscriptionId)
        //{
        //    var document = await _saiseRepository.QueryAsync<Document>(z =>
        //        z.FirstOrDefault(x => x.AssignedCircumscription.Id == assignedCircumscriptionId));

        //    var dtos = document.ReportParameterValues.Select(rp => new ReportParamValueResultsDto
        //    {
        //        ParameterValueId = rp.Id,
        //        DocumentId = document.Id,
        //        ReportParameterId = rp.ReportParameter.Id,
        //        ReportParameterName = rp.ReportParameter.ParameterName,
        //        ValueContent = rp.ValueContent,
        //        ElectionCompetitorName = rp.ElectionCompetitorName,
        //        ElectionCompetitorMemberName = rp.ElectionCompetitorMemberName,
        //        BallotCount = rp.BallotCount,
        //    }).ToList();

        //    return dtos;
        //}

        private async Task<List<ReportParamValueResultsDto>> ListDocumentParametersByCircumscription(long circumscriptionId)
        {
            var document = await _saiseRepository.QueryAsync<Document>(z =>
                z.FirstOrDefault(x => x.AssignedCircumscription.Id == circumscriptionId));

            var dtos = document.ReportParameterValues.Select(rp => new ReportParamValueResultsDto
            {
                ParameterValueId = rp.Id,
                DocumentId = document.Id,
                ReportParameterId = rp.ReportParameter.Id,
                ReportParameterName = rp.ReportParameter.ParameterName,
                ValueContent = rp.ValueContent,
                ElectionCompetitorId = rp.ElectionCompetitor.Id,
                ElectionCompetitorMemberId = rp.ElectionCompetitorMember.Id,
                ElectionCompetitorName = rp.ElectionCompetitor.NameRo,
                ElectionCompetitorMemberName = rp.ElectionCompetitorMember != null ? string.Format("{0} {1}", rp.ElectionCompetitorMember.NameRo, rp.ElectionCompetitorMember.LastNameRo) : null,
                BallotCount = rp.BallotCount,
            }).ToList();

            return dtos;
        }
        #endregion

        #region Final Report

        public async Task<List<UnconfirmedPollingStationsDto>> GetUnconfirmedPollingStations(long electionRoundId, long circumscriptionId, long templateNameId)
        {
            var assignedPollingStations = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                    z.Where(x => x.ElectionRound.Election.Id == electionRoundId && x.AssignedCircumscription.Id == circumscriptionId).ToList());
            var template = await GetTemplate(templateNameId);

            // Extract PollingStationIds from the projection
            var pollingStationIds = assignedPollingStations.Select(p => p.PollingStation.Id).ToList();

            var documentsConfirmedYet = await _saiseRepository.QueryAsync<Document>(z =>
                z.Where(x => x.ElectionRound.Election.Id == electionRoundId
                && x.Template.Id == template.Parent.Id
                && pollingStationIds.Contains(x.PollingStation.Id)
                && x.StatusId == (int)BallotPaperStatus.Approved).ToList());

            // Get the pollingStationIds that are not in documentsNotConfirmedYet
            var pollingStationIdsNotInDocuments = pollingStationIds.Except(documentsConfirmedYet.Select(x => x.PollingStation.Id)).ToList();

            var unconfirmedPollingStations = assignedPollingStations
                .Where(x => pollingStationIdsNotInDocuments.Contains(x.PollingStation.Id)) // Filter based on condition
                .Select(x => new UnconfirmedPollingStationsDto
                {
                    PollingStationNumber = x.PollingStation.Number,
                    Locality = x.PollingStation.Region.GetFullName(),
                    PollingStationName = x.PollingStation.NameRo,
                })
                .ToList();

            return unconfirmedPollingStations;

        }

        public async Task<DocumentDto> GetFinalReportDataAsync(long electionId, long circumscriptionId, long templateNameId)
        {
            try
            {
                var document = await GetCECEDocumentAsync(electionId, circumscriptionId, templateNameId);

                var assignedCircumscription = await _saiseRepository.QueryAsync<AssignedCircumscription>(z =>
                   z.FirstOrDefault(x => x.Id == circumscriptionId));
                var electionRound = await _saiseRepository.QueryAsync<ElectionRound>(z =>
                   z.FirstOrDefault(x => x.Election.Id == electionId));

                var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                    z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.AssignedCircumscription.Id == circumscriptionId));

                if (document == null)
                {
                    var result = new DocumentDto();
                    result.AssignedCircumscriptionId = assignedCircumscription.Id;
                    result.ElectionRoundId = electionRound.Id;
                    result.AlreadySent = false;

                    var parametersList = await ListTemplateParameters(templateNameId);
                    var parameterValueList = parametersList.Select(dto => new ReportParameterValueDto
                    {
                        ReportParameterId = dto.ParameterId,
                        ReportParameterName = dto.ParameterName,
                        ParameterCode = dto.ParameterCode,

                    }).ToList();
                    result.ReportParameterValues = parameterValueList;

                    //Totalization generation method here
                    var documentDto = await GetFinalCECEReportTotalizationAsync(result, electionRound.Id, circumscriptionId, templateNameId);

                    return documentDto;
                }

                var reportParamValues = await _saiseRepository.QueryAsync<ReportParameterValue>(x => x.Where(rp => rp.Document.Id == document.Id).ToList());
                return MapPollingStationDocumentAsync(document, reportParamValues, assignedPollingStation);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Document> GetCECEDocumentAsync(long electionRoundId, long circumscriptionId, long templateNameId)
        {
            var document = await _saiseRepository.QueryAsync<Document>(z =>
                 z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionRoundId
                 && x.AssignedCircumscription.Id == circumscriptionId
                 && x.Template.TemplateName.Id == templateNameId));
            return document;
        }

        private async Task<DocumentDto> GetFinalCECEReportTotalizationAsync(DocumentDto documentDto, long electionRoundId, long circumscriptionId, long templateNameId)
        {
            var confirmedPVDocumentsResult = await GetConfirmedPVDocuments(electionRoundId, circumscriptionId, templateNameId);

            try
            {
                var excludedParameters = new HashSet<string> {
                    "PlasticSealCodesOnVotingBox",
                    "PlasticSealCodesOnVotingBoxSlits",
                    "SeriesOfCertificatesIssuedToVoters",
                    "SeriesOfUnusedOrCancelledCertificates",
                    "UnusedOrCancelledCertificateSeries",
                    "PlasticSealCodesOnVotingBoxes",
                    "VotersCertificateSeries",
                    "SeriesOfIssuedCertificates",
                };

                var groupedReportParameterValues = confirmedPVDocumentsResult
                    .SelectMany(doc => doc.ReportParameterValues)
                    .Where(rp => !excludedParameters
                    .Contains(rp.ReportParameter.ParameterName))
                    .GroupBy(rp => rp.ReportParameter.ParameterName)
                    .Select(group => new
                    {
                        ReportParameterName = group.Key,
                        SumValueContent = group.Sum(rp => Convert.ToDecimal(rp.ValueContent))
                    })
                    .ToList();

                // Create ReportParameterValueDto objects from groupedReportParameterValues
                var reportParameterDtoList = groupedReportParameterValues
                    .Select(group =>
                        new ReportParameterValueDto
                        {
                            ReportParameterName = group.ReportParameterName,
                            ValueContent = group.SumValueContent.ToString() // Convert sum to string
                        })
                    .ToList();

                // Create a dictionary for faster lookup
                var groupedReportParameterValuesDict = groupedReportParameterValues.ToDictionary(g => g.ReportParameterName, g => g.SumValueContent);

                // Iterate through result.ReportParameterValues and update ValueContent if there's a match
                foreach (var reportParameterDto in documentDto.ReportParameterValues)
                {
                    if (groupedReportParameterValuesDict.TryGetValue(reportParameterDto.ReportParameterName, out var sumValueContent))
                    {
                        reportParameterDto.ValueContent = sumValueContent.ToString();
                    }
                }

                return documentDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<IList<Document>> GetConfirmedPVDocuments(long electionRoundId, long circumscriptionId, long templateNameId)
        {
            var assignedPollingStations = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.Where(x => x.ElectionRound.Id == electionRoundId && x.AssignedCircumscription.Id == circumscriptionId).ToList());

            var template = await GetTemplate(templateNameId);

            var pollingStationIds = assignedPollingStations.Select(p => p.PollingStation.Id).ToList();
            var documentsConfirmedYet = await _saiseRepository.QueryAsync<Document>(z =>
                z.Where(x => x.ElectionRound.Id == electionRoundId
                && pollingStationIds.Contains(x.PollingStation.Id)
                && x.StatusId == (int)BallotPaperStatus.Approved
                && x.Template.TemplateName.Id == template.Parent.Id).ToList());

            return documentsConfirmedYet;
        }
        #endregion

        #region BallotPaperDocument Retrieve For Printing
        public async Task<DocumentBallotPaperDto> GetBallotPaperDocumentAsync(long electionId, long templateNameId, long? pollingStationId, long? circumscriptionId)
        {
            try
            {
                Document document;
                if (pollingStationId != null)
                {
                    document = await _saiseRepository.QueryAsync<Document>(z =>
                        z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId
                        && x.PollingStation.Id == pollingStationId
                        && x.Template.TemplateName.Id == templateNameId));
                }
                else
                {
                    document = await _saiseRepository.QueryAsync<Document>(z =>
                        z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId
                        && x.AssignedCircumscription.Id == circumscriptionId
                        && x.Template.TemplateName.Id == templateNameId));
                }

                if (document == null)
                {
                    return null;
                }

                var documentDto = MapDocumentBallotPaperDto(document);
                documentDto.ReportParameterValues = documentDto.ReportParameterValues.OrderBy(x => x.Order).ToList();
                return documentDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private DocumentBallotPaperDto MapDocumentBallotPaperDto(Document document)
        {
            var reportParameterValueDtos = ListDocumentParametersDtoByDocumentId(document);
            string pollingStationName;


            if (document.PollingStation == null)
            {
                pollingStationName = string.Empty;
            }
            else
            {
                pollingStationName = document.PollingStation.NameRo;
            }

            var result = new DocumentBallotPaperDto
            {
                DocumentId = document.Id,
                TemplateId = document.Template.Id,
                PollingStationId = document.PollingStation?.Id,
                ElectionRoundId = document.ElectionRound.Id,
                DocumentName = document.Template.TemplateName + pollingStationName,
                EditDate = document.EditDate,
                EditUser = document.EditUser.UserName,
                IsResultsConfirmed = document.IsResultsConfirmed,
                ConfirmationUserId = document.ConfirmationUserId,
                ConfirmationDate = document.ConfirmationDate,
                Status = document.StatusId,
                ReportParameterValues = reportParameterValueDtos
            };

            return result;

        }
        private List<ReportParamValueResultsDto> ListDocumentParametersDtoByDocumentId(Document document)
        {
            var dtos = document.ReportParameterValues.Select(rp =>
            {
                var dto = new ReportParamValueResultsDto
                {
                    ParameterValueId = rp.Id,
                    DocumentId = document.Id,
                    ReportParameterId = rp.ReportParameter.Id,
                    ReportParameterName = rp.ReportParameter.ParameterName,
                    ValueContent = rp.ValueContent,
                    ElectionCompetitorId = rp.ElectionCompetitor != null ? (long?)rp.ElectionCompetitor.Id : null,
                    ElectionCompetitorMemberId = rp.ElectionCompetitorMember != null ? (long?)rp.ElectionCompetitorMember.Id : null,
                    ElectionCompetitorName = rp.ElectionCompetitor != null ? rp.ElectionCompetitor.NameRo : null,
                    ElectionCompetitorMemberName = rp.ElectionCompetitorMember != null ? string.Format("{0} {1}", rp.ElectionCompetitorMember.NameRo, rp.ElectionCompetitorMember.LastNameRo) : null,
                    BallotCount = rp.BallotCount,
                };

                if (rp.ElectionCompetitorMember != null)
                {
                    dto.Order = rp.ElectionCompetitorMember.CompetitorMemberOrder;
                }
                else if (rp.ElectionCompetitor != null)
                {
                    dto.Order = rp.ElectionCompetitor.BallotOrder;
                }
                else
                {
                    dto.Order = null;
                }

                return dto;
            }).ToList();

            return dtos;
        }
        #endregion

        #region DocumentGrids

        public async Task<PageResponse<PollintStationDocumentStageDto>> GetPollingStationDocuments(PageRequest pageRequest, long electionId, long templateNameId, long assignedCircumscriptionId)
        {
            var isCeceTemplate = await IsTemplateCECE(templateNameId);
            var electionRound = await GetElectionRoundByElection(electionId);

            var template = await GetTemplate(templateNameId);
            var assignedPollingStations = await GetAssignedPollingStations(electionId, assignedCircumscriptionId);
            var assignedCircumscription = await GetAssignedCircumscription(electionId, assignedCircumscriptionId);
            var documentDtos = new List<PollintStationDocumentStageDto>();

            if (!isCeceTemplate)
            {
                var assignedPollingStationIds = assignedPollingStations.Select(x => x.PollingStation.Id).ToList();
                var matchingDocuments = await GetMatchingDocumentsByPollingStationIds(electionId, assignedPollingStationIds, template.Id);
                documentDtos = CreateDocumentDtos(matchingDocuments, template, assignedCircumscription);

                var documentPollingStationIds = new HashSet<long>(matchingDocuments.Select(doc => doc.PollingStation.Id));
                var unmatchedPollingStations = GetUnmatchedPollingStations(assignedPollingStations, documentPollingStationIds);
                CreateDtosForUnmatchedPollingStations(unmatchedPollingStations, documentDtos, template);
            }
            else
            {
                var matchingDocuments = await GetMatchingDocumentsByCircumscriptionId(electionId, assignedCircumscription.Id, template.Id);
                documentDtos = CreateDocumentDtos(matchingDocuments, template, assignedCircumscription);

                var documentCircumscriptionIds = new HashSet<long>(matchingDocuments.Select(doc => doc.AssignedCircumscription.Id));
                var unmatchedCircumscriptions = GetUnmatchedCircumscriptions(assignedCircumscription, documentCircumscriptionIds);
                CreateDtosForUnmatchedCircumscriptions(unmatchedCircumscriptions, documentDtos, template);
            }
            // ApplyFiltersAndSorting is assumed to be defined elsewhere and accessible here
            var processedDocuments = ApplyFiltersAndSorting(documentDtos, pageRequest);

            // Now, with the documents filtered and sorted, we can paginate them
            return PaginateDocuments(processedDocuments, pageRequest);
        }


        private static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        #endregion


        #region DocumentPathMinIo
        public async Task<string> CreateDocumentName(long templateNameId, long electionId, long? pollingStationId, long? circumscriptionId)
        {
            var circumscriptionName = await GetAssignedCircumscriptionName(circumscriptionId);
            var pollingStationName = await GetPollingStationName(pollingStationId);
            var electionRoundName = await GetElectionRoundNameDate(electionId);
            var templateName = await GetTemplateName(templateNameId);
            var documentName = string.Concat(electionRoundName, circumscriptionName, "", templateName, Guid.NewGuid().ToString());
            return documentName;
        }

        public async Task<bool> SaveDocumentPath(long documentId, string fileName, int contentLength, string contentType)
        {
            try
            {
                var document = await GetDocumentAsync(documentId);
                if (document != null && document.DocumentPath == null)
                {
                    document.DocumentPath = fileName;
                    document.FileLength = contentLength;
                    // Extract just the extension part from the contentType
                    string fileExtension = contentType.Split('/').LastOrDefault();
                    document.FileExtension = fileExtension;
                    await _saiseRepository.SaveOrUpdateAsync(document);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DocumentIsUploaded(long documentId)
        {
            var document = await GetDocumentAsync(documentId);
            if (document.DocumentPath == null)
            {
                return false;
            }
            return true;
        }
        private async Task<Document> GetDocumentAsync(long documentId)
        {
            return await _saiseRepository.QueryAsync<Document>(z =>
                 z.FirstOrDefault(x => x.Id == documentId));
        }
        private async Task<string> GetAssignedCircumscriptionName(long? assignedCircumscriptionId)
        {
            var assignedCircumscription = await GetAssignedCircumscription(assignedCircumscriptionId);
            if (assignedCircumscription == null)
            {
                return string.Empty;
            }
            return assignedCircumscription.NameRo;
        }
        private async Task<string> GetPollingStationName(long? pollingStationId)
        {
            var pollingstation = await GetPollingStation(pollingStationId);
            if (pollingstation == null)
            {
                return string.Empty;
            }
            return pollingstation.NameRo;
        }
        private async Task<string> GetElectionRoundNameDate(long electionId)
        {
            var electionRound = await _saiseRepository.QueryAsync<ElectionRound>(z => z.FirstOrDefault(x => x.Election.Id == electionId));
            // Format the date in "ddMMyyyy" format
            string formattedDate = electionRound.ElectionDate.ToString("ddMMyyyy");
            return electionRound.Election.Comments + formattedDate;
        }
        private async Task<string> GetTemplateName(long templateNameId)
        {
            var template = await GetTemplate(templateNameId);
            return template.TemplateName.Title;
        }
        #endregion

        #region Helpers
        public async Task<long> GetElectionRoundIdByElection(long electionId)
        {
            var electionRound = await _saiseRepository.QueryAsync<ElectionRound>(z => z.FirstOrDefault(x => x.Election.Id == electionId));
            return electionRound.Id;
        }

        private async Task<AssignedCircumscription> GetAssignedCircumscription(long? assignedCircumscriptionId)
        {
            return assignedCircumscriptionId != null
                ? await _saiseRepository.QueryAsync<AssignedCircumscription>(z => z.FirstOrDefault(x => x.Id == assignedCircumscriptionId))
                : null;
        }

        private async Task<PollingStation> GetPollingStation(long? pollingstationId)
        {
            return pollingstationId != null
                ? await _saiseRepository.QueryAsync<PollingStation>(z => z.FirstOrDefault(x => x.Id == pollingstationId))
                : null;
        }

        private async Task<ElectionRound> GetElectionRound(long electionRoundId)
        {
            return await _saiseRepository.QueryAsync<ElectionRound>(z => z.FirstOrDefault(x => x.Id == electionRoundId));
        }

        private ElectionCompetitorMember GetElectionCompetitorMember(long? electionCompetitorMemberId)
        {
            return electionCompetitorMemberId != null
                ? _saiseRepository.QueryOver<ElectionCompetitorMember>().Where(x => x.Id == electionCompetitorMemberId).SingleOrDefault() // await _saiseRepository.QueryAsync<ElectionCompetitorMember>(z => z.FirstOrDefault(x => x.Id == electionCompetitorMemberId))
                : null;
        }

        private ElectionCompetitor GetElectionCompetitor(long? electionCompetitorId)
        {
            return electionCompetitorId != null
                ? _saiseRepository.QueryOver<ElectionCompetitor>().Where(x => x.Id == electionCompetitorId).SingleOrDefault()//await _saiseRepository.QueryAsync<ElectionCompetitor>(z => z.FirstOrDefault(x => x.Id == electionCompetitorId))
                : null;
        }
        private async Task<ElectionRound> GetElectionRoundByElection(long electionId)
        {
            return await _saiseRepository.QueryAsync<ElectionRound>(z => z.FirstOrDefault(x => x.Election.Id == electionId));
        }

        private async Task<Template> GetTemplate(long templateNameId)
        {
            return await _saiseRepository.QueryAsync<Template>(z => z.FirstOrDefault(x => x.TemplateName.Id == templateNameId));
        }
        private async Task<bool> IsTemplateCECE(long templateNameId)
        {
            var result = await _saiseRepository.QueryAsync<TemplateMapping>(z => z.FirstOrDefault(x => x.TemplateName.Id == templateNameId));
            return result.IsCECE;
        }

        private async Task<long?> GetCandidateId(long electionResultId)
        {
            var result = await _saiseRepository.QueryAsync<ElectionResult>(z => z.FirstOrDefault(x => x.Id == electionResultId));
            return result.Candidate?.Id;
        }

        private async Task<long?> GetPoliticalPartyId(long electionResultId)
        {
            var result = await _saiseRepository.QueryAsync<ElectionResult>(z => z.FirstOrDefault(x => x.Id == electionResultId));
            return result.PoliticalParty?.Id;
        }

        private async Task<int> GetElectionCompetitorOrder(long electionResultId)
        {
            var result = await _saiseRepository.QueryAsync<ElectionResult>(z => z.FirstOrDefault(x => x.Id == electionResultId));
            return result.Candidate.CompetitorMemberOrder;
        }

        private async Task<List<AssignedPollingStation>> GetAssignedPollingStations(long electionId, long assignedCircumscriptionId)
        {
            var result = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                        z.Where(x => x.ElectionRound.Election.Id == electionId && x.AssignedCircumscription.Id == assignedCircumscriptionId).ToList());
            return result.ToList();
        }

        private async Task<List<Document>> GetMatchingDocumentsByPollingStationIds(long electionId, List<long> pollingStationIds, long templateId)
        {
            var result = await _saiseRepository.QueryAsync<Document>(z =>
               z.Where(x => x.ElectionRound.Election.Id == electionId
                            && x.Template.Id == templateId
                            && (x.StatusId == (int)DocumentStatus.Approved || x.StatusId == (int)DocumentStatus.WaitingForApproval)
                            && pollingStationIds.Contains(x.PollingStation.Id)).ToList());
            return result.ToList();
        }

        private List<PollintStationDocumentStageDto> CreateDocumentDtos(List<Document> documents, Template template, AssignedCircumscription assignedCircumscription)
        {
            return documents.Select(doc => new PollintStationDocumentStageDto
            {
                Id = doc.Id,
                Circumscription = doc.AssignedCircumscription?.NameRo ?? assignedCircumscription.NameRo,
                CircumscriptionNumber = doc.AssignedCircumscription?.Number ?? assignedCircumscription.Number,
                Locality = $"{doc.PollingStation?.Region.RegionType.Name} {doc.PollingStation?.Region.Name}",
                PollingStation = doc.PollingStation?.NumberPerElection,
                TemplateName = doc.Template?.TemplateName.Title ?? template?.TemplateName.Title,
                DocumentStatusId = doc.StatusId,
                DocumentStatus = GetDescription((DocumentStatus)doc.StatusId)
            }).ToList();
        }

        private List<AssignedPollingStation> GetUnmatchedPollingStations(List<AssignedPollingStation> assignedPollingStations, HashSet<long> documentPollingStationIds)
        {
            // Filter out assigned polling stations whose IDs are not in the set of polling station IDs that have documents
            var unmatchedPollingStations = assignedPollingStations.Where(ps => !documentPollingStationIds.Contains(ps.PollingStation.Id)).ToList();
            return unmatchedPollingStations;
        }

        private void CreateDtosForUnmatchedPollingStations(List<AssignedPollingStation> unmatchedPollingStations, List<PollintStationDocumentStageDto> documentDtos, Template template)
        {
            foreach (var pollingStation in unmatchedPollingStations)
            {
                var dto = new PollintStationDocumentStageDto
                {
                    // Since there's no document, the ID will be null
                    Id = null,
                    Circumscription = pollingStation.AssignedCircumscription?.NameRo ?? " ",
                    CircumscriptionNumber = pollingStation.AssignedCircumscription?.Number ?? " ",
                    Locality = $"{pollingStation.PollingStation?.Region.RegionType.Name} {pollingStation.PollingStation?.Region.Name}",
                    PollingStation = pollingStation.PollingStation?.NumberPerElection,
                    TemplateName = template?.TemplateName.Title,
                    DocumentStatusId = (int)DocumentStatus.New,
                    DocumentStatus = GetDescription(DocumentStatus.New)
                };

                documentDtos.Add(dto);
            }
        }

        private async Task<AssignedCircumscription> GetAssignedCircumscription(long electionId, long assignedCircumscriptionId)
        {
            // Query the repository for AssignedCircumscription entities where the ElectionRound's Election ID matches the specified electionId
            var assignedCircumscription = await _saiseRepository.QueryAsync<AssignedCircumscription>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.Id == assignedCircumscriptionId));

            return assignedCircumscription;
        }

        private async Task<List<Document>> GetMatchingDocumentsByCircumscriptionId(long electionId, long circumscriptionId, long templateId)
        {
            // Perform an asynchronous query to fetch documents associated with the specified election and circumscription IDs
            // and have a status of either Approved or WaitingForApproval
            var matchingDocuments = await _saiseRepository.QueryAsync<Document>(z =>
                z.Where(x => x.ElectionRound.Election.Id == electionId
                            && x.Template.Id == templateId
                            && (x.StatusId == (int)DocumentStatus.Approved || x.StatusId == (int)DocumentStatus.WaitingForApproval) &&
                             x.AssignedCircumscription.Id == circumscriptionId)
                 .ToList());

            return matchingDocuments.ToList();
        }

        private List<AssignedCircumscription> GetUnmatchedCircumscriptions(AssignedCircumscription assignedCircumscription, HashSet<long> documentCircumscriptionIds)
        {
            var unmatchedCircumscriptions = new List<AssignedCircumscription>();

            // Check if the single assigned circumscription's ID is not in the HashSet of document circumscription IDs
            if (!documentCircumscriptionIds.Contains(assignedCircumscription.Id))
            {
                // If not, add it to the list of unmatched circumscriptions
                unmatchedCircumscriptions.Add(assignedCircumscription);
            }

            return unmatchedCircumscriptions;
        }

        private void CreateDtosForUnmatchedCircumscriptions(List<AssignedCircumscription> unmatchedCircumscriptions, List<PollintStationDocumentStageDto> documentDtos, Template template)
        {
            foreach (var circumscription in unmatchedCircumscriptions)
            {
                var dto = new PollintStationDocumentStageDto
                {
                    Id = null, // No corresponding document
                    Circumscription = circumscription.NameRo,
                    CircumscriptionNumber = circumscription.Number,
                    Locality = $"{circumscription.Region.RegionType.Name} {circumscription.Region.Name}",
                    PollingStation = "",
                    TemplateName = template?.TemplateName.Title,
                    DocumentStatusId = (int)DocumentStatus.New,
                    DocumentStatus = GetDescription(DocumentStatus.New)
                };

                documentDtos.Add(dto);
            }
        }

        private PageResponse<PollintStationDocumentStageDto> PaginateDocuments(IEnumerable<PollintStationDocumentStageDto> processedDocuments, PageRequest pageRequest)
        {

            int skip = (pageRequest.PageNumber - 1) * pageRequest.PageSize;
            var paginatedItems = processedDocuments.Skip(skip).Take(pageRequest.PageSize).ToList();

            var response = new PageResponse<PollintStationDocumentStageDto>
            {
                StartIndex = skip,
                PageSize = pageRequest.PageSize,
                Total = processedDocuments.Count(),
                Items = paginatedItems
            };
            return response;
        }

        private IEnumerable<PollintStationDocumentStageDto> ApplyFiltersAndSorting(IEnumerable<PollintStationDocumentStageDto> documents, PageRequest pageRequest)
        {
            // Apply filters
            var filteredDocuments = documents.AsQueryable(); // Ensure it's IQueryable to use LINQ dynamically

            foreach (var filterGroup in pageRequest.FilterGroups)
            {
                foreach (var filter in filterGroup.Filters)
                {
                    // Determine the property to filter on
                    var propertyInfo = typeof(PollintStationDocumentStageDto).GetProperty(filter.Property);
                    if (propertyInfo == null) continue; // Skip if property not found

                    // Apply filter based on operator
                    switch (filter.Operator)
                    {
                        case ComparisonOperator.IsEqualTo: // Equal
                            filteredDocuments = filteredDocuments.Where(d =>
                                propertyInfo.GetValue(d, null).ToString().ToLower().Equals(filter.Value.ToString().ToLower(), StringComparison.OrdinalIgnoreCase));
                            break;
                        case ComparisonOperator.Contains: // Contains
                            filteredDocuments = filteredDocuments.Where(d =>
                                propertyInfo.GetValue(d, null).ToString().ToLower().Contains(filter.Value.ToString().ToLower()));
                            break;
                        default:
                            break;
                    }
                }
            }

            // Apply sorting
            foreach (var sortField in pageRequest.SortFields)
            {
                filteredDocuments = sortField.Ascending ?
                    filteredDocuments.OrderBy(d => d.GetType().GetProperty(sortField.Property).GetValue(d, null)) :
                    filteredDocuments.OrderByDescending(d => d.GetType().GetProperty(sortField.Property).GetValue(d, null));
            }

            return filteredDocuments.ToList();
        }
        #endregion
    }
}
