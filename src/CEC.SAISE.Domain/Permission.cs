using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CEC.SAISE.Domain
{
    public class Permission : SaiseBaseEntity
    {
        private readonly IList<AssignedPermission> _assignedPermissions;

        public Permission()
        {
            _assignedPermissions = new List<AssignedPermission>();
        }

        public virtual string Name { get; set; }

        public virtual IReadOnlyCollection<AssignedPermission> AssignedPermissions
        {
            get { return new ReadOnlyCollection<AssignedPermission>(_assignedPermissions); }
        }

        public virtual IEnumerable<Role> GetRoles()
        {
            return _assignedPermissions.Select(x => x.Role);
        }
    }
}