using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class ElectionTurnoutMap : IAutoMappingOverride<ElectionTurnout>
    {
        public void Override(AutoMapping<ElectionTurnout> mapping)
        {
            mapping.Table("ElectionTurnout");
            mapping.Id(x => x.Id).Column("ElectionTurnoutId");
            mapping.Map(x => x.AssignedPollingStationId).Not.Nullable().Column("AssignedPollingStationId");
            mapping.Map(x => x.ListCount).Not.Nullable().Column("ListCount");
            mapping.Map(x => x.SupplementaryCount).Not.Nullable().Column("SupplementaryCount");
            mapping.Map(x => x.TimeOfEntry).Not.Nullable().Column("TimeOfEntry");
        }
    }
}
