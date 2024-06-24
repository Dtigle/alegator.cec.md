using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionMap : IAutoMappingOverride<Election>
    {
        public void Override(AutoMapping<Election> mapping)
        {
            mapping.Table("Election");
            mapping.Id(x => x.Id).Column("ElectionId");
            mapping.References(x => x.Type).Not.Nullable().Column("Type");
            mapping.Map(x => x.Status).Not.Nullable().Column("Status");
            mapping.Map(x => x.DateOfElection).Not.Nullable().Column("DateOfElection");
            mapping.Map(x => x.Comments).Not.Nullable().Column("Comments");
            mapping.Map(x => x.ReportsPath).Not.Nullable().Column("ReportsPath");
        }
    }
}
