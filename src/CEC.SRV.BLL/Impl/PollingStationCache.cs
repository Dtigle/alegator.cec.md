using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Impl
{
    public class PollingStationCache : IPollingStationCache
    {
        private readonly List<PollingStation> _pollingStations;

        public PollingStationCache(ISRVRepository repository)
        {
            _pollingStations = repository.Query<PollingStation>().ToList();
        }

        public IEnumerable<PollingStation> GetAll()
        {
            return _pollingStations;
        }

        public IEnumerable<PollingStation> GetByRegion(long regionId)
        {
            return _pollingStations.Where(x => x.Region.Id == regionId);
        }
    }
}