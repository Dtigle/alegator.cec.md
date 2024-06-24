using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace CEC.SRV.BLL.Impl
{
    public class AddressBll : Bll, IAddressBll
    {
        public AddressBll(ISRVRepository repository)
            : base(repository)
        {
        }

		public PageResponse<Address> Get(PageRequest pageRequest, long? regionId)
		{
			Street street = null;
			if (regionId.HasValue)
			{
				if (IsRegionAccessibleToCurrentUser(regionId.Value))
				{
					return Repository.QueryOver<Address>()
					  .JoinAlias(x => x.Street, () => street)
					  .Where(x => street.Region.Id == regionId.Value).RootCriteria.CreatePage<Address>(pageRequest);
				}
			}

			return new PageResponse<Address>() { Items = new Address[0] };
		}

		public IEnumerable<Street> GetStreets(long? regionId)
		{
		    Region region = null;
            //var query = Repository.QueryOver<Street>();//.Query<Street>().Fetch(x => x.StreetType);
            var query = Repository
                .QueryOver<Street>();
		    if (regionId.HasValue)
		    {
		        query = query
		            .JoinAlias(x => x.Region, () => region, JoinType.InnerJoin)
		            .Where(() => region.Id == regionId);
		        
                if (!SecurityHelper.LoggedUserIsInRole("Administrator"))
		        {
		            query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
		            .HasPropertyIn(x => region.Id);
		        }
		    }

		    return query.Fetch(x => x.StreetType).Eager.List();
		    //return regionId.HasValue ? query.Where(x => x.Region.Id == regionId) : query;
		}

		public PageResponse<Street> SearchStreets(PageRequest pageRequest, long? regionId)
		{
			Region region = null;
			var query = Repository.QueryOver<Street>();
			if (regionId.HasValue)
			{
				query = query
					.JoinAlias(x => x.Region, () => region, JoinType.InnerJoin)
					.Where(() => region.Id == regionId);

				if (!SecurityHelper.LoggedUserIsInRole("Administrator"))
				{
					query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
					.HasPropertyIn(x => region.Id);
				}
			}

			return query.Fetch(x => x.StreetType).Eager
				.Where(x =>x.Deleted == null).OrderBy(x => x.Name).Asc
				.RootCriteria.CreatePage<Street>(pageRequest);

		}

		public PageResponse<Street> SearchStreetsForPublic(PageRequest pageRequest, long? regionId)
		{
			Region region = null;
			var query = Repository.QueryOver<Street>();
			if (regionId.HasValue)
			{
				query = query
					.JoinAlias(x => x.Region, () => region, JoinType.InnerJoin)
					.Where(() => region.Id == regionId);
			}

			return query.Fetch(x => x.StreetType).Eager
				.Where(x => x.Deleted == null).OrderBy(x => x.Name).Asc
				.RootCriteria.CreatePage<Street>(pageRequest);

		}

		public IEnumerable<Street> GetStreetsByFilter(string term)
		{
			return Repository.Query<Street>().Fetch(x => x.StreetType).Where(x => x.Name.Contains(term));
		}

		public void SaveAddress(long addressId, long streetId, int? houseNumber, string suffix, BuildingTypes buildingType, long? pollingStationId)
        {
			//if (buildingType == BuildingTypes.ApartmentBuilding && pollingStationId == 0)
			//{
			//	throw new SrvException("Address_RequiredPollingStation", MUI.Address_RequiredPollingStation);
			//}
            var street = Repository.LoadProxy<Street>(streetId);
			var pollingStation = (pollingStationId != null && pollingStationId > 0) ? Repository.LoadProxy<PollingStation>((long)pollingStationId) : null;
			var verificationByUnique = GetAddress(streetId, houseNumber, suffix);

			if (verificationByUnique.Any() && verificationByUnique.Any(x => x.Id != addressId))
			{
                throw new SrvException("Create_UniqueError", MUI.Create_UniqueError);
			}

            var address = addressId != 0 ?  Repository.Get<Address>(addressId) : new Address();
            address.Street = street;
            address.HouseNumber = houseNumber;
            address.Suffix = suffix;
            address.BuildingType = buildingType;
			address.PollingStation = pollingStation;
            Repository.SaveOrUpdate(address);
        }


		public void UpdateLocation(long addressId, double latitude, double longitude)
        {
			var address = Repository.LoadProxy<Address>(addressId);
			address.GeoLocation = new GeoLocation {Longitude = longitude, Latitude = latitude};
            Repository.SaveOrUpdate(address);
        }

		public IEnumerable<PollingStation> GetPollingStations(long? regionId)
		{
			
            if (regionId.HasValue)
            {
				var query = Repository.Query<PollingStation>();
                if (IsRegionAccessibleToCurrentUser(regionId.Value))
                {
                    return query.Where(x => x.Region.Id == regionId && x.Deleted == null);
                }
            }
			return new List<PollingStation>();
		}

		public IEnumerable<PollingStation> GetPollingStationsByFilter(string term)
		{
			return Repository.Query<PollingStation>().Where(x => x.Number.StartsWith(term)&& x.Deleted == null);
		}

        public IEnumerable<Address> GetAddressesByStreetId(long streetId)
        {
            return Repository.Query<Address>()
                .Fetch(s => s.Street)
                .ThenFetch(s => s.StreetType)

                .Fetch(s => s.PollingStation)
                .ThenFetch(s => s.PollingStationAddress)
                .ThenFetch(s => s.Street)
                .ThenFetch(s => s.Region)
                .ThenFetch(s => s.RegionType)

                .Fetch(s => s.PollingStation)
                .ThenFetch(s => s.PollingStationAddress)
                .ThenFetch(s => s.Street)
                .ThenFetch(s => s.StreetType)

                .Fetch(s => s.PollingStation)
                .ThenFetch(spg => spg.GeoLocation)

                .Where(s => (s.Street != null) && (s.Street.Id == streetId) && (s.Deleted == null))
                .ToList();
        }

        public void DeleteAddress(long addressId)
        {
            var pollingStation = Repository.Query<PollingStation>().FirstOrDefault(x => x.PollingStationAddress.Id == addressId && x.Deleted == null);
            if (pollingStation != null)
            {
                throw new SrvException("Address_NotPermittedDeletingDueToPollingStation", MUI.Address_NotPermittedDeletingDueToPollingStation);
            }

            var personAddress = Repository.Query<PersonAddress>().FirstOrDefault(x => x.Address.Id == addressId && x.Deleted == null);
            if (personAddress != null)
            {
                throw new SrvException("Address_NotPermittedDeletingDueToPerson", MUI.Address_NotPermittedDeletingDueToPerson);
            }

            DeleteAddressMappings(addressId);

            Delete<Address>(addressId);
        }

        public void VerificationRegion(long id, long? addressId)
		{
			var region = Get<Region>(id);

			if (!region.HasStreets)
			{
			    if (addressId.HasValue)
			    {
                    throw new SrvException("Address_UpdateErrore_RegionHasNoStreets", MUI.Address_UpdateErrore_RegionHasNoStreets);
			    }

				throw new SrvException("Address_RegionNotAcceptStreets", MUI.Address_RegionNotAcceptStreets);
			}

			VerificationIsRegionDeleted(region.Id);
		}

        public void UpdatePollingStation(long pollingStationId, IEnumerable<long> addressIds)
        {
            var pollingStation = Get<PollingStation>(pollingStationId);
            if (pollingStation == null)
            {
                throw new SrvException("", "PollingSttion not found.");
            }

            foreach (var addressId in addressIds)
            {
                var address = Get<Address>(addressId);
                if (address != null)
                {
                    address.PollingStation = pollingStation;
                    Repository.SaveOrUpdate(address);
                }
            }
        }

		public PageResponse<AddressDto> GetAddresses(PageRequest pageRequest, long regionId)
		{
			if (IsRegionAccessibleToCurrentUser(regionId))
			{
				AddressDto addressDto = null;
				AddressWithCountOfPeople addressGrid = null;
				Region region = null;
				Address address = null;
				PollingStation pollingStation = null;
				Street street = null;
				StreetType streetType = null;
				IdentityUser createdBy = null;
				IdentityUser modifiedBy = null;
				IdentityUser deletedBy = null;

				return Repository.QueryOver(() => addressGrid)
					.JoinAlias(x => addressGrid.Address.Street.Region, () => region)
					.JoinAlias(x => addressGrid.Address.Street.StreetType, () => streetType)
					.JoinAlias(x => addressGrid.Address, () => address)
					.JoinAlias(x => addressGrid.Address.PollingStation, () => pollingStation, JoinType.LeftOuterJoin)
					.JoinAlias(x => addressGrid.Address.Street, () => street)
					.JoinAlias(x => address.CreatedBy, () => createdBy, JoinType.LeftOuterJoin)
					.JoinAlias(x => address.DeletedBy, () => deletedBy, JoinType.LeftOuterJoin)
					.JoinAlias(x => address.ModifiedBy, () => modifiedBy, JoinType.LeftOuterJoin)
					.Where(() => region.Id == regionId)
					.Select(Projections.ProjectionList()
						.Add(Projections.Property<AddressWithCountOfPeople>(x => x.Id).WithAlias(() => addressDto.Id))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => street.Name).WithAlias(() => addressDto.StreetName))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => address.HouseNumber).WithAlias(() => addressDto.HouseNumber))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => address.Suffix).WithAlias(() => addressDto.Suffix))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => pollingStation.FullNumber).WithAlias(() => addressDto.PollingStation))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => x.PeopleCount).WithAlias(() => addressDto.PeopleCount))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => address.Created).WithAlias(() => addressDto.Created))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => address.Modified).WithAlias(() => addressDto.Modified))
						.Add(Projections.Property<AddressWithCountOfPeople>(x => address.Deleted).WithAlias(() => addressDto.Deleted))
						.Add(Projections.Property<IdentityUser>(x => createdBy.UserName).WithAlias(() => addressDto.CreatedBy))
						.Add(Projections.Property<IdentityUser>(x => modifiedBy.UserName).WithAlias(() => addressDto.ModifiedBy))
						.Add(Projections.Property<IdentityUser>(x => deletedBy.UserName).WithAlias(() => addressDto.DeletedBy))
						.Add(Projections.Property<GeoLocation>(x => address.GeoLocation).WithAlias(() => addressDto.GeoLocation))
						.Add(Projections.Property<StreetType>(x => streetType.Name).WithAlias(() => addressDto.StreetTypeName))
						).TransformUsing(Transformers.AliasToBean<AddressDto>())

					.RootCriteria
					.CreatePage<AddressDto>(pageRequest);
			}

			return new PageResponse<AddressDto> { Items = new List<AddressDto>(), PageSize = pageRequest.PageSize, Total = 0 };
		}

		public void VerificationRegionForChangePollingStation(long id)
		{
			var region = Get<Region>(id);

			if (!region.HasStreets)
			{
				throw new SrvException("Address_RegionNotAcceptChangePollingStation", MUI.Address_RegionNotAcceptChangePollingStation);
			}

			if (region.Deleted != null)
			{
				throw new SrvException("Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
			}
		}

		public void VerificationRegionForUpdateGeolocation(long id)
		{
			var region = Get<Region>(id);

			if (!region.HasStreets)
			{
				throw new SrvException("Address_RegionNotAcceptUpdateGeolocation", MUI.Address_RegionNotAcceptUpdateGeolocation);
			}

			if (region.Deleted != null)
			{
				throw new SrvException("Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
			}
		}

		public void VerificationRegionForAdjustmentAddresses(long id)
		{
			var region = Get<Region>(id);

			if (!region.HasStreets)
			{
				throw new SrvException("Address_RegionNotAcceptAdjustmentAddresses", MUI.Address_RegionNotAcceptAdjustmentAddresses);
			}

			if (region.Deleted != null)
			{
				throw new SrvException("Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
			}
		}

		public void VerificationIsStreetDeleted(long streetId)
		{
			var street = Get<Street>(streetId);
			if (street.Deleted !=null)
			{
				throw new SrvException("Address_UnDeleteAddressError", MUI.Address_UnDeleteAddressError);
			}
			
		}

		public void AdjustmentAddresses(long oldAddressId, long newAddressId)
		{
			var oldAddress = Get<Address>(oldAddressId);
			var newAddress = Get<Address>(newAddressId);

			var personAddresse = Repository.Query<PersonAddress>().Where(x => x.Address.Id == oldAddress.Id).ToList();

			foreach (var personAddress in personAddresse)
			{
				personAddress.Address = newAddress;
				Repository.SaveOrUpdate(personAddress);
			}

			Repository.Delete(oldAddress);

			var addressStreet = Repository.Query<Address>().Count(x => x.Street.Id == oldAddress.Street.Id);
			if (addressStreet == 0)
			{
				Repository.Delete(oldAddress.Street);
			}

			var srvAddress = Repository.Query<MappingAddress>().FirstOrDefault(x => x.SrvAddressId == oldAddress.Id);
			if (srvAddress != null)
			{
				Repository.Delete(srvAddress);	
			}
		}

		public void VerificationIfAddressHasReference(long addressId)
		{
			var personAddress = Repository.Query<PersonAddress>().FirstOrDefault(x => x.Address.Id == addressId && x.Deleted == null);
			if (personAddress != null)
			{
				throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
				personAddress.Address.GetObjectType(), personAddress.GetObjectType()));
			}

			var pollingStation = Repository.Query<PollingStation>().FirstOrDefault(x => x.PollingStationAddress.Id == addressId && x.Deleted == null);
			if (pollingStation != null)
			{
				throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
				pollingStation.PollingStationAddress.GetObjectType(), pollingStation.GetObjectType()));
			}
		}

		public IList<Region> GetAssignedRegions()
		{
			var query = Repository.QueryOver<Region>();
			if (!SecurityHelper.LoggedUserIsInRole("Administrator"))
			{
				query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.Id);
			}

			return query.List();
		}

        public void DeleteAddressMappings(long addressId)
        {
            var addressMappings = Repository.Query<MappingAddress>().Where(x => x.SrvAddressId == addressId).ToList();

            foreach (var addressMapping in addressMappings)
            {
                Repository.Delete(addressMapping);
            }
        }

        public PageResponse<RspAddressMapping> ListAddressMappings(PageRequest pageRequest)
        {
            var query = Repository.QueryOver<RspAddressMapping>();

            //TODO: Add filtering by region accessibility
            if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.SrvRegionId);
            }

            return query
                .RootCriteria
                .CreatePage<RspAddressMapping>(pageRequest);
        }

        public void DeleteAddressMapping(long addressMappingId)
        {
            var addressMapping = Get<MappingAddress>(addressMappingId);

            if (addressMapping != null)
            {
                Repository.Delete(addressMapping);
            }
        }
    }
}