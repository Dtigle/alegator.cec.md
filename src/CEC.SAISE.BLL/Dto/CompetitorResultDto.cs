using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto
{
    public class CompetitorResultDto
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
