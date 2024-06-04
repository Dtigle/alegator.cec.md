using System;

namespace CEC.SRV.Domain.ViewItem
{
    public class VoterViewItem : SRVBaseEntity
    {

        public virtual long RegionId { get; set; }
        public virtual long PollingStationId { get; set; }


        public virtual string Idnp { get; set; }
        public virtual string Surname { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }


        public virtual string Address { get; set; }

        public virtual int HouseNumber { get; set; }

        public virtual int ApNumber { get; set; }

        public virtual string ApSuffix { get; set; }

        public virtual string DocumentNumber { get; set; }

        public virtual long StatusId { get; set; }

        public virtual string Status { get; set; }

        public virtual long AddressTypeId { get; set; }

        public virtual string AddressType { get; set; }

        public virtual string Gender { get; set; }

        public virtual int Age { get; set; }

        public virtual bool RegionHasStreets { get; set; }

        public virtual DateTime? AddressExpirationDate { get; set; }

        public virtual long AddressId { get; set; }

        public virtual long StreeetId { get; set; }

        public virtual long? electionListNr { get; set; }

    }
}