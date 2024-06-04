using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.BLL.Dto.Concurents;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using NHibernate.Util;

namespace CEC.SAISE.BLL.Impl
{
    public class ConcurentsBll : IConcurentsBll
    {
        private readonly ISaiseRepository _saiseRepository;

        public ConcurentsBll(ISaiseRepository saiseRepository)
        {
            _saiseRepository = saiseRepository;
        }

        public Election GetElection(long electionId)
        {
            return _saiseRepository.Get<Election>(electionId);
        }

        public IList<PoliticalPartyDto> GetAllParties(DelimitationDto delimitation)
        {
            PoliticalPartyDto partyDto = null;
            ElectionCompetitor ppAlias = null;
            ElectionCompetitor ppAlias1 = null;
            ElectionCompetitorMember candidateAlias = null;
            AssignedCircumscription cir = null;
            AssignedCircumscription cir1 = null;
            AssignedPollingStation ap = null;
            PollingStation p = null;
            ElectionRound er = null;
            ElectionRound er1 = null;
            Election e = null;
            Election e1 = null;
            Region r = null;
            Region r1 = null;
            var candidatesCountSubQuery = QueryOver.Of<ElectionCompetitorMember>(() => candidateAlias).
                JoinAlias(() => candidateAlias.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => e)
                .JoinAlias(() => candidateAlias.AssignedCircumscription, () => cir)          
                .Where(x => e.Id == delimitation.ElectionId &&
                            cir.Id == delimitation.GetCircumscriptionId() &&
                            candidateAlias.ElectionCompetitor.Id == ppAlias1.Id)
                .ToRowCountInt64Query();

            var parties = _saiseRepository.QueryOver<ElectionCompetitor>(() => ppAlias1)
                .JoinAlias(() => ppAlias1.ElectionRound, () => er1)
                .JoinAlias(() => er1.Election, () => e1)
                .JoinAlias(() => ppAlias1.AssignedCircumscription, () => cir)             
                .Where(x => (e1.Id == delimitation.ElectionId) &&
                (cir.Id == delimitation.GetCircumscriptionId()) && x.Id > 0);

            var t = parties.SelectList(list => list
                  .Select(x => x.Id).WithAlias(() => partyDto.Id)
                  .Select(x => x.Code).WithAlias(() => partyDto.Code)
                  .Select(x => x.NameRo).WithAlias(() => partyDto.NameRo)
                  .Select(x => x.NameRu).WithAlias(() => partyDto.NameRu)
                  .Select(x => x.DateOfRegistration).WithAlias(() => partyDto.DateOfRegistration)
                  .Select(x => x.Status).WithAlias(() => partyDto.Status)
                  .Select(x => x.BallotOrder).WithAlias(() => partyDto.BallotOrder)
                  .Select(x => x.IsIndependent).WithAlias(() => partyDto.IsIndependent)
                  .SelectSubQuery(candidatesCountSubQuery).WithAlias(() => partyDto.CandidateCount)
               )
               .OrderBy(() => ppAlias1.PartyOrder).Desc
               .ThenBy(() => ppAlias1.Code).Asc
               .TransformUsing(Transformers.AliasToBean<PoliticalPartyDto>())
               .List<PoliticalPartyDto>();

          //  SetFirstCandidates(delimitation, t);
            return t;
        }

        public IList<PoliticalPartyDto> GetAllocatedParties(DelimitationDto delimitation)
        {
            PoliticalPartyDto partyDto = null;
            ElectionCompetitor ppAlias = null;
            ElectionCompetitorMember candidateAlias = null;
            ElectionCompetitor pp2Alias = null;
            BallotPaper bpAlias = null;
            AssignedPollingStation ap = null;
            PollingStation psAlias = null;
            PollingStation ps1Alias = null;
            BallotPaper bp1Alias = null;
            Region rAlias = null;
            Region r1Alias = null;
            Region r2Alias = null;
            ElectionRound er = null;
            ElectionRound er1 = null;
            ElectionRound er2 = null;
            Election e = null;
            Election e1 = null;
            Election e2 = null;
            AssignedCircumscription cir = null;
            var candidatesCountSubQuery = QueryOver.Of<ElectionCompetitorMember>(() => candidateAlias)
                .JoinAlias(() => candidateAlias.ElectionRound, () => er2)
                .JoinAlias(() => er2.Election, () => e2)
                .JoinAlias(() => candidateAlias.AssignedCircumscription, () => cir)               
                .Where(x => e2.Id == delimitation.GetElectionId() &&
                             cir.Id == delimitation.GetCircumscriptionIdOrTBD() &&
                            candidateAlias.ElectionCompetitor.Id == pp2Alias.Id)
                .ToRowCountInt64Query();

            var partiesQuery = QueryOver.Of<ElectionResult>()
                .JoinAlias(x => x.PoliticalParty, () => ppAlias)
                .JoinAlias(x => x.BallotPaper, () => bpAlias)
                .JoinAlias(() => bpAlias.PollingStation, () => psAlias)
                .JoinAlias(() => bpAlias.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => e)
                .JoinAlias(() => psAlias.Region, () => rAlias)
                .Where(x => e.Id == delimitation.GetElectionId() &&
                            x.PoliticalParty.Id > 0);

            var ballotOrderQuery = QueryOver.Of<ElectionResult>()
                .JoinAlias(x => x.BallotPaper, () => bp1Alias)
                .JoinAlias(() => bp1Alias.PollingStation, () => ps1Alias)
                .JoinAlias(() => bp1Alias.ElectionRound, () => er1)
                .JoinAlias(() => er1.Election, () => e1)
                .JoinAlias(() => ps1Alias.Region, () => r1Alias)
                .Where(x => e1.Id == delimitation.GetElectionId() &&
                            x.PoliticalParty.Id > 0 &&
                            x.PoliticalParty.Id == pp2Alias.Id);

            //var statusQuery = QueryOver.Of<PoliticalPartyStatusOverride>()
            //	.JoinAlias(x => x.ElectionCompetitor, ()=> pp2Alias)


            //if (delimitation.CircumscriptionId.HasValue && delimitation.RegionId.HasValue)
            //{
            //    partiesQuery = partiesQuery.Where(x =>
            //        ppAlias.AssignedCircumscription.Id == delimitation.GetCircumscriptionId());
               

            //    ballotOrderQuery = ballotOrderQuery.Where(x =>
            //       ppAlias.AssignedCircumscription.Id == delimitation.GetCircumscriptionId());
             
            //}

            ballotOrderQuery =
                ballotOrderQuery.Select(Projections.Max(Projections.Property<ElectionResult>(x => x.BallotOrder)));

            partiesQuery = partiesQuery.Select(Projections.Distinct(Projections.Property(() => ppAlias.Id)));

            var parties = _saiseRepository.QueryOver(() => pp2Alias)
                .Where(Subqueries.WhereProperty<ElectionCompetitor>(x => x.Id).In(partiesQuery))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => partyDto.Id)
                    .Select(x => x.Code).WithAlias(() => partyDto.Code)
                    .Select(x => x.NameRo).WithAlias(() => partyDto.NameRo)
                    .Select(x => x.NameRu).WithAlias(() => partyDto.NameRu)
                    .Select(x => x.DateOfRegistration).WithAlias(() => partyDto.DateOfRegistration)
                    .Select(x => x.Status).WithAlias(() => partyDto.Status)
                    //.Select(x => x.BallotOrder).WithAlias(() => partyDto.BallotOrder)
                    .Select(x => x.IsIndependent).WithAlias(() => partyDto.IsIndependent)
                    .SelectSubQuery(candidatesCountSubQuery).WithAlias(() => partyDto.CandidateCount)
                    .SelectSubQuery(ballotOrderQuery).WithAlias(() => partyDto.BallotOrder)
                )
                .OrderBy(x => x.BallotOrder).Asc
                .TransformUsing(Transformers.AliasToBean<PoliticalPartyDto>())
                .List<PoliticalPartyDto>();

