using Amdaris.NHibernateProvider.Audit;
using NHibernate.Envers.Configuration.Fluent;

namespace CEC.SRV.Domain.NHibernateMappings.Audit
{
    public class ConfigurationSettingAuditMap : AuditMap<ConfigurationSetting>
    {
        protected override void Override(IFluentAudit<ConfigurationSetting> fluentAudit)
        {
            fluentAudit.ExcludeRelationData(x => x.CreatedBy);
            fluentAudit.ExcludeRelationData(x => x.ModifiedBy);
            fluentAudit.ExcludeRelationData(x => x.DeletedBy);
        }
    }
}