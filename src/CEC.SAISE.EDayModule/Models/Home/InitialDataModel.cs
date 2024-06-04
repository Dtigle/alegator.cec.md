using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.EDayModule.Models.Voting;

namespace CEC.SAISE.EDayModule.Models.Home
{
    public class DashboardModel : InitialDataModel
    {
        public TimeSpan PSOpenningStartTime { get; set; }
        public TimeSpan PSTournoutsStartTime { get; set; }
        public TimeSpan PSElectionResultsStartTime { get; set; }
        public bool ShowExercises { get; set; }
    }
}