            SetOverrideStatus(delimitation, parties);
           // SetFirstCandidates(delimitation, parties);
            return parties;
        }

        private void SetOverrideStatus(DelimitationDto delimitation, IList<PoliticalPartyDto> parties)
        {
            var overridenStatuses = _saiseRepository.Query<PoliticalPartyStatusOverride>()
                .Where(x => x.ElectionRound.Election.Id == delimitation.ElectionId
                            && x.AssignedCircumscription.Id == delimitation.GetCircumscriptionId()                           
                            ).ToList();

            foreach (var status in overridenStatuses)
            {
                var party = parties.FirstOrDefault(x => x.Id == status.PoliticalParty.Id);
                if (party != null)
                {
                    party.Status = status.Status;
                }
            }
        }

        public IList<CandidateDto> GetCandidatesForParty(DelimitationDto delimitation, long partyId)
        {
            return _saiseRepository.Query<ElectionCompetitorMember>()
                .Where(x => x.ElectionRound.Election.Id == delimitation.GetElectionId() &&
                            x.ElectionCompetitor.Id == partyId
                          /*  && x.AssignedCircumscription.Region.Id == delimitation.GetRegionIdOrTBD()*/)
                .OrderBy(x => x.CompetitorMemberOrder)
                .ThenBy(x => x.Id)
                .Select(x => new CandidateDto
                {
                    Id = x.Id,
                    CandidateRegionRelId = x.AssignedCircumscription.Id,
                    PoliticalPartyId = x.ElectionCompetitor.Id,
                    Idnp = x.Idnp,
                    NameRo = x.NameRo,
                    NameRu = x.NameRu,
                    LastNameRo = x.LastNameRo,
                    LastNameRu = x.LastNameRu,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Occupation = x.Occupation,
                    OccupationRu = x.OccupationRu,
                    Workplace = x.Workplace,
                    WorkplaceRu = x.WorkplaceRu,
                    Status = x.Status,
                    CandidateOrder = x.CompetitorMemberOrder,
                }).ToList();
        }

        public bool UpdateCandidateStatus(long candidateId, CandidateStatus status)
        {
            var candidate = _saiseRepository.Get<ElectionCompetitorMember>(candidateId);

            if (candidate == null)
            {
                return false;
            }

            candidate.Status = status;
            candidate.EditDate = DateTime.Now;
            candidate.EditUser = _saiseRepository.LoadProxy<SystemUser>(SecurityHelper.GetLoggedUserId());
            _saiseRepository.SaveOrUpdate(candidate);

            return true;
        }

        public bool DeleteCandidates(List<DeleteCandidateDto> itemsToDelete)
        {
            //todo: double check if any operations with BallotPaper and/or ElectionResults are required.
            const string deleteCandidateVillageRelQry =
                    @"delete cvr
                    from CandidateVillageRel cvr
                    inner join ElectionCompetitorMember c on cvr.CandidateId = c.CandidateId
                    inner join ElectionCompetitor pp on c.PoliticalPartyId = pp.PoliticalPartyId
                    where pp.IsIndependent=0 and pp.PoliticalPartyId in (:ppIds) and c.CandidateId in (:ccIds);";
            const string deleteCandidatesQry =
                    @"delete cc
                    from ElectionCompetitorMember cc
                    inner join ElectionCompetitor pp on cc.PoliticalPartyId = pp.PoliticalPartyId
                    where pp.IsIndependent=0 and cc.PoliticalPartyId in (:ppIds) and cc.CandidateId in (:ccIds)";

            var query = _saiseRepository.CreateSqlStringBuilder(deleteCandidateVillageRelQry, null)
                .Sql(deleteCandidatesQry)
                .SetParameterList("ppIds", itemsToDelete.Select(x => x.PoliticalPartyId).Distinct().ToList())
                .SetParameterList("ccIds", itemsToDelete.Select(x => x.CandidateId).Distinct().ToList())
                .ToSqlQuery();

            query.ExecuteUpdate();

            return true;
        }

        public bool UpdateCandidatesOrder(List<UpdateCandidateOrderDto> itemsToUpdate)
        {
            const string updateCandidateOrderQry =
                "update CandidateVillageRel set CandidateOrder = :order where CandidateVillageRelId = :cvrId";
            foreach (var item in itemsToUpdate.Where(x => x.CandidateRegionRelId.HasValue))
            {
                var query = _saiseRepository.CreateSqlStringBuilder(updateCandidateOrderQry, null)
                    .SetParameter("order", item.CandidateOrder)
                    .SetParameter("cvrId", item.CandidateRegionRelId.Value)
                    .ToSqlQuery()
                    .ExecuteUpdate();
            }

            return true;
        }

        public async Task<PersonalDataResponse> RequestPersonalData(string idnp)
        {
            PersonalDataResponse result = new PersonalDataResponse();
            var srvServiceAddress = ConfigurationManager.AppSettings["SRV.ServiceAddress"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(srvServiceAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(idnp);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<PersonalDataResponse>();

                    if (data != null)
                    {
                        data.Gender = data.Gender - 1;
                        data.Success = true;
                        result = data;
                    }
                }
            }

            return result;
        }

        public void SaveUpdateParty(DelimitationDto delimitation, PoliticalPartyDto partyToUpdate, byte[] logo)
        {

            var election = _saiseRepository.Query<ElectionRound>().FirstOrDefault(x => x.Election.Id == delimitation.ElectionId);
            if (delimitation.CircumscriptionId != null)
            {
                var assignCirc = _saiseRepository.Get<AssignedCircumscription>(delimitation.CircumscriptionId.Value);

                if (partyToUpdate == null)
                {
                    throw new ArgumentNullException("partyToUpdate");
                }

                var userId = SecurityHelper.GetLoggedUserId();
                var user = _saiseRepository.LoadProxy<SystemUser>(userId);

                ElectionCompetitor politicalParty = null;
                var newCreatedParty = partyToUpdate.Id == 0;
                politicalParty = newCreatedParty ? new ElectionCompetitor() : _saiseRepository.Get<ElectionCompetitor>(partyToUpdate.Id);

                politicalParty.Code = partyToUpdate.Code;
                politicalParty.NameRo = partyToUpdate.NameRo;
                politicalParty.NameRu = partyToUpdate.NameRu;
                politicalParty.Status = partyToUpdate.Status;
                politicalParty.DateOfRegistration = partyToUpdate.DateOfRegistration;
                politicalParty.IsIndependent = partyToUpdate.IsIndependent;
                politicalParty.BallotOrder = partyToUpdate.BallotOrder;
                politicalParty.EditDate = DateTime.Now;
                politicalParty.EditUser = user;
                politicalParty.ElectionRound = election;
                politicalParty.AssignedCircumscription = assignCirc;
                if (logo != null)
                {
                    politicalParty.ColorLogo = logo;
                }

                _saiseRepository.SaveOrUpdate(politicalParty);

                if (partyToUpdate.IsIndependent)
                {
                    SaveUpdateCandidate(delimitation, politicalParty, partyToUpdate.CandidateData);
                }

                if (newCreatedParty)
                {
                    var electionId = (partyToUpdate.IsIndependent)
                        ? delimitation.GetElectionId()
                        : -1;

                    var villageId = (delimitation.ElectionIsLocal && partyToUpdate.IsIndependent)
                        ? delimitation.GetRegionId()
                        : -1;

                    var defaultElection = _saiseRepository.LoadProxy<Election>(electionId);
                    var defaultVillage = _saiseRepository.LoadProxy<Region>(villageId);
                    //var partyRelation = new PartyVillageElectionRel
                    //{
                    //    ElectionCompetitor = politicalParty,
                    //    Election = defaultElection,
                    //    Village = defaultVillage
                    //};
                    _saiseRepository.SaveOrUpdate(politicalParty);
                }
            }

            //return MapPoliticalParty(politicalParty);
        }

        public void SaveUpdateCandidate(DelimitationDto delimitation, CandidateDto candidateToUpdate)
        {
            var politicalParty = _saiseRepository.Get<ElectionCompetitor>(candidateToUpdate.PoliticalPartyId);
            SaveUpdateCandidate(delimitation, politicalParty, candidateToUpdate);
        }

        public void FireAllocation(DelimitationDto delimitation, IList<AllocationItemDto> itemsToAllocate)
        {
            var election = _saiseRepository.Get<Election>(delimitation.GetElectionId());
            if (!election.IsSubTypeOfLocalElection())
            {
                AllocateForNonLocals(delimitation, itemsToAllocate);
                return;
            }

            if ((election.Type.Id == ElectionType.Local_ConsilieriLocal) ||
                (election.Type.Id == ElectionType.Local_PrimarLocal))
            {
                AllocateForVillage(delimitation, itemsToAllocate);
            }
            else
            {
                AllocateForDistrict(delimitation, itemsToAllocate);
            }
        }

        public IList<CandidateConflictDto> CheckPersonAllocation(DelimitationDto delimitationDto, CandidateDto candidateToUpdate)
        {
            var currentElection = _saiseRepository.Get<Election>(delimitationDto.ElectionId);

            var electionIds = _saiseRepository.Query<Election>()
                .Where(x => x.DateOfElection == currentElection.DateOfElection)
                .Select(x => x.Id).ToArray();

            var candidateList = _saiseRepository.Query<ElectionCompetitorMember>()
                .Where(x => electionIds.Contains(x.ElectionRound.Id)
                            && x.AssignedCircumscription.Id == delimitationDto.GetCircumscriptionId()
                            && x.Idnp == candidateToUpdate.Idnp)
                .Select(x => new CandidateConflictDto()
                {
                    ElectionRound = x.ElectionRound,
                    PoliticalParty = x.ElectionCompetitor,
                    AssignedCircumscription = x.AssignedCircumscription
                })
                .ToList();

            return candidateList;
        }

        public bool DeleteConcurents(DelimitationDto delimitation, IEnumerable<long> itemsToDelete)
        {
            var election = _saiseRepository.Get<Election>(delimitation.GetElectionId());

            var electionResultsQuery = _saiseRepository.Query<ElectionResult>()
                .Where(x => x.BallotPaper.ElectionRound.Election.Id == delimitation.ElectionId
                            && itemsToDelete.Contains(x.PoliticalParty.Id));

            IList<ElectionResult> electionResults = null;

            if (!election.IsSubTypeOfLocalElection())
            {
                electionResults = electionResultsQuery.ToList();
                RemoveElectionResults(electionResults);
                return true;
            }

            if ((election.Type.Id == ElectionType.Local_ConsilieriLocal) ||
                (election.Type.Id == ElectionType.Local_PrimarLocal))
            {
                electionResults = electionResultsQuery.Where(x => x.BallotPaper.PollingStation.Region.Id == delimitation.RegionId).ToList();

                RemoveElectionResults(electionResults);
            }
            else
            {
                electionResults = electionResultsQuery.Where(x => x.BallotPaper.PollingStation.Region.Id == delimitation.CircumscriptionId).ToList();

                RemoveElectionResults(electionResults);
            }
            return true;
        }

        public void ExportCandidatesToExcel(out Stream stream, out string fileName, DelimitationDto delimitation)
        {
            stream = new MemoryStream();
            fileName = GenerateFileName(delimitation);
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                CreateXlDocStructure(document, delimitation);
            }
            stream.Position = 0;
        }

        public bool UpdatePartyStatus(long partyId, PoliticalPartyStatus status)
        {
            var party = _saiseRepository.Get<ElectionCompetitor>(partyId);

            if (party == null)
            {
                throw new ArgumentNullException("partyId", "partyId doesn't reference to any ElectionCompetitor");
            }

            party.Status = status;

            _saiseRepository.SaveOrUpdate(party);

            return true;
        }

        public bool OverridePartyStatus(DelimitationDto delimitationDto, long partyId, PoliticalPartyStatus status)
        {
            var election = _saiseRepository.Get<Election>(delimitationDto.ElectionId);
            var electionRound = _saiseRepository.Query<ElectionRound>().FirstOrDefault(x => x.Election.Id == delimitationDto.ElectionId);
            var party = _saiseRepository.Get<ElectionCompetitor>(partyId);
            var userId = SecurityHelper.GetLoggedUserId();
            var user = _saiseRepository.LoadProxy<SystemUser>(userId);

            long districtId = -1;
            long villageId = -1;

            if (election.IsSubTypeOfLocalElection())
            {
                if (election.Type.Id == ElectionType.Local_PrimarLocal ||
                    election.Type.Id == ElectionType.Local_ConsilieriLocal)
                {
                    districtId = delimitationDto.GetCircumscriptionIdOrTBD();
                    villageId = delimitationDto.GetRegionIdOrTBD();
                }
                else
                {
                    districtId = delimitationDto.GetCircumscriptionIdOrTBD();
                }
            }

            var newStatus = new PoliticalPartyStatusOverride
            {
                 AssignedCircumscription = _saiseRepository.LoadProxy<AssignedCircumscription>(districtId),               
                ElectionRound = electionRound,
                Status = status,
                EditDate = DateTime.Now,
                EditUser = user
            };

            party.OverrideStatus(newStatus);
            _saiseRepository.SaveOrUpdate(party);

            return true;
        }

        private string GenerateFileName(DelimitationDto delimitation)
        {
            const string toate = "Toate";

            var election = _saiseRepository.Get<Election>(delimitation.ElectionId);
            var electionName = election.Comments.Replace(' ', '_');
            var villageName = (delimitation.RegionId.HasValue)
                ? _saiseRepository.Get<Region>(delimitation.RegionId.Value).Name.Replace(' ', '_')
                : toate;
            var districtName = (delimitation.CircumscriptionId.HasValue)
                ? _saiseRepository.Get<AssignedCircumscription>(delimitation.CircumscriptionId.Value).NameRo.Replace(' ', '_')
                : toate;


            return string.Format("{0}_{1}_{2}-{3}.xlsx",
                electionName,
                districtName,
                villageName,
                DateTime.Now.ToString("yyyy-mm-dd_HH-mm-ss"));
        }

        private void CreateXlDocStructure(SpreadsheetDocument document, DelimitationDto delimitation)
        {
            //create workbook part
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            var allocatedParties = GetAllocatedParties(delimitation);

            var sheet = CreateMainWorksheet(workbookPart, allocatedParties);
            sheet.SheetId = new UInt32Value((uint)sheets.ChildElements.Count + 1);
            sheets.Append(sheet);

            foreach (var allocatedParty in allocatedParties)
            {
                var sheetName = (allocatedParty.IsIndependent) ? string.Format("{0}_{1}", allocatedParty.Code, allocatedParty.Id) : allocatedParty.Code;

                var candidates = GetCandidatesForParty(delimitation, allocatedParty.Id);
                sheet = CreateCandidateWorksheet(workbookPart, sheetName, candidates);
                sheet.SheetId = new UInt32Value((uint)sheets.ChildElements.Count + 1);
                sheets.Append(sheet);
            }

            workbookPart.Workbook.Save();
        }

        private Sheet CreateMainWorksheet(WorkbookPart workbookPart, IList<PoliticalPartyDto> allocatedParties)
        {
            //create worksheet part, and add it to the sheets collection in workbook
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                Name = "General"
            };

            var writer = OpenXmlWriter.Create(worksheetPart);

            var headers = new List<string>() { "Ordinea", "Cod", "DenumireRo", "DenumireRu", "Statut", "Numarul de candidati" };
            writer.WriteStartElement(new Worksheet());
            writer.WriteStartElement(new SheetData());

            WriteRow(headers, writer);

            foreach (var allocatedParty in allocatedParties)
            {
                WriteRow(new object[]{
                    allocatedParty.BallotOrder,
                    allocatedParty.Code,
                    allocatedParty.NameRo,
                    allocatedParty.NameRu,
                    PoliticalPartyStatusExtension.GetEnumDescription(allocatedParty.Status),
                    allocatedParty.CandidateCount
                }, writer);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();

            return sheet;
        }


        private Sheet CreateCandidateWorksheet(WorkbookPart workbookPart, string sheetName, IEnumerable<CandidateDto> candidates)
        {


            //create worksheet part, and add it to the sheets collection in workbook
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                Name = sheetName
            };

            var worksheet = new Worksheet();

            var columns = new Columns();
            columns.Append(new Column() { Min = 2, Max = 4, Width = 18, CustomWidth = true });
            columns.Append(new Column() { Min = 7, Max = 7, Width = 20, CustomWidth = true });

            worksheet.Append(columns);

            var writer = OpenXmlWriter.Create(worksheetPart);

            var headers = new List<string>() { "Ordinea", "NumeRo", "NumeRu", "PrenumeRo", "PrenumeRu",
                "Data naşterii",  "OcupaţiaRo", "OcupaţiaRu", "Funcția și locul de muncă (Ro)", "Funcția și locul de muncă (Ru)", "Statut" };
            writer.WriteStartElement(worksheet);
            writer.WriteStartElement(new SheetData());

            WriteRow(headers, writer);

            foreach (var candidate in candidates)
            {
                WriteRow(new Object[]
                         {
                             candidate.CandidateOrder,
                             candidate.NameRo,
                             candidate.NameRu,
                             candidate.LastNameRo,
                             candidate.LastNameRu,
                             candidate.DateOfBirth.ToString("d"),
                             candidate.Occupation,
                             candidate.OccupationRu,
                             candidate.Workplace,
                             candidate.WorkplaceRu,
                             PoliticalPartyStatusExtension.GetEnumDescription(candidate.Status)
                         }, writer);
            }


            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();




            //var columns = worksheet.GetFirstChild<Columns>();
            //if (columns != null)
            //{
            //	var nameCols = columns.Elements<Column>().Skip(1).Take(4);
            //	nameCols.ForEach(x => x.Width = 130);

            //	var occupationCols = columns.Elements<Column>().Skip(6).Take(4);
            //	occupationCols.ForEach(x => x.Width = 200);
            //}


            return sheet;

        }

        private void WriteRow(IEnumerable<object> cellValues, OpenXmlWriter writer)
        {
            writer.WriteStartElement(new Row());

            foreach (var cellValue in cellValues)
            {
                var val = new CellValue(Convert.ToString(cellValue));
                var cell = new Cell() { CellValue = val, DataType = CellValues.String };
                writer.WriteElement(cell);
            }

            writer.WriteEndElement();
        }

        private void AllocateForVillage(DelimitationDto delimitation, IList<AllocationItemDto> itemsToAllocate)
        {
            var electionround = _saiseRepository.Query<ElectionRound>()
                .FirstOrDefault(x => x.Election.Id == delimitation.ElectionId);
            if (delimitation.CircumscriptionId == null)
            {
                throw new ArgumentNullException("delimitation.CircumscriptionId");
            }

            if (delimitation.RegionId == null)
            {
                throw new ArgumentNullException("delimitation.RegionId");
            }

            #region Queries
            const string deleteElectionResultsQry = @"
                    delete er from ElectionResult er
                    where er.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId 
                            inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                            inner join Region v on ps.RegionId = v.RegionId
                            inner join AssignedCircumscription d on aps.AssignedCircumscriptionId = d.AssignedCircumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCircumscriptionId = :circumscriptionId and v.RegionId = :regionId
                    )";

            const string deleteBallotPapersQry = @"
                    delete bp from BallotPaper bp 
                    where bp.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId 
                            inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                            inner join Region v on ps.RegionId = v.RegionId
                            inner join AssignedCircumscription d on aps.AssignedCircumscriptionId = d.AssignedCircumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCircumscriptionId = :circumscriptionId and v.RegionId = :regionId
                    );";

            const string insertBallotPapersQry = @"
                    insert into BallotPaper (EntryLevel, [Type], [Status], [Description], DateOfEntry, PollingStationId, ElectionRoundId, EditUserId, EditDate)
                    select :entryLevel as EntryLevel, :bpType as [Type], :bpStatus as [Status], :descript as [Description], GETDATE() as DateOfEntry, 
		                    aps.PollingStationId as PollingStationId, :electionId as ElectionRoundId, :userId as EditUserId, GETDATE() as EditDate 
                    from AssignedPollingStation aps
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCicumscription d on aps.AssignedCicumscriptionId = d.AssignedtCicumscriptionId
                    where aps.ElectionRoundId = :electionId and d.AssignedCicumscriptionId = :circumscriptionId and v.RegionId = :regionId;";

            const string insertElectionResultsQry = @"
                    insert into ElectionResult (BallotPaperId, PoliticalPartyId, CandidateId, BallotOrder, Comments, DateOfEntry, [Status], EditUserId, EditDate)
                    select bp.BallotPaperId as BallotPaperId, :ppId as PoliticalPartyId, :candidateId as CandidateId, :bpOrder as BallotOrder, :comments as Comments, 
		                    GETDATE() as DateOfEntry, :status as [Status], :userId as EditUserId, GETDATE() as EditDate from 
                    BallotPaper bp
                    where bp.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId 
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCicumscription d on aps.AssignedCicumscriptionId = d.AssignedCicumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCicumscriptionId = :circumscriptionId and v.RegionId = :regionId
                    );";

            #endregion

            var currentUserId = SecurityHelper.GetLoggedUserId();

            var affected = _saiseRepository.CreateSqlStringBuilder(deleteElectionResultsQry, null)
                .Sql(deleteBallotPapersQry)
                .Sql(insertBallotPapersQry)
                .SetParameter("electionId", electionround?.Id)
                .SetParameter("circumscriptionId", delimitation.CircumscriptionId)
                .SetParameter("regionId", delimitation.RegionId)
                .SetParameter("entryLevel", (int)DelimitationType.PollingStation)
                .SetParameter("bpType", 0)
                .SetParameter("bpStatus", (int)BallotPaperStatus.New)
                .SetParameter("descript", "No Description")
                .SetParameter("userId", currentUserId)
                .ToSqlQuery()
                .ExecuteUpdate();

            foreach (var allocationItem in itemsToAllocate)
            {
                var ppId = allocationItem.PoliticalPartyId;
                long candidateId = -1;

                if (delimitation.IsMayorElection || allocationItem.IsIndependent)
                {
                    var candidates = _saiseRepository.Query<ElectionCompetitorMember>()
                        .Where(x => x.ElectionRound.Election.Id == delimitation.GetElectionId() &&
                                    //x.AssignedCircumscription.Region.Id == delimitation.RegionId &&
                                    x.ElectionCompetitor.Id == ppId)
                        .GroupBy(
                            x => new { ElectionRoundId = x.ElectionRound.Id, /*RegionId = x.AssignedCircumscription.Region.Id,*/ CandidateId = x.Id })
                        .Select(x => new { x.Key.CandidateId, CandidateOrder = x.Min(y => y.CompetitorMemberOrder) }).ToList()
                        .OrderBy(x => x.CandidateId);

                    if (candidates.Any())
                    {
                        candidateId = candidates.First().CandidateId;
                    }
                }

                _saiseRepository.CreateSqlStringBuilder(insertElectionResultsQry, null)
                    .SetParameter("ppId", allocationItem.PoliticalPartyId)
                    .SetParameter("candidateId", candidateId)
                    .SetParameter("bpOrder", allocationItem.BallotOrder)
                    .SetParameter("comments", "No Comments")
                    .SetParameter("status", (int)ElectionResultStatus.New)
                    .SetParameter("userId", currentUserId)
                    .SetParameter("electionId", electionround?.Id)
                    .SetParameter("circumscriptionId", delimitation.CircumscriptionId)
                    .SetParameter("regionId", delimitation.RegionId)
                    .ToSqlQuery()
                    .ExecuteUpdate();
            }
        }

        private void AllocateForDistrict(DelimitationDto delimitation, IList<AllocationItemDto> itemsToAllocate)
        {
            var electionround = _saiseRepository.Query<ElectionRound>()
                .FirstOrDefault(x => x.Election.Id == delimitation.ElectionId);
            if (delimitation.CircumscriptionId == null)
            {
                throw new ArgumentNullException("delimitation.CircumscriptionId");
            }

            const string deleteElectionResultsQry = @"
                    delete er from ElectionResult er
                    where er.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId 
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCicumscription d on v.AssignedCicumscriptionId = aps.AssignedCicumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCicumscriptionId = :circumscriptionId
                    )";

            const string deleteBallotPapersQry = @"
                    delete bp from BallotPaper bp 
                    where bp.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCicumscription d on v.AssignedCicumscriptionId = aps.AssignedCicumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCicumscriptionId = :circumscriptionId);";

            const string insertBallotPapersQry = @"
                    insert into BallotPaper (EntryLevel, [Type], [Status], [Description], DateOfEntry, PollingStationId, ElectionRoundId, EditUserId, EditDate)
                    select :entryLevel as EntryLevel, :bpType as [Type], :bpStatus as [Status], :descript as [Description], GETDATE() as DateOfEntry, 
		                    aps.PollingStationId as PollingStationId, :electionId as ElectionRoundId, :userId as EditUserId, GETDATE() as EditDate 
                    from AssignedPollingStation aps
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCicumscription d on v.AssignedCicumscriptionId = aps.AssignedCicumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCicumscriptionId = :circumscriptionId);";

            const string insertElectionResultsQry = @"
                    insert into ElectionResult (BallotPaperId, PoliticalPartyId, CandidateId, BallotOrder, Comments, DateOfEntry, [Status], EditUserId, EditDate)
                    select bp.BallotPaperId as BallotPaperId, :ppId as PoliticalPartyId, :candidateId as CandidateId, :bpOrder as BallotOrder, :comments as Comments, 
		                    GETDATE() as DateOfEntry, :status as [Status], :userId as EditUserId, GETDATE() as EditDate from 
                    BallotPaper bp
                    where bp.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId 
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCicumscription d on v.AssignedCicumscriptionId = aps.AssignedCicumscriptionId
                        where aps.ElectionRoundId = :electionId and d.AssignedCicumscriptionId = :circumscriptionId
                    );";

            var currentUserId = SecurityHelper.GetLoggedUserId();

            var affected = _saiseRepository.CreateSqlStringBuilder(deleteElectionResultsQry, null)
                .Sql(deleteBallotPapersQry)
                .Sql(insertBallotPapersQry)
                .SetParameter("electionId", electionround?.Id)
                .SetParameter("circumscriptionId", delimitation.CircumscriptionId)
                .SetParameter("entryLevel", (int)DelimitationType.PollingStation)
                .SetParameter("bpType", 0)
                .SetParameter("bpStatus", (int)BallotPaperStatus.New)
                .SetParameter("descript", "No Description")
                .SetParameter("userId", currentUserId)
                .ToSqlQuery()
                .ExecuteUpdate();

            foreach (var allocationItem in itemsToAllocate)
            {
                var ppId = allocationItem.PoliticalPartyId;
                long candidateId = -1;

                if (delimitation.IsMayorElection || allocationItem.IsIndependent)
                {
                    var candidates = _saiseRepository.Query<ElectionCompetitorMember>()
                        .Where(x => x.ElectionRound.Election.Id == delimitation.GetElectionId() &&
                                    //x.AssignedCircumscription.Region.Id == delimitation.RegionId &&
                                    x.ElectionCompetitor.Id == ppId)
                        .GroupBy(x => new { ElectionRoundId = x.ElectionRound.Id, /*RegionId = x.AssignedCircumscription.Region.Id,*/ CandidateId = x.Id })
                        .Select(x => new { x.Key.CandidateId, CandidateOrder = x.Min(y => y.CompetitorMemberOrder) }).ToList()
                        .OrderBy(x => x.CandidateId);

                    if (candidates.Any())
                    {
                        candidateId = candidates.First().CandidateId;
                    }
                }

                _saiseRepository.CreateSqlStringBuilder(insertElectionResultsQry, null)
                    .SetParameter("ppId", allocationItem.PoliticalPartyId)
                    .SetParameter("candidateId", candidateId)
                    .SetParameter("bpOrder", allocationItem.BallotOrder)
                    .SetParameter("comments", "No Comments")
                    .SetParameter("status", (int)ElectionResultStatus.New)
                    .SetParameter("userId", currentUserId)
                    .SetParameter("electionId", electionround?.Id)
                    .SetParameter("circumscriptionId", delimitation.CircumscriptionId)
                    .ToSqlQuery()
                    .ExecuteUpdate();
            }
        }

        private void AllocateForNonLocals(DelimitationDto delimitation, IList<AllocationItemDto> itemsToAllocate)
        {
            const string deleteElectionResultsQry = @"
                    delete er from ElectionResult er
                    where er.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Region v on ps.RegionId = v.RegionId
                        inner join AssignedCircumscription d on aps.AssignedCircumscriptionId = d.AssignedCircumscriptionId
                        where aps.ElectionRoundId = :electionId 
                    )";

            const string deleteBallotPapersQry = @"
                    delete bp from BallotPaper bp 
                    where bp.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Village v on ps.VillageId = v.VillageId
                        inner join District d on v.DistrictId = d.DistrictId
                        where aps.ElectionId = :electionId 
                    );";

            const string insertBallotPapersQry = @"
                    insert into BallotPaper (EntryLevel, [Type], [Status], [Description], DateOfEntry, PollingStationId, ElectionId, EditUserId, EditDate)
                    select :entryLevel as EntryLevel, :bpType as [Type], :bpStatus as [Status], :descript as [Description], GETDATE() as DateOfEntry, 
		                    aps.PollingStationId as PollingStationId, :electionId as ElectionId, :userId as EditUserId, GETDATE() as EditDate 
                    from AssignedPollingStation aps
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Village v on ps.VillageId = v.VillageId
                        inner join District d on v.DistrictId = d.DistrictId
                    where aps.ElectionId = :electionId;";

            const string insertElectionResultsQry = @"
                    insert into ElectionResult (BallotPaperId, PoliticalPartyId, CandidateId, BallotOrder, Comments, DateOfEntry, [Status], EditUserId, EditDate)
                    select bp.BallotPaperId as BallotPaperId, :ppId as PoliticalPartyId, :candidateId as CandidateId, :bpOrder as BallotOrder, :comments as Comments, 
		                    GETDATE() as DateOfEntry, :status as [Status], :userId as EditUserId, GETDATE() as EditDate from 
                    BallotPaper bp
                    where bp.BallotPaperId IN (
                        select bp.BallotPaperId from BallotPaper bp 
                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
                        inner join Village v on ps.VillageId = v.VillageId
                        inner join District d on v.DistrictId = d.DistrictId
                        where aps.ElectionId = :electionId 
                    );";

            var currentUserId = SecurityHelper.GetLoggedUserId();

            var affected = _saiseRepository.CreateSqlStringBuilder(deleteElectionResultsQry, null)
                .Sql(deleteBallotPapersQry)
                .Sql(insertBallotPapersQry)
                .SetParameter("electionId", delimitation.GetElectionId())
                .SetParameter("entryLevel", (int)DelimitationType.PollingStation)
                .SetParameter("bpType", 0)
                .SetParameter("bpStatus", (int)BallotPaperStatus.New)
                .SetParameter("descript", "No Description")
                .SetParameter("userId", currentUserId)
                .ToSqlQuery()
                .ExecuteUpdate();

            foreach (var allocationItem in itemsToAllocate)
            {
                var ppId = allocationItem.PoliticalPartyId;
                long candidateId = -1;

                if (allocationItem.IsIndependent)
                {
                    var candidates = _saiseRepository.Query<ElectionCompetitorMember>()
                        .Where(x => x.ElectionRound.Id == delimitation.GetElectionId() &&
                                    //x.AssignedCircumscription.Region.Id == delimitation.RegionId &&
                                    x.ElectionCompetitor.Id == ppId)
                        .GroupBy(
                            x => new { ElectionRoundId = x.ElectionRound.Id,/* RegionId = x.AssignedCircumscription.Region.Id,*/ CandidateId = x.Id })
                        .Select(x => new { x.Key.CandidateId, CandidateOrder = x.Min(y => y.CompetitorMemberOrder) }).ToList()
                        .OrderBy(x => x.CandidateId);

                    if (candidates.Any())
                    {
                        candidateId = candidates.First().CandidateId;
                    }
                }

                _saiseRepository.CreateSqlStringBuilder(insertElectionResultsQry, null)
                    .SetParameter("ppId", allocationItem.PoliticalPartyId)
                    .SetParameter("candidateId", candidateId)
                    .SetParameter("bpOrder", allocationItem.BallotOrder)
                    .SetParameter("comments", "No Comments")
                    .SetParameter("status", (int)ElectionResultStatus.New)
                    .SetParameter("userId", currentUserId)
                    .SetParameter("electionId", delimitation.GetElectionId())
                    .ToSqlQuery()
                    .ExecuteUpdate();
            }
        }

        private void SaveUpdateCandidate(DelimitationDto delimitation, ElectionCompetitor politicalParty, CandidateDto candidateData)
        {
            if (candidateData == null)
            {
                return;
            }

            ElectionCompetitorMember candidate = null;
            ElectionCompetitorMember cveRel = null;
            if (candidateData.Id == 0)
            {
                if ((politicalParty.IsIndependent || delimitation.IsMayorElection)
                    && politicalParty.ElectionCompetitorMembers.Count == 1)
                {
                    return;
                }

                candidate = new ElectionCompetitorMember { ElectionCompetitor = politicalParty, Idnp = candidateData.Idnp };
                politicalParty.AddCandidate(candidate);

                var electionId = delimitation.GetElectionId();

                var villageId = (delimitation.ElectionIsLocal)
                    ? delimitation.GetRegionId()
                    : -1;

                cveRel = new ElectionCompetitorMember
                {
                    ElectionRound = _saiseRepository.LoadProxy<ElectionRound>(electionId),
                    AssignedCircumscription = _saiseRepository.Query<AssignedCircumscription>().FirstOrDefault(/*s => s.Region.Id == villageId*/),
                    CompetitorMemberOrder = candidateData.CandidateOrder
                };
            }
            else
            {
                candidate = _saiseRepository.Get<ElectionCompetitorMember>(candidateData.Id);
            }

            if ((politicalParty.IsIndependent && candidate.Id == 0) || !politicalParty.IsIndependent)
            {
                candidate.DateOfBirth = candidateData.DateOfBirth;
                candidate.Gender = candidateData.Gender;
                candidate.LastNameRo = string.IsNullOrWhiteSpace(candidateData.LastNameRo) ? candidateData.LastNameRo : candidateData.LastNameRo.ToUpper();
                candidate.NameRo = string.IsNullOrWhiteSpace(candidateData.NameRo) ? candidateData.NameRo : candidateData.NameRo.ToUpper();
                candidate.PlaceOfBirth = string.Empty;
            }

            candidate.NameRu = string.IsNullOrWhiteSpace(candidateData.NameRu) ? candidateData.NameRu : candidateData.NameRu.ToUpper();
            candidate.LastNameRu = string.IsNullOrWhiteSpace(candidateData.LastNameRu) ? candidateData.LastNameRu : candidateData.LastNameRu.ToUpper();
            candidate.Occupation = candidateData.Occupation;
            candidate.OccupationRu = candidateData.OccupationRu;
            candidate.Workplace = candidateData.Workplace;
            candidate.WorkplaceRu = candidateData.WorkplaceRu;
            candidate.Status = candidateData.Status;
            candidate.EditDate = DateTime.Now;
            candidate.EditUser = _saiseRepository.LoadProxy<SystemUser>(SecurityHelper.GetLoggedUserId());

            _saiseRepository.SaveOrUpdate(candidate);

            if (cveRel != null)
            {
                _saiseRepository.SaveOrUpdate(cveRel);
            }
        }

        private void DeleteCandidate(DeleteCandidateDto itemToDelete)
        {
            if (itemToDelete.CandidateRegionRelId == null)
            {
                return;
            }

            var electionResults = _saiseRepository.Query<ElectionResult>()
                .Where(x => x.Candidate.Id == itemToDelete.CandidateId).ToList();
            foreach (var electionResult in electionResults)
            {
                _saiseRepository.Delete(electionResult);
            }

            var candidateVillageRel = _saiseRepository.LoadProxy<ElectionCompetitorMember>(itemToDelete.CandidateRegionRelId.Value);
            _saiseRepository.Delete(candidateVillageRel);

            var candidate = _saiseRepository.LoadProxy<ElectionCompetitorMember>(itemToDelete.CandidateId);
            _saiseRepository.Delete(candidate);
        }

        private void RemoveElectionResults(IEnumerable<ElectionResult> electionResults)
        {
            foreach (var electionResult in electionResults)
            {
                var ballotPaper = electionResult.BallotPaper;

                ballotPaper.RemoveElectionResult(electionResult.Id);

                _saiseRepository.Delete(electionResult);

                if (ballotPaper.ElectionResults.Count == 0)
                {
                    _saiseRepository.Delete(ballotPaper);
                }
            }
        }


        private void SetFirstCandidates(DelimitationDto delimitation, IList<PoliticalPartyDto> parties)
        {
            CandidateDto ccDto = null;
            ElectionCompetitorMember ccAlias = null;
            ElectionCompetitor pp2Alias = null;

            var partiesToCheck = delimitation.IsMayorElection ? parties : parties.Where(x => x.IsIndependent);

            foreach (var party in partiesToCheck)
            {
                var firstCandidateSubQuery = _saiseRepository.QueryOver<ElectionCompetitorMember>()
                    .JoinAlias(() => ccAlias.ElectionCompetitor, () => pp2Alias)
                    .Where(x => pp2Alias.Id == party.Id &&
                                x.ElectionRound.Id == 30045
                                /*&& x.AssignedCircumscription.Region.Id == delimitation.GetRegionIdOrTBD()*/);
                var s = firstCandidateSubQuery.SelectList(list => list
                          .Select(() => ccAlias.Id).WithAlias(() => ccDto.Id)
                          //.Select(x => x.Id).WithAlias(() => ccDto.CandidateRegionRelId)
                          .Select(x => x.CompetitorMemberOrder).WithAlias(() => ccDto.CandidateOrder)
                          .Select(() => pp2Alias.Id).WithAlias(() => ccDto.PoliticalPartyId)
                          .Select(() => ccAlias.DateOfBirth).WithAlias(() => ccDto.DateOfBirth)
                          .Select(() => ccAlias.Gender).WithAlias(() => ccDto.Gender)
                          .Select(() => ccAlias.Idnp).WithAlias(() => ccDto.Idnp)
                          .Select(() => ccAlias.NameRo).WithAlias(() => ccDto.NameRo)
                          .Select(() => ccAlias.NameRu).WithAlias(() => ccDto.NameRu)
                          .Select(() => ccAlias.LastNameRo).WithAlias(() => ccDto.LastNameRo)
                          .Select(() => ccAlias.LastNameRu).WithAlias(() => ccDto.LastNameRu)
                          .Select(() => ccAlias.Occupation).WithAlias(() => ccDto.Occupation)
                          .Select(() => ccAlias.OccupationRu).WithAlias(() => ccDto.OccupationRu)
                          .Select(() => ccAlias.Workplace).WithAlias(() => ccDto.Workplace)
                          .Select(() => ccAlias.WorkplaceRu).WithAlias(() => ccDto.WorkplaceRu)
                          .Select(() => ccAlias.Status).WithAlias(() => ccDto.Status)
                      )
                      .OrderBy(x => x.CompetitorMemberOrder).Asc
                      .TransformUsing(Transformers.AliasToBean<CandidateDto>())
                      .Take(1).List<CandidateDto>()
                      .FirstOrDefault<CandidateDto>();

                party.CandidateData = s;
            }

        }
    }
}