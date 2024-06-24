using Amdaris.Domain.Paging;
using CEC.SAISE.BLL.Dto;
using System;
using System.Collections.Generic;

namespace CEC.SAISE.BLL
{
	public interface IPollingStationStageBll
	{
		PageResponse<PollingStationStageEnablerDto> GetPollingStation(PageRequest pageRequest, long electionId);
	    void ProcessOptions(OptionsToggleDto data);
        long? GetPollingStationId(int id);
        PageResponse<VotingProcessStatsDto> GetVotingStatsForUser(PageRequest pageRequest, long electionId, long turnoutDataElectionid);
        bool UpdatePollingStationActivity(UpdatePollingStationActivityDto updatePSActivity);
        List<ElectionDurationDto> GetElectionDurations();
        bool SuspendPollingStationActivity(List<long> ids, bool suspended);
        bool ActivateCaptureSignature(List<long> ids, bool capture);

    }
}