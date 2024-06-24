using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Models.PoliticalParty
{
    public class PoliticalPartyModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NameRo { get; set; }
        public string NameRu { get; set; }
        public PoliticalPartyStatus Status { get; set; }
        public long CandidateCount { get; set; }
        public long BallotOrder { get; set; }
        public DateTime? DateOfRegistration { get; set; }
        public bool IsIndependent { get; set; }

        public CandidateModel CandidateData { get; set; }
    }
}