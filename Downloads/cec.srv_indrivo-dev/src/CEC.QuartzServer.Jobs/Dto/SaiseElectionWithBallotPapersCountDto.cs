using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.Dto
{

    public class SaiseElectionWithBallotPapersCountDto : IEntity
    {
        public SaiseElection Election { get; set; }

        public int BallotPapersCount { get; set; }
    }
}
