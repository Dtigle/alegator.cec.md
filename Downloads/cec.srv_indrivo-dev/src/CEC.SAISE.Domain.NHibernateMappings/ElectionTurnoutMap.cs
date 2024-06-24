using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionTurnoutMap : IAutoMappingOverride<ElectionTurnout>
    {
        public void Override(AutoMapping<ElectionTurnout> mapping)
        {
            mapping.Table("ElectionTurnout");
            mapping.Id(x => x.Id).Column("ElectionTurnoutId");
            mapping.Map(x => x.ListCount).Not.Nullable();
            mapping.Map(x => x.SupplementaryCount).Not.Nullable();
            mapping.Map(x => x.TimeOfEntry).Not.Nullable();
            mapping.References(x => x.AssignedPollingStation).Not.Nullable();
        }
    }
}