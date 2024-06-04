using Amdaris.Domain;

namespace CEC.SRV.Domain
{
    public class StreetView : Entity
    {
        public virtual string FullName { get; set; }
        public virtual long RegionId { get; set; }
        public virtual long StreetId { get; set; }
    }
}
