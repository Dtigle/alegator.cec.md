using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionResultMap : IAutoMappingOverride<ElectionResult>
    {
        public void Override(AutoMapping<ElectionResult> mapping)
        {
            mapping.Table("ElectionResult");
            mapping.Id(x => x.Id).Column("ElectionResultId");
            mapping.Map(x => x.BallotOrder).Not.Nullable();
            mapping.Map(x => x.BallotCount).Not.Nullable();
            mapping.Map(x => x.Comments).Not.Nullable();
            mapping.Map(x => x.DateOfEntry).Not.Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<ElectionResultStatus>>().Not.Nullable();
            
            mapping.References(x => x.BallotPaper).Column("BallotPaperId").Not.Nullable();
            mapping.References(x => x.PoliticalParty).Column("ElectionCompetitorId").Not.Nullable();
            mapping.References(x => x.ElectionRound).Column("ElectionRoundId").Not.Nullable();
            mapping.References(x => x.Candidate).Column("ElectionCompetitorMemberId").Nullable();
        }
    }
}