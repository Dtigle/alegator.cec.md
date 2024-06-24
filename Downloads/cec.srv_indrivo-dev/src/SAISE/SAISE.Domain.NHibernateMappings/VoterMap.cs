using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class VoterMap : IAutoMappingOverride<Voter>
    {
        public void Override(AutoMapping<Voter> mapping)
        {
            mapping.Table("VoterSRV");
            mapping.Id(x => x.Id).Column("VoterId");
            mapping.Map(x => x.NameRo).Length(100).Not.Nullable().Column("NameRo");
            mapping.Map(x => x.LastNameRo).Length(100).Not.Nullable().Column("LastNameRo");
            mapping.Map(x => x.PatronymicRo).Length(100).Nullable().Column("PatronymicRo");
            mapping.Map(x => x.DateOfBirth).Not.Nullable().Column("DateOfBirth");
            mapping.Map(x => x.PlaceOfBirth).Nullable().Column("PlaceOfBirth");
            mapping.Map(x => x.PlaceOfResidence).Nullable().Column("PlaceOfResidence");
            mapping.Map(x => x.Gender).Not.Nullable().Column("Gender");
            mapping.Map(x => x.DateOfRegistration).Column("DateOfRegistration");
            mapping.Map(x => x.Idnp).Not.Nullable().Column("Idnp");
            mapping.Map(x => x.DocumentNumber).Not.Nullable().Length(50).Column("DocumentNumber");
            mapping.Map(x => x.DateOfIssue).Nullable().Column("DateOfIssue");
            mapping.Map(x => x.DateOfExpiry).Nullable().Column("DateOfExpiry");
            mapping.Map(x => x.Status).Not.Nullable().Column("Status");
            mapping.Map(x => x.StreetId).Nullable().Column("StreetId");
            mapping.Map(x => x.StreetName).Nullable().Column("StreetName");
            mapping.Map(x => x.StreetNumber).Nullable().Column("StreetNumber");
            mapping.Map(x => x.StreetSubNumber).Length(50).Nullable().Column("StreetSubNumber");
            mapping.Map(x => x.BlockNumber).Nullable().Column("BlockNumber");
            mapping.Map(x => x.BlockSubNumber).Length(50).Nullable().Column("BlockSubNumber");
            mapping.Map(x => x.PollingStationId).Nullable().Column("PollingStationId");
            mapping.Map(x => x.RegionId).Nullable().Column("RegionId");
            mapping.Map(x => x.ElectionListNr).Nullable().Column("ElectionListNr");

        }
    }
}