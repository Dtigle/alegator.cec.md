using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Models.PoliticalParty
{
    public class AllocationItemModel
    {
        public long PoliticalPartyId { get; set; }

        public int BallotOrder { get; set; }

        public PoliticalPartyStatus Status { get; set; }

	    public bool IsIndependent { get; set; }
    }
}