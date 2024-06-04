using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.PermissionManage
{
    public class OptionsToggleModel
    {
        public OptionsToggleModel()
        {
            SelectedAPSIds = new List<long>();
        }

        public long ElectionId { get; set; }

        public bool EnableOpening { get; set; }

        public bool EnableTurnout { get; set; }

        public bool EnableElectionResults { get; set; }

        public int Action { get; set; }

        public List<long> SelectedAPSIds { get; set; }
    }
}