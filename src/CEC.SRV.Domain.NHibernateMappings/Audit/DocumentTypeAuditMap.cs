using Amdaris.NHibernateProvider.Audit;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class DocumentTypeAuditMap : AuditMap<DocumentType>
    {
        protected override void Override(NHibernate.Envers.Configuration.Fluent.IFluentAudit<DocumentType> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}