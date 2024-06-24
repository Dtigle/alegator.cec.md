using System;
using System.Collections.Generic;
using Amdaris.Domain;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface ICache<T> where T : Entity
    {
        IEnumerable<T> GetAll();
    }

    public interface IStreetTypeCache : ICache<StreetType>
    {
    }

    public interface IRegionCache : ICache<Region>
    {
        [Obsolete]
        IEnumerable<Region> GetByRegistruId(long getAdministrativeCode);
        Region GetByStatisticCode(long statisticCode);
    }

    public interface IPollingStationCache : ICache<PollingStation>
    {
        IEnumerable<PollingStation> GetByRegion(long regionId);
    }

    public interface IStreetTypeCodeCache : ICache<StreetTypeCode>
    {
        StreetTypeCode GetByStreetTypeCode(long id);
    }

    public interface ILinkedRegionCache : ICache<LinkedRegion>
    {
        IEnumerable<LinkedRegion> GetByRegion(long id);
    }
}