using Amdaris.NHibernateProvider.Audit;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class PollingStationAuditMap : AuditMap<PollingStation>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<PollingStation> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
			fluentAudit.Exclude(x => x.OwingCircumscription);
			fluentAudit.Exclude(x => x.FullNumber);
        }
    }
}