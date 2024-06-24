using Amdaris.NHibernateProvider.Audit;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class PersonAddressAuditMap : AuditMap<PersonAddress>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<PersonAddress> fluentAudit)
        {
            fluentAudit.Exclude(x => x.PersonFullAddress);
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}