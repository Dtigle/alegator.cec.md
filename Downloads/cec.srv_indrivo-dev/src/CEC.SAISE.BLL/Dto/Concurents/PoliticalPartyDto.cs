using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto.Concurents
{
    public class PoliticalPartyDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NameRo { get; set; }
        public string NameRu { get; set; }
        public PoliticalPartyStatus Status { get; set; }
        public long CandidateCount { get; set; }
        public int BallotOrder { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public bool IsIndependent { get; set; }

        public CandidateDto CandidateData { get; set; }
    }
}
