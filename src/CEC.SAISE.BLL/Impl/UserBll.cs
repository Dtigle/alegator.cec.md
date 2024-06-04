using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using NHibernate.Criterion;
using Filter = DocumentFormat.OpenXml.Office2010.Excel.Filter;

namespace CEC.SAISE.BLL.Impl
{
    public class UserBll : IUserBll
    {
        private readonly ISaiseRepository _repository;

        public UserBll(ISaiseRepository repository)
        {
            _repository = repository;
        }

        public SystemUser GetById(long userId)
        {
            return _repository.Get<SystemUser>(userId);
        }

        public async Task<SystemUser> GetByIdAsync(long userId)
        {
            return await _repository.GetAsync<SystemUser>(userId);
        }

        public async Task<UserDataDto> GetCurrentUserData()
        {
            var principal = SecurityHelper.GetLoggedUser();
            var userId = SecurityHelper.GetLoggedUserId();
            var isAdmin = principal.IsInRole("Administrator");
            var user = await _repository.GetAsync<SystemUser>(userId);

            var circumscriptionAcces = user.CircumscriptionId != null;
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return await MapSystemUser(user, isAdmin, circumscriptionAcces);
        }

        public async Task<IList<Election>> GetAccessibleElections()
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var elections = await _repository.QueryAsync<Election>(z =>
                z.OrderByDescending(x => x.DateOfElection).ToList());

            if (SecurityHelper.LoggedUserIsInRole("Administrator"))
            {
                return elections;
            }

            var systemUser = await GetByIdAsync(userId);

            //var restrictedElectionRole = await _repository.QueryAsync<AssignedRole, Role>(z =>
            //    z.Where(x => x.SystemUser.Id == userId &&
            //                 x.Role.Name.StartsWith("RestrictElection"))
            //        .Select(x => x.Role).SingleOrDefault());

            //return restrictedElectionRole == null
            //    ? elections.Where(x => x.Id == systemUser.ElectionId).ToList()
            //    : FilterByPermission(elections, restrictedElectionRole);

            var restrictedElectionRoles = await _repository.QueryAsync<AssignedRole, IList<Role>>(z =>
                z.Where(x => x.SystemUser.Id == userId &&
                             x.Role.Name.StartsWith("RestrictElection"))
                    .Select(x => x.Role).ToList());

            List<Election> filteredItems = new List<Election>();
            if (!restrictedElectionRoles.Any())
            {
                filteredItems.AddRange(elections.Where(x => x.Id == systemUser.ElectionId).ToList());
            }
            else
            {
                foreach (var restrictedElectionRole in restrictedElectionRoles)
                {
                    var xx = FilterByPermission(elections, restrictedElectionRole);
                    filteredItems.AddRange(xx);
                }
            }

            return filteredItems.Distinct().ToList();
        }

