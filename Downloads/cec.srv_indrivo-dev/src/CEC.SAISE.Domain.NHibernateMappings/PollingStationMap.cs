using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class PollingStationMap : IAutoMappingOverride<PollingStation>
    {
        public void Override(AutoMapping<PollingStation> mapping)
        {
            mapping.Table("PollingStation");
            mapping.Id(x => x.Id).Column("PollingStationId");
            mapping.Map(x => x.Number).Not.Nullable();
            mapping.Map(x => x.SubNumber).Nullable().Length(50);
            mapping.Map(x => x.NameRo).Not.Nullable();
            mapping.Map(x => x.NameRu).Nullable();
            mapping.Map(x => x.OldName).Nullable();
            mapping.Map(x => x.StreetId).Nullable();
            mapping.Map(x => x.StreetNumber).Nullable();
            mapping.Map(x => x.StreetSubNumber).Nullable().Length(50);
            mapping.Map(x => x.LocationLatitude).Nullable();
            mapping.Map(x => x.LocationLongitude).Nullable();
            mapping.Map(x => x.ExcludeInLocalElections).Not.Nullable();
            mapping.Map(x => x.Type).CustomType<EnumType<PollingStationType>>().Not.Nullable();
            mapping.References(x => x.Region);
            mapping.HasMany(x => x.BallotPapers)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.Map(x => x.NumberPerElection).Formula("(SELECT TOP 1 aps.numberPerElection  FROM [dbo].[AssignedPollingStation] as aps  WHERE  aps.PollingStationId = [PollingStationId] and aps.numberPerElection is not null and aps.numberPerElection <> '')").Access.CamelCaseField(Prefix.Underscore);

            //mapping.Map(x => x.FullName).Formula("right('000' + cast([Number] as varchar(3)), 3) + ' - ' + [NameRo]").Access.CamelCaseField(Prefix.Underscore);
            mapping.Map(x => x.FullName).Formula("CONCAT((SELECT TOP 1 aps.numberPerElection  FROM [dbo].[AssignedPollingStation] as aps  WHERE  aps.PollingStationId = [PollingStationId] and aps.numberPerElection is not null and aps.numberPerElection <> ''),' - ',[NameRo])").Access.CamelCaseField(Prefix.Underscore);
        }
    }
}