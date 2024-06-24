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
    public class DocumentMap : IAutoMappingOverride<Document>
    {
        public void Override(AutoMapping<Document> mapping)
        {
            mapping.Table("Documents");
            mapping.Schema("dbo");
            mapping.Id(x => x.Id).Column("DocumentId");
            mapping.References(x => x.Template).Column("TemplateId");
            mapping.Map(x => x.DocumentName);
            mapping.Map(x => x.DocumentPath);
            mapping.Map(x => x.FileLength);
            mapping.Map(x => x.FileExtension);
        }
    }
}
