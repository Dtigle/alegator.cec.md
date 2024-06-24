using System.Collections.Generic;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL
{
	public interface IPoliticalPartyBll
	{
		IList<PoliticalPartyDto> Get(long electionId, long regionId);
		IList<PoliticalPartyDto> Get(long electionId);
        List<ElectionCompetitorMember> GetCandidates(long partyId, long regionId, long electionRoundId);
		void UpdatePartyStatus(long partyId, PoliticalPartyStatus statusId);
		void UpdateCandidateStatus(long candidateId, CandidateStatus statusId);
        IList<PoliticalPartyDto> GetAll(long electionRoundId, long regionId);
		long GetCandidateCount(long partyId, long electionId, long villageId);
		IList<PoliticalPartyDto> GetAll(long electionId);
		PoliticalPartyDto SaveUpdateParty(PoliticalPartyUpdateDto model);
	}
}