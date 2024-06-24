using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using NHibernate.Type;

namespace CEC.SRV.BLL.Impl
{
    public class StreetTypeCodeCache : IStreetTypeCodeCache
    {
        private readonly List<StreetTypeCode> _streetTypeCodes;

        public StreetTypeCodeCache(ISRVRepository repository)
        {
            _streetTypeCodes = repository.Query<StreetTypeCode>().ToList();
        }
        public IEnumerable<StreetTypeCode> GetAll()
        {
            return _streetTypeCodes;
        }

        public StreetTypeCode GetByStreetTypeCode(long id)
        {
            return _streetTypeCodes.SingleOrDefault(x => x.Id == id);
        }
    }
}