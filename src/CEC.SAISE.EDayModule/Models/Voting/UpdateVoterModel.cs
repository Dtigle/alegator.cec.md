using CEC.SAISE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class UpdateVoterModel
    {
        public long VoterId { get; set; }

        public long ElectionId { get; set; }

        public long? AssignedVoterId { get; set; }

        public VoterStatus VoterStatus { get; set; }

        public AssignedVoterStatus AssignedVoterStatus { get; set; }
    }
}