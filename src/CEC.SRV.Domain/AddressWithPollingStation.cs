using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class AddressWithPollingStation : Entity
    {
        public virtual Address Address { get; set; }
        public virtual string FullAddress { get; set; }
        public virtual long RegionId { get; set; }
        public virtual long StreetId { get; set; }
    }
}