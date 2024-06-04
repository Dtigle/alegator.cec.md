using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class RegionTypeMap : IAutoMappingOverride<RegionType>
    {
        public void Override(AutoMapping<RegionType> mapping)
        {
            mapping.Table("RegionType");
            mapping.Id(x => x.Id).Column("RegionTypeId");
            mapping.Map(x => x.Name).Not.Nullable();
            mapping.Map(x => x.Description).Nullable();
            mapping.Map(x => x.Rank).Not.Nullable();
        }
    }
}
