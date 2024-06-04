using Amdaris.NHibernateProvider.Audit;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class RegionAuditMap : AuditMap<Region>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<Region> fluentAudit)
        {
            fluentAudit.Exclude(x => x.StatisticCode);
            fluentAudit.Exclude(x => x.StatisticIdentifier);
            fluentAudit.Exclude(x => x.GeoLocation);
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}