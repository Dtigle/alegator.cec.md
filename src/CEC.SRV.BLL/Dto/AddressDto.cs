using System;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Dto
{
	public class AddressDto : BaseDto<long>, IEntity 
    {
		public string StreetName { get; set; }

		public int? HouseNumber { get; set; }

		public string Suffix { get; set; }

		public string PollingStation { get; set; }

		public virtual GeoLocation GeoLocation { get; set; }

		public int PeopleCount { get; set; }

		public string StreetTypeName { get; set; }

        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DeletedBy { get; set; }

		public virtual string GetFullName(bool setStreetNameFirst = true)
		{
			return setStreetNameFirst
				? string.Format("{0} {1}", StreetName, StreetTypeName)
				: string.Format("{0} {1}", StreetTypeName, StreetName);
		}

    }
}
