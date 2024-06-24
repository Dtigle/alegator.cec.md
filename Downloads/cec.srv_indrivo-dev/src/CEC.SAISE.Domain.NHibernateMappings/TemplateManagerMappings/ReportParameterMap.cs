using CEC.SAISE.Domain.TemplateManager;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings.TemplateManagerMappings
{
    public class ReportParameterMap : IAutoMappingOverride<ReportParameter>
    {
        public void Override(AutoMapping<ReportParameter> mapping)
        {
            mapping.Table("ReportParameters");
            mapping.Id(x => x.Id).Column("ReportParameterId");
            mapping.References(x => x.Template).Column("TemplateId");
            mapping.Map(x => x.ParameterName);
            mapping.Map(x => x.ParameterDescription);
            mapping.Map(x => x.IsLookup);
        }
    }
}
