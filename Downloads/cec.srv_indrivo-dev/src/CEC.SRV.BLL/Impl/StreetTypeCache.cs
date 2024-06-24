using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Impl
{
    public class StreetTypeCache :IStreetTypeCache
    {
        private readonly List<StreetType> _streetTypes;

        public StreetTypeCache(ISRVRepository repository)
        {
            _streetTypes = repository.Query<StreetType>().ToList();
        }

        public IEnumerable<StreetType> GetAll()
        {
            return _streetTypes;
        }
    }
}