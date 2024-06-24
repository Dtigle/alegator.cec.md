using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.NHibernateProvider.Audit;
using NHibernate.Envers.Configuration.Fluent;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class ElectionAuditMap : AuditMap<Election>
    {
        protected override void Override(IFluentAudit<Election> fluentAudit)
        {
            fluentAudit.Exclude(x => x.ElectionRounds);
            fluentAudit.ExcludeRelationData(x => x.ElectionRounds);
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}
