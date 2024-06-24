using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;

namespace CEC.SRV.Domain
{
    public class RspAddressMapping : Entity
    {
        public virtual long SrvRegionId { get; set; }

        public virtual RegionWithFullyQualifiedName SrvRegion { get; set; }

        public virtual long SrvAddressId { get; set; }

        public virtual string SrvFullAddress { get; set; }

        public virtual RegionWithFullyQualifiedName RspRegion { get; set; }

        public virtual long RspRegionCode { get; set; }

        public virtual long? RspStreetCode { get; set; }

        public virtual string RspStreetName { get; set; }

        public virtual int? RspHouseNumber { get; set; }

        public virtual string RspHouseSuffix { get; set; }
    }
}
