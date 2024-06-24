using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class StreetViewMap : IAutoMappingOverride<StreetView>
    {
        public void Override(AutoMapping<StreetView> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "streetId");
            mapping.Map(x => x.RegionId);
            mapping.Map(x => x.StreetId);
            mapping.Table("v_Streets");
            mapping.Schema(Schemas.RSA);
        }
    }
}
