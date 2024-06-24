using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class UpdatePollingStationActivityDto
    {
        public List<long> Ids { get; set; }
        public TimeSpan ElectionStartTime { get; set; }
        public TimeSpan ElectionEndTime { get; set; }
        public int TimeDifferenceMoldova { get; set; }
        public int ActivityTimeExtendedFirstDay { get; set; }
        public int ActivityTimeExtendedSecondDay { get; set; }
        public int ElectionDurationId { get; set; }
    }
}
