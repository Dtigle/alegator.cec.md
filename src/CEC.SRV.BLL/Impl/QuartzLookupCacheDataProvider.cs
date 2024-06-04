using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Repositories;
using NHibernate;

namespace CEC.SRV.BLL.Impl
{
    public class QuartzLookupCacheDataProvider : ILookupCacheDataProvider
    {
        private readonly ISRVRepository _srvRepository;
        private  IRegionCache _regionCache;
        private  IPollingStationCache _pollingStationCache;
        private  IStreetTypeCache _streetTypeCache;
        private  IStreetTypeCodeCache _streetTypeCodeCache;
        private  ILinkedRegionCache _linkedRegionCache;
        
        public QuartzLookupCacheDataProvider(ISRVRepository srvRepository)
        {
            _srvRepository = srvRepository;
        }

        public IRegionCache RegionCache
        {
            get { return _regionCache ?? (_regionCache = new RegionCache(_srvRepository)); }
        }

        public IPollingStationCache PollingStationCache
        {
            get { return _pollingStationCache ?? (_pollingStationCache = new PollingStationCache(_srvRepository)); }
        }

        public IStreetTypeCache StreetTypeCache
        {
            get { return _streetTypeCache ?? (_streetTypeCache = new StreetTypeCache(_srvRepository)); }
        }

        public IStreetTypeCodeCache StreetTypeCodeCache
        {
            get { return _streetTypeCodeCache ?? (_streetTypeCodeCache = new StreetTypeCodeCache(_srvRepository)); }
        }

        public ILinkedRegionCache LinkedRegionCache
        {
            get { return _linkedRegionCache ?? (_linkedRegionCache = new LinkedRegionCache(_srvRepository)); }
        }
    }
}
