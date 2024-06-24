using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.Synchronizer
{
	public class SynchTransactionViewModel
	{
		public string RoleName { get; set; }

		public List<string> Transactions { get; set; }

		public bool Remove { get; set; }

	}
}