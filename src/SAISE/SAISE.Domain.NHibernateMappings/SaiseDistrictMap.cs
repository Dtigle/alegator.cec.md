using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    //public class SaiseDistrictMap : IAutoMappingOverride<SaiseDistrict>
    //{
    //    public void Override(AutoMapping<SaiseDistrict> mapping)
    //    {
    //        mapping.ReadOnly();
    //        mapping.Table("District");
    //        mapping.Id(x => x.Id).Column("DistrictId");
    //        mapping.Map(x => x.Number).Not.Nullable().Column("Number");
    //        mapping.Map(x => x.NameRo).Column("NameRo");
    //    }
    //}
}
