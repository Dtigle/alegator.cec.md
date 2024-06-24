using System.Collections.Generic;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface ILookupBll : IBll
    {
        void Update<T>(long id, string name, string description) where T : Lookup;
        void Add<T>(string name, string description) where T : Lookup, new();
		void SaveOrUpdate<T>(long? id, string name, string description) where T : Lookup, new();
		void SaveOrUpdatePersonStatus(long? id, string name, string description, bool isExcludable);
		void SaveOrUpdateDocType(long? id, string name, string description, bool isPrimary);
		void SaveOrUpdateRegionType(long? id, string name, string description, byte rank);
        void DeleteRegion(long id);
		void VerificationRegion(long id);
        PageResponse<StreetDto> GetStreets(PageRequest pageRequest, long regionId);
		PageResponse<Circumscription> GetCircumscriptions(PageRequest pageRequest);
		IEnumerable<Circumscription> GetCircumscriptions();
        PageResponse<RegionRow> GetRegions(PageRequest pageRequest, long parentRegionId);
		PageResponse<RegionRow> GetRegionsGrid(PageRequest pageRequest);
        void CreateUpdateStreet(long id, string name, string description, long regionId, long streetTypeId, long? ropId, long? saiseId);
        void CreateUpdateRegion(long id, string name, string description, long? parentId, long regionTypeId, bool hasStreets, long? saiseId, long? cuatm);
        void UpdateAdministrativeInfo(long id, string name, string surname, long regionId, long managerTypeId);
        bool IsUnique(long id, string name, long parent, long regionTypeId);
        bool IsUnique(long id, long regionId, string name, long regionTypeId);
		bool IsUnique<T>(long? id, string name) where T : Lookup, new();
        IEnumerable<Region> GetRegionsOfLevel1ByFilter(string regionNameFilter);
        IEnumerable<Region> GetRegionsByParentIdAndFilter(long parentId, string regionNameFilter);
		void UpdateCircumscription(long id, int? circumscriptionNumber);
	    bool UniqueValidationCircumscription(long regionId, int? circumscriptionNumber);
        void DeleteStreet(long streetId);
        IEnumerable<PollingStation> GetPollingStationsHierarhicallyByRegion(long regionId);
        IEnumerable<Street> GetStreetsHierarhicallyByRegionAndFilter(long regionId, string streetNameFilter);
		void VerificationIfManagerTypeHasReference(long managerTypeId);
		void VerificationIfPersonStatusHasReference(long personStatusId);
		void VerificationIfGenderHasReference(long genderId);
		void VerificationIfDocTypeHasReference(long docTypeId);
		void VerificationIfElectionTypeHasReference(long electionTypeId);
		void VerificationIfPersonAddressTypeHasReference(long personAddressTypeId);
		void VerificationIfRegionTypeHasReference(long regionTypeId);
		void VerificationIfRegionHasReference(long regionId);
		void VerificationIfStreetTypeHasReference(long streetTypeId);
		void VerificationIfStreetHasReference(long streetId);
        void VerificationIfConflictShareReasonHasReference(long conflictShareReasonId);
        PageResponse<RegionWithFullyQualifiedName> GetAvailableRegions(PageRequest pageRequest);
        IList<LinkedRegionsFullName> GetLinkedRegionsByRegionId(long regionId);
        void SaveLinkedRegions(long regionId, long[] linkedRegionIds);
        RegionWithFullyQualifiedName GetFullyQualifiedName(long regionId);
    }
}