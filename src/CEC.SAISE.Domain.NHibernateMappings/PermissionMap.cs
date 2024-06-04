using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class PermissionMap : IAutoMappingOverride<Permission>
    {
        public void Override(AutoMapping<Permission> mapping)
        {
            mapping.Table("Permission");
            mapping.Id(x => x.Id).Column("PermissionId");
            mapping.Map(x => x.Name).Not.Nullable();

            mapping.HasMany(x => x.AssignedPermissions)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}