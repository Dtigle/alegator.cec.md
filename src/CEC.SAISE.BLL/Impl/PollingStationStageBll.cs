using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Linq;
using NHibernate;

namespace CEC.SAISE.BLL.Impl
{
    public class PollingStationStageBll : IPollingStationStageBll
    {
        private readonly ISaiseRepository _repository;

        public PollingStationStageBll(ISaiseRepository repository)
        {
            _repository = repository;
        }

        public PageResponse<PollingStationStageEnablerDto> GetPollingStation(PageRequest pageRequest, long electionId)
        {
            PollingStationStageEnablerDto dto = null;
            PollingStation ps = null;
            PollingStation ps1 = null;
            BallotPaper bp = null;
            AssignedCircumscription ac = null;
            ElectionRound er = null;
            Election el = null;
            Region r = null;
            RegionType rt = null;

            var result = _repository.QueryOver<AssignedPollingStation>()
                .JoinAlias(x => x.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => el)
                .JoinAlias(x => x.PollingStation, () => ps)
                .JoinAlias(x => x.AssignedCircumscription, () => ac)
                .JoinAlias(() => ps.Region, () => r)
                .JoinAlias(() => r.RegionType, () => rt)
                .JoinAlias(() => ps.BallotPapers, () => bp, JoinType.LeftOuterJoin)
                .Where(x => el.Id == electionId && bp.ElectionRound.Id == er.Id)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => dto.Id)
                    .Select(x => x.IsOpeningEnabled).WithAlias(() => dto.EnableOpening)
                    .Select(x => x.IsTurnoutEnabled).WithAlias(() => dto.EnableTurnout)
                    .Select(x => x.IsElectionResultEnabled).WithAlias(() => dto.EnabelElectionResult)
                    .Select(x => x.OpeningVoters).WithAlias(() => dto.OpeningVoters)
                    .Select(x => x.IsOpen).WithAlias(() => dto.PSIsOpen)
                    .Select(() => ps.NumberPerElection).WithAlias(() => dto.PollingStation)
                    .Select(Projections.SqlFunction("concat", 
                    NHibernateUtil.String, 
                    Projections.Property(() => rt.Name), 
                    Projections.Constant(" "), 
                    Projections.Property(() => r.Name)))
                    .WithAlias(() => dto.Lacality)
                    .Select(() => ac.NameRo).WithAlias(() => dto.Circumscription)
                    .Select(() => ac.Number).WithAlias(() => dto.CircumscriptionNumber)
                    .Select(() => bp.Status).WithAlias(() => dto.BallotPaperStatus)
                    .Select(() => bp.Id).WithAlias(() => dto.BallotPaperId)
                )
                .TransformUsing(Transformers.AliasToBean<PollingStationStageEnablerDto>())
                .RootCriteria.CreatePage<PollingStationStageEnablerDto>(pageRequest);

