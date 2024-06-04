using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using FluentNHibernate.Utils;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
	public class ElectionCompetitorMap : IAutoMappingOverride<ElectionCompetitor>
    {
        public void Override(AutoMapping<ElectionCompetitor> mapping)
        {
            mapping.Table("ElectionCompetitor");
            mapping.Id(x => x.Id).Column("ElectionCompetitorId");
            mapping.Map(x => x.PoliticalPartyId).Column("PoliticalPartyId").Nullable();
            mapping.Map(x => x.Code).Not.Nullable().Length(100);
            mapping.Map(x => x.NameRo).Not.Nullable();
            mapping.Map(x => x.NameRu).Not.Nullable();
            mapping.Map(x => x.DateOfRegistration).Not.Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<PoliticalPartyStatus>>().Not.Nullable();
            mapping.Map(x => x.IsIndependent).Not.Nullable();
            mapping.Map(x => x.BallotOrder).Not.Nullable();
            mapping.Map(x => x.PartyOrder).Nullable();
            mapping.Map(x => x.DisplayFromNameRo).Nullable();
            mapping.Map(x => x.DisplayFromNameRu).Nullable();
            mapping.Map(x => x.RegistryNumber).Nullable();
            mapping.Map(x => x.ColorLogo).LazyLoad().CustomSqlType("VARBINARY(MAX)").Length(int.MaxValue);
            mapping.Map(x => x.BlackWhiteLogo).LazyLoad().CustomSqlType("VARBINARY(MAX)").Length(int.MaxValue);
            mapping.References(x => x.ElectionRound).Column("ElectionRoundId").Not.Nullable();
            mapping.References(x => x.AssignedCircumscription).Column("AssignedCircumscriptionId").Not.Nullable();


            mapping.HasMany(x => x.StatusOverrides)
		        .Not.Inverse()
				.Not.KeyNullable()
				.Not.KeyUpdate()
				.KeyColumn("ElectionCompetitorId")
		        .Cascade.All();

			mapping.HasMany(x => x.ElectionCompetitorMembers)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.HasMany(x => x.ElectionResults)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}