using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class AssignedPollingstationMap : IAutoMappingOverride<AssignedPollingStation>
    {
        public void Override(AutoMapping<AssignedPollingStation> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("AssignedPollingStation");
            mapping.Id(x => x.Id).Column("AssignedPollingStationId");
            mapping.Map(x => x.Type).Not.Nullable().Column("Type");
            mapping.Map(x => x.Status).Not.Nullable().Column("Status");
            mapping.Map(x => x.IsOpen).Not.Nullable().Column("IsOpen");

            mapping.References(x => x.ElectionRound).ReadOnly().Not.Nullable().Column("ElectionRoundId");
            mapping.References(x => x.PollingStation).ReadOnly().Not.Nullable().Column("PollingStationId");
        }
    }
}