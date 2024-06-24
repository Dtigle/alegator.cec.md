using CEC.SRV.Domain.Importer;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RawAddressComponentMap : ComponentMap<RawAddress>
    {
        public RawAddressComponentMap()
        {
            Map(x => x.ExternalRegionId);
            Map(x => x.RegionName);
            Map(x => x.ExternalStreetId);
            Map(x => x.StreetName);
            Map(x => x.HomeNr);
            Map(x => x.HomeSuffix);
        }
    }
}