using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL
{
    public interface ILookupCacheDataProvider
    {
        IRegionCache RegionCache { get; }
        IPollingStationCache PollingStationCache { get; }
        IStreetTypeCache StreetTypeCache { get; }
        IStreetTypeCodeCache StreetTypeCodeCache { get; }
        ILinkedRegionCache LinkedRegionCache { get; }
    }
}
