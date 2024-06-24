using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class SaiseElectionMap : IAutoMappingOverride<SaiseElection>
    {
        public void Override(AutoMapping<SaiseElection> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("Election");
            mapping.Id(x => x.Id).Column("ElectionId");
            mapping.Map(x => x.DateOfElection).Not.Nullable().Column("DateOfElection");
            mapping.References(x => x.Type).Not.Nullable().Column("Type");
        }
    }
}