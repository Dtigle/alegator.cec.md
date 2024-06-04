using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain.Identity;

namespace CEC.SRV.Domain
{
	public class RoleTransaction : SRVBaseEntity
	{
		public virtual IdentityRole Role { get; set; }

		public virtual Transaction Transaction { get; set; }
	}
}
