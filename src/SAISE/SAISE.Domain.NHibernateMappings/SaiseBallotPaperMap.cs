using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class SaiseBallotPaperMap : IAutoMappingOverride<SaiseBallotPaper>
    {
        public void Override(AutoMapping<SaiseBallotPaper> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("BallotPaper");
            mapping.Id(x => x.Id).Column("BallotPaperId");

            mapping.Map(x => x.Status);

            mapping.References(x => x.Election);
            mapping.References(x => x.PollingStation);
        }
    }
}