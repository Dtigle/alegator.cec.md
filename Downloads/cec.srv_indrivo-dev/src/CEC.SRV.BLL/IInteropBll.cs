using System;
using System.Collections.Generic;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain.Interop;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IInteropBll : IBll
    {


        void SaveOrUpdateInteropSystem(long? id, string name, string description, TransactionProcessingTypes transactionProcessType,
            bool personStatusConsignment, long? personStatusTypeId, bool temporaryAddressConsignment
            );
        
        void SaveOrUpdateInstitution(long? id, string name, string description, long institutionTypeId, long legacyId, long addressId);
        
        void SaveOrUpdateTransaction(long? id, string idnp, string lastName, string firstName, DateTime dateOfBirth, long institutionTypeId, long? institutionId);

        bool IsUnique(long? id, long institutionTypeId, long legacyId);

        PageResponse<InteropSystem> SearchInstitutionTypes(PageRequest pageRequest);

        PageResponse<Institution> SearchInstitutions(PageRequest pageRequest, long? institutionTypeId);

        bool VerificationSameSystemAndInstitution(List<long> transactionIds);

        void ProcessTransactions(List<long> transactionIds, long pollingStationId, long electionId, ref int success, ref int error);

        void UndoTransactions(List<long> transactionIds, ref int success, ref int error);
    }
}