using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CEC.SAISE.Domain
{
    public class ElectionCompetitor : SaiseBaseEntity
    {
        private readonly IList<ElectionCompetitorMember> _electionCompetitorMembers;
        private readonly IList<ElectionResult> _electionResults;

        public ElectionCompetitor()
        {
            _electionCompetitorMembers = new List<ElectionCompetitorMember>();
            _electionResults = new List<ElectionResult>();
            StatusOverrides = new List<PoliticalPartyStatusOverride>();
        }

        public virtual ElectionRound ElectionRound { get; set; }

        public virtual AssignedCircumscription AssignedCircumscription { get; set; }

        public virtual long? PoliticalPartyId { get; set; }

        public virtual string Code { get; set; }

        public virtual string NameRo { get; set; }

        public virtual string NameRu { get; set; }
     

        public virtual DateTime DateOfRegistration { get; set; }

        public virtual PoliticalPartyStatus Status { get; set; }

        public virtual IList<PoliticalPartyStatusOverride> StatusOverrides { get; set; }

        public virtual bool IsIndependent { get; set; }

        public virtual int BallotOrder { get; set; }

        public virtual int PartyOrder { get; set; }

        public virtual string DisplayFromNameRo { get; set; }

        public virtual string DisplayFromNameRu { get; set; }

        public virtual int? RegistryNumber { get; set; }

        public virtual byte[] ColorLogo { get; set; }
        public virtual byte[] BlackWhiteLogo { get; set; }

        public virtual IReadOnlyCollection<ElectionCompetitorMember> ElectionCompetitorMembers
        {
            get { return new ReadOnlyCollection<ElectionCompetitorMember>(_electionCompetitorMembers); }
        }

        public virtual IReadOnlyCollection<ElectionResult> ElectionResults
        {
            get { return new ReadOnlyCollection<ElectionResult>(_electionResults); }
        }

        public virtual void AddCandidate(ElectionCompetitorMember electionCompetitorMember)
        {
            _electionCompetitorMembers.Add(electionCompetitorMember);
        }

        public virtual void OverrideStatus(PoliticalPartyStatusOverride statusOverride)
        {
            var overridedStatus = StatusOverrides.FirstOrDefault(x =>
                x.ElectionRound.Id == statusOverride.ElectionRound.Id &&
                x.AssignedCircumscription.Id == statusOverride.AssignedCircumscription.Id);

            if (overridedStatus == null)
            {
                StatusOverrides.Add(statusOverride);
            }
            else
            {
                overridedStatus.Status = statusOverride.Status;
            }
        }
    }
}