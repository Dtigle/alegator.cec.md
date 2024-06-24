using Amdaris.Domain;

namespace CEC.SRV.Domain
{
	public class AddressWithoutPollingStation : Entity
    {
        public virtual long ParentRegionId { get; set; }
	    public virtual string ParentRegionName { get; set; }
	    public virtual long RegionId { get; set; }
	    public virtual string Region { get; set; }
        
	    public virtual long RegionTypeId { get; set; }
	    public virtual string RegionType { get; set; }

        public virtual string Street { get; set; }
        public virtual int HouseNumber { get; set; }
        public virtual string Suffix{ get; set; }

        //public virtual Address Address { get; set; }

		public virtual int Persons { get; set; }
    }
}