using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class IdentityUserRegionViewMap : IAutoMappingOverride<IdentityUserRegionView>
    {
        public void Override(AutoMapping<IdentityUserRegionView> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("v_IdentityUserRegions");
            mapping.Schema(Schemas.RSA);
            mapping.References(x => x.IdentityUser);
            mapping.References(x => x.Region);

            mapping.Id(x => x.Id, "regionId");
        }
    }
}