using System;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Dto
{
	public class PollingStationDto : BaseDto<long>, IEntity 
    {
		public int OwingCircumscription { get; set; }

		public string Number { get; set; }

	    public PollingStationTypes PollingStationType { get; set; }
        public string VotersListOrderTypes { get; set; }


        public string Location { get; set; }

		public string FullAddress { get; set; }

		public string ContactInfo { get; set; }

		public int TotalAddress { get; set; }

		public long? SaiseId { get; set; }

        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DeletedBy { get; set; }

        public string OrderType { get; set; }
    }
}
