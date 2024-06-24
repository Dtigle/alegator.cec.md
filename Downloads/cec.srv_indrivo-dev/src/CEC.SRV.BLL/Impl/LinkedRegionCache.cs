using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Impl
{
    public class LinkedRegionCache : ILinkedRegionCache
    {
        private readonly List<LinkedRegion> _linkedRegions;

        public LinkedRegionCache(ISRVRepository repository)
        {
            _linkedRegions = repository.Query<LinkedRegion>().ToList();
        }

        public IEnumerable<LinkedRegion> GetAll()
        {
            return _linkedRegions;
        }

        public IEnumerable<LinkedRegion> GetByRegion(long id)
        {
            return _linkedRegions.Where(x => x.Regions.Count(r => r.Id == id) > 0);
        }
    }
}