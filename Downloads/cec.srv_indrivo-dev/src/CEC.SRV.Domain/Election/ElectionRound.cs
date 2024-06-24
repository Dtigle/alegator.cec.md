using CEC.SRV.Domain.Lookup;
using System;

namespace CEC.SRV.Domain
{
    public class ElectionRound : SRVBaseEntity
    {
        public virtual Election Election { get; set; }

        public virtual int Number { get; set; }

        public virtual string NameRo { get; set; }

        public virtual string NameRu { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime ElectionDate { get; set; }

        public virtual ElectionRoundStatus Status { get; set; }

        public virtual DateTime? CampaignStartDate { get; set; }

        public virtual DateTime? CampaignEndDate { get; set; }

        public virtual string ReportsPath { get; set; }
    }
}
