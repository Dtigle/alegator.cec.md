using CEC.SRV.Domain.Lookup;
using System;
using System.Collections.Generic;

namespace CEC.SRV.Domain
{
    public class Election : SRVBaseEntity
    {
        public virtual string NameRo { get; set; }

        public virtual string NameRu { get; set; }

        public virtual string Description { get; set; }

        public virtual ElectionType ElectionType { get; set; }

        public virtual ElectionStatus Status { get; set; }

        public virtual DateTimeOffset StatusDate { get; set; }

        public virtual string StatusReason { get; set; }

        public virtual string ReportsPath { get; set; }

        public virtual IList<ElectionRound> ElectionRounds { get; set; }
    }
}
