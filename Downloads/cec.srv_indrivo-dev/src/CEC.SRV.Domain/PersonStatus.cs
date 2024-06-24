using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
	public class PersonStatus : SRVBaseEntity
	{
	    public virtual Person Person { get; set; }

	    public virtual PersonStatusType StatusType { get; set; }

		public virtual string Confirmation { get; set; }

	    public virtual bool IsCurrent { get; set; }
	}
}
