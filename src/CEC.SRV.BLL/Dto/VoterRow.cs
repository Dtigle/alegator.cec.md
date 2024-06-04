using System;
using Amdaris.Domain;

namespace CEC.SRV.BLL.Dto
{
    public class VoterRow : BaseDto<long>, IEntity
    {
        public string Idnp { get; set; }

        public string Surname { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public bool RegionHasStreets { get; set; }

        public int? ApartmentNumber { get; set; }

        public string ApartmentSuffix { get; set; }

        public string DocumentNumber { get; set; }

        public string Status { get; set; }

        public long? StatusId { get; set; }

        public string AddressType { get; set; }

        public long AddressTypeId { get; set; }
        public long? electionListNr { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public DateTime? AddressExpirationDate { get; set; }

        public DateTimeOffset? Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }
}