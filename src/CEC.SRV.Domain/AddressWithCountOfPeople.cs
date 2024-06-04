using System;
using System.Collections.Specialized;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
	public class AddressWithCountOfPeople : Entity
    {
		public virtual Address Address { get; set; }
		public virtual int PeopleCount { get; set; }
    }
}