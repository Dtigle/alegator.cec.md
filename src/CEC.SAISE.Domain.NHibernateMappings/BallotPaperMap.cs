using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class BallotPaperMap : IAutoMappingOverride<BallotPaper>
    {
        public void Override(AutoMapping<BallotPaper> mapping)
        {
            mapping.Table("BallotPaper");
            mapping.Id(x => x.Id).Column("BallotPaperId");
            mapping.Map(x => x.EntryLevel).CustomType<EnumType<DelimitationType>>().Not.Nullable();
            mapping.Map(x => x.Type).Not.Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<BallotPaperStatus>>().Not.Nullable();
            mapping.Map(x => x.RegisteredVoters).Not.Nullable();
            mapping.Map(x => x.Supplementary).Not.Nullable();
            mapping.Map(x => x.BallotsIssued).Not.Nullable();
            mapping.Map(x => x.BallotsCasted).Not.Nullable();
            mapping.Map(x => x.DifferenceIssuedCasted).Not.Nullable();
            mapping.Map(x => x.BallotsValidVotes).Not.Nullable();
            mapping.Map(x => x.BallotsReceived).Not.Nullable();
            mapping.Map(x => x.BallotsUnusedSpoiled).Not.Nullable();
            mapping.Map(x => x.BallotsSpoiled).Not.Nullable();
            mapping.Map(x => x.BallotsUnused).Not.Nullable();
            mapping.Map(x => x.Description).Nullable();
            mapping.Map(x => x.Comments).Nullable();
            mapping.Map(x => x.DateOfEntry).Not.Nullable();
            mapping.Map(x => x.VotingPointId).Nullable();
            mapping.Map(x => x.IsResultsConfirmed).Nullable();
            mapping.Map(x => x.ConfirmationUserId).Nullable();
            mapping.Map(x => x.ConfirmationDate).Nullable();

            mapping.References(x => x.PollingStation).Column("PollingStationId");
            mapping.References(x => x.ElectionRound).Column("ElectionRoundId");

            mapping.HasMany(x => x.ElectionResults)
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}