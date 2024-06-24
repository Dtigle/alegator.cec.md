using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PollingStationStatisticsMap : IAutoMappingOverride<PollingStationStatistics>
    {
        public void Override(AutoMapping<PollingStationStatistics> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "pollingStationId");
            mapping.Table("v_PollingStationStatistics");
            mapping.Schema(Schemas.RSA);
        }
    }
}