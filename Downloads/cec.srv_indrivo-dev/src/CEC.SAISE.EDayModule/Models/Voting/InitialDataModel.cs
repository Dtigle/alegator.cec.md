using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class InitialDataModel
    {
        public UserDataModel UserData { get; set; }

        public PollingStationStatisticsModel PollingStationStatistics { get; set; }
    }
}