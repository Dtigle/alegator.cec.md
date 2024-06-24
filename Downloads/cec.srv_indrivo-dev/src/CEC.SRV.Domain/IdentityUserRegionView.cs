using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class IdentityUserRegionView : Entity
    {
        public virtual Region Region { get; set; }

        public virtual SRVIdentityUser IdentityUser { get; set; }

    }
}