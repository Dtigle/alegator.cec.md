using CEC.SRV.Domain.Importer.ToSaise;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class SaiseExporterStageMap : IAutoMappingOverride<SaiseExporterStage>
    {
        public void Override(AutoMapping<SaiseExporterStage> mapping)
        {
            mapping.Schema(Schemas.Importer);
            mapping.Map(x => x.Description).Length(4001);
            mapping.Map(x => x.ErrorMessage).Length(4001);
            mapping.References(x => x.SaiseExporter)
               .LazyLoad()
               .Not.Nullable();
        }
    }
}