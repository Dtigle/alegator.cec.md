using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class VoterMap : IAutoMappingOverride<Voter>
    {
        public void Override(AutoMapping<Voter> mapping)
        {
            mapping.Table("Voter");
            mapping.Id(x => x.Id).Column("VoterId");
            mapping.Map(x => x.NameRo).Not.Nullable().Length(100);
            mapping.Map(x => x.LastNameRo).Not.Nullable().Length(100);
            mapping.Map(x => x.PatronymicRo).Nullable().Length(100);
            mapping.Map(x => x.NameRu).Nullable().Length(100);
            mapping.Map(x => x.LastNameRu).Nullable().Length(100);
            mapping.Map(x => x.PatronymicRu).Nullable().Length(100);
            mapping.Map(x => x.DateOfBirth).Not.Nullable();
            mapping.Map(x => x.PlaceOfBirth).Nullable();
            mapping.Map(x => x.PlaceOfResidence).Nullable();
            mapping.Map(x => x.Gender).CustomType<EnumType<GenderType>>().Not.Nullable();
            mapping.Map(x => x.DateOfRegistration).Not.Nullable();
            mapping.Map(x => x.Idnp).Not.Nullable();
            mapping.Map(x => x.DocumentNumber).Not.Nullable().Length(50);
            mapping.Map(x => x.DateOfIssue).Nullable();
            mapping.Map(x => x.DateOfExpiry).Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<VoterStatus>>().Not.Nullable();
            mapping.Map(x => x.BatchId).Nullable();
            mapping.Map(x => x.StreetId).Nullable();
            mapping.Map(x => x.StreetName).Nullable();
            mapping.Map(x => x.StreetNumber).Nullable();
            mapping.Map(x => x.StreetSubNumber).Nullable().Length(50);
            mapping.Map(x => x.BlockNumber).Nullable();
            mapping.Map(x => x.BlockSubNumber).Nullable().Length(50);
            mapping.References(x => x.Region).Not.Nullable();
            mapping.Map(x => x.ElectionListNumber).Column("ElectionListNr").Nullable();
            mapping.HasMany(x => x.AssignedVoters)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);

          
      







        }
    }
}