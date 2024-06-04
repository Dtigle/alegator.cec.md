using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class GeoLocationMap : ComponentMap<GeoLocation>
    {
        public GeoLocationMap()
        {
            Map(x => x.Latitude);
            Map(x => x.Longitude);
        }
    }
}