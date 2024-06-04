using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using Microsoft.AspNet.Identity;
using NHibernate.Criterion;
using NHibernate.Transform;
using Amdaris;
using CEC.SRV.BLL.Extensions;
using NHibernate;

namespace CEC.SAISE.BLL.Impl
{
    public class VotingBll : IVotingBll
    {
        private readonly ISaiseRepository _repository;
        private readonly ILogger _logger;
        private readonly IAuditEvents _audit;
        private readonly ISessionFactory _sessionFactory;
        private static readonly AssignedVoterStatus[] SupplimentaryListStatuses =
        {
            AssignedVoterStatus.ReceivedBallotAbsentee,
            AssignedVoterStatus.ReceivedBallotSupplementary,
            AssignedVoterStatus.ReceivedBallotSupplementaryNew
        };

        public VotingBll(ISaiseRepository repository, ILogger logger, IAuditEvents auditEvents, ISessionFactory sessionFactory)
        {
            _repository = repository;
            _logger = logger;
            _audit = auditEvents;
            _sessionFactory = sessionFactory;
        }

        public bool HasOpeningVotersAssigned(long electionId, long pollingStationId)
        {
            var assignedPollingStation = _repository.Query<AssignedPollingStation>()
                .FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.PollingStation.Id == pollingStationId && x.IsOpen);
            return assignedPollingStation != null;
        }

        public Voter GetVoterByIdnp(string idnp)
        {
            return GetVoterByIdnpAsync(idnp).Result;
        }

        public async Task<Voter> GetVoterByIdnpAsync(string idnp)
        {
            long l_idnp;
            if (!IsValidIdnp(idnp, out l_idnp))
            {
                throw new ArgumentException("Invalid idnp");
            }

            return await _repository.QueryAsync<Voter>(z => z.FirstOrDefault(x => x.Idnp == l_idnp));
        }

        public StatisticsDto GetStatistics(long electionId, long pollingStationId)
        {
            StatisticsDto result = null;
            result = _repository.QueryOver<AssignedVoter>()
                .Where(x => x.PollingStation.Id == pollingStationId)
                .Select(
                    Projections.Count<AssignedVoter>(x => x.Id).WithAlias(() => result.BaseListCounter),
                    Projections.Sum(
                        Projections.Conditional(
                            Restrictions.Where<AssignedVoter>(x => x.Status >= AssignedVoterStatus.ReceivedBallot),
                            Projections.Constant(1),
                            Projections.Constant(0))).WithAlias(() => result.VotedCounter),
                    Projections.Sum(
                        Projections.Conditional(
                            Restrictions.InG(Projections.Property<AssignedVoter>(x => x.Status), SupplimentaryListStatuses),
                            Projections.Constant(1),
                            Projections.Constant(0))).WithAlias(() => result.SupplimentaryListCounter)
                )
                .TransformUsing(Transformers.AliasToBean<StatisticsDto>())
                .SingleOrDefault<StatisticsDto>();

            // override BaseListCounter based on value entered by PollingStation Officer 
            var assignedPollingStation = _repository.Query<AssignedPollingStation>()
                .FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.PollingStation.Id == pollingStationId);

            if (assignedPollingStation != null)
            {
                result.BaseListCounter = assignedPollingStation.OpeningVoters;
            }

