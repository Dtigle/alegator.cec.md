using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto
{
    public class VotingProcessStatsDto : IEntity
    {
        public long Id { get; set; }

        public string Circumscription { get; set; }

        public string Lacality { get; set; }

        public string CircumscriptionNumber { get; set; }

        public string PollingStation { get; set; }

        public bool PSIsOpen { get; set; }

        public long OpeningVoters { get; set; }

        public long TotalVotes { get; set; }

        public long SupplementaryVotes { get; set; }


        public BallotPaperStatus? BallotPaperStatus { get; set; }
    }
}
