using System.Collections.Generic;

namespace CEC.SAISE.Domain.TemplateManager
{
    public class TemplateName : SaiseBaseEntity
    {
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual TemplateType TemplateType { get; set; }
        public virtual IList<TemplateMapping> TemplateMappings { get; set; }
        public TemplateName()
        {
            TemplateMappings = new List<TemplateMapping>();
        }

    }
}
