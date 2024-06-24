using System;
using System.Collections.Specialized;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class StreetWithCountOfAddresses : Entity
    {
        public virtual Street Street { get; set; }
        public virtual int HousesCount { get; set; }
    }
}