        public async Task<PageResponse<Election>> GetAccessibleElectionsAsync(PageRequest request)
        {
            var electionsQry = _repository.QueryOver<Election>()
                .OrderBy(x => x.DateOfElection)
                .Desc;

            if (SecurityHelper.LoggedUserIsInRole("Administrator"))
            {
                return electionsQry.RootCriteria.CreatePage<Election>(request);
            }


            var userId = SecurityHelper.GetLoggedUserId();
            var systemUser = await GetByIdAsync(userId);
            var elections = electionsQry.List<Election>();
            List<Election> filteredItems = new List<Election>();
            if (systemUser.CircumscriptionId != null)
            {
                var userACirc = _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.CircumscriptionId == systemUser.CircumscriptionId);
                var circRegions = _repository.Query<CircumscriptionRegion>().Where(x => x.AssignedCircumscription.Id == userACirc.Id).Select(x => x.Region.Id);
                var otherACirc = _repository.Query<CircumscriptionRegion>().Where(x => circRegions.Contains(x.Region.Id)).Select(x => x.AssignedCircumscription.Id);
                var elect = _repository.Query<AssignedCircumscription>().Where(x => x.CircumscriptionId == systemUser.CircumscriptionId || otherACirc.Contains(x.Id)).Select(x => x.ElectionRound.Election).Distinct().ToList();
                filteredItems.AddRange(elect);
            }
            else if (systemUser.PollingStationId != null)
            {
                var elect = _repository.Query<AssignedPollingStation>().Where(x => x.PollingStation.Id == systemUser.PollingStationId).Select(x => x.ElectionRound.Election).ToList();
                filteredItems.AddRange(elect);
            }
            else
            {
                var restrictedElectionRoles = await _repository.QueryAsync<AssignedRole, IList<Role>>(z =>
                z.Where(x => x.SystemUser.Id == userId &&
                             x.Role.Name.StartsWith("RestrictElection"))
                    .Select(x => x.Role).ToList());


                if (!restrictedElectionRoles.Any())
                {
                    filteredItems.AddRange(elections.Where(x => x.Id == systemUser.ElectionId).ToList());
                }
                else
                {
                    foreach (var restrictedElectionRole in restrictedElectionRoles)
                    {
                        var xx = FilterByPermission(elections, restrictedElectionRole);
                        filteredItems.AddRange(xx);
                    }
                }
            }



