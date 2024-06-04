using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PollingStationMap : IAutoMappingOverride<PollingStation>
    {
        public void Override(AutoMapping<PollingStation> mapping)
        {
            mapping.References(x => x.PollingStationAddress);
            mapping.HasMany(x => x.Addresses)
                .Access.CamelCaseField(Prefix.Underscore)
                .ExtraLazyLoad();

            mapping.Map(x => x.PollingStationType).CustomType<EnumType<PollingStationTypes>>().Not.Nullable();

            mapping.Component(x => x.GeoLocation).ColumnPrefix("geo");

            mapping.References(x => x.Region)
               .LazyLoad()
               .Not.Nullable()
               .Access.CamelCaseField(Prefix.Underscore);
            mapping.Map(x => x.OwingCircumscription).Formula("SRV.fn_GetCircumscription(regionId)")
               .Access.CamelCaseField(Prefix.Underscore);

            mapping.Map(x => x.FullNumber).Formula("CASE WHEN number is null THEN '[N/A]' ELSE cast(SRV.fn_GetCircumscription(regionId) as nvarchar) + '/' + cast(number as nvarchar) END")
               .Access.CamelCaseField(Prefix.Underscore);

            mapping.References(x => x.VotersListOrderType).Nullable();
        }
    }
}