using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto.Concurents
{
    public class AllocationItemDto
    {
        public long PoliticalPartyId { get; set; }

        public int BallotOrder { get; set; }

        public PoliticalPartyStatus Status { get; set; }
	    public bool IsIndependent { get; set; }
    }
}