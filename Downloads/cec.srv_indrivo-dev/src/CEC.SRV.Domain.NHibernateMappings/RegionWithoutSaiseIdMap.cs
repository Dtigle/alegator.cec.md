using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RegionWithoutSaiseIdMap : IAutoMappingOverride<RegionWithoutSaiseId>
    {
        public void Override(AutoMapping<RegionWithoutSaiseId> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "regionId");
            mapping.Table("v_RegionWithoutSaiseId");
            mapping.Schema(Schemas.Lookup);
            
        }
    }
}