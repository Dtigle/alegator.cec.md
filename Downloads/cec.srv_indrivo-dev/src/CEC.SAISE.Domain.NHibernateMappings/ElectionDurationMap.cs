using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionDurationMap : IAutoMappingOverride<ElectionDuration>
    {
        public void Override(AutoMapping<ElectionDuration> mapping)
        {
            mapping.Table("ElectionDuration");
            mapping.Schema("dbo");
            mapping.Id(x => x.Id).Column("ElectionDurationId");
            mapping.Map(x => x.Name).Not.Nullable();
        }
    }
}
