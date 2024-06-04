using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
	public class PollingStationWithFullAddressMap : IAutoMappingOverride<PollingStationWithFullAddress>
    {
		public void Override(AutoMapping<PollingStationWithFullAddress> mapping)
        {
            mapping.ReadOnly();
			mapping.Id(x => x.Id, "pollingStationId");
			mapping.Table("v_PollingStationWithFullAddress");
            mapping.Schema(Schemas.RSA);
        }
    }
}