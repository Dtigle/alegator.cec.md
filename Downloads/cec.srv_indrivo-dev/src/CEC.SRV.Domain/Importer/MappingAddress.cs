using Amdaris.Domain;

namespace CEC.SRV.Domain.Importer
{
    public class MappingAddress : Entity
    {
        public virtual long SrvAddressId { get; set; }
        public virtual long RspAdministrativeCode { get; set; }
        public virtual long RspStreetCode { get; set; }
        public virtual int? RspHouseNr { get; set; }
        public virtual string RspHouseSuf { get; set; }
    }
}