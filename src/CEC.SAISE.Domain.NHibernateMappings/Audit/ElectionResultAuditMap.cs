using Amdaris.NHibernateProvider.Audit;
using NHibernate.Envers.Configuration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings.Audit
{
    public class ElectionResultAuditMap : AuditMap<ElectionResult>
    {
        protected override void Override(IFluentAudit<ElectionResult> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.PoliticalParty);
            fluentAudit.ExcludeRelationData(x => x.EditUser);
            fluentAudit.ExcludeRelationData(x => x.Candidate);
            fluentAudit.ExcludeRelationData(x => x.BallotPaper);
            fluentAudit.ExcludeRelationData(x => x.ElectionRound);
        }
    }
}
