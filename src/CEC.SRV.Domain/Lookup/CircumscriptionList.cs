using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SRV.Domain.Lookup
{
    public class CircumscriptionList : AuditedEntity<IdentityUser>
    {
        public virtual string Name { get; set; }
    }
}
