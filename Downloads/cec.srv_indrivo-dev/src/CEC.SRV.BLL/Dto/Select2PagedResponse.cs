using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Dto
{
	public class Select2PagedResponse
	{
		public Select2PagedResponse(List<Select2Item> items, int total, int pageSize)
		{
			Items = items;
			Total = total;
			PageSize = pageSize;
		}

	    public Select2PagedResponse(object toList, int count, int pageSize)
	    {
	        throw new NotImplementedException();
	    }

	    public List<Select2Item> Items { get; set; }

		public int Total { get; set; }

		public int PageSize { get; set; }
	}
}
