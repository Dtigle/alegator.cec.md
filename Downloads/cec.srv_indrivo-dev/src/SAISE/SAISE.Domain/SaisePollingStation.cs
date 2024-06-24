using System.Collections.Generic;

namespace SAISE.Domain
{
    public class SaisePollingStation : SaiseEntity
    {
        public SaisePollingStation()
        {
            AssignedPollingStations = new List<AssignedPollingStation>();
        }

        public virtual int Number { get; set; }

        public virtual string SubNumber { get; set; }

        public virtual IEnumerable<AssignedPollingStation> AssignedPollingStations { get; set; }

        public virtual SaiseRegion Region { get; set; }
    }
}