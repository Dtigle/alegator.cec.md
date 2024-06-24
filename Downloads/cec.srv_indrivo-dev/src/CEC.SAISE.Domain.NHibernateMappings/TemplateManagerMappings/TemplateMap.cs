using CEC.SAISE.Domain.TemplateManager;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings.TemplateManagerMappings
{
    public class TemplateMap : IAutoMappingOverride<Template>
    {
        public void Override(AutoMapping<Template> mapping)
        {
            mapping.Table("Templates");
            mapping.Schema("dbo");
            mapping.Id(x => x.Id).Column("TemplateId");
            mapping.References(x => x.TemplateName).Column("TemplateNameId");
            mapping.Map(x => x.Content);
            mapping.Map(x => x.UploadDate);
            mapping.References(x => x.Parent) // Map the Parent property
                .Column("ParentId")   // The column that represents the parent-child relationship
                .Cascade.None();
        }
    }
}
