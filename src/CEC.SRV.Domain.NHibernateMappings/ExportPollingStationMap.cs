using CEC.SRV.Domain.Print;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ExportPollingStationMap : IAutoMappingOverride<ExportPollingStation>
    {
        public void Override(AutoMapping<ExportPollingStation> mapping)
        {
            mapping.Map(x => x.Status).CustomType<EnumType<PrintStatus>>().Not.Nullable();

            mapping.References(x => x.ElectionRound).Nullable().LazyLoad();

            mapping.References(x => x.Circumscription).Nullable().LazyLoad();

            mapping.References(x => x.PrintSession).Access.CamelCaseField(Prefix.Underscore)
               .LazyLoad()
               .Not.Nullable();
            mapping.References(x => x.PollingStation).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
        }
    }
}