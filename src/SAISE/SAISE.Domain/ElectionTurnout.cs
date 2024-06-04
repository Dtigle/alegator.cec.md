using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAISE.Domain
{
    public class ElectionTurnout : SaiseEntity
    {
        public virtual long ListCount { get; set; }

        public virtual long SupplementaryCount { get; set; }

        public virtual string TimeOfEntry { get; set; }

        public virtual long AssignedPollingStationId { get; set; }
    }
}
