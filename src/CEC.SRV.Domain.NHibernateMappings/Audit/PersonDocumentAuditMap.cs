using Amdaris.NHibernateProvider.Audit;
using NHibernate.Envers.Configuration.Fluent;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class PersonDocumentAuditMap : AuditMap<PersonDocument>
    {
        protected override void Override(IFluentAudit<PersonDocument> fluentAudit)
        {
            fluentAudit.Exclude(x => x.DocumentNumber);
        }
    }
}