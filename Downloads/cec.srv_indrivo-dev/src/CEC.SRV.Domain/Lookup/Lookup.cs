using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SRV.Domain.Lookup
{
    public abstract class Lookup : AuditedEntity<IdentityUser>
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
		public virtual string GetObjectType()
		{
			return GetType().Name;
		}
    }
}