using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.Synchronizer
{
	public class SynchRoleViewModel
	{
		public long? Id { get; set; }

		public long ApplicationId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}