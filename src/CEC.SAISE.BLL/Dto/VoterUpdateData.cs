using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto
{
    public class VoterUpdateData
    {
        public long VoterId { get; set; }

        public long ElectionId { get; set; }

        public long? AssignedVoterId { get; set; }

        public VoterStatus VoterStatus { get; set; }

        public AssignedVoterStatus AssignedVoterStatus { get; set; }
    }
}
