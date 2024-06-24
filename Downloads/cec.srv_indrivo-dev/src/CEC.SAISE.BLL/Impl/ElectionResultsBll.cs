using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
    public class ElectionResultsBll : IElectionResultsBll
    {
        private readonly ISaiseRepository _saiseRepository;
        private readonly IDocumentsBll _documentsBll;

        public ElectionResultsBll(ISaiseRepository saiseRepository, IDocumentsBll documentsBll)
        {
            _saiseRepository = saiseRepository;
            _documentsBll = documentsBll;
        }

        public async Task<BallotPaperDto> GetBallotPaperAsync(long electionRoundId, long pollingStationId)
        {
            var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionRoundId && x.PollingStation.Id == pollingStationId));
            var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionRoundId && x.PollingStation.Id == pollingStationId));


            if (ballotPaper == null)
            {
                return null;
            }

            return MapBallotPaper(ballotPaper, assignedPollingStation);

        }

        public async Task<BallotPaperHeaderDataDto> GetBallotPaperHeaderData(long electionId, long? pollingStationId, long circumscriptionId, long templateNameId)
        {
            string circumscriptionText = "Consiliul electoral al circumscripției electorale";
            AssignedPollingStation assignedPollingStation = null;
            if (pollingStationId != null)
            {
                assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                    z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.PollingStation.Id == pollingStationId));
            }

            var assignedCircumscription = await _saiseRepository.QueryAsync<AssignedCircumscription>(z =>
                    z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.Id == circumscriptionId));
            var electionRound = await GetElectionRound(electionId);
            var pollingStation = await GetPollingStation(pollingStationId);

            string headerElectionDetails = await GetHeaderElectionDetails(electionRound, assignedCircumscription, templateNameId);
            headerElectionDetails = headerElectionDetails + HandleElectionDetailsSecondPart(electionRound, assignedCircumscription);

            var result = new BallotPaperHeaderDataDto
            {
                ElectionName = GetHeaderElectionName(electionRound),
                ElectionDate = electionRound.Election.DateOfElection.ToString(),
                ReferendumQuestion = await GetHeaderReferendumQuestion(electionRound),
                CircumscriptionRegion = assignedCircumscription != null ? circumscriptionText
                                                                        : string.Empty,
                CircumscriptionName = assignedCircumscription != null ? string.Concat(GetHeaderRegionTypeName(assignedCircumscription.Region.RegionType.Id)," ", assignedCircumscription?.Region.Name
                                                                                    , " NR. ", assignedCircumscription.Number)
                                                                      : string.Empty,
                PollingStationRegion = assignedPollingStation != null ? string.Concat(assignedPollingStation.PollingStation.Region.Parent.GetFullName(), "/")
                                                                      : string.Empty,
                PollingStationNumber = assignedPollingStation != null ? string.Concat(assignedPollingStation.PollingStation.Region.GetFullName()
                                                                                    , ", nr. "
                                                                                    , GetCircumscriptionExtractedNumber(assignedCircumscription.Number)
                                                                                    , assignedPollingStation.PollingStation.Number.ToString())
                                                                      : string.Empty,

                DocumentHeaderElectionDetails = headerElectionDetails
            };
            return result;
        }
        private string GetCircumscriptionExtractedNumber(string circumscriptionNumber)
        {
            int slashIndex = circumscriptionNumber.IndexOf('/');

            if (slashIndex != -1)
            {
                return circumscriptionNumber.Substring(0, slashIndex + 1);
            }
            else
            {
                return string.Concat(circumscriptionNumber, "/");
            }
        }
        private static string GetHeaderElectionName(ElectionRound electionRound)
        {
            string description = electionRound.Election.Type.Description;

            if (electionRound.Number == 2)
            {
                description += " (TUR II)";
            }

            if (electionRound.RepeatElection)
            {
                description += ". VOTARE REPETATĂ";
            }

            return description;
        }

        private async Task<string> GetHeaderReferendumQuestion(ElectionRound electionRound)
        {
            if (electionRound.Election.Type.ElectionCompetitorType != 3)
            {
                return string.Empty;
            }
            var electionCompetitor = await _saiseRepository.QueryAsync<ElectionCompetitor>(z => z.FirstOrDefault());
            return string.Concat("ÎNTREBAREA: ", electionCompetitor.NameRo);
        }

        private string GetHeaderRegionTypeName(long regionTypeId)
        {
            if (DocumentsHelper.RegionTypeNames.TryGetValue(regionTypeId, out string name))
            {
                return name;
            }

            return string.Empty; // Default value if not found
        }
        private async Task<string> GetHeaderElectionDetails(ElectionRound electionRound, AssignedCircumscription assignedCircumscription, long templateNameId)
        {
            string result;
            var templateMapping = await _saiseRepository.QueryAsync<TemplateMapping>(z => z.FirstOrDefault(x => x.TemplateName.Id == templateNameId));

            if (templateMapping == null)
            {
                return string.Empty; // Or throw an exception, or return a default value, as per your application logic.
            }

            if (!templateMapping.IsCECE)
            {
                result = electionRound.Election.Type.ElectionCompetitorType != 3
                    ? " privind rezultatele numărării voturilor pentru alegerea "
                    : " privind rezultatele numărării voturilor la referendum ";
            }
            else
            {
                result = electionRound.Election.Type.ElectionCompetitorType != 3
                    ? " privind centralizarea rezultatelor votării la alegerea "
                    : " privind centralizarea rezultatelor votării la referendum ";
            }
            return result;

        }
        private string HandleElectionDetailsSecondPart(ElectionRound electionRound, AssignedCircumscription assignedCircumscription)
        {
            if (electionRound.Election.Type.ElectionCompetitorType == 2 && electionRound.Election.Type.ElectionArea == 1)
            {
                DocumentsHelper.ElectionDetailsTypeDescription.TryGetValue(assignedCircumscription.Region.Parent.RegionType.Id, out var result2);
                return DocumentsHelper.ElectionDetailsType2Area1.TryGetValue(assignedCircumscription.Region.RegionType.Id, out var result)
                    ? string.Concat(result, CapitalizeFirstLetter(assignedCircumscription.Region.Name), ", ", result2, CapitalizeFirstLetter(assignedCircumscription.Region.Parent.Name))
                    : " ";
            }
            else if (electionRound.Election.Type.ElectionCompetitorType == 1 && electionRound.Election.Type.ElectionArea == 1)
            {
                switch (assignedCircumscription.Region.RegionType.Id)
                {
                    case 1:
                        return "" + assignedCircumscription.Region.GetFullPath();
                    case 2:
                        return "" + assignedCircumscription.Region.GetFullPath();
                    case 3:
                        return "" + assignedCircumscription.Region.GetFullPath();
                    case 4:
                        if (assignedCircumscription.Region.StatisticIdentifier == 100)
                        {
                            return "Primarului general al municipiului " + assignedCircumscription.Region.Name;
                        }
                        else return "Primarului municipiului " + assignedCircumscription.Region.Name;
                    case 5:
                        return "" + assignedCircumscription.Region.GetFullPath(); ;
                    case 6:
                        return "Primarului orașului " + assignedCircumscription.Region.Name + " raionul/municipiul " + assignedCircumscription.Region.Parent.Name;
                    case 7:
                        return "Primarului orașului " + assignedCircumscription.Region.Name + " raionul/municipiul " + assignedCircumscription.Region.Parent.Name;
                    case 8:
                        return "Primarului comunei " + assignedCircumscription.Region.Name + " raionul/municipiul " + assignedCircumscription.Region.Parent.Name;
                    case 9:
                        return "Primarului satului " + assignedCircumscription.Region.Name + " raionul/municipiulc" + assignedCircumscription.Region.Parent.Name;
                    case 10:
                        return "Primarului municipiului " + assignedCircumscription.Region.Name;
                    default:
                        return "";
                }
            }
            else if (electionRound.Election.Type.ElectionCompetitorType == 2 && electionRound.Election.Type.ElectionArea == 2)
            {
                return "Parlamentului Republicii Moldova";
            }
            else if (electionRound.Election.Type.ElectionCompetitorType == 1 && electionRound.Election.Type.ElectionArea == 2)
            {
                return "Președintelui Republicii Moldova";
            }
            return "";
        }


        public async Task<BallotPaperDto> GetBallotPaperAsync(long ballotPaperId)
        {
            var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.FirstOrDefault(x => x.Id == ballotPaperId));

            if (ballotPaper == null)
            {
                return null;
            }

            var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Id == ballotPaper.ElectionRound.Id && x.PollingStation.Id == ballotPaper.PollingStation.Id));

            if (assignedPollingStation == null)
            {
                return null;
            }

            return MapBallotPaper(ballotPaper, assignedPollingStation);
        }

        public async Task<SaveUpdateResult> SaveUpdateResults(BallotPaperDataDto ballotPaperDto, BallotPaperStatus bpStatusToBeSet, long templateNameId)
        {
            var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.FirstOrDefault(x => x.Id == ballotPaperDto.BallotPaperId));

            var validationResult = ValidateBallotPaper(ballotPaper, ballotPaperDto, bpStatusToBeSet);

            if (validationResult != BallotPaperValidationStatus.IsValid)
            {
                return new SaveUpdateResult { Success = false, ValidationStatus = validationResult };
            }

            var userIsAdmin = SecurityHelper.LoggedUserIsInRole("Administrator");

            if (ballotPaper.IsResultsConfirmed && !userIsAdmin)
            {
                return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.MultipleConfirmationsProhibited };
            }

            TransferData(ballotPaperDto, ballotPaper, bpStatusToBeSet);

            await _saiseRepository.SaveOrUpdateAsync(ballotPaper);

            //Section Saving Document Data
            await _documentsBll.SaveUpdateDocumentElectionResults(ballotPaperDto, templateNameId, (DocumentStatus)bpStatusToBeSet);

            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);

            return new SaveUpdateResult { Success = true, ValidationStatus = validationResult, UpdatedAt = DateTime.Now, UserName = userProxy.UserName };
        }

        private void TransferData(BallotPaperDataDto ballotPaperDto, BallotPaper ballotPaper, BallotPaperStatus bpStatusToBeSet)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);

            ballotPaper.RegisteredVoters = ballotPaperDto.RegisteredVoters;
            ballotPaper.Supplementary = ballotPaperDto.Supplementary;
            ballotPaper.BallotsIssued = ballotPaperDto.BallotsIssued;
            ballotPaper.BallotsCasted = ballotPaperDto.BallotsCasted;
            ballotPaper.DifferenceIssuedCasted = ballotPaperDto.DifferenceIssuedCasted;
            ballotPaper.BallotsValidVotes = ballotPaperDto.BallotsValidVotes;
            ballotPaper.BallotsReceived = ballotPaperDto.BallotsReceived;
            ballotPaper.BallotsUnusedSpoiled = ballotPaperDto.BallotsUnusedSpoiled;
            ballotPaper.BallotsSpoiled = ballotPaperDto.BallotsSpoiled;
            ballotPaper.BallotsUnused = ballotPaperDto.BallotsUnused;
            ballotPaper.Status = bpStatusToBeSet;
            ballotPaper.EditUser = userProxy;

            foreach (var competitorResult in ballotPaperDto.CompetitorResults)
            {
                var electionResult = ballotPaper.ElectionResults.First(x => x.Id == competitorResult.ElectionResultId);
                if (electionResult.PoliticalParty.Status != PoliticalPartyStatus.Rejected)
                {
                    electionResult.BallotCount = competitorResult.BallotCount;
                    electionResult.Status = (bpStatusToBeSet == BallotPaperStatus.Approved)
                        ? ElectionResultStatus.Approved
                        : ElectionResultStatus.WaitingForApproval;

                    electionResult.EditUser = userProxy;
                    electionResult.EditDate = DateTime.Now;
                }
            }

            if (bpStatusToBeSet == BallotPaperStatus.WaitingForApproval)
            {
                ballotPaper.EditDate = DateTime.Now;
            }

            if (bpStatusToBeSet == BallotPaperStatus.Approved)
            {
                ballotPaper.IsResultsConfirmed = true;
                ballotPaper.ConfirmationUserId = userId;
                ballotPaper.ConfirmationDate = DateTime.Now;
            }
        }

        private BallotPaperValidationStatus ValidateBallotPaper(BallotPaper ballotPaper, BallotPaperDataDto ballotPaperDto, BallotPaperStatus bpStatusToBeSet)
        {
            if (ballotPaper == null)
            {
                return BallotPaperValidationStatus.NotFound;
            }

            //if (bpStatusToBeSet == BallotPaperStatus.WaitingForApproval && ballotPaper.Status != BallotPaperStatus.New)
            //{
            //    return BallotPaperValidationStatus.InvalidStatus;
            //}

            if (bpStatusToBeSet == BallotPaperStatus.Approved &&
                ballotPaper.Status != BallotPaperStatus.WaitingForApproval)
            {
                return BallotPaperValidationStatus.InvalidStatus;
            }

            return BallotPaperValidationStatus.IsValid;
        }

        private BallotPaperDto MapBallotPaper(BallotPaper ballotPaper, AssignedPollingStation assignedPollingStation)
        {
            try
            {
                var result = new BallotPaperDto
                {
                    BallotPaperId = ballotPaper.Id,
                    RegisteredVoters = ballotPaper.RegisteredVoters,
                    Supplementary = ballotPaper.Supplementary,
                    BallotsIssued = ballotPaper.BallotsIssued,
                    BallotsCasted = ballotPaper.BallotsCasted,
                    DifferenceIssuedCasted = ballotPaper.DifferenceIssuedCasted,
                    BallotsValidVotes = ballotPaper.BallotsValidVotes,
                    BallotsReceived = ballotPaper.BallotsReceived,
                    BallotsUnusedSpoiled = ballotPaper.BallotsUnusedSpoiled,
                    BallotsSpoiled = ballotPaper.BallotsSpoiled,
                    BallotsUnused = ballotPaper.BallotsUnused,
                    EditDate = ballotPaper.EditDate,
                    EditUser = ballotPaper.EditUser.UserName,
                    IsResultsConfirmed = ballotPaper.IsResultsConfirmed,
                    ConfirmationUserId = ballotPaper.ConfirmationUserId,
                    ConfirmationDate = ballotPaper.ConfirmationDate,
                    Status = ballotPaper.Status,
                    ElectionType = ballotPaper.ElectionRound.Election.Type.ElectionCompetitorType,
                    OpeningVotersCount = assignedPollingStation.OpeningVoters,
                    CompetitorResults = ballotPaper.ElectionResults
                  .Where(x => (x.PoliticalParty.AssignedCircumscription != null && x.PoliticalParty.AssignedCircumscription.Id == assignedPollingStation.AssignedCircumscription.Id) || (x.PoliticalParty.AssignedCircumscription == null))
                  .OrderBy(x => x.BallotOrder)
                  .Select(x => MapElectionResult(x, assignedPollingStation))
                  .ToList(),

                };
                SetPermissionsForCurrentUser(result, assignedPollingStation.ImplementsEVR);
                return result;

            }
            catch (Exception e)
            {
                return null;

            }
        }

        private void SetPermissionsForCurrentUser(BallotPaperDto result, bool apsImplementsEVR)
        {
            var principal = SecurityHelper.GetLoggedUser();
            var isAdmin = principal.IsInRole("Administrator");
            var isPollingOfficer = principal.Identity.HasPermission(SaisePermissions.ResultsEdit);
            if (result.Status == BallotPaperStatus.New)
            {
                result.AllowSubmitResults = ((!(isPollingOfficer ^ apsImplementsEVR)) || isAdmin);
            }
            else
            {
                if (!principal.Identity.HasPermission(SaisePermissions.AllowElectionResultsVerification))
                {
                    return;
                }

                result.AllowSubmitResults = ((!(isPollingOfficer ^ apsImplementsEVR)) || isAdmin) &&
                                            (result.Status != BallotPaperStatus.Approved);
                if (principal.Identity.HasPermission(SaisePermissions.AllowElectionResultsVerification))
                {
                    result.AllowSubmitConfirmation = (result.Status != BallotPaperStatus.Approved) && (isAdmin);
                }
            }
        }

        private CompetitorResultDto MapElectionResult(ElectionResult electionResult, AssignedPollingStation assignedPollingStation)
        {

            var result = new CompetitorResultDto
            {
                PoliticalPartyId = electionResult.PoliticalParty?.Id,
                CandidateId = electionResult.Candidate?.Id,
                ElectionResultId = electionResult.Id,
                BallotOrder = electionResult.BallotOrder,
                BallotCount = electionResult.BallotCount,
                PartyStatus = GetPartyStatus(electionResult.PoliticalParty, assignedPollingStation)
            };

            SetCompetitorName(electionResult, ref result);

            return result;
        }

        private PoliticalPartyStatus GetPartyStatus(ElectionCompetitor politicalParty, AssignedPollingStation assignedPollingStation)
        {
            var result = politicalParty.Status;
            var election = assignedPollingStation.ElectionRound.Election;
            var electionOverrides = politicalParty.StatusOverrides.Where(x => x.ElectionRound.Election.Id == election.Id).ToList();
            PoliticalPartyStatusOverride overridenStatus = null;

            if (electionOverrides.Count > 0)
            {
                if (election.IsSubTypeOfLocalElection())
                {
                    if ((election.Type.Id == ElectionType.Local_ConsilieriLocal) ||
                        (election.Type.Id == ElectionType.Local_PrimarLocal))
                    {
                        overridenStatus = electionOverrides.FirstOrDefault(
                                x => x.AssignedCircumscription.Id == assignedPollingStation.AssignedCircumscription.Id);
                    }
                    else
                    {
                        overridenStatus = electionOverrides.FirstOrDefault(
                                x => x.AssignedCircumscription.Id == assignedPollingStation.AssignedCircumscription.Id /*&& x.AssignedCircumscription.Region.Id == -1*/);
                    }
                }
                else
                {
                    overridenStatus = electionOverrides.FirstOrDefault(x => x.AssignedCircumscription.Id == -1 /*&& x.AssignedCircumscription.Region.Id == -1*/);
                }
            }

            return overridenStatus != null ? overridenStatus.Status : result;
        }

        private void SetCompetitorName(ElectionResult electionResult, ref CompetitorResultDto result)
        {
            var candidateName = "";

            if (electionResult.Candidate != null && electionResult.Candidate.Id > 0)
            {
                candidateName = string.Format("{0} {1}", electionResult.Candidate.LastNameRo,
                    electionResult.Candidate.NameRo);
            }

            result.IsIndependent = electionResult.PoliticalParty.IsIndependent;
            result.PoliticalPartyCode = electionResult.PoliticalParty.Code;

            result.PoliticalPartyName = electionResult.PoliticalParty.NameRo;
            result.CandidateName = candidateName;
        }

        public void TransferEDayData(string serverIpAddress, string remoteUserName, string remotePassword)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["AmdarisConnectionString"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[MoveDataToRepository]", cn))
                {
                    cmd.CommandTimeout = 30000;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@serverName", SqlDbType.VarChar)
                    {
                        Value = "SAISE.RepLink",
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@serverIpAddress", SqlDbType.VarChar)
                    {
                        Value = serverIpAddress,
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@localUsername", SqlDbType.VarChar)
                    {
                        Value = "sa",
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@remoteUsername", SqlDbType.VarChar)
                    {
                        Value = remoteUserName,
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@remotePassword", SqlDbType.VarChar)
                    {
                        Value = remotePassword,
                        Direction = ParameterDirection.Input
                    });

                    SqlParameter execStatus = new SqlParameter("@execStatus", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(execStatus);

                    SqlParameter execMsg = new SqlParameter("@execMsg", SqlDbType.VarChar)
                    {
                        Size = 5000,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(execMsg);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }


        public List<DataTransferStage> GetDataTransferStages()
        {
            if (CheckLinkedServerExists())
            {
                ISession session = ((SaiseRepository)_saiseRepository).SessionFactory.GetCurrentSession();
                IQuery query = session.CreateSQLQuery(@"exec GetDataTransferStages @serverName=:serverName, @execStatus=:execStatus OUTPUT, @execMsg=:execMsg OUTPUT");
                query.SetParameter("serverName", "SAISE.RepLink", NHibernateUtil.String);
                query.SetParameter("execStatus", "", NHibernateUtil.String);
                query.SetParameter("execMsg", "", NHibernateUtil.String);
                var result = query.SetResultTransformer(Transformers.AliasToBean<DataTransferStage>()).List<DataTransferStage>();
                return result.ToList();
            }
            return null;
        }


        public bool CheckLinkedServerExists()
        {
            ISession session = ((SaiseRepository)_saiseRepository).SessionFactory.GetCurrentSession();
            IQuery query = session.CreateSQLQuery(@"
                        SET NOCOUNT ON;
                        DECLARE @retval int = 0,
                                @sysservername sysname;
                        BEGIN TRY
                            SELECT  @sysservername = CONVERT(sysname, :serverName);
                            EXEC @retval = sys.sp_testlinkedserver @sysservername;
                            SELECT 1;
                        END TRY
                        BEGIN CATCH
                            SELECT 0;
                        END CATCH;");
            query.SetParameter("serverName", "SAISE.RepLink", NHibernateUtil.String);
            var result = query.UniqueResult();
            return result.ToString() == "0" ? false : true;
        }

        public bool AproveBallotPaper(List<long> model)
        {
            try
            {
                var userId = SecurityHelper.GetLoggedUserId();
                const string baseQry = @"
               update BallotPaper set Status = :status , ConfirmationUserId = :UserId  , IsResultsConfirmed=:Confirmation, ConfirmationDate = :date   where Status=1 and BallotPaperId IN(:bpList); ";

                var queryBuilder = _saiseRepository.CreateSqlStringBuilder(baseQry, null)
                    .SetParameter("status", 2)
                    .SetParameter("UserId", userId)
                    .SetParameter("Confirmation", true)
                    .SetParameter("date", DateTime.Now)
                    .SetParameterList("bpList", model);
                queryBuilder.ToSqlQuery().ExecuteUpdate();

                const string baseQryDocument = @"
               update Documents set StatusId = :status , ConfirmationUserId = :UserId  , IsResultsConfirmed=:Confirmation, ConfirmationDate = :date   where StatusId=1 and BallotPaperId IN(:bpList); ";

                var queryBuilderDocument = _saiseRepository.CreateSqlStringBuilder(baseQryDocument, null)
                    .SetParameter("status", 2)
                    .SetParameter("UserId", userId)
                    .SetParameter("Confirmation", true)
                    .SetParameter("date", DateTime.Now)
                    .SetParameterList("bpList", model);
                queryBuilderDocument.ToSqlQuery().ExecuteUpdate();

                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }
        private async Task<ElectionRound> GetElectionRound(long electionRoundId)
        {
            return await _saiseRepository.QueryAsync<ElectionRound>(z => z.FirstOrDefault(x => x.Election.Id == electionRoundId));
        }
        private async Task<PollingStation> GetPollingStation(long? pollingstationId)
        {
            return pollingstationId != null
                ? await _saiseRepository.QueryAsync<PollingStation>(z => z.FirstOrDefault(x => x.Id == pollingstationId))
                : null;
        }

        #region ElectionResultsConsolidation
        public async Task<List<UnconfirmedPollingStationsDto>> GetUnconfirmedBallotPapers(long assignedCircumscriptionId, long electionRoundId)
        {
            var pollingStations = await _saiseRepository.QueryAsync<AssignedPollingStation>(ap =>
                    ap.Where(x => x.AssignedCircumscription.Id == assignedCircumscriptionId).ToList());

            var pollingStationIds = pollingStations.Select(x => x.PollingStation.Id).ToList();
            var ballotPapers = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                    z.Where(x => x.ElectionRound.Election.Id == electionRoundId && pollingStationIds.Contains(x.PollingStation.Id) && x.Status != BallotPaperStatus.Approved).ToList());
            ballotPapers = ballotPapers.Where(x => x != null).ToList();

            return ballotPapers.Select(x => new UnconfirmedPollingStationsDto
            {
                PollingStationNumber = x.PollingStation.Number,
                Locality = x.PollingStation.Region.GetFullName(),
                PollingStationName = x.PollingStation.NameRo
            }).ToList();
        }


        public async Task<BallotPaperConsolidationDataExtendedDto> GetElectionResultsByCircumscription(long assignedCircumscriptionId, long electionId, long templateNameId)
        {
            try
            {
                var pollingStations = await _saiseRepository.QueryAsync<AssignedPollingStation>(ap =>
                    ap.Where(x => x.AssignedCircumscription.Id == assignedCircumscriptionId).ToList());

                var pollingstationIds = pollingStations.Select(x => x.PollingStation.Id).ToList();

                var ballotPaperDtoList = await GetBallotPaperAsync(electionId, pollingstationIds);

                //TO DO fix temporary solution
                ballotPaperDtoList = ballotPaperDtoList.Where(bp => bp != null).ToList();

                var ballotPaperUnconfirmed = ballotPaperDtoList.Where(x => x.Status != BallotPaperStatus.Approved).ToList();
                //TO DO : Uncomment after testing and feature created by Frontend to catch 400 code related to unconfirmed results
                //if (ballotPaperUnconfirmed.Any())
                //{
                //    return null;
                //}

                return await ConsolidateBallotPapers(ballotPaperDtoList, assignedCircumscriptionId, electionId, templateNameId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SaveUpdateResult> SaveUpdateConsolidatedResults(BallotPaperConsolidationDataExtendedDto ballotPaperDto, long templateNameId)
        {
            //Map BallotPaperConsolidationDataDto to another object without AlreadySent property
            var standardDto = MapExtendedToStandardDto(ballotPaperDto);

            var result = await _documentsBll.SaveConsolidatedResultsDocument(standardDto, templateNameId);
            return result;
        }

        private BallotPaperConsolidationDataDto MapExtendedToStandardDto(BallotPaperConsolidationDataExtendedDto extendedDto)
        {
            var standardDto = new BallotPaperConsolidationDataDto
            {
                AssignedCircumscriptionId = extendedDto.AssignedCircumscriptionId,
                RegisteredVoters = extendedDto.RegisteredVoters,
                Supplementary = extendedDto.Supplementary,
                BallotsIssued = extendedDto.BallotsIssued,
                BallotsCasted = extendedDto.BallotsCasted,
                DifferenceIssuedCasted = extendedDto.DifferenceIssuedCasted,
                BallotsValidVotes = extendedDto.BallotsValidVotes,
                BallotsReceived = extendedDto.BallotsReceived,
                BallotsUnusedSpoiled = extendedDto.BallotsUnusedSpoiled,
                BallotsSpoiled = extendedDto.BallotsSpoiled,
                BallotsUnused = extendedDto.BallotsUnused,
                OpeningVotersCount = extendedDto.OpeningVotersCount,
                CompetitorResults = new List<CompetitorResultDto>(extendedDto.CompetitorResults)
            };

            return standardDto;
        }
        private async Task<List<BallotPaperDto>> GetBallotPaperAsync(long electionId, List<long> pollingStationIds)
        {
            // Fetch all related BallotPapers and AssignedPollingStations using the list of pollingStationIds
            var ballotPapers = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.Where(x => x.ElectionRound.Election.Id == electionId && pollingStationIds.Contains(x.PollingStation.Id)).ToList());

            var assignedPollingStations = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.Where(x => x.ElectionRound.Election.Id == electionId && pollingStationIds.Contains(x.PollingStation.Id)).ToList());

            // Map the BallotPapers to BallotPaperDtos using the associated AssignedPollingStations
            var ballotPaperDtos = new List<BallotPaperDto>();

            foreach (var ballotPaper in ballotPapers)
            {
                var associatedPollingStation = assignedPollingStations.FirstOrDefault(aps => aps.PollingStation.Id == ballotPaper.PollingStation.Id);
                if (associatedPollingStation != null)
                {
                    ballotPaperDtos.Add(MapBallotPaper(ballotPaper, associatedPollingStation));
                }
            }

            return ballotPaperDtos;
        }
        private async Task<BallotPaperConsolidationDataExtendedDto> ConsolidateBallotPapers(List<BallotPaperDto> ballotPapers, long assignedCircumscriptionId, long electionId, long templateNameId)
        {

            var consolidatedData = new BallotPaperConsolidationDataExtendedDto
            {
                AssignedCircumscriptionId = assignedCircumscriptionId,
                RegisteredVoters = ballotPapers.Sum(bp => bp.RegisteredVoters),
                Supplementary = ballotPapers.Sum(bp => bp.Supplementary),
                BallotsIssued = ballotPapers.Sum(bp => bp.BallotsIssued),
                BallotsCasted = ballotPapers.Sum(bp => bp.BallotsCasted),
                DifferenceIssuedCasted = ballotPapers.Sum(bp => bp.DifferenceIssuedCasted),
                BallotsValidVotes = ballotPapers.Sum(bp => bp.BallotsValidVotes),
                BallotsReceived = ballotPapers.Sum(bp => bp.BallotsReceived),
                BallotsUnusedSpoiled = ballotPapers.Sum(bp => bp.BallotsUnusedSpoiled),
                BallotsSpoiled = ballotPapers.Sum(bp => bp.BallotsSpoiled),
                BallotsUnused = ballotPapers.Sum(bp => bp.BallotsUnused),
                OpeningVotersCount = ballotPapers.Sum(bp => bp.OpeningVotersCount),
                AlreadySent = await DocumentAlreadySent(assignedCircumscriptionId, electionId, templateNameId)
            };

            var groupedResults = ballotPapers
                .SelectMany(bp => bp.CompetitorResults)
                .GroupBy(cr => cr.BallotOrder)
                .Select(group => new CompetitorResultDto
                {
                    BallotOrder = group.First().BallotOrder,
                    PoliticalPartyId = group.First().PoliticalPartyId,
                    PoliticalPartyCode = group.First().PoliticalPartyCode,
                    PoliticalPartyName = group.First().PoliticalPartyName,
                    CandidateId = group.First().CandidateId,
                    CandidateName = group.First().CandidateName,
                    IsIndependent = group.First().IsIndependent,
                    BallotCount = group.Sum(cr => cr.BallotCount),
                    PartyStatus = group.First().PartyStatus
                })
                .OrderBy(cr => cr.BallotOrder)
                .ToList();

            consolidatedData.CompetitorResults = groupedResults;

            return consolidatedData;
        }

        private async Task<bool> DocumentAlreadySent(long assignedCircumscriptionId, long electionId, long templateNameId)
        {
            var documentEntity = await _saiseRepository.QueryAsync<Document>(z =>
                 z.FirstOrDefault(x => x.AssignedCircumscription.Id == assignedCircumscriptionId
                 && x.ElectionRound.Election.Id == electionId
                 && x.Template.TemplateName.Id == templateNameId));
            if (documentEntity == null)
            {
                return false;
            }
            return true;

        }
        #endregion

        #region Helpers
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = input.ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
        }
        #endregion
    }
}
