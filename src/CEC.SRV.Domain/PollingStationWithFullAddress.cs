using Amdaris.Domain;

namespace CEC.SRV.Domain
{
	public class PollingStationWithFullAddress : Entity
	{
		public virtual PollingStation PollingStation { get; set; }
		public virtual string FullAddress { get; set; }
		public virtual int TotalAddress { get; set; }
		public virtual string name { get; set; }
	}
}