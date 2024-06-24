using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class AddressWithPollingStationMap : IAutoMappingOverride<AddressWithPollingStation>
    {
        public void Override(AutoMapping<AddressWithPollingStation> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "addressId");
            mapping.Map(x => x.RegionId);
            mapping.Map(x => x.StreetId);
            mapping.Table("v_AddressWithPollingStations");
            mapping.Schema(Schemas.RSA);
        }
    }
}