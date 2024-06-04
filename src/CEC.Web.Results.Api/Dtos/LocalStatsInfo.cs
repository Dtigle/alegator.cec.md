using System.Runtime.Serialization;
using System.ServiceModel;

namespace CEC.Web.Results.Api.Dtos
{
	[DataContract]
	public class LocalStatsInfo
	{
		[DataMember]
		public StatsInfo StatsInfo { get; set; }

		[DataMember]
		public long ElectionId { get; set; }

		[DataMember]
		public long DistrictId { get; set; }
	}
}