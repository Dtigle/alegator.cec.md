using CEC.SRV.Domain.Importer.ToSaise;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class SaiseExporterMap : IAutoMappingOverride<SaiseExporter>
    {
        public void Override(AutoMapping<SaiseExporter> mapping)
        {
            mapping.Schema(Schemas.Importer);
            //mapping.References(x => x.Election).Not.Nullable().Access.CamelCaseField(Prefix.Underscore).Not.LazyLoad();
            mapping.Map(x => x.ElectionDayId).Not.Nullable();
            mapping.Map(x => x.ExportAllVoters).Not.Nullable().Default("0");
            mapping.Map(x => x.ErrorMessage).Length(4000).Nullable();
            mapping.HasMany(x => x.Stages).Cascade.AllDeleteOrphan().Not.LazyLoad();
        }
    }
}