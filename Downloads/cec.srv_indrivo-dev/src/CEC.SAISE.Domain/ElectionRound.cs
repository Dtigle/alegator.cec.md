using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
    public class ElectionRound : SaiseBaseEntity
    {
        public virtual IEnumerable<AssignedPollingStation> AssignedPollingStations { get; set; }

        public virtual Election Election { get; set; }

        public virtual int Number { get; set; }

        public virtual string NameRo { get; set; }

        public virtual string NameRu { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime ElectionDate { get; set; }

        public virtual ElectionRoundStatus Status { get; set; }

        public virtual DateTime? CampaignStartDate { get; set; }

        public virtual DateTime? CampaignEndDate { get; set; }
        public virtual bool RepeatElection { get; set; }
    }

    public enum ElectionRoundStatus
    {
        [Description("Nou")]
        New = 1,

        [Description("Aprobat")]
        Aproved = 2,

        [Description("Cu rezultate")]
        WithResults = 3,

        [Description("Validat")]
        Validated = 4,
    }
}
