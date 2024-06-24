using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class StayStatementMap : IAutoMappingOverride<StayStatement>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<StayStatement> mapping)
        {
            mapping.References(x => x.Person).Not.Nullable().Access.CamelCaseField(Prefix.Underscore);
            mapping.References(x => x.BaseAddress).Not.Nullable().Access.CamelCaseField(Prefix.Underscore);
            mapping.References(x => x.DeclaredStayAddress).Not.Nullable().Access.CamelCaseField(Prefix.Underscore).Cascade.All();
            mapping.References(x => x.ElectionInstance).Not.Nullable().Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
