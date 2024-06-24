using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{
    [DataContract]
    public class StatsInfo
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string CurrentTime { get; set; }

        [DataMember]
        public int TotalConnections { get; set; }

        [DataMember]
        public IEnumerable<StatBaseInfo> BaseInfo { get; set; }

        [DataMember]
        public StatPercentInfo PercentInfo { get; set; }

        [DataMember]
        public IEnumerable<StatPreliminaryResult> Preliminary { get; set; }

        [DataMember]
        public long PreliminaryTotalCalculated { get; set; }

        [DataMember]
        public long TotalParticipants { get; set; }

        [DataMember]
        public string PercentCalculated {
	        get
	        {
				return string.Format("{0:P}", (TotalParticipants == 0) ? 0 :
				(double)PreliminaryTotalCalculated / TotalParticipants);
	        }
		}

        [DataMember]
        public int TotalBallotPapers { get; set; }

        [DataMember]
        public int TotalProcessedBallotPapers { get; set; }

        [DataMember]
		public string PercentProcessedBallotPapers
		{
			get
			{
				return string.Format("{0:P}",
				(TotalBallotPapers != 0)
					? (double)TotalProcessedBallotPapers / TotalBallotPapers
					: 0.0);
			}
		}

        [DataMember]
        public long TotalValidVotes { get; set; }

        [DataMember]
        public long TotalSpoiledVotes { get; set; }

        [DataMember]
        public long TotalVotes { get; set; }

        [DataMember]
        public bool ResultsProcessingStarted { get; set; }
    }
}