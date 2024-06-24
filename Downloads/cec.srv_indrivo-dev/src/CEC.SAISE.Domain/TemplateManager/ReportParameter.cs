using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.TemplateManager
{
    public class ReportParameter : SaiseBaseEntity
    {
        //public virtual int ReportparameterId { get; set; }
        public virtual string ParameterName { get; set; }
        public virtual string ParameterDescription { get; set; }
        public virtual string ParameterCode { get; set; }
        public virtual bool IsLookup { get; set; }
        public virtual Template Template { get; set; }
    }
}
