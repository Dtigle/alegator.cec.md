using System.IO;
using System.Threading.Tasks;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL
{
    public interface IVotingBll
    {
        
        bool HasOpeningVotersAssigned(long electionId, long pollingStationId);

        Voter GetVoterByIdnp(string idnp);

        //StatisticsDto GetStatistics(long electionId, long pollingStationId);
        Task<StatisticsDto> GetStatisticsAsync(long electionId, long pollingStationId);

        //UpdateVoterResult SaveUpdateVoter(VoterUpdateData updateData);
        Task<UpdateVoterResult> SaveUpdateVoterAsync(VoterUpdateData updateData);

        bool IsPollingStationOpen(long electionId, long pollingStationId);

        Task<SearchResult> SearchVoterAsync(string idnp, string loger);
        SearchResult SearchVoterAsyncForWebService(string idnp);
        Task<PollingStationOpeningData> GetOpeningDataAsync(long electionId, long pollingStationId);
        Task<PollingStationOpeningData> GetOpeningDataAsync(long assignedPollingStationId);
        Task<OpenPollingStationResult> OpenPollingStationAsync(long assignedPollingStationId, int openingVoters, string ipUser);
        Task LogSearchEventAsync(string idnp, SearchResult searchResult, string ipUser);
        Task<AssignedPollingStation> GetAssignedPollingStationAsync(long electionId, long pollingStationId);
        VoterUtanStatus CheckUserRegionValidation(long idnp, long circumscriptionId);
        Task<AssignedPollingStation> GetAssignedPollingStationAsync(long pollingStationId);
        Task<bool> CaptureSignature(string idnp, byte[] signatureData);
        Task<VoterDataSignature> GetVoterDataSignature(long voterId);
    }
}
