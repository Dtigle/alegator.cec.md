using System;

namespace SAISE.Domain
{
    public class AssignedVoter : SaiseEntity
    {
        public virtual long RegionId { get; set; }

        public virtual SaisePollingStation RequestingPollingStation { get; set; }

        public virtual SaisePollingStation PollingStation { get; set; }

        public virtual Voter Voter { get; set; }

        public virtual long Category { get; set; }

        public virtual long Status { get; set; }

        public virtual DateTime EditedDate { get; set; }

        public virtual long EditedUserId { get; set; }

        public virtual int Version { get; set; }
    }
}