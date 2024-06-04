using System;

namespace SAISE.Domain
{
    public class ElectionDay : SaiseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTimeOffset ElectionDayDate { get; set; }
        public virtual DateTimeOffset DeployDbDate { get; set; }
    }
}
