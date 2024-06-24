using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionCompetitorMemberMap : IAutoMappingOverride<ElectionCompetitorMember>
    {
        public void Override(AutoMapping<ElectionCompetitorMember> mapping)
        {
            mapping.Table("ElectionCompetitorMember");
            mapping.Id(x => x.Id).Column("ElectionCompetitorMemberId");
            mapping.Map(x => x.LastNameRo).Not.Nullable().Length(100);
            mapping.Map(x => x.LastNameRu).Nullable().Length(100);
            mapping.Map(x => x.NameRo).Not.Nullable().Length(100);
            mapping.Map(x => x.NameRu).Nullable().Length(100);
            mapping.Map(x => x.PatronymicRo).Nullable().Length(100);
            mapping.Map(x => x.PatronymicRu).Nullable().Length(100);
            mapping.Map(x => x.DateOfBirth).Not.Nullable();
            mapping.Map(x => x.PlaceOfBirth).Not.Nullable().Length(100);
            mapping.Map(x => x.Gender).CustomType<EnumType<GenderType>>().Not.Nullable();
            mapping.Map(x => x.Occupation).Nullable().Length(50);
            mapping.Map(x => x.OccupationRu).Nullable().Length(50);
            mapping.Map(x => x.Designation).Nullable().Length(50);
            mapping.Map(x => x.DesignationRu).Nullable().Length(50);
            mapping.Map(x => x.Workplace).Nullable().Length(100);
            mapping.Map(x => x.WorkplaceRu).Nullable().Length(100);
            mapping.Map(x => x.Idnp).Not.Nullable();
            mapping.Map(x => x.DateOfRegistration).Nullable();
            mapping.Map(x => x.ColorLogo).CustomSqlType("VARBINARY(MAX)").Nullable();
            mapping.Map(x => x.BlackWhiteLogo).LazyLoad().CustomSqlType("VARBINARY(MAX)").Length(int.MaxValue);
            mapping.Map(x => x.Status).CustomType<EnumType<CandidateStatus>>().Not.Nullable();
            mapping.Map(x => x.CompetitorMemberOrder).Not.Nullable();

            mapping.References(x => x.ElectionCompetitor).Column("ElectionCompetitorId").Not.Nullable();
            mapping.References(x => x.ElectionRound).Column("ElectionRoundId").Not.Nullable();
            mapping.References(x => x.AssignedCircumscription).Column("AssignedCircumscriptionId").Not.Nullable();

            mapping.HasMany(x => x.ElectionResults)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}