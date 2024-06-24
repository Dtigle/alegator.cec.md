using CEC.SAISE.Domain.TemplateManager;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings.TemplateManagerMappings
{
    public class TemplateNameMap : IAutoMappingOverride<TemplateName>
    {
        public void Override(AutoMapping<TemplateName> mapping)
        {
            mapping.Table("TemplateNames");
            mapping.Id(x => x.Id).Column("TemplateNameId");
            mapping.Map(x => x.Title);
            mapping.Map(x => x.Description);
            mapping.HasMany(x => x.TemplateMappings)
            .KeyColumn("TemplateNameId");
            //.Cascade.AllDeleteOrphan(); // Specify cascade options if needed
        }
    }
}
