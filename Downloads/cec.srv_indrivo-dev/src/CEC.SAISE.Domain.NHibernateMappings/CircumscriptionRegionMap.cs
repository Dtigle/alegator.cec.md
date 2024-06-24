using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{

    public class CircumscriptionRegionMap : IAutoMappingOverride<CircumscriptionRegion>
    {
        public void Override(AutoMapping<CircumscriptionRegion> mapping)
        {
            mapping.Table("CircumscriptionRegion");
            mapping.Id(x => x.Id).Column("CircumscriptionRegionId");
            mapping.References(x => x.ElectionRound).Not.Nullable();
            mapping.References(x => x.AssignedCircumscription).Not.Nullable();
            mapping.References(x => x.Region).Not.Nullable();
        }
    }
}
