using System;
using System.Collections.Specialized;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
	public class RegionWithFullyQualifiedName : Entity
    {
		public virtual Region Region { get; set; }
	    public virtual long? SaiseId { get; set; }
	    public virtual string FullyQualifiedName { get; set; }
    }

    public class RegionWithoutSaiseId : Entity
    {
        public virtual Region Region { get; set; }
        public virtual string FullyQualifiedName { get; set; }
    }
}