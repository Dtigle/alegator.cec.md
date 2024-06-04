using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class PollingStationOpeningData
    {
        public long AssignedPollingStationId { get; set; }

        public long OpeningVoters { get; set; }

        public bool IsOpen { get; set; }

        public bool IsOpeningEnabled { get; set; }

        public long AssignedVotersCount { get; set; }
    }
}