            return result;
        }

        public void ProcessOptions(OptionsToggleDto data)
        {
            const string baseQry = @"
                update AssignedPollingStation set isOpeningEnabled = :opening, isTurnoutEnabled = :turnout, isElectionResultEnabled = :results ";

            var queryBuilder = _repository.CreateSqlStringBuilder(baseQry, null)
                .SetParameter("opening", data.EnableOpening)
                .SetParameter("turnout", data.EnableTurnout)
                .SetParameter("results", data.EnableElectionResults);

            var queryBuilder2 = _repository.CreateSqlStringBuilder(baseQry, null)
                .SetParameter("opening", data.EnableOpening)
                .SetParameter("turnout", data.EnableTurnout)
                .SetParameter("results", data.EnableElectionResults);

            switch (data.Action)
            {
                case OptionsToggleActions.SelectedAssignedPollingStations:
                    if (data.SelectedAPSIds.Count == 0)
                    {
                        return;
                    }
                    var poling = GetAffectedAssignedCircumscription(data.SelectedAPSIds);
                    queryBuilder = queryBuilder.Sql(" where PollingStationId IN (:apsIds)")
                        .SetParameterList("apsIds", poling);
                    break;
                case OptionsToggleActions.All:
                    var electionIds = GetAffectedElections(data.ElectionId);
                    if (electionIds.Count > 2000)
                    {
                        queryBuilder = queryBuilder.Sql(" where PollingStationId IN (:apsIds)")
                            .SetParameterList("apsIds", electionIds.Take(1500).ToList());
                        queryBuilder.ToSqlQuery().ExecuteUpdate();
                        var a = electionIds.Skip(1500).ToList();
                        queryBuilder2 = queryBuilder2.Sql(" where PollingStationId IN (:apsIds2)")
                            .SetParameterList("apsIds2", a);
                        queryBuilder2.ToSqlQuery().ExecuteUpdate();
                        return;
                    }
                    else
                    {
                        queryBuilder = queryBuilder.Sql(" where PollingStationId IN (:apsIds)")
                            .SetParameterList("apsIds", electionIds);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("data.Action");
            }

            queryBuilder.ToSqlQuery().ExecuteUpdate();
        }

        public PageResponse<VotingProcessStatsDto> GetVotingStatsForUser(PageRequest pageRequest, long electionId, long turnoutDataElectionid)
        {
            VotingProcessStatsDto dto = null;
            PollingStation ps = null;
            PollingStation ps1 = null;
            AssignedCircumscription ac = null;
            BallotPaper bp = null;
            AssignedPollingStation aps = null;

            ElectionRound er = null;
            Election el = null;
            Region r = null;
            RegionType rt = null;

            var currentUser = _repository.Get<SystemUser>(SecurityHelper.GetLoggedUserId());


            var totalVotesQry = QueryOver.Of<AssignedVoter>()
                .Where(x => x.PollingStation.Id == aps.PollingStation.Id && x.Status >= AssignedVoterStatus.ReceivedBallot && x.Status < AssignedVoterStatus.Invalid)
                .Select(Projections.RowCountInt64());

            var supplementaryListQry = QueryOver.Of<AssignedVoter>()
                .Where(x => x.PollingStation.Id == aps.PollingStation.Id)
                .AndRestrictionOn(x => x.Status).IsIn(new[] { AssignedVoterStatus.ReceivedBallotSupplementary, AssignedVoterStatus.ReceivedBallotAbsentee })
                .Select(Projections.RowCountInt64());

            var qry = _repository.QueryOver(() => aps)
                .JoinAlias(() => aps.PollingStation, () => ps)
                .JoinAlias(x => x.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => el)
                .JoinAlias(() => ps.Region, () => r)
                .JoinAlias(() => r.RegionType, () => rt)
                .JoinAlias(() => aps.AssignedCircumscription, () => ac)
                .JoinAlias(() => ps.BallotPapers, () => bp, JoinType.LeftOuterJoin)
                .Where(x => el.Id == electionId && bp.ElectionRound.Id == er.Id);

            //if (currentUser.RegionId.HasValue && currentUser.RegionId != -2)
            //{
            //    qry = qry.And(() => r.Id == currentUser.RegionId);
            //}

            if (currentUser.PollingStationId.HasValue && currentUser.PollingStationId != -2)
            {
                qry = qry.And(() => aps.PollingStation.Id == currentUser.PollingStationId );
            }


            if(currentUser.CircumscriptionId.HasValue&& currentUser.CircumscriptionId !=-2 )
            {
                var userACirc = _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.CircumscriptionId == currentUser.CircumscriptionId);
                var circRegions = _repository.Query<CircumscriptionRegion>().Where(x => x.AssignedCircumscription.Id == userACirc.Id).Select(x => x.Region.Id);
                var otherACirc = _repository.Query<CircumscriptionRegion>().Where(x => circRegions.Contains(x.Region.Id)).Select(x => x.AssignedCircumscription.Id).ToList();
                //qry = qry.And(() => ac.CircumscriptionId == currentUser.CircumscriptionId);
                qry = qry.WhereRestrictionOn(val => val.AssignedCircumscription.Id).IsIn(otherACirc);
            }
          
            return qry.SelectList(list => list
                    .Select(() => aps.Id).WithAlias(() => dto.Id)
                    .Select(() => aps.OpeningVoters).WithAlias(() => dto.OpeningVoters)
                    .Select(() => aps.IsOpen).WithAlias(() => dto.PSIsOpen)
                    .Select(() => ps.NumberPerElection).WithAlias(() => dto.PollingStation)
                    .Select(() => r.Name).WithAlias(() => dto.Lacality)
                    .Select(() => ac.NameRo).WithAlias(() => dto.Circumscription)
                    .Select(() => ac.Number).WithAlias(() => dto.CircumscriptionNumber)
                    .Select(() => bp.Status).WithAlias(() => dto.BallotPaperStatus)
                    .SelectSubQuery(totalVotesQry).WithAlias(() => dto.TotalVotes)
                    .SelectSubQuery(supplementaryListQry).WithAlias(() => dto.SupplementaryVotes)
                )
                .TransformUsing(Transformers.AliasToBean<VotingProcessStatsDto>())
                .RootCriteria.CreatePage<VotingProcessStatsDto>(pageRequest);
        }

        private List<long> GetAffectedElections(long electionId)
        {
            var election = _repository.Get<Election>(electionId);
            var electionRound = _repository.Query<ElectionRound>()
                .FirstOrDefault(x => x.Election.Id == electionId);
            var pollingstation = _repository.Query<AssignedPollingStation>()
                .Where(x => x.ElectionRound.Id == electionRound.Id)
                .Select(x=>x.PollingStation.Id)
                .ToList();
            var electionIds = new List<long> ();
           
            //var electionPollingstation = new List<long>();
            //if (election.IsSubTypeOfLocalElection())
            //{
            //    electionIds = _repository.Query<Election>()
            //        .Where(x => x.DateOfElection == election.DateOfElection)
            //        .Select(x => x.Id)
            //        .ToList();
            //}

            return pollingstation;
        }

        private List<long> GetAffectedAssignedCircumscription(List<long> ascirc)
        {
            var pol = QueryOver.Of<AssignedPollingStation>()
                .Select(x => x.Id)
                .Where(x => x.Id.IsIn(ascirc));
            var polling = _repository.QueryOver<AssignedPollingStation>()
            .WithSubquery.WhereProperty(x => x.Id).In(pol);         

            return polling.Select(x=>x.PollingStation.Id).List<long>().ToList();
        }

        public long? GetPollingStationId(int id)
        {
            var result = _repository.Get<AssignedPollingStation>(id);
            return result.PollingStation?.Id;
        }
    }
}