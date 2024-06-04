using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class SaiseElectionTypeMap : IAutoMappingOverride<SaiseElectionType>
    {
        public void Override(AutoMapping<SaiseElectionType> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("ElectionType");
            mapping.Id(x => x.Id).Column("ElectionTypeId");
            mapping.Map(x => x.TypeName).Not.Nullable().Length(50).Column("TypeName");
        }
    }
}