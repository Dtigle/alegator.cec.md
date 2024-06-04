using System.Collections.Generic;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IPollingStationBll : IBll
    {
        IEnumerable<Address> GetPollingStationAddresses(long regionId);

        void CreateUpdatePollingStation(long id, long regionId, string number, string location, long addressId,
            string contactInfo, long? saiseId, PollingStationTypes pollingStationType);

	    IList<PollingStation> GetPollingStationsByRegion(long[] selectedRegions);

		PageResponse<PollingStationDto> GetPollingStation(PageRequest pageRequest, long regionId);

		void DeletePollingStation(long pollingStationId);

        IList<PollingStation> GetAccessiblePollingStations();

		PageResponse<PollingStation> GetPollingStationsByRegions(PageRequest pageRequest, long[] selectedRegion);

		int? GetCircumscription(long regionId);

		void VerificationIfPollingStationHasReference(long pollingStationId);
        Task DeleteElectionNumberList();
        Task AdddElectionNumberList(ElectionNumberListOrderByDto model);
        PageResponse<VotersListOrderType> SearchOrderType(PageRequest pageRequest);
    }
}