            return result;
        }

        public async Task<StatisticsDto> GetStatisticsAsync(long electionId, long pollingStationId)
        {
            StatisticsDto result = null;
            result = _repository.QueryOver<AssignedVoter>()
                .Where(x => x.PollingStation.Id == pollingStationId)
                .Select(
                    Projections.Count<AssignedVoter>(x => x.Id).WithAlias(() => result.BaseListCounter),
                    Projections.Sum(
                        Projections.Conditional(
                            Restrictions.Where<AssignedVoter>(x => x.Status >= AssignedVoterStatus.ReceivedBallot),
                            Projections.Constant(1),
                            Projections.Constant(0))).WithAlias(() => result.VotedCounter),
                    Projections.Sum(
                        Projections.Conditional(
                            Restrictions.InG(Projections.Property<AssignedVoter>(x => x.Status), SupplimentaryListStatuses),
                            Projections.Constant(1),
                            Projections.Constant(0))).WithAlias(() => result.SupplimentaryListCounter)
                )
                .TransformUsing(Transformers.AliasToBean<StatisticsDto>())
                .SingleOrDefault<StatisticsDto>();

            // override BaseListCounter based on value entered by PollingStation Officer 
            var assignedPollingStation = await _repository.QueryAsync<AssignedPollingStation>(
                    z => z.FirstOrDefault(x => x.PollingStation.Id == pollingStationId));

            if (assignedPollingStation != null)
            {
                result.BaseListCounter = assignedPollingStation.OpeningVoters;
                result.IsOpen = assignedPollingStation.IsOpen;
            }

            return result;
        }

        public async Task<UpdateVoterResult> SaveUpdateVoterAsync(VoterUpdateData updateData)
        {
            var currentUserId = SecurityHelper.GetLoggedUserId();
            var user = await _repository.GetAsync<SystemUser>(currentUserId);

            switch (updateData.AssignedVoterStatus)
            {
                case AssignedVoterStatus.ReceivedBallot:
                case AssignedVoterStatus.ReceivedBallotMobile:
                    await SetVoterReceivedBallotAsync(user, updateData);
                    break;
                case AssignedVoterStatus.ReceivedBallotSupplementary:
                case AssignedVoterStatus.ReceivedBallotAbsentee:
                    await ReceivedBallotSupplementaryActionAsync(user, updateData);
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("AssignedVoteStatus with value '{0}' is not supported.", updateData.AssignedVoterStatus));
            }

            var result = new UpdateVoterResult
            {
                Success = true,
            };

            return result;
        }

        private async Task ReceivedBallotSupplementaryActionAsync(SystemUser user, VoterUpdateData updateData)
        {
            var assignedVoter = await _repository.QueryAsync<AssignedVoter>(
                    z => z.FirstOrDefault(x => x.Voter.Id == updateData.VoterId));
            PollingStation poling = null;
            if (user.PollingStationId != null)
            {
                poling = _repository.Get<PollingStation>(user.PollingStationId.Value);
            }
            var nullPollingStation = _repository.LoadProxy<PollingStation>(-1);
            var nullRegion = _repository.LoadProxy<Region>(-1);

            if (assignedVoter == null)
            {
                assignedVoter = new AssignedVoter
                {
                    Voter = _repository.LoadProxy<Voter>(updateData.VoterId),
                    PollingStation = user.PollingStationId != null ? poling : nullPollingStation,
                    RequestingPollingStation = nullPollingStation,
                    Region = nullRegion,
                    Category = 99
                };
            }
            else
            {
                if (assignedVoter.Status > AssignedVoterStatus.BaseList)
                {
                    return;
                }
            }

            var pollingStation = _repository.LoadProxy<PollingStation>(user.PollingStationId.GetValueOrDefault());
            var region = _repository.LoadProxy<Region>(user.RegionId.GetValueOrDefault());

            assignedVoter.PollingStation = pollingStation;
            assignedVoter.Region = region;

            assignedVoter.Status = updateData.AssignedVoterStatus;
            assignedVoter.EditUser = user;
            assignedVoter.EditDate = DateTime.Now;
            assignedVoter.ElectionListNr = null;

            var voter = await _repository.QueryAsync<Voter>(z => z.FirstOrDefault(x => x.Id == updateData.VoterId));
            voter.ElectionListNumber = null;
            await _repository.SaveOrUpdateAsync(voter);
            await _repository.SaveOrUpdateAsync(assignedVoter);
        }

        private async Task SetVoterReceivedBallotAsync(SystemUser user, VoterUpdateData updateData)
        {
            var assignedVoter = await _repository.GetAsync<AssignedVoter>(updateData.AssignedVoterId.GetValueOrDefault());

            if (assignedVoter == null)
            {
                return;
            }

            if (assignedVoter.Status > AssignedVoterStatus.BaseList)
            {
                return;
            }

            assignedVoter.Status = updateData.AssignedVoterStatus;
            assignedVoter.EditUser = user;
            assignedVoter.EditDate = DateTime.Now;

            await _repository.SaveOrUpdateAsync(assignedVoter);
        }

        public bool IsValidIdnp(string idnp, out long idnp2)
        {
            //todo: implement validation logic
            idnp2 = 0;
            if (string.IsNullOrWhiteSpace(idnp))
            {
                return false;
            }

            if (idnp.Length < 12 || idnp.Length > 13)
            {
                return false;
            }

            return long.TryParse(idnp, out idnp2);
        }

        public bool IsPollingStationOpen(long electionId, long pollingStationId)
        {
            var assignedPollingStation = _repository.Query<AssignedPollingStation>()
                .FirstOrDefault(x => x.ElectionRound.Election.Id == electionId && x.PollingStation.Id == pollingStationId);

            if (assignedPollingStation == null)
            {
                throw new Exception(string.Format("No AssignedPollingStation found for ElectionId: {0} and PollingStationId: {1}",
                        electionId, pollingStationId));
            }

            return assignedPollingStation.IsOpen;
        }

        public async Task<SearchResult> SearchVoterAsync(string idnp, string loger)
        {
            VoterSearchStatus searchStatus;
            var voter = await GetVoterByIdnpAsync(idnp);
            if (voter == null)
            {
                searchStatus = VoterSearchStatus.NotFound;
                return new SearchResult { Status = searchStatus };
            }
            // Valid

            AssignedCircumscription circumscription = null;
            AssignedCircumscription circumscriptionV = null;
            var currentPrincipal = SecurityHelper.GetLoggedUser();
            var user = await _repository.GetAsync<SystemUser>(currentPrincipal.Identity.GetUserId<long>());
            var region = await _repository.GetAsync<Region>(user.GetRegionId());
            var p = await _repository.GetAsync<PollingStation>(user.GetPollingStationId());

            var ap = _repository.Query<AssignedPollingStation>().Where(x => x.PollingStation.Id == p.Id).ToList();


            if (ap.Any(s => !string.IsNullOrEmpty(s.NumberPerElection)))
            {
                circumscription = ap.FirstOrDefault(s => !string.IsNullOrEmpty(s.NumberPerElection))?.AssignedCircumscription;
            }
            else
            {
                circumscription = ap.FirstOrDefault()?.AssignedCircumscription;
            }

            //var cirscumscription = _repository.Query<AssignedCircumscription>()
            //    .FirstOrDefault(x => x. == region.Id);
            var electionDay = _repository.Query<ElectionDay>().FirstOrDefault();

            PollingStation poling = null;
            string CompletAdress = "";

            if (user.PollingStationId.HasValue)
            {
                poling = _repository.Get<PollingStation>(user.PollingStationId.Value);

                if (poling != null)
                {
                    string type = poling.Region.RegionType.Name;
                    string villge = poling.Region.Name;
                    string street = poling.StreetNumber != null ? " " + poling.StreetNumber : " ";
                    string subnumber = poling.StreetSubNumber != null ? " / " + poling.StreetSubNumber : " ";
                    string sub = poling.SubNumber != null ? " / " + poling.SubNumber : "";
                    CompletAdress = type + villge + street + subnumber + sub;
                }
            }



            if ((electionDay.ElectionDayDate.DateTime < voter.DateOfBirth.AddYears(18)))
            {
                searchStatus = VoterSearchStatus.NotFound;
                return new SearchResult { Status = searchStatus };
            }

            var assignedVoterData = voter.AssignedVoters.ToList();

            if (assignedVoterData.Count == 0)
            {
                searchStatus = VoterSearchStatus.NotAssigned;
            }
            else
            {
                searchStatus = VoterSearchStatus.Success;
            }

            var assignedVoter = assignedVoterData.FirstOrDefault();
            var voterData = new VoterData(voter, assignedVoter);

            voterData.CompletPolingStationAdress = CompletAdress;
            voterData.ValidateForUser(currentPrincipal.Identity, user);

            var apWithNumber = ap.Where(s => !string.IsNullOrEmpty(s.NumberPerElection)).FirstOrDefault();

            if (apWithNumber != null && voterData.Assignement != null)
            {
                voterData.Assignement.PollingStation.Name = string.Format("{0,3:D3} - {1}", apWithNumber.NumberPerElection, apWithNumber.PollingStation.NameRo);
            }

            try
            {
                if (voterData.HasVoted == true)
                {
                    var alert = new Alerts
                    {
                        Voter = voter,
                        Idnp = voter.Idnp,
                        FirstName = voter.NameRo,
                        LastName = voter.LastNameRo,
                        Patronymic = voter.PatronymicRo,
                        DateOfBirth = voter.DateOfBirth,
                        DocumentNumber = voter.DocumentNumber,
                        Adress = voterData.Address,
                        PollingStation = poling,
                        DateRegistration = DateTime.Now,
                        PollingStationAdress = CompletAdress,
                        EditDate = DateTime.Now,
                        EditUser = user
                    };

                    await _repository.SaveOrUpdateAsync(alert);



                }
            }
            catch
            {
                //
            }

            if (searchStatus != VoterSearchStatus.NotAssigned)
            {
                var apV = _repository.Query<AssignedPollingStation>().Where(x => x.PollingStation.Id == voterData.Assignement.PollingStation.Id).ToList();

                if (apV.Any(s => !string.IsNullOrEmpty(s.NumberPerElection)))
                {
                    circumscriptionV = apV.FirstOrDefault(s => !string.IsNullOrEmpty(s.NumberPerElection))?.AssignedCircumscription;
                }
                else
                {
                    circumscriptionV = apV.FirstOrDefault()?.AssignedCircumscription;
                }

                if (circumscriptionV != null)
                {
                    string cirNmae = circumscriptionV.Number + " - " + circumscriptionV.NameRo;
                    voterData.Assignement.Circumscription = new ValueNamePair(circumscriptionV.Id, cirNmae);
                }
                voterData.Assignement.IsSameCircumscription = circumscription == circumscriptionV;

                var apvWithNumber = apV.Where(s => !string.IsNullOrEmpty(s.NumberPerElection)).FirstOrDefault();

                if (apvWithNumber != null && voterData.Assignement != null)
                {
                    voterData.Assignement.PollingStation.Name = string.Format("{0,3:D3} - {1}", apvWithNumber.NumberPerElection, apvWithNumber.PollingStation.NameRo);
                }


            }



            return new SearchResult
            {
                Status = searchStatus,
                VoterData = voterData
            };
        }

        public async Task<PollingStationOpeningData> GetOpeningDataAsync(long assignedPollingStationId)
        {
            var assignedPollingStation = await _repository.GetAsync<AssignedPollingStation>(assignedPollingStationId);

            AssignedVoterStatus[] validStatuses =
                {
                    AssignedVoterStatus.BaseList, AssignedVoterStatus.ReceivedBallot,
                    AssignedVoterStatus.ReceivedBallotMobile
                };
            var assignedVotersCount = await _repository.QueryAsync<AssignedVoter, long>(
                z => z.LongCount(x => x.PollingStation.Id == assignedPollingStation.PollingStation.Id &&
                                      validStatuses.Contains(x.Status)));

            return new PollingStationOpeningData
            {
                AssignedPollingStationId = assignedPollingStation.Id,
                IsOpen = assignedPollingStation.IsOpen,
                OpeningVoters = assignedPollingStation.OpeningVoters,
                IsOpeningEnabled = assignedPollingStation.IsOpeningEnabled,
                AssignedVotersCount = assignedVotersCount
            };
        }

        public async Task<PollingStationOpeningData> GetOpeningDataAsync(long electionId, long pollingStationId)
        {
            var openingData = await _repository.QueryAsync<AssignedPollingStation, PollingStationOpeningData>(
                z => z.Where(x => x.PollingStation.Id == pollingStationId)
                    .Select(x => new PollingStationOpeningData
                    {
                        AssignedPollingStationId = x.Id,
                        IsOpen = x.IsOpen,
                        OpeningVoters = x.OpeningVoters,
                        IsOpeningEnabled = x.IsOpeningEnabled
                    }).FirstOrDefault());

            if (openingData != null)
            {
                AssignedVoterStatus[] validStatuses =
                {
                    AssignedVoterStatus.BaseList, AssignedVoterStatus.ReceivedBallot,
                    AssignedVoterStatus.ReceivedBallotMobile
                };
                var assignedVotersCount = await _repository.QueryAsync<AssignedVoter, long>(
                    z => z.LongCount(x => x.PollingStation.Id == pollingStationId &&
                                      validStatuses.Contains(x.Status)));

                openingData.AssignedVotersCount = assignedVotersCount;
            }

            return openingData;
        }

        public async Task<OpenPollingStationResult> OpenPollingStationAsync(long assignedPollingStationId, int openingVoters, string ipUser)
        {

            var currentUserId = SecurityHelper.GetLoggedUserId();

            var assignedPollingStation = await _repository.GetAsync<AssignedPollingStation>(assignedPollingStationId);
            if (assignedPollingStation.IsOpen)
            {
                return OpenPollingStationResult.AlreadyOpen;
            }

            var otherAssignedPS_forElectionDate = await _repository.QueryAsync<AssignedPollingStation, IList<AssignedPollingStation>>(z =>
                z.Where(x =>
                        x.ElectionRound.ElectionDate == assignedPollingStation.ElectionRound.ElectionDate &&
                        x.PollingStation.Id == assignedPollingStation.PollingStation.Id).ToList());

            var currentUserProxy = _repository.LoadProxy<SystemUser>(currentUserId);
            string messgae = $"Secția de votare : {assignedPollingStation.PollingStation.NameRo} , Numarul de alegatori : {openingVoters}";
            foreach (var aps in otherAssignedPS_forElectionDate)
            {
                aps.IsOpen = true;
                aps.OpeningVoters = openingVoters;
                aps.EditUser = currentUserProxy;
                aps.EditDate = DateTime.Now;

                await _repository.SaveOrUpdateAsync(aps);
                try
                {
                    await _audit.InsertEvents(AuditEventTypeDto.OpenElction.GetEnumDescription(), currentUserProxy, messgae, ipUser);
                }
                catch (Exception)
                {

                    throw;
                }

            }



            return OpenPollingStationResult.Success;
        }

        public async Task LogSearchEventAsync(string idnp, SearchResult searchResult, string ipUser)
        {
            string message = "";
            var systemUserId = SecurityHelper.GetLoggedUserId();
            var user = _repository.Get<SystemUser>(systemUserId);

            message = searchResult.Status != VoterSearchStatus.NotFound
                ? $"IDNP: { idnp} , Numele/Prenumele: {searchResult.VoterData.FirstName}  {searchResult.VoterData.LastName}"
                : $"Nu a fost găsit persoana cu IDNP : {idnp} ";

            await _audit.InsertEvents(AuditEventTypeDto.SearchIdnp.GetEnumDescription(), user, message, ipUser);


        }

        public Task<AssignedPollingStation> GetAssignedPollingStationAsync(long electionId, long pollingStationId)
        {
            return _repository.QueryAsync<AssignedPollingStation>(
                    z => z.FirstOrDefault(x => x.PollingStation.Id == pollingStationId));
        }

        public SearchResult SearchVoterAsyncForWebService(string idnp)
        {


            var voter = GetVoterByIdnpAsync(idnp);

            var assignedVoterData = voter.Result.AssignedVoters.ToList();

            var assignedVoter = assignedVoterData.FirstOrDefault();
            var voterData = new VoterData(voter.Result, assignedVoter);

            var pollingStation = assignedVoter?.PollingStation;

            if (pollingStation != null)
            {
                var ap = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation.Id == pollingStation.Id && x.NumberPerElection != null)
                    ?? _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation.Id == pollingStation.Id);

                voterData.CircuscriptionName = ap.AssignedCircumscription?.NameRo;
                voterData.CircuscriptionNumber = ap.AssignedCircumscription?.Number;
                voterData.PolingStationNumber = ap.NumberPerElection != null ? ap.NumberPerElection.ToString() : ap.PollingStation?.Number.ToString();
            }

            return new SearchResult
            {
                Status = VoterSearchStatus.Success,
                VoterData = voterData
            };


        }

        public VoterUtanStatus CheckUserRegionValidation(long idnp, long circumscriptionId)
        {
            try
            {
                var pollingStationCircumscription = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation.Id == circumscriptionId && x.NumberPerElection != null);
                var pollingStationIsInUtan = _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.Id == pollingStationCircumscription.AssignedCircumscription.Id);
                if (pollingStationIsInUtan != null && pollingStationIsInUtan.IsFromUtan)
                {
                    var voterRegionId = _repository.Query<Voter>().FirstOrDefault(x => x.Idnp == idnp);
                    var voterRegion = _repository.Query<CircumscriptionRegion>().FirstOrDefault(x => x.Region.Id == voterRegionId.Region.Id && x.AssignedCircumscription.Id == pollingStationIsInUtan.Id);
                    if (voterRegion != null)
                    {
                        return VoterUtanStatus.Success;
                    }
                    else
                    {
                        return VoterUtanStatus.WrongUtan;
                    }
                }
                else
                {
                    return VoterUtanStatus.NotUtan;
                }
            }
            catch
            {
                return VoterUtanStatus.NotUtan;
            }

        }

    }
}
    