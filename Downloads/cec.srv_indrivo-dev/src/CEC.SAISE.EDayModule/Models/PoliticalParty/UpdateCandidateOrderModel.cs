using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.PoliticalParty
{
    public class UpdateCandidateOrderModel
    {
        public long CandidateId { get; set; }

        public long? CandidateVillageRelId { get; set; }

        public int CandidateOrder { get; set; }
    }
}