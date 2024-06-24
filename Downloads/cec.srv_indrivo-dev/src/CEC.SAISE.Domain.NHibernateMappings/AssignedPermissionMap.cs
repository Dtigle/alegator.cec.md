using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AssignedPermissionMap : IAutoMappingOverride<AssignedPermission>
    {
        public void Override(AutoMapping<AssignedPermission> mapping)
        {
            mapping.Table("AssignedPermission");
            mapping.Id(x => x.Id).Column("AssignedPermissionId");
            mapping.References(x => x.Role);
            mapping.References(x => x.Permission);
        }
    }
}