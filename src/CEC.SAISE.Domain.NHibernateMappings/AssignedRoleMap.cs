using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AssignedRoleMap : IAutoMappingOverride<AssignedRole>
    {
        public void Override(AutoMapping<AssignedRole> mapping)
        {
            mapping.Table("AssignedRole");
            mapping.Id(x => x.Id).Column("AssignedRoleId");
            mapping.References(x => x.Role);
            mapping.References(x => x.SystemUser);
        }
    }
}