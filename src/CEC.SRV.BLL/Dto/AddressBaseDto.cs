using System;
using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SRV.BLL.Dto
{
	public class AddressBaseDto : BaseDto<long>, IEntity 
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; }

        public string StreetName { get; set; }
		public int? HouseNumber { get; set; }
		public string Suffix { get; set; }
		public string PollingStation { get; set; }
		public string StreetTypeName { get; set; }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public IdentityUser CreatedBy { get; set; }
        public IdentityUser ModifiedBy { get; set; }
        public IdentityUser DeletedBy { get; set; }

		public virtual string GetFullName(bool setStreetNameFirst = true)
		{
			return setStreetNameFirst
				? string.Format("{0} {1}", StreetName, StreetTypeName)
				: string.Format("{0} {1}", StreetTypeName, StreetName);
		}

    }
}
