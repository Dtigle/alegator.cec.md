using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.BLL.Dto.Concurents;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Impl
{
//    class BallotPapersGenerator
//    {
//        private readonly ISaiseRepository _saiseRepository;

//        private const string BaseSelectorQry = @"
//                    select bp.BallotPaperId from BallotPaper bp 
//                    inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
//                    inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
//                    inner join Village v on ps.VillageId = v.VillageId
//                    inner join District d on v.DistrictId = d.DistrictId
//                    where aps.ElectionId = :electionId";

//        const string DeleteElectionResultsBaseQry = @"
//                    delete er from ElectionResult er
//                    where er.BallotPaperId IN (
//                        select bp.BallotPaperId from BallotPaper bp 
//                        inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
//                        inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
//                        inner join Village v on ps.VillageId = v.VillageId
//                        inner join District d on v.DistrictId = d.DistrictId
//                        where aps.ElectionId = :electionId
//                    )";

//        const string DeleteBallotPapersBaseQry = @"
//                    delete bp from BallotPaper bp where bp.ElectionId = :electionId;";

//        const string InsertBallotPapersBaseQry = @"
//                    insert into BallotPaper (EntryLevel, [Type], [Status], [Description], DateOfEntry, PollingStationId, ElectionId, EditUserId, EditDate)
//                    select :entryLevel as EntryLevel, :bpType as [Type], :bpStatus as [Status], :descript as [Description], GETDATE() as DateOfEntry, 
//		                    aps.PollingStationId as PollingStationId, :electionId as ElectionId, :userId as EditUserId, GETDATE() as EditDate 
//                    from AssignedPollingStation aps
//                    inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
//                    inner join Village v on ps.VillageId = v.VillageId
//                    inner join District d on v.DistrictId = d.DistrictId
//                    where aps.ElectionId = :electionId;";

//        const string InsertElectionResultsQry = @"
//                    insert into ElectionResult (BallotPaperId, PoliticalPartyId, CandidateId, BallotOrder, Comments, DateOfEntry, [Status], EditUserId, EditDate)
//                    select bp.BallotPaperId as BallotPaperId, :ppId as PoliticalPartyId, :candidateId as CandidateId, :bpOrder as BallotOrder, :comments as Comments, 
//		                    GETDATE() as DateOfEntry, :status as [Status], :userId as EditUserId, GETDATE() as EditDate 
//                    from BallotPaper bp
//                    where bp.ElectionId = :electionId";

//        public BallotPapersGenerator(ISaiseRepository saiseRepository)
//        {
//            _saiseRepository = saiseRepository;
//        }

//        public void GenerateForNonLocals(DelimitationDto delimitation, IList<AllocationItemDto> itemsToAllocate)
//        {
//            var currentUserId = SecurityHelper.GetLoggedUserId();

//            var affected = _saiseRepository.CreateSqlStringBuilder(DeleteElectionResultsBaseQry, null)
//                .Sql(DeleteBallotPapersBaseQry)
//                .Sql(InsertBallotPapersBaseQry)
//                .SetParameter("electionId", delimitation.GetElectionId())
//                .SetParameter("entryLevel", (int)DelimitationType.PollingStation)
//                .SetParameter("bpType", 0)
//                .SetParameter("bpStatus", (int)BallotPaperStatus.New)
//                .SetParameter("descript", "No Description")
//                .SetParameter("userId", currentUserId)
//                .ToSqlQuery()
//                .ExecuteUpdate();

//            foreach (var allocationItem in itemsToAllocate)
//            {
//                _saiseRepository.CreateSqlStringBuilder(InsertElectionResultsQry, null)
//                    .SetParameter("ppId", allocationItem.PoliticalPartyId)
//                    .SetParameter("candidateId", -1)
//                    .SetParameter("bpOrder", allocationItem.BallotOrder)
//                    .SetParameter("comments", "No Comments")
//                    .SetParameter("status", (int)ElectionResultStatus.New)
//                    .SetParameter("userId", currentUserId)
//                    .SetParameter("electionId", delimitation.GetElectionId())
//                    .ToSqlQuery()
//                    .ExecuteUpdate();
//            }
//        }
//    }
}
