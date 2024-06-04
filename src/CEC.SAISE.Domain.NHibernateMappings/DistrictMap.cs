using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    //public class DistrictMap : IAutoMappingOverride<District>
    //{
    //    public void Override(AutoMapping<District> mapping)
    //    {
    //        mapping.Table("District");
    //        mapping.Id(x => x.Id).Column("DistrictId");
    //        mapping.Map(x => x.RopUniqueId).Not.Nullable();
    //        mapping.Map(x => x.Number).Not.Nullable();
    //        mapping.Map(x => x.NameRo).Not.Nullable();
    //        mapping.Map(x => x.NameRu).Nullable();
    //        mapping.Map(x => x.OldName).Nullable();
    //        mapping.Map(x => x.DistrictCouncilSeats).Nullable();
    //        mapping.HasMany(x => x.Villages);
    //        mapping.HasMany(x => x.PollingStations);
    //    }
    //}
}
