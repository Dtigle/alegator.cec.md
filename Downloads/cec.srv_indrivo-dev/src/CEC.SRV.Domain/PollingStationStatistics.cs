using Amdaris.Domain;

namespace CEC.SRV.Domain
{
    public class PollingStationStatistics : Entity
    {
        public virtual long PollingStationId { get; set; }

        public virtual long RegionId { get; set; }

        public virtual string RegionName { get; set; }

        public virtual string PollingStation { get; set; }

        public virtual int VotersCount { get; set; }
    }
}