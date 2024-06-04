using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class SaiseRegionMap : IAutoMappingOverride<SaiseRegion>
    {
        public void Override(AutoMapping<SaiseRegion> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("Region");
            mapping.Id(x => x.Id).Column("RegionId");
            mapping.Map(x => x.Name).Not.Nullable().Column("Name");
            mapping.Map(x => x.NameRu).Column("NameRu");
            mapping.Map(x => x.Description).Column("Description");
        }
    }
}
