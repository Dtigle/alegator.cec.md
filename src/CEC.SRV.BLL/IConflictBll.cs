using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.ViewItem;
using System.Collections.Generic;

namespace CEC.SRV.BLL
{
    public interface IConflictBll : IBll
	{

	    BllResult ShareConflict(long conflictDataId, long regionId, long conflictReasonId, string note);
	    BllResult CancelConflictShare(long conflictDataId, long regionId);
        BllResult CancelAllConflictShares(long conflictDataId);

        IList<ConflictShare> GetAllConflictShares(long conflictDataId);

        PageResponse<ConflictViewItem> GetConflictSharedWithMyRegionsList(PageRequest pageRequest,ConflictStatusCode[] conflictCodes);
        PageResponse<ConflictViewItem> GetConflictSharedByMyRegionsList(PageRequest pageRequest,ConflictStatusCode[] conflictCodes);

	    PageResponse<RspConflictData> GetConflictListSharedByMyRegionsListForLinkedRegions(PageRequest pageRequest,
	        ConflictStatusCode[] conflictCodes);

        PageResponse<RspConflictData> GetConflictListSharedWithMyRegionsListForLinkedRegions(PageRequest pageRequest,
            ConflictStatusCode[] conflictCodes);


        PageResponse<RspConflictData> GetAllConflicts(PageRequest pageRequest, ConflictStatusCode[] conflictCodes);
        PageResponse<ConflictViewItem> GetAllConflicts2(PageRequest pageRequest, ConflictStatusCode[] conflictCodes);

        long GetAllConflictCount(ConflictStatusCode[] conflictCodes);

        long GetAllConflictCountForAdmin(ConflictStatusCode[] conflictCodes);


        PageResponse<RspModificationData> GetConflictHistory(PageRequest pageRequest, string idnp);

        PageResponse<RspConflictData> GetConflictList(PageRequest pageRequest, ConflictStatusCode conflictCode);
        PageResponse<ConflictViewItem> GetConflictList2(PageRequest pageRequest, ConflictStatusCode conflictCode);
        PageResponse<RspConflictData> GetConflictList(PageRequest pageRequest, ConflictStatusCode[] conflictCodes);
        PageResponse<ConflictViewItem> GetConflictList2(PageRequest pageRequest, ConflictStatusCode[] conflictCodes);
        PageResponse<RspConflictData> GetConflictListForLinkedRegions(PageRequest pageRequest, ConflictStatusCode conflictCode);
        PageResponse<RspConflictDataAdmin> GetConflictListForAdmin(PageRequest pageRequest, ConflictStatusCode conflictCode);

        PageResponse<ConflictViewItem> GetConflictListForAdmin2(PageRequest pageRequest, ConflictStatusCode conflictCode);

        PageResponse<RspConflictDataAdmin> GetConflictListForAdmin(PageRequest pageRequest, ConflictStatusCode[] conflictCodes);
        PageResponse<ConflictViewItem> GetConflictListForAdmin2(PageRequest pageRequest, ConflictStatusCode[] conflictCodes);
        //PageResponse<Address> GetAddresses(PageRequest pageRequest, long? regionId);
        PageResponse<AddressBaseDto> GetAddresses(PageRequest pageRequest, long? regionId);

        PageResponse<RspConflictData> GetConflictListByConflictAddress(PageRequest pageRequest, long conflictId);






        VoterConflictDataDto GetVoter(string idnp);
        Person GetPerson(string idnp);

	    Region GetRegionByAdministrativeCode(long registruId);
	    Street GetStreetByRopId(long streetCodeId);
	    Street GetStreetByRopId(long streetCodeId, long regionId);
	    Street GetStreetNameAndRegionId(string streetName, long regionId);
	    long CreateStreet(string name, string description, long regionId, long streetTypeId, long? ropId, long? saiseId);
        List<long> GetPersonIdbyRspIds(List<long> conflictIds);
	    long GetPersonIdByConflictId(long conflictId);
        void WriteNotification(ConflictStatusCode conflictStatus, RspModificationData conflictData, string notificationMessage, long regionId = -1, long regionId2 = -1);
	    Address SaveAddress(long streetId, int? houseNumber, string suffix, BuildingTypes buildingType, long? pollingStationId);
        void UpdateStatusToRetry(long conflictId, string message, ConflictStatusCode conflictCode);
	    RspModificationData GetConflict(long id);
        RspConflictData GetConflictFromRspConflictData(long id);
        string GetUserName();
        PageResponse<Region> GetUserRegions(PageRequest pageRequest);
	    void ChangeAddress(long addressId, long personId);
	    RspRegistrationData GetConflictAddress(long conflictId);


        PageResponse<AddressWithoutPollingStation> GetAddressesWithoutPollingStation(PageRequest pageRequest);
    }
}