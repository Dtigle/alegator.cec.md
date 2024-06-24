using Amdaris.NHibernateProvider.Audit;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class PersonAuditMap : AuditMap<Person>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<Person> fluentAudit)
        {
	        fluentAudit.Exclude(x => x.Age);
            fluentAudit.Exclude(x => x.FullName);
            fluentAudit.Exclude(x => x.ExportedToSaise);
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}