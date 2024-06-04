using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
	public class ProblematicDataPollingStationStatisticsMap : IAutoMappingOverride<ProblematicDataPollingStationStatistics>
    {
		public void Override(AutoMapping<ProblematicDataPollingStationStatistics> mapping)
        {
			mapping.ReadOnly();
			mapping.Id(x => x.Id, "pollingStationId");
			mapping.Table("v_ProblematicDataPollingStationStatistics");
			mapping.Schema(Schemas.RSA);
        }
    }
}