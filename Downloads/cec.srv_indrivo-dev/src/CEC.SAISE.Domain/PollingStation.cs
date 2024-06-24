using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CEC.SAISE.Domain
{
    public class PollingStation : SaiseBaseEntity
    {
        private readonly IList<BallotPaper> _ballotPapers;
        private string _fullName;
        private string _numberPerElection;

        public PollingStation()
        {
            _ballotPapers = new List<BallotPaper>();
        }

        public virtual PollingStationType Type { get; set; }

        public virtual int Number { get; set; }

        public virtual string SubNumber { get; set; }

        public virtual string NameRo { get; set; }

        public virtual string NameRu { get; set; }

        public virtual string OldName { get; set; }

        public virtual Region Region { get; set; }

        public virtual long? StreetId { get; set; }

        public virtual int? StreetNumber { get; set; }

        public virtual string StreetSubNumber { get; set; }

        public virtual double? LocationLatitude { get; set; }

        public virtual double? LocationLongitude { get; set; }

        public virtual bool ExcludeInLocalElections { get; set; }

        public virtual IReadOnlyCollection<BallotPaper> BallotPapers
        {
            get { return new ReadOnlyCollection<BallotPaper>(_ballotPapers); }
        }

        public virtual string GetFullName()
        {
            return string.Format("{0,3:D3} - {1}", this.Number, this.NameRo);
        }

        public virtual string NumberPerElection
        {
            get { return _numberPerElection; }
        }

        public virtual string FullName
        {
            get { return _fullName; }
        }
    }
}