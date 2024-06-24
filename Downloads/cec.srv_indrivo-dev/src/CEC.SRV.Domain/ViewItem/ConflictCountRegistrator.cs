using Amdaris.Domain;

namespace CEC.SRV.Domain.ViewItem
{
    public class ConflictCountRegistrator : Entity
    {
        public virtual long RegionId { get; set; }

        public virtual int ConflictCount { get; set; }
    }
}