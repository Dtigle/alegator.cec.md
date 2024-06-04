using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class AbroadVoterRegistrationMap : IAutoMappingOverride<AbroadVoterRegistration>
    {
        public void Override(AutoMapping<AbroadVoterRegistration> mapping)
        {
            mapping.Schema("SRV");
            mapping.References(x => x.Person).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            mapping.Map(x => x.AbroadAddress).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            mapping.Map(x => x.ResidenceAddress).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            mapping.Map(x => x.Email).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            mapping.Map(x => x.IpAddress).Access.CamelCaseField(Prefix.Underscore);
            mapping.Map(x => x.Created).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            mapping.Map(x => x.CreationDate).Formula("convert(date, created)").Access.CamelCaseField(Prefix.Underscore);
        }

    }
}
