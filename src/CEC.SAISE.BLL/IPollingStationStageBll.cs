using Amdaris.Domain.Paging;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.BLL
{
	public interface IPollingStationStageBll
	{
		PageResponse<PollingStationStageEnablerDto> GetPollingStation(PageRequest pageRequest, long electionId);
	    void ProcessOptions(OptionsToggleDto data);
        long? GetPollingStationId(int id);
        PageResponse<VotingProcessStatsDto> GetVotingStatsForUser(PageRequest pageRequest, long electionId, long turnoutDataElectionid);
	}
}