using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.Domain
{
	public class Transaction : SRVBaseEntity
	{
		public virtual string Code { get; set; }

		public virtual string Name { get; set; }

		public virtual string Description { get; set; }
	}
}
