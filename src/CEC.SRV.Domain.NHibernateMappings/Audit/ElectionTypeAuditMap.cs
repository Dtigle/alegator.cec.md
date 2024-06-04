using Amdaris.NHibernateProvider.Audit;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class ElectionTypeAuditMap : AuditMap<ElectionType>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<ElectionType> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.CircumscriptionList);
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}