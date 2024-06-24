using Amdaris.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.TemplateManager
{
    public class TemplateMapping : EntityWithTypedId<long>
    {
        public virtual int ElectionTypeCode { get; set; }
        public virtual int ElectionRoundCode { get; set; }
        public virtual bool IsCECE { get; set; }
        public virtual bool OneDay { get; set; }
        public virtual bool TwoDayFirstDay { get; set; }
        public virtual bool TwoDaySecondDay { get; set; }
        public virtual TemplateName TemplateName { get; set; }
    }
}
