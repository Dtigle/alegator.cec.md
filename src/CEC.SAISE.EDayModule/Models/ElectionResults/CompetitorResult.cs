using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Models.ElectionResults
{
    public class CompetitorResult
    {
        public long ElectionResultId { get; set; }

        public int BallotOrder { get; set; }

        public string PoliticalPartyCode { get; set; }

        public string PoliticalPartyName { get; set; }

        public string CandidateName { get; set; }

        public bool IsIndependent { get; set; }

        public long BallotCount { get; set; }

        public PoliticalPartyStatus PartyStatus { get; set; }
    }
}