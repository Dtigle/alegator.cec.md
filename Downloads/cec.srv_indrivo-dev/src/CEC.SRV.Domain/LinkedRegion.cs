using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class LinkedRegion : SRVBaseEntity
    {
        private readonly IList<Region> _regions;

        protected LinkedRegion()
        {
            _regions = new List<Region>();
        }

        public LinkedRegion(IEnumerable<Region> regions)
        {
            _regions = new List<Region>(regions);
        }

        public virtual IReadOnlyCollection<Region> Regions
        {
            get { return new ReadOnlyCollection<Region>(_regions); }
        }
    }
}
