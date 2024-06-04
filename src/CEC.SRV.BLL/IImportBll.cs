using System.Collections.Generic;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;

namespace CEC.SRV.BLL
{
    public interface IImportBll
    {
        void AcceptRspStatus(RspModificationData data);
        void RejectRspStatus(RspModificationData data);
        void MapAddress(RspModificationData rawData, Address address, int appNr, string apSuffix, bool addressWasCreatedByUserForConflictSolving = false);
        void AssignRspAddress(RspModificationData data);
        void RejectRspAddress(RspModificationData data);
        void AssignRspPollingStation(RspModificationData data);
        void RejectRspPollingStation(RspModificationData data);
        Person Import(RspModificationData data, ref StatisticChanges status);
        void AcceptRsvLocality(long conflictId);
        void ResolveByMappingAddress(long conflictId, long addressId, long[] applyToConflicts);

        void UpdateGeneralInformation(RspModificationData rawData, Person person, ref StatisticChanges status);
    }
}