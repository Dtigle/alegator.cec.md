using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
    public class AuditEventTypes : SaiseBaseEntity
    {
        public virtual string code { get; set; }
        public virtual int auditStrategy { get; set; }
        public virtual string name { get; set; }
        public virtual string description { get; set; }

    }
}
