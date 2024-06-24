using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class RegionMap : IAutoMappingOverride<Region>
    {
        public void Override(AutoMapping<Region> mapping)
        {
            mapping.Table("Region");
            mapping.Id(x => x.Id).Column("RegionId");
            mapping.References(x => x.RegionType).Not.Nullable();
            mapping.References(x => x.Parent).Not.Nullable();
            mapping.Map(x => x.Name).Not.Nullable();
            mapping.Map(x => x.NameRu).Nullable();
            mapping.Map(x => x.Description).Nullable();
            mapping.Map(x => x.RegistryId).Nullable();
            mapping.Map(x => x.StatisticCode).Nullable();
            mapping.Map(x => x.StatisticIdentifier).Nullable();
            mapping.Map(x => x.HasStreets).Not.Nullable();
            mapping.Map(x => x.GeoLatitude).Nullable();
            mapping.Map(x => x.GeoLongitude).Nullable();

        }
    }
}
