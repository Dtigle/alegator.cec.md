using Amdaris.Domain;

namespace CEC.SRV.Domain
{
	public class ProblematicDataPollingStationStatistics : Entity
    {
        public virtual long RegionId { get; set; }

		public virtual string FullRegionName { get; set; }

        public virtual string PollingStation { get; set; }

		public virtual int VotersCount { get; set; }
    }
}