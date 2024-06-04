using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AuditEventTypesMap : IAutoMappingOverride<AuditEventTypes>
    {
        public void Override(AutoMapping<AuditEventTypes> mapping)
        {
            mapping.Table("AuditEventTypes");
            mapping.Id(x => x.Id).Column("auditEventTypeId");
            mapping.Map(x => x.code).Not.Nullable().Column("code");
            mapping.Map(x => x.auditStrategy).Not.Nullable().Column("auditStrategy");
            mapping.Map(x => x.name).Not.Nullable().Column("name");
            mapping.Map(x => x.description).Nullable().Column("description");            
        }

    }
}
