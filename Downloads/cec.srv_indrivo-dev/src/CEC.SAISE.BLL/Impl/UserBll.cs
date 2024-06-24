using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.Constants;
using CEC.SAISE.Domain.TemplateManager;
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
                //var circRegions = _repository.Query<CircumscriptionRegion>().Where(x => x.AssignedCircumscription.Id == userACirc.Id).Select(x => x.Region.Id);
                //var otherACirc = _repository.Query<CircumscriptionRegion>().Where(x => circRegions.Contains(x.Region.Id)).Select(x => x.AssignedCircumscription.Id);
                //var elect = _repository.Query<AssignedCircumscription>().Where(x => x.CircumscriptionId == systemUser.CircumscriptionId || otherACirc.Contains(x.Id)).Select(x => x.ElectionRound.Election).Distinct().ToList();
                var elect = _repository.Query<AssignedCircumscription>().Where(x => x.CircumscriptionId == systemUser.CircumscriptionId).Select(x => x.ElectionRound.Election).Distinct().ToList();
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

        public PageResponse<AssignedCircumscription> GetAccessibleCircumscriptions(PageRequest request, long? electionId)
        {
            var userData = GetCurrentUserData();

            var isAdmin = userData.Result.IsAdmin;
            var isCircumscriptionAccess = userData.Result.CircumscriptionAcces;
            var isAssignedPollingStation = userData.Result.AssignedPollingStation != null;

            AssignedPollingStation assignedPollingStation = null;
            AssignedCircumscription assignedCircumscription;

            if (isAdmin)
            {
                return ListCircumscriptionsForElection(request, electionId.Value);
            }
            if (isCircumscriptionAccess)
            {
                var initialassignedCircumscription = _repository.Query<AssignedCircumscription>()
                    .FirstOrDefault(x => x.Id == userData.Result.AssignedCircumscription.Id);
                assignedCircumscription = _repository.Query<AssignedCircumscription>()
                    .FirstOrDefault(x => x.CircumscriptionId == initialassignedCircumscription.CircumscriptionId && x.ElectionRound.Election.Id == electionId.Value);
                return new PageResponse<AssignedCircumscription>
                {
                    Items = new List<AssignedCircumscription> { assignedCircumscription },
                    PageSize = 1,
                    StartIndex = 0,
                    Total = 1
                };
            }
            else if (isAssignedPollingStation)
            {
                assignedPollingStation = _repository.Query<AssignedPollingStation>()
                    .FirstOrDefault(x => x.PollingStation.Id == userData.Result.AssignedPollingStation.Id);
            }

            var fixedCircumscription = assignedPollingStation != null 
                ? _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.CircumscriptionId == assignedPollingStation.AssignedCircumscription.CircumscriptionId) 
                : null;
            return new PageResponse<AssignedCircumscription>
            {
                Items = new List<AssignedCircumscription> { fixedCircumscription },
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }
        public PageResponse<Region> GetAccessibleRegions(PageRequest request, long electionId, long? assignedcircumscriptionId)
        {
            var userData = GetCurrentUserData();

            var isAdmin = userData.Result.IsAdmin;
            var isCircumscriptionAccess = userData.Result.CircumscriptionAcces;
            var isAssignedPollingStation = userData.Result.AssignedPollingStation != null;
            AssignedPollingStation assignedPollingStation = null;
            AssignedCircumscription assignedCircumscription;

            if (isAdmin)
            {
                return ListRegionsForElection(request, electionId, assignedcircumscriptionId.GetValueOrDefault());
            }
            if (isCircumscriptionAccess)
            {
                assignedCircumscription = _repository.Query<AssignedCircumscription>()
                    .FirstOrDefault(x => x.Id == userData.Result.AssignedCircumscription.Id);

                return new PageResponse<Region>
                {
                    Items = new List<Region> {  },
                    PageSize = 1,
                    StartIndex = 0,
                    Total = 1
                };
            }
            else if (isAssignedPollingStation)
            {
                assignedPollingStation = _repository.Query<AssignedPollingStation>()
                    .FirstOrDefault(x => x.PollingStation.Id == userData.Result.AssignedPollingStation.Id);
                
            }
            var fixedCircumscription = assignedPollingStation != null ? _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.CircumscriptionId == assignedPollingStation.AssignedCircumscription.CircumscriptionId) : null;
            var fixedRegion = _repository.Get<Region>(fixedCircumscription.Region.Id);
            return new PageResponse<Region>
            {
                Items = new List<Region> { fixedRegion },
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }
        public PageResponse<Region> GetAccessibleRegionsOld(PageRequest request, long electionId, long? regionId)
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
                AssignedCircumscription circumscription = null;


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
                    circumscription = assignedPollingStation?.AssignedCircumscription;

                }
                else
                {
                    circumscription = _repository.Query<AssignedCircumscription>().FirstOrDefault(x => x.CircumscriptionId == user.CircumscriptionId);
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

                if (circumscription != null)
                {
                    string constructCircumscription = !string.IsNullOrWhiteSpace(circumscription.Number) ? circumscription.Number + " - " +
                         circumscription.NameRo : circumscription.NameRo;

                    result.AssignedCircumscription = new ValueNamePair(circumscription.Id, constructCircumscription);

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

        public PageResponse<TemplateNameDto> GetAccessibleTemplateNames(PageRequest request, long electionId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var userData = GetCurrentUserData();
            var electionRound = GetElectionRound(electionId);
            if (electionRound == null)
            {
                // Handle the case where electionRound is not found
                return null;
            }
            //Temporary solution for testing purspose. Remove the if case after testing
            //if (userData.Result.IsAdmin)
            //{
            //    TemplateName tns = null;

            //    var templatess = _repository.QueryOver<Template>()
            //        .JoinAlias(x => x.TemplateName, () => tns)
            //        .OrderBy(x => tns.Id).Asc
            //        .SelectList(list => list
            //            .Select(() => tns.Id)
            //            .Select(() => tns.Title)
            //        )
            //        .List<object[]>();

            //    var templateNamess = templatess.Select(result =>
            //        new TemplateNameDto
            //        {
            //            Id = (long)result[0],
            //            Title = (string)result[1]
            //        }).ToList();

            //    return new PageResponse<TemplateNameDto>
            //    {
            //        Items = templateNamess,
            //        PageSize = 1,
            //        StartIndex = 0,
            //        Total = 1
            //    };
            //}


            var templateMappings = GetTemplateMappingUser(electionRound);
            if (templateMappings == null)
            {
                return new PageResponse<TemplateNameDto>();
            }

            var templateNameIds = templateMappings
                    .Select(x => x.TemplateName.Id)
                    .Distinct()
                    .ToList();

            TemplateName tn = null;

            var templates = _repository.QueryOver<Template>()
                .JoinAlias(x => x.TemplateName, () => tn)
                .WhereRestrictionOn(() => tn.Id).IsIn(templateNameIds)
                .OrderBy(x => tn.Id).Asc
                .SelectList(list => list
                    .Select(() => tn.Id)
                    .Select(() => tn.Title)
                )
                .List<object[]>();

            var templateNames = templates.Select(result =>
                new TemplateNameDto
                {
                    Id = (long)result[0],
                    Title = (string)result[1]
                }).ToList();

            return new PageResponse<TemplateNameDto>
            {
                Items = templateNames,
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }

        private List<TemplateMapping> GetTemplateMappingUser(ElectionRound electionRound)
        {
            var userData = GetCurrentUserData();

            var isAdmin = userData.Result.IsAdmin;
            var isCircumscriptionAccess = userData.Result.CircumscriptionAcces;
            var isAssignedPollingStation = userData.Result.AssignedPollingStation != null;
            AssignedPollingStation assignedPollingStation = null;
            if (isAssignedPollingStation)
            {
                assignedPollingStation = _repository.Query<AssignedPollingStation>()
                    .FirstOrDefault(x => x.PollingStation.Id == userData.Result.AssignedPollingStation.Id);
            }

            if (isAdmin)
            {
                return _repository.Query<TemplateMapping>()
                    .Where(x => x.ElectionTypeCode == electionRound.Election.Type.Code
                             && x.ElectionRoundCode == electionRound.Number)
                    .ToList();
            }
            else if (isCircumscriptionAccess)
            {
                var hasPollingStationsWith2Days = _repository.Query<AssignedPollingStation>()
                    .Any(x => x.AssignedCircumscription.Id == userData.Result.AssignedCircumscription.Id
                           && x.ElectionDuration.Name.Equals(Constants.ElectionDuration.Days2));

                return _repository.Query<TemplateMapping>()
                    .Where(x => x.IsCECE == true
                        && x.ElectionRoundCode == electionRound.Number
                        && x.ElectionTypeCode == electionRound.Election.Type.Code
                        && (hasPollingStationsWith2Days ? x.TwoDayFirstDay || x.TwoDaySecondDay : x.OneDay)).ToList();
            }

            else if (isAssignedPollingStation && assignedPollingStation.ElectionDuration != null)
            {
                if (assignedPollingStation.ElectionDuration.Name.Equals(Constants.ElectionDuration.Days1))
                {
                    return _repository.Query<TemplateMapping>()
                        .Where(x => x.ElectionTypeCode == electionRound.Election.Type.Code
                                 && x.ElectionRoundCode == electionRound.Number
                                 && x.IsCECE == false
                                 && x.OneDay).ToList();
                }

                return _repository.Query<TemplateMapping>()
                    .Where(x => x.IsCECE == false
                        && x.ElectionRoundCode == electionRound.Number
                        && x.ElectionTypeCode == electionRound.Election.Type.Code
                        && (x.TwoDayFirstDay || x.TwoDaySecondDay)).ToList();
            }

            return null;
        }

        public PageResponse<TemplateNameDto> GetAccessibleTemplateNamesGrid(PageRequest request, long electionId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var userData = GetCurrentUserData();
            var electionRound = GetElectionRound(electionId);
            if (electionRound == null)
            {
                // Handle the case where electionRound is not found
                return null;
            }

            var templateMappings = GetTemplateMappingUserGrid(electionRound);
            if (templateMappings == null)
            {
                return new PageResponse<TemplateNameDto>();
            }

            var templateNameIds = templateMappings
                    .Select(x => x.TemplateName.Id)
                    .Distinct()
                    .ToList();

            TemplateName tn = null;

            var templates = _repository.QueryOver<Template>()
                .JoinAlias(x => x.TemplateName, () => tn)
                .WhereRestrictionOn(() => tn.Id).IsIn(templateNameIds)
                .OrderBy(x => tn.Id).Asc
                .SelectList(list => list
                    .Select(() => tn.Id)
                    .Select(() => tn.Title)
                )
                .List<object[]>();

            var templateNames = templates.Select(result =>
                new TemplateNameDto
                {
                    Id = (long)result[0],
                    Title = (string)result[1]
                }).ToList();

            return new PageResponse<TemplateNameDto>
            {
                Items = templateNames,
                PageSize = 1,
                StartIndex = 0,
                Total = 1
            };
        }

        private List<TemplateMapping> GetTemplateMappingUserGrid(ElectionRound electionRound)
        {
            var userData = GetCurrentUserData();

            var isAdmin = userData.Result.IsAdmin;
            var isCircumscriptionAccess = userData.Result.CircumscriptionAcces;
            var isAssignedPollingStation = userData.Result.AssignedPollingStation != null;
            AssignedPollingStation assignedPollingStation = null;
            if (isAssignedPollingStation)
            {
                assignedPollingStation = _repository.Query<AssignedPollingStation>()
                    .FirstOrDefault(x => x.PollingStation.Id == userData.Result.AssignedPollingStation.Id);
            }

            if (isAdmin)
            {
                return _repository.Query<TemplateMapping>()
                    .Where(x => x.ElectionTypeCode == electionRound.Election.Type.Code
                             && x.ElectionRoundCode == electionRound.Number)
                    .ToList();
            }
            else if (isCircumscriptionAccess)
            {
                var hasPollingStationsWith2Days = _repository.Query<AssignedPollingStation>()
                    .Any(x => x.AssignedCircumscription.Id == userData.Result.AssignedCircumscription.Id
                           && x.ElectionDuration.Name.Equals(Constants.ElectionDuration.Days2));

                return _repository.Query<TemplateMapping>()
                    .Where(x => x.IsCECE == false
                        && x.ElectionRoundCode == electionRound.Number
                        && x.ElectionTypeCode == electionRound.Election.Type.Code
                        && (hasPollingStationsWith2Days ? x.TwoDayFirstDay || x.TwoDaySecondDay : x.OneDay)).ToList();
            }

            return null;
        }

        private ElectionRound GetElectionRound(long electionId)
        {
            return _repository.Query<ElectionRound>()
                    .Where(x => x.Election.Id == electionId).FirstOrDefault();
        }
    }
}
