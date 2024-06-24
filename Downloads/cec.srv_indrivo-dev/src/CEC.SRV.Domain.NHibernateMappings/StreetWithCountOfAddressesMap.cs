using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class StreetWithCountOfAddressesMap : IAutoMappingOverride<StreetWithCountOfAddresses>
    {
        public void Override(AutoMapping<StreetWithCountOfAddresses> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "streetId");
            mapping.Table("v_StreetWithCountOfAddresses");
            mapping.Schema(Schemas.RSA);
        }
    }
}