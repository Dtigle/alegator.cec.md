using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class AddressWithoutPollingStationMap : IAutoMappingOverride<AddressWithoutPollingStation>
    {
        public void Override(AutoMapping<AddressWithoutPollingStation> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "addressId");
            mapping.Table("v_AddressesWithoutPoolingStation");
            mapping.Schema(Schemas.RSA);
        }
    }
}