using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Models.PoliticalParty
{
    public class CandidateModel
    {
        public long Id { get; set; }
        public long? CandidateRegionRelId { get; set; }
        public long PoliticalPartyId { get; set; }
        public string NameRo { get; set; }
        public string LastNameRo { get; set; }
        public string NameRu { get; set; }
        public string LastNameRu { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GenderType Gender { get; set; }
        public string Occupation { get; set; }
        public string OccupationRu { get; set; }
        public string Workplace { get; set; }
        public string WorkplaceRu { get; set; }
        public string Idnp { get; set; }
        public long CandidateOrder { get; set; }
        public CandidateStatus Status { get; set; }
    }
}