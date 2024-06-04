using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CEC.SRV.BLL.Impl
{
    public class LookupBll : Bll, ILookupBll, IStreetTypeCache,
        IRegionCache, IPollingStationCache, IStreetTypeCodeCache, ILinkedRegionCache
    {

        public LookupBll(ISRVRepository repository)
            : base(repository)
        {

        }

        public void Update<T>(long id, string name, string description) where T : Lookup
        {
            var entity = Get<T>(id);
            entity.Name = name;
            entity.Description = description;

            Repository.SaveOrUpdate(entity);
        }

        public void Add<T>(string name, string description) where T : Lookup, new()
        {
            var entity = new T();
            entity.Name = name;
            entity.Description = description;

            Repository.SaveOrUpdate(entity);
        }

        public void SaveOrUpdate<T>(long? id, string name, string description) where T : Lookup, new()
        {
            var entity = id == null ? new T() : Get<T>((long)id);
            entity.Name = name;
            entity.Description = description;
            Repository.SaveOrUpdate(entity);
        }

        public void SaveOrUpdateDocType(long? id, string name, string description, bool isPrimary)
        {
            var entity = id == null ? new DocumentType() : Get<DocumentType>((long)id);
            if (isPrimary && !entity.IsPrimary)
            {
                RemoveOtherPrimaryDocs();
            }

            entity.Name = name;
            entity.Description = description;
            entity.IsPrimary = isPrimary;
            Repository.SaveOrUpdate(entity);
        }

        public void SaveOrUpdatePersonStatus(long? id, string name, string description, bool isExcludable)
        {
            var entity = id == null ? new PersonStatusType() : Get<PersonStatusType>((long)id);
            entity.Name = name;
            entity.Description = description;
            entity.IsExcludable = isExcludable;
            Repository.SaveOrUpdate(entity);
        }


        public void SaveOrUpdateRegionType(long? id, string name, string description, byte rank)
        {
            var entity = id == null ? new RegionType() : Get<RegionType>((long)id);
            entity.Name = name;
            entity.Description = description;
            entity.Rank = rank;
            Repository.SaveOrUpdate(entity);
        }

        public void UpdatePersonStatus(long id, string name, string description, bool isExcludable)
        {
            var entity = Get<PersonStatusType>(id);
            entity.Name = name;
            entity.Description = description;
            entity.IsExcludable = isExcludable;

            Repository.SaveOrUpdate(entity);
        }

        public void DeleteRegion(long id)
        {
            var region = Get<Region>(id);
            var street = Repository.Query<Street>().Where(x => x.Region.Id == id && x.Deleted == null).ToList();
            var pollingStation =
                Repository.Query<PollingStation>().Where(x => x.Region.Id == id && x.Deleted == null).ToList();

            if (region.Children.Count > 0 && region.Children.Any(x => x.Deleted == null))
            {
                throw new SrvException("Error_SubRegionsChildren", MUI.Error_SubRegionsChildren);
            }

            if (street.Count > 0)
            {
                throw new SrvException("Error_RegionsHasStreet", MUI.Error_RegionsHasStreet);
            }

            if (pollingStation.Count > 0)
            {
                throw new SrvException("Error_RegionsHasPollingStation", MUI.Error_RegionsHasPollingStation);
            }

            Delete<Region>(id);
        }

        public PageResponse<StreetDto> GetStreets(PageRequest pageRequest, long regionId)
        {
            if (IsRegionAccessibleToCurrentUser(regionId))
            {
                pageRequest.SortFields.Add(new SortField { Ascending = true, Property = "Id" });

                StreetDto streetDto = null;
                StreetWithCountOfAddresses streetGrid = null;
                StreetType streetType = null;
                Region region = null;
                Street street = null;
                IdentityUser createdBy = null;
                IdentityUser modifiedBy = null;
                IdentityUser deletedBy = null;

                return Repository.QueryOver(() => streetGrid)
                    .JoinAlias(x => streetGrid.Street.StreetType, () => streetType)
                    .JoinAlias(x => streetGrid.Street.Region, () => region)
                    .JoinAlias(x => streetGrid.Street, () => street)
                    .JoinAlias(x => street.DeletedBy, () => deletedBy, JoinType.LeftOuterJoin)
                    .JoinAlias(x => street.CreatedBy, () => createdBy, JoinType.LeftOuterJoin)
                    .JoinAlias(x => street.ModifiedBy, () => modifiedBy, JoinType.LeftOuterJoin)
                    .Where(() => region.Id == regionId)
                    .Select(Projections.ProjectionList()
                        .Add(Projections.Property<StreetWithCountOfAddresses>(x => x.Id).WithAlias(() => streetDto.Id))
                        .Add(
                            Projections.Property<Street>(x => street.Name)
                                .WithAlias(() => streetDto.Name))
                        .Add(
                            Projections.Property<Street>(x => street.Description)
                                .WithAlias(() => streetDto.Description))
                        .Add(Projections.Property<StreetType>(x => streetType.Name)
                            .WithAlias(() => streetDto.StreetType))
                        .Add(Projections.Property<StreetType>(x => streetType.Id)
                            .WithAlias(() => streetDto.StreetTypeId))
                        .Add(
                            Projections.Property<StreetWithCountOfAddresses>(x => x.HousesCount)
                                .WithAlias(() => streetDto.HousesCount))
                        .Add(
                            Projections.Property<Street>(x => street.Created)
                                .WithAlias(() => streetDto.Created))
                        .Add(
                            Projections.Property<Street>(x => street.Modified)
                                .WithAlias(() => streetDto.Modified))
                        .Add(
                            Projections.Property<Street>(x => street.Deleted)
                                .WithAlias(() => streetDto.Deleted))
                        .Add(
                            Projections.Property<IdentityUser>(x => createdBy.UserName)
                                .WithAlias(() => streetDto.CreatedBy))
                        .Add(
                            Projections.Property<IdentityUser>(x => modifiedBy.UserName)
                                .WithAlias(() => streetDto.ModifiedBy))
                        .Add(
                            Projections.Property<IdentityUser>(x => deletedBy.UserName)
                                .WithAlias(() => streetDto.DeletedBy))
                        ).TransformUsing(Transformers.AliasToBean<StreetDto>())

                    .RootCriteria
                    .CreatePage<StreetDto>(pageRequest);
            }

            return new PageResponse<StreetDto>
            {
                Items = new List<StreetDto>(),
                PageSize = pageRequest.PageSize,
                Total = 0
            };
        }

        public PageResponse<Circumscription> GetCircumscriptions(PageRequest pageRequest)
        {
            return Repository.QueryOver<Circumscription>()
               //.Where(x => x.Parent.Id == 1)
               .Where(x => x.Deleted == null)
               .OrderBy(x => x.Number).Asc
               .TransformUsing(Transformers.DistinctRootEntity)
               .RootCriteria
               .CreatePage<Circumscription>(pageRequest);
        }

        public IEnumerable<Circumscription> GetCircumscriptions()
        {
            //x.Parent.Id == 1 &&
            return Repository.QueryOver<Circumscription>()
                .Where(x => x.Deleted == null)
                .List();
        }

        public PageResponse<RegionRow> GetRegions(PageRequest pageRequest, long parentRegionId)
        {
            RegionRow regionRow = null;
            Region region = null;
            Region region1 = null;
            LinkedRegion linkedRegion = null;
            IdentityUser createdBy = null;
            IdentityUser modifiedBy = null;
            IdentityUser deletedBy = null;

            var hasLinkedRegionsSubQuery =
                QueryOver.Of(() => linkedRegion)
                    .JoinAlias(() => linkedRegion.Regions, () => region1)
                    .Where(() => region1.Id == region.Id && linkedRegion.Deleted == null)
                    .Select(Projections.RowCount());

            return Repository.QueryOver(() => region)
                .JoinAlias(x => region.DeletedBy, () => deletedBy, JoinType.LeftOuterJoin)
                .JoinAlias(x => region.CreatedBy, () => createdBy, JoinType.LeftOuterJoin)
                .JoinAlias(x => region.ModifiedBy, () => modifiedBy, JoinType.LeftOuterJoin)
                .Where(() => region.Parent.Id == parentRegionId)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property<Region>(x => region.Id).WithAlias(() => regionRow.Id))
                    .Add(Projections.Property<Region>(x => region.Name).WithAlias(() => regionRow.Name))
                    .Add(Projections.Property<Region>(x => region.Description).WithAlias(() => regionRow.Description))
                    .Add(Projections.Property<Region>(x => region.Parent.Id).WithAlias(() => regionRow.ParentId))
                    .Add(Projections.Property<RegionType>(x => region.RegionType).WithAlias(() => regionRow.RegionType))
                    .Add(Projections.Property<Region>(x => region.HasStreets).WithAlias(() => regionRow.HasStreets))
                    .Add(Projections.Property<Region>(x => region.SaiseId).WithAlias(() => regionRow.SaiseId))
                    .Add(Projections.Property<Region>(x => region.RegistruId).WithAlias(() => regionRow.RegistruId))
                    .Add(Projections.Property<Region>(x => region.StatisticIdentifier).WithAlias(() => regionRow.Cuatm))
                    .Add(Projections.Property<Region>(x => region.Created).WithAlias(() => regionRow.Created))
                    .Add(Projections.Property<Region>(x => region.Modified).WithAlias(() => regionRow.Modified))
                    .Add(Projections.Property<Region>(x => region.Deleted).WithAlias(() => regionRow.Deleted))
                    .Add(Projections.Property<IdentityUser>(x => createdBy.UserName).WithAlias(() => regionRow.CreatedBy))
                    .Add(Projections.Property<IdentityUser>(x => modifiedBy.UserName).WithAlias(() => regionRow.ModifiedBy))
                    .Add(Projections.Property<IdentityUser>(x => deletedBy.UserName).WithAlias(() => regionRow.DeletedBy))
                    .Add(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(hasLinkedRegionsSubQuery), 0),
                        Projections.Constant(false, NHibernateUtil.Boolean),
                        Projections.Constant(true, NHibernateUtil.Boolean)).WithAlias(() => regionRow.HasLinkedRegions))
                ).TransformUsing(Transformers.AliasToBean<RegionRow>())
                .RootCriteria
                .CreatePage<RegionRow>(pageRequest);

            //return Repository.QueryOver<Region>()
            //    .Fetch(x => x.RegionType).Eager
            //    .Where(x => x.Parent.Id == parentRegionId)
            //    .RootCriteria
            //    .CreatePage<Region>(pageRequest);
        }

        public PageResponse<RegionRow> GetRegionsGrid(PageRequest pageRequest)
        {
            RegionRow regionRow = null;
            RegionWithFullyQualifiedName regionWithParentUrl = null;
            Region region = null;
            Region region1 = null;
            LinkedRegion linkedRegion = null;
            IdentityUser createdBy = null;
            IdentityUser modifiedBy = null;
            IdentityUser deletedBy = null;

            var hasLinkedRegionsSubQuery =
                QueryOver.Of(() => linkedRegion)
                    .JoinAlias(() => linkedRegion.Regions, () => region1)
                    .Where(() => region1.Id == region.Id && linkedRegion.Deleted == null)
                    .Select(Projections.RowCount());

            return Repository.QueryOver(() => regionWithParentUrl)
                .JoinAlias(x => regionWithParentUrl.Region, () => region)
                .JoinAlias(x => region.DeletedBy, () => deletedBy, JoinType.LeftOuterJoin)
                .JoinAlias(x => region.CreatedBy, () => createdBy, JoinType.LeftOuterJoin)
                .JoinAlias(x => region.ModifiedBy, () => modifiedBy, JoinType.LeftOuterJoin)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property<RegionWithFullyQualifiedName>(x => region.Id)
                        .WithAlias(() => regionRow.Id))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.Name)
                            .WithAlias(() => regionRow.Name))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => regionWithParentUrl.FullyQualifiedName)
                            .WithAlias(() => regionRow.FullyQualifiedName))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.Description)
                            .WithAlias(() => regionRow.Description))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.Parent.Id)
                            .WithAlias(() => regionRow.ParentId))
                        .Add(Projections.Property<RegionType>(x => region.RegionType).WithAlias(() => regionRow.RegionType))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.HasStreets)
                            .WithAlias(() => regionRow.HasStreets))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.SaiseId)
                            .WithAlias(() => regionRow.SaiseId))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.RegistruId)
                            .WithAlias(() => regionRow.RegistruId))
                    .Add(
                        Projections.Property<RegionWithFullyQualifiedName>(x => region.StatisticIdentifier)
                            .WithAlias(() => regionRow.Cuatm))
                    .Add(
                        Projections.Property<StreetWithCountOfAddresses>(x => region.Created)
                            .WithAlias(() => regionRow.Created))
                    .Add(
                        Projections.Property<StreetWithCountOfAddresses>(x => region.Modified)
                            .WithAlias(() => regionRow.Modified))
                    .Add(
                        Projections.Property<StreetWithCountOfAddresses>(x => region.Deleted)
                            .WithAlias(() => regionRow.Deleted))
                    .Add(
                        Projections.Property<IdentityUser>(x => createdBy.UserName)
                            .WithAlias(() => regionRow.CreatedBy))
                    .Add(
                        Projections.Property<IdentityUser>(x => modifiedBy.UserName)
                            .WithAlias(() => regionRow.ModifiedBy))
                    .Add(
                        Projections.Property<IdentityUser>(x => deletedBy.UserName)
                            .WithAlias(() => regionRow.DeletedBy))
                    .Add(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(hasLinkedRegionsSubQuery), 0),
                        Projections.Constant(false, NHibernateUtil.Boolean),
                        Projections.Constant(true, NHibernateUtil.Boolean)).WithAlias(() => regionRow.HasLinkedRegions))
                        ).TransformUsing(Transformers.AliasToBean<RegionRow>())
                    .RootCriteria
                    .CreatePage<RegionRow>(pageRequest);
        }

        private void RemoveOtherPrimaryDocs()
        {
            Repository.Query<DocumentType>()
                .Where(x => x.IsPrimary)
                .ForEach(x =>
                {
                    x.IsPrimary = false;
                    Repository.SaveOrUpdate(x);
                });
        }

        public void CreateUpdateStreet(long id, string name, string description, long regionId, long streetTypeId,
            long? ropId, long? saiseId)
        {
            var streetType = Repository.LoadProxy<StreetType>(streetTypeId);
            var region = Repository.LoadProxy<Region>(regionId);

            var street = id == 0 ? new Street(region, streetType, name) : Get<Street>(id);
            street.Name = name;
            street.StreetType = streetType;
            street.Description = description;
            street.RopId = ropId;
            street.SaiseId = saiseId;
            Repository.SaveOrUpdate(street);
        }

        public void CreateUpdateRegion(long id, string name, string description, long? parentId, long regionTypeId,
            bool hasStreets, long? saiseId, long? cuatm)
        {
            var regionType = Repository.LoadProxy<RegionType>(regionTypeId);
            var parentRegion = parentId != null ? Repository.LoadProxy<Region>(parentId.Value) : null;

            Region region;
            if (id == 0)
            {
                region = new Region(parentRegion, regionType);
            }
            else
            {
                region = Get<Region>(id);
                try
                {
                    region.ChangeParent(parentRegion);
                    region.ChangeRegionType(regionType);
                }
                catch (NotSupportedException nse)
                {
                    throw new SrvException("", nse.Message, nse);
                }
            }

            region.Name = name;
            region.Description = description;
            region.HasStreets = hasStreets;
            region.SaiseId = saiseId;
            region.StatisticIdentifier = cuatm;

            Repository.SaveOrUpdate(region);
        }

        public void UpdateAdministrativeInfo(long id, string name, string surname, long regionId, long managerTypeId)
        {
            var managerType = Repository.LoadProxy<ManagerType>(managerTypeId);
            var region = Repository.LoadProxy<Region>(regionId);

            var publicAdministration = id == 0
                ? new PublicAdministration(region, managerType)
                : Get<PublicAdministration>(id);

            publicAdministration.Name = name;
            publicAdministration.Surname = surname;
            publicAdministration.ManagerType = managerType;

            Repository.SaveOrUpdate(publicAdministration);
        }

        public bool IsUnique(long id, string name, long parentId, long regionTypeId)
        {
            var region =
                Repository.Query<Region>()
                    .FirstOrDefault(x => x.Name == name && x.Parent.Id == parentId && x.RegionType.Id == regionTypeId);
            return region == null || region.Id == id;
        }

        public bool IsUnique(long id, long regionId, string name, long streetTypeId)
        {
            var street =
                Repository.Query<Street>()
                    .FirstOrDefault(x => x.Name == name && x.Region.Id == regionId && x.StreetType.Id == streetTypeId);

            return street == null || street.Id == id;
        }

        public bool IsUnique<T>(long? id, string name) where T : Lookup, new()
        {
            var entity = GetByName<T>(name);
            return (entity == null || entity.Id == id);
        }

        public IEnumerable<Region> GetRegionsOfLevel1ByFilter(string regionNameFilter)
        {
            return Repository.Query<Region>()
                .Where(r => r.Parent == null && r.Deleted == null)
                .ToList()
                .Aggregate(new List<Region>(),
                    (current, next) => current.Concat(
                        GetRegionsByParentIdAndFilter(next.Id, regionNameFilter)
                      ).ToList());
        }

        public IEnumerable<Region> GetRegionsByParentIdAndFilter(long parentId, string regionNameFilter)
        {
            return Repository.Query<Region>()
                .Fetch(x => x.RegionType)
                .Where(
                    x =>
                        (x.Parent != null) && (x.Parent.Id == parentId || (x.Id == parentId && x.HasStreets)) && x.Name.Contains(regionNameFilter) &&
                        x.Deleted == null)
                .OrderBy(x => x.RegionType.Name)
                .ThenBy(x => x.Name);
        }

        public IEnumerable<PollingStation> GetPollingStationsHierarhicallyByRegion(long regionId)
        {
            List<PollingStation> pollingStations = Repository.Query<PollingStation>()
                .Where(x => (x.Region != null) && (x.Region.Id == regionId) && (x.Deleted == null)).ToList();

            List<long> childRegionIds =
                Repository.Query<Region>()
                    .Where(x => (x.Parent != null) && (x.Parent.Id == regionId))
                    .Select(y => y.Id)
                    .ToList();

            List<PollingStation> childPollingStations = childRegionIds.Aggregate(new List<PollingStation>(),
                (current, next) => current.Concat(
                    this.GetPollingStationsHierarhicallyByRegion(next)).ToList());

            if (childPollingStations.Count > 0)
            {
                return pollingStations.Concat(childPollingStations);
            }
            else
            {
                return pollingStations.OrderBy(x => x.Number);
            }
        }

        public IEnumerable<Street> GetStreetsHierarhicallyByRegionAndFilter(long regionId, string streetNameFilter)
        {
            List<Street> streets = Repository.Query<Street>()
                .Fetch(x => x.StreetType)
                .Fetch(x => x.Region)
                .ThenFetch(x => x.RegionType)
                .Where(
                    x =>
                        (x.Region != null) && (x.Region.Id == regionId) && x.Name.Contains(streetNameFilter) &&
                        x.Deleted == null)
                .ToList();

            List<long> childRegionIds =
                Repository.Query<Region>()
                    .Where(x => (x.Parent != null) && (x.Parent.Id == regionId))
                    .Select(y => y.Id)
                    .ToList();

            List<Street> childStreets = childRegionIds.Aggregate(new List<Street>(),
                (current, next) => current.Concat(
                    this.GetStreetsHierarhicallyByRegionAndFilter(next, streetNameFilter)).ToList());

            return streets.Concat(childStreets);
        }

        public void VerificationRegion(long id)
        {
            var region = Get<Region>(id);

            if (!region.HasStreets)
            {
                throw new SrvException("Lookups_RegionNotAcceptStreets", MUI.Lookups_RegionNotAcceptStreets);
            }

            if (region.Deleted != null)
            {
                throw new SrvException("Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
            }
        }

        public void VerificationIfManagerTypeHasReference(long managerTypeId)
        {
            var publicAdministration = Repository.Query<PublicAdministration>().FirstOrDefault(x => x.ManagerType.Id == managerTypeId && x.Deleted == null);
            if (publicAdministration != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                publicAdministration.ManagerType.GetObjectType(), publicAdministration.GetObjectType()));
            }
        }

        public void VerificationIfPersonStatusHasReference(long personStatusId)
        {
            var personStatus = Repository.Query<PersonStatus>().FirstOrDefault(x => x.StatusType.Id == personStatusId && x.Deleted == null);
            if (personStatus != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                personStatus.StatusType.GetObjectType(), personStatus.GetObjectType()));
            }
        }

        public void VerificationIfGenderHasReference(long genderId)
        {
            var additionalUserInfo = Repository.Query<AdditionalUserInfo>().FirstOrDefault(x => x.Gender.Id == genderId && x.Deleted == null);
            if (additionalUserInfo != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                additionalUserInfo.Gender.GetObjectType(), additionalUserInfo.GetObjectType()));
            }

            var person = Repository.Query<Person>().FirstOrDefault(x => x.Gender.Id == genderId && x.Deleted == null);
            if (person != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                person.Gender.GetObjectType(), person.GetObjectType()));
            }
        }

        public void VerificationIfDocTypeHasReference(long docTypeId)
        {
            var person = Repository.Query<Person>().FirstOrDefault(x => x.Document.Type.Id == docTypeId && x.Deleted == null);
            if (person != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                person.Document.Type.GetObjectType(), person.GetObjectType()));
            }
        }

        public void VerificationIfElectionTypeHasReference(long electionTypeId)
        {
            var election = Repository.Query<Election>().FirstOrDefault(x => x.ElectionType.Id == electionTypeId && x.Deleted == null);
            if (election != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                election.ElectionType.GetObjectType(), election.GetObjectType()));
            }
        }

        public void VerificationIfPersonAddressTypeHasReference(long personAddressId)
        {
            var personAddress = Repository.Query<PersonAddress>().FirstOrDefault(x => x.PersonAddressType.Id == personAddressId && x.Deleted == null);
            if (personAddress != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                personAddress.PersonAddressType.GetObjectType(), personAddress.GetObjectType()));
            }
        }

        public void VerificationIfRegionTypeHasReference(long regionTypeId)
        {
            var region = Repository.Query<Region>().FirstOrDefault(x => x.RegionType.Id == regionTypeId && x.Deleted == null);
            if (region != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                region.RegionType.GetObjectType(), region.GetObjectType()));
            }
        }

        public void VerificationIfRegionHasReference(long regionId)
        {
            var rsaUser = Repository.Query<RsaUser>().FirstOrDefault(x => x.Region.Id == regionId);
            if (rsaUser != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                rsaUser.Region.GetObjectType(), rsaUser.GetObjectType()));
            }

            var street = Repository.Query<Street>().FirstOrDefault(x => x.Region.Id == regionId && x.Deleted == null);
            if (street != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                street.Region.GetObjectType(), street.GetObjectType()));
            }

            var pollingStation = Repository.Query<PollingStation>().FirstOrDefault(x => x.Region.Id == regionId && x.Deleted == null);
            if (pollingStation != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                pollingStation.Region.GetObjectType(), pollingStation.GetObjectType()));
            }

            var publicAdministration = Repository.Query<PublicAdministration>().FirstOrDefault(x => x.Region.Id == regionId && x.Deleted == null);
            if (publicAdministration != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                publicAdministration.Region.GetObjectType(), publicAdministration.GetObjectType()));
            }
        }

        public void VerificationIfStreetTypeHasReference(long streetTypeId)
        {
            var streetType = Repository.Query<Street>().FirstOrDefault(x => x.StreetType.Id == streetTypeId && x.Deleted == null);
            if (streetType != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                streetType.StreetType.GetObjectType(), streetType.GetObjectType()));
            }
        }

        public void VerificationIfStreetHasReference(long streetId)
        {
            var street = Repository.Query<Address>().FirstOrDefault(x => x.Street.Id == streetId && x.Deleted == null);
            if (street != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                street.Street.GetObjectType(), street.GetObjectType()));
            }
        }

        public void VerificationIfConflictShareReasonHasReference(long conflictShareReasonId)
        {

            var publicAdministration = Repository.Query<PublicAdministration>().FirstOrDefault(x => x.ManagerType.Id == conflictShareReasonId && x.Deleted == null);
            if (publicAdministration != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                publicAdministration.ManagerType.GetObjectType(), publicAdministration.GetObjectType()));
            }
        }

        public void UpdateCircumscription(long id, int? circumscriptionNumber)
        {
            var entity = Get<Region>(id);
            entity.Circumscription = circumscriptionNumber;
            Repository.SaveOrUpdate(entity);
        }

        public bool UniqueValidationCircumscription(long regionId, int? circumscription)
        {
            var region = Repository.Query<Region>().FirstOrDefault(x => x.Circumscription == circumscription);
            return region == null || region.Id == regionId;
        }

        public void DeleteStreet(long streetId)
        {
            var address = Repository.Query<Address>().FirstOrDefault(x => x.Street.Id == streetId && x.Deleted == null);
            if (address != null)
            {
                throw new SrvException("Street_NotPermittedDeleting", MUI.Street_NotPermittedDeleting);
            }

            Delete<Street>(streetId);
        }

        public PageResponse<RegionWithFullyQualifiedName> GetAvailableRegions(PageRequest pageRequest)
        {
            Region region = null;
            LinkedRegion linkedRegion = null;
            RegionWithFullyQualifiedName regionWithParentUrl = null;

            var regions = Repository.QueryOver(() => regionWithParentUrl)
                .JoinAlias(() => regionWithParentUrl.Region, () => region)
                .Where(() => region.Deleted == null)
                .RootCriteria
                .CreatePage<RegionWithFullyQualifiedName>(pageRequest);

            return regions;
        }

        public IList<LinkedRegionsFullName> GetLinkedRegionsByRegionId(long regionId)
        {
            LinkedRegionsFullName linkedRegionFullName = null;
            IList<LinkedRegionsFullName> linkedRegions = new List<LinkedRegionsFullName>();
            var linkedRegion = GetLinkedRegion(regionId);
            if (linkedRegion != null)
            {
                linkedRegions = Repository.QueryOver(() => linkedRegionFullName)
                    .Where(() => linkedRegionFullName.LinkedRegion.Id == linkedRegion.Id)
                    .List()
                    ;
            }

            return linkedRegions;
        }

        private LinkedRegion GetLinkedRegion(long regionId)
        {
            LinkedRegion linkedRegion = null;
            Region region = null;
            return Repository.QueryOver(() => linkedRegion)
                .JoinAlias(() => linkedRegion.Regions, () => region)
                .Where(x => region.Id == regionId && linkedRegion.Deleted == null)
                .SingleOrDefault();
        }

        public void SaveLinkedRegions(long regionId, long[] linkedRegionIds)
        {
            var regions = linkedRegionIds.Select(regId => Repository.Get<Region>(regId)).ToList();
            var linkedRegion = GetLinkedRegion(regionId);
            if (linkedRegion != null)
            {
                Repository.Delete(linkedRegion);
            }
            if (linkedRegionIds.Length > 1)
            {
                var linkedRegions = new LinkedRegion(regions);
                Repository.SaveOrUpdate(linkedRegions);
            }
        }

        public RegionWithFullyQualifiedName GetFullyQualifiedName(long regionId)
        {
            return Repository.QueryOver<RegionWithFullyQualifiedName>()
                .Where(x => x.Region.Id == regionId)
                .SingleOrDefault();
        }

        IEnumerable<StreetType> ICache<StreetType>.GetAll()
        {
            return GetAll<StreetType>();
        }

        [Obsolete]
        IEnumerable<Region> IRegionCache.GetByRegistruId(long getAdministrativeCode)
        {
            return Repository.Query<Region>().Where(x => x.RegistruId == getAdministrativeCode);
        }

        public Region GetByStatisticCode(long statisticCode)
        {
            return Repository.Query<Region>().FirstOrDefault(x => x.StatisticIdentifier == statisticCode);
        }

        IEnumerable<Region> ICache<Region>.GetAll()
        {
            return GetAll<Region>();
        }

        IEnumerable<PollingStation> IPollingStationCache.GetByRegion(long regionId)
        {
            return Repository.Query<PollingStation>().Where(x => x.Region.Id == regionId);
        }

        IEnumerable<PollingStation> ICache<PollingStation>.GetAll()
        {
            return GetAll<PollingStation>();
        }

        StreetTypeCode IStreetTypeCodeCache.GetByStreetTypeCode(long id)
        {
            return Repository.Query<StreetTypeCode>().Where(x => x.Id == id).SingleOrDefault();
        }

        IEnumerable<StreetTypeCode> ICache<StreetTypeCode>.GetAll()
        {
            return GetAll<StreetTypeCode>();
        }

        IEnumerable<LinkedRegion> ILinkedRegionCache.GetByRegion(long id)
        {
            return Repository.Query<LinkedRegion>().Where(x => x.Regions.Count(r => r.Id == id) > 0);
        }

        IEnumerable<LinkedRegion> ICache<LinkedRegion>.GetAll()
        {
            return GetAll<LinkedRegion>();
        }
    }
}



