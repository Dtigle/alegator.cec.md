using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.TemplateManager
{
    public class Template : SaiseBaseEntity
    {
        private Template _parent;
        public virtual IList<ReportParameter> ReportParameters { get; set; }
        public Template()
        {
            ReportParameters = new List<ReportParameter>();
        }
        public virtual string Content { get; set; }
        public virtual DateTime UploadDate { get; set; }
        public virtual Template Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        // Navigation properties
        public virtual TemplateName TemplateName { get; set; }
        public virtual ElectionType ElectionType { get; set; }
    }
}
