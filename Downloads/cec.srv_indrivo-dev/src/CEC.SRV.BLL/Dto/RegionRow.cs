using System;
using System.Collections.Generic;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Dto
{
	public class RegionRow : BaseDto<long>, IEntity
    {

		public virtual string Name { get; set; }

		public virtual string FullyQualifiedName { get; set; }

		public virtual string Description { get; set; }

		public virtual long? ParentId { get; set; }

		public virtual RegionType RegionType{ get; set; }

		public virtual bool HasStreets { get; set; }
        
		public virtual long? SaiseId { get; set; }

		public virtual long? RegistruId { get; set; }

        public virtual long? Cuatm { get; set; }

		public virtual bool HasChildren { get; set; }

        public virtual bool HasLinkedRegions { get; set; }

		public virtual DateTimeOffset? Created { get; set; }
		public virtual DateTimeOffset? Modified { get; set; }
		public virtual DateTimeOffset? Deleted { get; set; }
		public virtual string CreatedBy { get; set; }
		public virtual string ModifiedBy { get; set; }
		public virtual string DeletedBy { get; set; }
    }
}