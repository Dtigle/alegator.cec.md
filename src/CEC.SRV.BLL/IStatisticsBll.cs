using System;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IStatisticsBll : IBll
    {
        long GetTotalNumberOfPeople(long? regionId, long? pollingStationId);
        long GetTotalNumberOfPeopleWithoutDeads(long? regionId, long? pollingStationId);
        long GetTotalNumberOfVoters(long? regionId, long? pollingStationId);

        long GetTotalNumberOfDeads(long? regionId, long? pollingStationId);

        long GetTotalNumberOfMilitary(long? regionId, long? pollingStationId);

        long GetTotalNumberOfDetainee(long? regionId, long? pollingStationId);

        long GetTotalNumberOfStatementAbroad(long? regionId, long? pollingStationId);

        long GetTotalNumberOfStayStatementDeclarations(long? regionId, long? pollingStationId);
        long GetTotalNumberOfPeopleByGender(GenderTypes gender, long? regionId, long? pollingStationId);
        long GetTotalNumberOfPeopleWithDoBMissing(long? regionId, long? pollingStationId);
		long GetTotalNumberOfPeopleWithAddressMissing(long? regionId, long? pollingStationId);
        long GetTotalNumberOfPeopleWithDocMissing(long? regionId, long? pollingStationId);
        long GetTotalNumberOfPeopleWithDocExpired(long? regionId, long? pollingStationId);
        long GetNumberOfPeopleForAgeIntervals(int interval, long? regionId, long? pollingStationId);
        long GetNewVoters();
        long GetNewDeadPeople();
        long GetPersonalDataChanges();
        long GetAddressesChanges();
        long GetImportSuccessful();
        long GetImportFailed();
        PageResponse<PollingStationStatistics> GetStatisticsForPollingStation(PageRequest pageRequest, long? regionId, long? pollingStationId);
		PageResponse<ProblematicDataPollingStationStatistics> GetStatisticsForProblematicDataPollingStation(PageRequest pageRequest);
        PageResponse<ImportStatisticsGridDto> GetImportStatistics(PageRequest pageRequest, long? regionId);
        ImportStatisticsDto GetImportStatistics(DateTime importDataTime, long regionId);
    }
}