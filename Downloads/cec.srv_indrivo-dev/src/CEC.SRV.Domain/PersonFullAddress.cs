using System;
using System.Collections.Generic;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class PersonFullAddress : Entity
    {
        public virtual Region Region { get; set; }

        public virtual bool RegionHasStreets { get; set; }

        public virtual PollingStation PollingStation { get; set; }

        public virtual Person Person { get; set; }

        public virtual PersonAddressType PersonAddressType { get; set;}

        public virtual string FullAddress { get; set; }

        public virtual int? ApNumber { get; set; }

        public virtual string ApSuffix { get; set; }

        public virtual bool IsEligible { get; set; }

        public virtual IdentityUserRegionView AssignedUser { get; set; }

        public virtual DateTime? DateOfExpiration { get; set; }
    }
}