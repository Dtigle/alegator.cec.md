using Amdaris.NHibernateProvider.Audit;
using NHibernate.Envers.Configuration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings.Audit
{
    public  class BallotPapperAuditMap : AuditMap<BallotPaper>
    {
        protected override void Override(IFluentAudit<BallotPaper> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.PollingStation);
            fluentAudit.ExcludeRelationData(x => x.EditUser);
            fluentAudit.ExcludeRelationData(x => x.ElectionRound);
        }
    }
}
