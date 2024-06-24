using System.Collections.Generic;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IAddressBll : IBll
    {
		PageResponse<Address> Get(PageRequest pageRequest, long? regionId);
        IEnumerable<Street> GetStreets(long? regionId);
		PageResponse<Street> SearchStreets(PageRequest pageRequest, long? regionId);
		PageResponse<Street> SearchStreetsForPublic(PageRequest pageRequest, long? regionId);
		IEnumerable<Street> GetStreetsByFilter(string term);
		IEnumerable<PollingStation> GetPollingStations(long? regionId);
		IEnumerable<PollingStation> GetPollingStationsByFilter(string term);
		void SaveAddress(long addressId, long streetId, int? houseNumber, string suffix, BuildingTypes buildingType, long? pollingStationId);
		void UpdateLocation(long addressId, double latitude, double longitude);
        IEnumerable<Address> GetAddressesByStreetId(long streetId);
        void DeleteAddress(long addressId);
        void VerificationRegion(long id, long? addressId);
		void VerificationRegionForChangePollingStation(long id);
		void VerificationRegionForUpdateGeolocation(long id);
		void VerificationRegionForAdjustmentAddresses(long id);
        void UpdatePollingStation(long pollingStationId, IEnumerable<long> addressIds);
		void AdjustmentAddresses(long oldAddressId, long newAddressId);
		PageResponse<AddressDto> GetAddresses(PageRequest pageRequest, long regionId);
		void VerificationIsStreetDeleted(long streetId);
		void VerificationIfAddressHasReference(long addressId);
		IList<Region> GetAssignedRegions();
        PageResponse<RspAddressMapping> ListAddressMappings(PageRequest pageRequest);
        void DeleteAddressMapping(long addressMappingId);
    }
}