using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class AdditionalUserInfoMap : IAutoMappingOverride<AdditionalUserInfo>
    {
        public void Override(AutoMapping<AdditionalUserInfo> mapping)
        {
            mapping.Schema(Schemas.RSA);
            mapping.Table("AdditionalUserInfos");
            mapping.Map(x => x.FirstName);
            mapping.Map(x => x.LastName);
            mapping.Map(x => x.DateOfBirth);
            mapping.Map(x => x.Email);
            mapping.Map(x => x.LandlinePhone);
            mapping.Map(x => x.MobilePhone, "mobPhone");
            mapping.Map(x => x.WorkInfo);
            mapping.Map(x => x.FullName).Formula("FirstName + ' ' + LastName").Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
