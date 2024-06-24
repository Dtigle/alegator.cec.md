using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Dto
{
	public class Select2Request
	{
		public string q { get; set; }

		public int pageLimit { get; set; }

		public int page { get; set; }
	}
}
