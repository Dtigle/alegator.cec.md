using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SRV.Domain
{
    public abstract class SRVBaseEntity : AuditedEntity<IdentityUser>
    {
		public virtual string GetObjectType()
		{
			return GetType().Name;
		}
    }
}
