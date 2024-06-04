using System;
using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{
	[DataContract]
	public class LocalElectionResult
	{
		[DataMember]
		public long TotalPollingStations { get; set; }

		[DataMember]
		public long BallotPapersCount { get; set; }

		[DataMember]
		public double BallotPapersPercentage {
			get
			{
				return (TotalPollingStations == 0) ? 0 : BallotPapersCount / (double)TotalPollingStations;
			} 
		}

		[DataMember]
		public string BallotPapersPercentageFormatted {
			get { return string.Format("{0:P}", BallotPapersPercentage); }
		}

	}
}