using Amdaris.NHibernateProvider.Audit;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class AddressAuditMap : AuditMap<Address>
    {
	    protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<Address> fluentAudit)
	    {
		    fluentAudit.ExcludeRelationData(x => x.CreatedBy);
		    fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
		    fluentAudit.ExcludeRelationData(x => x.DeletedBy);
			fluentAudit.ExcludeRelationData(x => x.Street);
			fluentAudit.ExcludeRelationData(x => x.PollingStation);
	    }
    }
}