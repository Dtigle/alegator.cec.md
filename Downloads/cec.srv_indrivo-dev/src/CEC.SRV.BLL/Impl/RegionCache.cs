using System;
using System.Collections.Generic;
using System.Linq;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Impl
{
    public class RegionCache : IRegionCache
    {
        private readonly List<Region> _regions;

        public RegionCache(ISRVRepository repository)
        {
            _regions = repository.Query<Region>().ToList();
        }

        public IEnumerable<Region> GetAll()
        {
            return _regions;
        }

        [Obsolete]
        public IEnumerable<Region> GetByRegistruId(long AdministrativeCode)
        {
            return _regions.Where(x => x.RegistruId == AdministrativeCode);
        }

        public Region GetByStatisticCode(long statisticCode)
        {
            return _regions.FirstOrDefault(x => x.StatisticIdentifier == statisticCode);
        }
    }
}