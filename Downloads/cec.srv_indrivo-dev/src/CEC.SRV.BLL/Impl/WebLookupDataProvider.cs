using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Impl
{
    public class WebLookupCacheDataProvider : ILookupCacheDataProvider
    {
        private readonly ILookupBll _lookupBll;

        public WebLookupCacheDataProvider(ILookupBll lookupBll)
        {
            _lookupBll = lookupBll;
        }

        public IRegionCache RegionCache
        {
            get { return (IRegionCache) _lookupBll; }
        }

        public IPollingStationCache PollingStationCache
        {
            get { return (IPollingStationCache)_lookupBll; }
        }

        public IStreetTypeCache StreetTypeCache
        {
            get { return (IStreetTypeCache)_lookupBll; }
        }

        public IStreetTypeCodeCache StreetTypeCodeCache
        {
            get { return (IStreetTypeCodeCache)_lookupBll; }
        }

        public ILinkedRegionCache LinkedRegionCache
        {
            get { return (ILinkedRegionCache)_lookupBll; }
        }
    }
}
