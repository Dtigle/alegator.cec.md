using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CEC.SAISE.Domain
{
    public class ElectionCompetitorMember : SaiseBaseEntity
    {
        private readonly IList<ElectionResult> _electionResults;

        public ElectionCompetitorMember()
        {
            _electionResults = new List<ElectionResult>();
        }

        public virtual string LastNameRo { get; set; }

        public virtual string LastNameRu { get; set; }

        public virtual string NameRo { get; set; }

        public virtual string NameRu { get; set; }

        public virtual string PatronymicRo { get; set; }

        public virtual string PatronymicRu { get; set; }

        public virtual DateTime DateOfBirth { get; set; }

        public virtual string PlaceOfBirth { get; set; }

        public virtual GenderType Gender { get; set; }

        public virtual string Occupation { get; set; }

        public virtual string OccupationRu { get; set; }

        public virtual string Designation { get; set; }

        public virtual string DesignationRu { get; set; }

        public virtual string Workplace { get; set; }

        public virtual string WorkplaceRu { get; set; }

        public virtual long Idnp { get; set; }

        public virtual ElectionCompetitor ElectionCompetitor { get; set; }

        public virtual ElectionRound ElectionRound { get; set; }

        public virtual AssignedCircumscription AssignedCircumscription { get; set; }

        public virtual DateTime? DateOfRegistration { get; set; }

        public virtual byte[] ColorLogo { get; set; }
        public virtual byte[] BlackWhiteLogo { get; set; }

        public virtual CandidateStatus Status { get; set; }

        public virtual int CompetitorMemberOrder { get; set; }

        public virtual IReadOnlyCollection<ElectionResult> ElectionResults
        {
            get { return new ReadOnlyCollection<ElectionResult>(_electionResults); }
        }
    }
}