            return new PageResponse<Election>
            {
                Items = filteredItems.Distinct().ToList(),
                PageSize = filteredItems.Count,
                StartIndex = 0,
                Total = filteredItems.Count
            };
        }

        public PageResponse<AssignedCircumscription> GetAccessibleCircumscriptions(PageRequest request, long? electionRoundId)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var systemUser = GetById(userId);


            if (SecurityHelper.LoggedUserIsInRole("Administrator") ||
                (systemUser.RegionId == null || systemUser.RegionId == -2))
            {
                return ListCircumscriptionsForElection(request, electionRoundId.Value);
            }

            var p = _repository.Get<PollingStation>(systemUser.GetPollingStationId());
            var ap = _repository.Query<AssignedPollingStation>()
                         .FirstOrDefault(x => x.PollingStation.Id == p.Id && x.NumberPerElection != null && x.ElectionRound.Election.Id == electionRoundId.Value) != null ?
                          _repository.Query<AssignedPollingStation>()
                          .FirstOrDefault(x => x.PollingStation.Id == p.Id && x.NumberPerElection != null && x.ElectionRound.Election.Id == electionRoundId.Value).AssignedCircumscription
                         : _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation.Id == p.Id && x.ElectionRound.Election.Id == electionRoundId.Value)?.AssignedCircumscription;

            var fixedCircumscription = ap != null ? _repository.Get<AssignedCircumscription>(ap.Id) : null;
            return new PageResponse<AssignedCircumscription>
            {
                Items = new List<AssignedCircumscription> { fixedCircumscription },
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }

        public PageResponse<Region> GetAccessibleRegions(PageRequest request, long electionId, long? regionId)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var systemUser = GetById(userId);

            if (SecurityHelper.LoggedUserIsInRole("Administrator") || systemUser.RegionId == null || systemUser.RegionId == -2)
            {
                return ListRegionsForElection(request, electionId, regionId.GetValueOrDefault());
            }

            var fixedRegion = _repository.Get<Region>(systemUser.GetRegionId());
            return new PageResponse<Region>
            {
                Items = new List<Region> { fixedRegion },
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }

        private PageResponse<AssignedCircumscription> ListCircumscriptionsForElection(PageRequest request, long electionId)
        {
            PollingStation ps = null;
            Region region = null;
            Election e = null;
            ElectionRound er = null;

            //var subQuery = QueryOver.Of<AssignedPollingStation>()
            //    .JoinAlias(x => x.ElectionRound, () => er)
            //    .JoinAlias(() => er.Election, () => e)
            //    .JoinAlias(x => x.PollingStation, () => ps)
            //    .JoinAlias(() => ps.Region, () => region)
            //    .Where(x => e.Id == electionId)
            //    .Select(Projections.Distinct(Projections.Property(() => region.Id)));

            return _repository.QueryOver<AssignedCircumscription>()
                .JoinAlias(x => x.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => e)
                .Where(x => e.Id == electionId)
                .OrderBy(x => x.Number).Asc
                .RootCriteria
                .CreatePage<AssignedCircumscription>(request);
        }

        private PageResponse<Region> ListRegionsForElection(PageRequest request, long electionId, long circumscriptionId)
        {
            PollingStation ps = null;
            Region region = null;
            Election e = null;
            ElectionRound er = null;
            var subQuery = QueryOver.Of<AssignedPollingStation>()
                .JoinAlias(x => x.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => e)
                .JoinAlias(x => x.PollingStation, () => ps)
                .JoinAlias(() => ps.Region, () => region)
                .Where(x => e.Id == electionId && x.AssignedCircumscription.Id == circumscriptionId)
                .Select(Projections.Distinct(Projections.Property(() => region.Id)));


            return _repository.QueryOver<Region>()
                .Where(Subqueries.WhereProperty<Region>(x => x.Id).In(subQuery))
                .OrderBy(x => x.Name).Asc
                .RootCriteria
                .CreatePage<Region>(request);
        }

        private PageResponse<PollingStation> ListPollingStationsForElection(PageRequest request, long electionId, long? circumscriptionId, long regionId)
        {

            PollingStation ps = null;
            Region region = null;
            Election e = null;
            ElectionRound er = null;
            AssignedCircumscription ac = null;
            var subQuery = QueryOver.Of<AssignedPollingStation>()
                .JoinAlias(x => x.PollingStation, () => ps)
                .JoinAlias(x => x.AssignedCircumscription, () => ac)
                .JoinAlias(x => x.ElectionRound, () => er)
                .JoinAlias(() => er.Election, () => e)
                .JoinAlias(() => ps.Region, () => region)
                .Where(x => e.Id == electionId && ps.Region.Id == regionId && ac.Id == circumscriptionId)
                .Select(Projections.Distinct(Projections.Property(() => ps.Id)));

            return _repository.QueryOver<PollingStation>()
                .Where(Subqueries.WhereProperty<PollingStation>(x => x.Id).In(subQuery))
                .OrderBy(x => x.Number).Asc
                .RootCriteria
                .CreatePage<PollingStation>(request);
        }

        private IList<Election> FilterByPermission(IList<Election> elections, Role restrictedElectionRole)
        {
            var splitedPermissions = restrictedElectionRole.AssignedPermissions.Select(x => x.Permission.Name.Split('~'));
            return elections.Where(x => splitedPermissions.Any(y => IsPermitedElection(x, y))).ToList();
        }

        private bool IsPermitedElection(Election election, string[] permissionParts)
        {
            var permitedElectionDate = DateTime.ParseExact(permissionParts.First(), "dd.MM.yyyy", null);
            if (election.DateOfElection != permitedElectionDate)
            {
                return false;
            }

            var electionComments = election.Comments.ToLower();
            return permissionParts.Skip(1).All(x => electionComments.Contains(x.ToLower()));
        }

        public IList<Region> GetAccessibleCircumscription(long electionId)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var user = _repository.Get<SystemUser>(userId);

            var query =
                _repository.Query<AssignedPollingStation>()
                    .Where(x => x.ElectionRound.Election.Id == electionId);


            if (user.RegionId.HasValue && user.RegionId != -2)
            {
                query = query.Where(x => x.PollingStation.Region.Id == user.RegionId);
            }

            return query.Select(x => x.PollingStation.Region)
                    .Distinct()
                    .ToList();
        }

        public IList<Region> GetAccessibleRegions(long electionId, long regionId)
        {
            var query = _repository.Query<AssignedPollingStation>()
                .Where(x => x.ElectionRound.Election.Id == electionId && x.PollingStation.Region.Id == regionId);

            var userId = SecurityHelper.GetLoggedUserId();
            var user = _repository.Get<SystemUser>(userId);

            if (user.RegionId.HasValue && user.RegionId != -2)
            {
                query = query.Where(x => x.PollingStation.Region.Id == user.RegionId);
            }

            return query.Select(x => x.PollingStation.Region).Distinct().ToList();
        }

        public PageResponse<PollingStation> GetAccessiblePollingStations(PageRequest request, long electionId, long? circumscriptionId, long? regionId)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var systemUser = GetById(userId);

            if (SecurityHelper.LoggedUserIsInRole("Administrator") ||
                (systemUser.RegionId == null && systemUser.RegionId == null && systemUser.PollingStationId == null))
            {
                return ListPollingStationsForElection(request, electionId, circumscriptionId, regionId.GetValueOrDefault());
            }

            if (systemUser.PollingStationId == null || systemUser.PollingStationId == -2)
            {
                return ListPollingStationsForElection(request, electionId, circumscriptionId, regionId.GetValueOrDefault());
            }

            var fixedPollingStation = _repository.Get<PollingStation>(systemUser.GetPollingStationId());
            return new PageResponse<PollingStation>
            {
                Items = new List<PollingStation> { fixedPollingStation },
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }

        private async Task<UserDataDto> MapSystemUser(SystemUser user, bool isAdmin, bool circumscriptionAcces)
        {

            var result = new UserDataDto();
            if (!isAdmin)
            {
                AssignedCircumscription cirscumscription = null;


                var election = _repository.Query<ElectionDay>().FirstOrDefault();
                var region = await _repository.GetAsync<Region>(user.GetRegionId());
                var pollingStation = _repository.Get<PollingStation>(user.GetPollingStationId());
                AssignedPollingStation assignedPollingStation = null;
                if (pollingStation != null)
                {
                    assignedPollingStation = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation.Id == pollingStation.Id && x.NumberPerElection != null);
                }
                if (assignedPollingStation == null && pollingStation != null)
                {
                    assignedPollingStation = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation.Id == pollingStation.Id);
                }


                if (pollingStation != null)
                {
                    cirscumscription = assignedPollingStation?.AssignedCircumscription;

                }
                else
                {
                    cirscumscription = _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.CircumscriptionId == user.CircumscriptionId);
                }

                if (election != null)
                {
                    string constructScrutin = election.ElectionDayDate != null ? election.ElectionDayDate.DateTime.ToShortDateString() + " - " +
                        election.Name : election.Name;
                    result.AssignedElection = new ValueNamePair(election.Id,
                    constructScrutin);
                }

                result.AssignedRegion = new ValueNamePair(user.GetRegionId(),
                    region != null ? region.GetFullPath() : string.Empty);

                if (cirscumscription != null)
                {
                    string constructCircumscription = !string.IsNullOrWhiteSpace(cirscumscription.Number) ? cirscumscription.Number + " - " +
                         cirscumscription.NameRo : cirscumscription.NameRo;

                    result.AssignedCircumscription = new ValueNamePair(cirscumscription.Id, constructCircumscription);

                }
                result.AssignedPollingStation = new ValueNamePair(user.GetPollingStationId(), pollingStation != null ? pollingStation.GetFullName() : string.Empty);

                if (assignedPollingStation != null)
                {
                    result.AssignedPollingStation = new ValueNamePair(user.GetPollingStationId(), string.Format("{0,3:D3} - {1}", assignedPollingStation.NumberPerElection, assignedPollingStation.PollingStation.NameRo));
                }
            }
            result.IsAdmin = isAdmin;
            result.CircumscriptionAcces = circumscriptionAcces;

            return result;
        }
    }
}
