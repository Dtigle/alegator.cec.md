using System;
using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SRV.BLL.Dto
{
    public class StreetDto : BaseDto<long>, IEntity 
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string StreetType { get; set; }

        public long StreetTypeId { get; set; }

        public int HousesCount { get; set; }

        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public string CreatedBy { get; set; }
		public string ModifiedBy { get; set; }
        public string DeletedBy { get; set; }

    }
}
