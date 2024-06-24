using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class RoleMap : IAutoMappingOverride<Role>
    {
        public void Override(AutoMapping<Role> mapping)
        {
            mapping.Table("Role");
            mapping.Id(x => x.Id).Column("RoleId");
            mapping.Map(x => x.Name).Not.Nullable();
            mapping.Map(x => x.Level).CustomType<EnumType<RoleLevel>>().Not.Nullable();

            mapping.HasMany(x => x.AssignedRoles)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.HasMany(x => x.AssignedPermissions)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}