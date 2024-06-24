using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CEC.SAISE.Domain
{
    public class AssignedPollingStation : SaiseBaseEntity
    {
        //private readonly IList<ElectionTurnout> _electionTurnouts;

        //public AssignedPollingStation()
        //{
        //    _electionTurnouts = new List<ElectionTurnout>();
        //}

        public virtual ElectionRound ElectionRound { get; set; }

        public virtual AssignedCircumscription AssignedCircumscription { get; set; }

        public virtual PollingStation PollingStation { get; set; }

        public virtual AssignedPollingStationType Type { get; set; }

        public virtual AssignedPollingStationStatus Status { get; set; }

        public virtual bool IsOpen { get; set; }

        public virtual bool IsOpeningEnabled { get; set; }

        public virtual bool IsTurnoutEnabled { get; set; }

        public virtual bool IsElectionResultEnabled { get; set; }

        public virtual int OpeningVoters { get; set; }

        public virtual int EstimatedNumberOfVoters { get; set; }

        public virtual int NumberOfRoBallotPapers { get; set; }

        public virtual int NumberOfRuBallotPapers { get; set; }

        public virtual bool ImplementsEVR { get; set; }

        public virtual string NumberPerElection { get; set; }

        //public virtual IReadOnlyCollection<ElectionTurnout> ElectionTurnouts
        //{
        //    get { return new ReadOnlyCollection<ElectionTurnout>(_electionTurnouts); }
        //}

        /* GEstionarea Functionala extended */
        public virtual TimeSpan? ElectionStartTime { get; set; }
        public virtual TimeSpan? ElectionEndTime { get; set; }
        public virtual int TimeDifferenceMoldova { get; set; }
        public virtual int ActivityTimeExtendedFirstDay { get; set; }
        public virtual int ActivityTimeExtendedSecondDay { get; set; }
        public virtual bool IsSuspended { get; set; }
        public virtual bool IsCapturingSignature { get; set; }
        public virtual ElectionDuration ElectionDuration { get; set; }

    }
}