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
    public class TemplateMappingMap : IAutoMappingOverride<TemplateMapping>
    {
        public void Override(AutoMapping<TemplateMapping> mapping)
        {
            mapping.Table("TemplateMapping");
            mapping.Schema("dbo");
            mapping.Id(x => x.Id).Column("TemplateMappingId");
            mapping.References(x => x.TemplateName)
                    .Column("TemplateNameId");

        }
    }
}
