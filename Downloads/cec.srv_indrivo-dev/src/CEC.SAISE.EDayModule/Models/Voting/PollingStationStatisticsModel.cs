using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class PollingStationStatisticsModel
    {
        public long BaseListCounter { get; set; }

        public long SupplimentaryListCounter { get; set; }

        public long VotedCounter { get; set; }

        public bool IsOpen { get; set; }
    }
}