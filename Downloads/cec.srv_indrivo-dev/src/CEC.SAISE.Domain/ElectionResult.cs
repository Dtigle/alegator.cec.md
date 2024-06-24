using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CEC.SAISE.Domain
{
    public class ElectionResult : SaiseBaseEntity
    {
        public virtual int BallotOrder { get; set; }

        public virtual long BallotCount { get; set; }
        public virtual ElectionRound ElectionRound { get; set; }

        public virtual string Comments { get; set; }

        public virtual DateTime DateOfEntry { get; set; }

        public virtual ElectionResultStatus Status { get; set; }

        public virtual ElectionCompetitor PoliticalParty { get; set; }

        public virtual ElectionCompetitorMember Candidate { get; set; }

        public virtual BallotPaper BallotPaper { get; set; }
    }
}