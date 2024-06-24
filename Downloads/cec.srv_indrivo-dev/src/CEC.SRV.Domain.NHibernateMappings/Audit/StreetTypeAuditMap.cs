using Amdaris.NHibernateProvider.Audit;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class DStreetTypeAuditMap : AuditMap<StreetType>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<StreetType> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}