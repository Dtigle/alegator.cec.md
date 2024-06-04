using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.NHibernateProvider.Audit;
using NHibernate.Envers.Configuration.Fluent;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class StayStatementAuditMap : AuditMap<StayStatement>
    {
        protected override void Override(IFluentAudit<StayStatement> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}
