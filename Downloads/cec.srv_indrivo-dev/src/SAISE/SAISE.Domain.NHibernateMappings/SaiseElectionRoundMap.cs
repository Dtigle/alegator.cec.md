using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAISE.Domain.NHibernateMappings
{
    public class SaiseElectionRoundMap : IAutoMappingOverride<SaiseElectionRound>
    {
        public void Override(AutoMapping<SaiseElectionRound> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("ElectionRound");
            mapping.Id(x => x.Id).Column("ElectionRoundId");
            mapping.References(x => x.Election).Not.Nullable();
            mapping.Map(x => x.Status).CustomType<ElectionRoundStatus>().Not.Nullable();
            mapping.HasMany(x => x.AssignedPollingStations).KeyColumn("ElectionRoundId");
        }
    }
}
