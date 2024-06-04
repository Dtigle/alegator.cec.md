using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class LinkedRegionsFullName : Entity
    {
    //    public virtual Region Region { get; set; }

        public virtual LinkedRegion LinkedRegion { get; set; }
        public virtual string FullyQualifiedName { get; set; }
    }
}