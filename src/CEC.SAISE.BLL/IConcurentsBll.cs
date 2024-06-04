using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.BLL.Dto.Concurents;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL
{
    public interface IConcurentsBll
    {
        Election GetElection(long electionId);
        IList<PoliticalPartyDto> GetAllParties(DelimitationDto delimitation);
        IList<PoliticalPartyDto> GetAllocatedParties(DelimitationDto delimitation);
        IList<CandidateDto> GetCandidatesForParty(DelimitationDto delimitation, long partyId);
        bool UpdateCandidateStatus(long candidateId, CandidateStatus status);
        bool DeleteCandidates(List<DeleteCandidateDto> itemsToDelete);
        bool UpdateCandidatesOrder(List<UpdateCandidateOrderDto> itemsToUpdate);
        Task<PersonalDataResponse> RequestPersonalData(string idnp);
        void SaveUpdateParty(DelimitationDto delimitation, PoliticalPartyDto partyToUpdate, byte[] logo);
        void SaveUpdateCandidate(DelimitationDto delimitation, CandidateDto candidateToUpdate);
        void FireAllocation(DelimitationDto delimitation, IList<AllocationItemDto> itemsToAllocate);
	    IList<CandidateConflictDto> CheckPersonAllocation(DelimitationDto delimitationDto, CandidateDto candidateToUpdate);
	    bool DeleteConcurents(DelimitationDto delimitationDto, IEnumerable<long> itemsToDelete);
	    void ExportCandidatesToExcel(out Stream stream, out string fileName, DelimitationDto map);
		bool UpdatePartyStatus(long partyId, PoliticalPartyStatus status);
		bool OverridePartyStatus(DelimitationDto delimitationDto, long partyId, PoliticalPartyStatus status);
    }
}
