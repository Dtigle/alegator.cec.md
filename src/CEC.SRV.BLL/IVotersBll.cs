using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.ViewItem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CEC.SRV.BLL
{
    public interface IVotersBll : IBll
    {


        PageResponse<PersonAddress> GetAddressHistory(PageRequest pageRequest, long personId);
        PageResponse<PersonDocument> GetIdentityDocumentsHistory(long personId);
        PageResponse<PollingStation> GetPollingStationHistory(PageRequest pageRequest, long personId);
        PageResponse<Person> Get(PageRequest pageRequest);
        PageResponse<Person> GetIdnp(PageRequest pageRequest, string idnp);
		PageResponse<VoterViewItem> GetByFilters(PageRequest pageRequest, long? regionId, long? pollingStationId);

        PageResponse<VoterViewItem> GetByFilters(PageRequest pageRequest,
            long? regionId,
            long? pollingStationId,
            long? localityId = null,
            long? streetId = null,
            long? addressId = null,
            int? houseNumber = null,
            int? apNumber = null,
            string apSuffix = null,
            string surname = null,
            string excludeIdnp = null
        );

        PageResponse<VoterRow> GetByFilters2(PageRequest pageRequest,
            long? regionId,
            long? pollingStationId,
            long? localityId = null,
            long? streetId = null,
            long? addressId = null,
            int? houseNumber = null,
            int? apNumber = null,
            string apSuffix = null,
            string surname = null,
            string excludeIdnp = null
        );

        PageResponse<StayStatement> GetStayStatements(PageRequest pageRequest);
        PageResponse<StayStatement> GetStayStatementForPerson(PageRequest pageRequest, long personId);

        Person GetByIdnp(string idnp);
		string UpdateStatus(long personId, long statusId, string comments);
		string UpdateAddress(long personId, long addressId, int? apNumber, string apSufix);
        int GetVotersCount(PersonAddress address);
        Task DeleteElectionNumberList();
         Task AdddElectionNumberList(string query);
	    PageResponse<Region> SearchRegion(PageRequest pageRequest);
		PageResponse<Election> SearchActiveElections(PageRequest pageRequest, DateTime? electionDate);
        PageResponse<ElectionRound> SearchElectionRounds(PageRequest pageRequest, long electionId);
        PageResponse<PollingStation> SearchPollingStations(PageRequest pageRequest, long? regionId);
        PageResponse<AddressWithPollingStation> SearchAddress(PageRequest pageRequest, long? regionId, long? streetId = null);
        PageResponse<StreetView> SearchStreets(PageRequest pageRequest, long? regionId, long? streetId = null);
        long CreateStayStatement(long id, long personId, long addressId, int? apNumber, string apSuffix, long electionId);
		long CreateStayStatement(long personId, long pollingStationId, long electionId);
        bool ElectionUniqueStayStatement(long personId, long electionId);
		bool VerificationSameAddress(long personAddressId, long declaredAddressId);
		bool VerificationSameRegion(long personAddressId, long regionId);
        void CancelStayStatement(long stayStatementId);
        Person GetPersonWithEligibleResidence(long personId);
		IList<Election> GetElection();
        long SaveAbroadVoterRegistration(long personId, string abroadAddress, string residenceAddress, string abroadAddresCountry, double abroadAddressLat, double abroadAddressLong, string email, string ipAddress);
        List<AbroadVoterRegistration> GetAbroadVotersAddress();
        IList<CountryStatisticGroupedDto> GetRegionOfVotes(string country = null);
        bool IsRegisteredToElection(long id);
        IList<PollingStation> GetRegionPollingStationsByPerson();
        void UpdatePollingStation(long personId, long pollingStationId);
        IList<StatisticTimelineDto> GetAbroadVotersTimeline();
        void VerificationSameRegion(long[] peopleIds);
        void ChangePollingStation(long pollingStationId, List<long> peopleIds);
		RegionStreetsType GetRegionStreetsType();
    }
}