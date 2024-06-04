using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.Domain
{
    public class Role : SaiseBaseEntity, IRole<long>
    {
        private readonly IList<AssignedRole> _assignedRoles;
        private readonly IList<AssignedPermission> _assignedPermissions;

        public Role()
        {
            _assignedRoles = new List<AssignedRole>();
            _assignedPermissions = new List<AssignedPermission>();
        }

        public virtual string Name { get; set; }

        public virtual RoleLevel Level { get; set; }

        public virtual IReadOnlyCollection<AssignedRole> AssignedRoles
        {
            get { return new ReadOnlyCollection<AssignedRole>(_assignedRoles); }
        }

        public virtual IReadOnlyCollection<AssignedPermission> AssignedPermissions
        {
            get { return new ReadOnlyCollection<AssignedPermission>(_assignedPermissions); }
        }

        public virtual IEnumerable<SystemUser> GetUsers()
        {
            return _assignedRoles.Select(x => x.SystemUser);
        }

        public virtual IEnumerable<Permission> GetPermissions()
        {
            return _assignedPermissions.Select(x => x.Permission);
        }
    }
}