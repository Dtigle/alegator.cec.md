using System;
using CEC.SRV.Domain.Lookup;

namespace CEC.Web.SRV.Models.Conflict
{
    public class ConflictShareViewModel
    {
        public virtual long SourceRegionId { get; set; }
        public virtual string SourceRegionName { get; set; }
        public virtual long DestinationRegionId { get; set; }
        public virtual string DestinationRegionName { get; set; }

        public virtual string Reason { get; set; }

        public virtual string Note { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual string Created { get; set; }

    }
}