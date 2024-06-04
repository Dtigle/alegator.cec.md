using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class SaiseBaseEntityMap : IAutoMappingOverride<SaiseBaseEntity>
    {
        public void Override(AutoMapping<SaiseBaseEntity> mapping)
        {
            mapping.References(x => x.EditUser).Not.Nullable().Column("EditUserId");
            mapping.Map(x => x.EditDate).Not.Nullable().Column("EditDate");
            mapping.Map(x => x.Version).Not.Nullable().Column("Version");
        }
    }
}