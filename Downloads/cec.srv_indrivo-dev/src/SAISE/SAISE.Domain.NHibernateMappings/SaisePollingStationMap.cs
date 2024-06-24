using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class SaisePollingStationMap : IAutoMappingOverride<SaisePollingStation>
    {
        public void Override(AutoMapping<SaisePollingStation> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("PollingStation");
            mapping.Id(x => x.Id).Column("PollingStationId");
            mapping.Map(x => x.Number).Not.Nullable().Column("Number");
            mapping.Map(x => x.SubNumber).Length(50).Nullable().Column("SubNumber");
            mapping.HasMany(x => x.AssignedPollingStations).KeyColumn("PollingStationId");
            mapping.References(x => x.Region).Not.Nullable().Column("RegionId");
        }
    }
}