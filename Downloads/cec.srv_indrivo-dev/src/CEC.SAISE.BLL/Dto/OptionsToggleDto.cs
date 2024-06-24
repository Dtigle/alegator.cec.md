using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class OptionsToggleDto
    {
        public OptionsToggleDto()
        {
            SelectedAPSIds = new List<long>();
        }

        public long ElectionId { get; set; }

        public bool EnableOpening { get; set; }

        public bool EnableTurnout { get; set; }

        public bool EnableElectionResults { get; set; }

        public OptionsToggleActions Action { get; set; }

        public List<long> SelectedAPSIds { get; set; }
    }

    public enum OptionsToggleActions
    {
        SelectedAssignedPollingStations = 1,
        All = 2
    }
}
