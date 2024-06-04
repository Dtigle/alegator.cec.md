using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CEC.SAISE.Domain
{
    public class BallotPaper : SaiseBaseEntity
    {
        private readonly IList<ElectionResult> _electionResults;

        public BallotPaper()
        {
            _electionResults = new List<ElectionResult>();
        }

        public virtual PollingStation PollingStation { get; set; }

        public virtual ElectionRound ElectionRound { get; set; }

        public virtual BallotPaperStatus Status { get; set; }

        public virtual DelimitationType EntryLevel { get; set; }

        public virtual int Type { get; set; }

        public virtual long RegisteredVoters { get; set; }

        public virtual long Supplementary { get; set; }

        public virtual long BallotsIssued { get; set; }

        public virtual long BallotsCasted { get; set; }

        public virtual long DifferenceIssuedCasted { get; set; }

        public virtual long BallotsValidVotes { get; set; }

        public virtual long BallotsReceived { get; set; }

        public virtual long BallotsUnusedSpoiled { get; set; }

        public virtual long BallotsSpoiled { get; set; }

        public virtual long BallotsUnused { get; set; }

        public virtual string Description { get; set; }

        public virtual string Comments { get; set; }

        public virtual DateTime DateOfEntry { get; set; }

        public virtual long? VotingPointId { get; set; }


        public virtual bool IsResultsConfirmed { get; set; }

        public virtual long? ConfirmationUserId { get; set; }

        public virtual DateTime? ConfirmationDate { get; set; }

        public virtual IReadOnlyCollection<ElectionResult> ElectionResults
        {
            get { return new ReadOnlyCollection<ElectionResult>(_electionResults); }
        }

	    public virtual void RemoveElectionResult(ElectionResult electionResult)
	    {
		    _electionResults.Remove(electionResult);
	    }

	    public virtual void RemoveElectionResult(long electionResultId)
	    {
		    var electionResult = _electionResults.First(x => x.Id == electionResultId);
		    _electionResults.Remove(electionResult);
	    }
    }
}