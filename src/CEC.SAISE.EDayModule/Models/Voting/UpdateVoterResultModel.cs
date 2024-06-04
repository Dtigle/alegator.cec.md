using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class UpdateVoterResultModel
    {
        public bool Success { get; set; }

        public PollingStationStatisticsModel Statistics { get; set; }
    }
}