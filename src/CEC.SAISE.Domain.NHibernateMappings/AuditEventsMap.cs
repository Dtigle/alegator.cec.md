using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AuditEventsMap : IAutoMappingOverride<AuditEvents>
    {
        public void Override(AutoMapping<AuditEvents> mapping)
        {
            mapping.Table("AuditEvents");
            mapping.Id(x => x.Id).Column("auditEventId");
            mapping.References(x => x.AuditEventsTypes).Not.Nullable().Column("auditEventTypeId");
            mapping.Map(x => x.level).Not.Nullable().Column("level");
            mapping.Map(x => x.generatedAt).Not.Nullable().Column("generatedAt");
            mapping.Map(x => x.message).Nullable().Column("message").Length(1000); ;
            mapping.Map(x => x.userId).Nullable().Column("userId");
            mapping.Map(x => x.userMachineIp).Nullable().Column("userMachineIp");
            
        }

    }
}
