using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
    public class AuditEvents: SaiseBaseEntity
    {
        public virtual AuditEventTypes AuditEventsTypes { get; set; }

        public virtual int level { get; set; }

        public virtual DateTime generatedAt { get; set; }

        public virtual string message { get; set; }

        public virtual string userId  { get; set; }

        public virtual string userMachineIp { get; set; }


    }
}
