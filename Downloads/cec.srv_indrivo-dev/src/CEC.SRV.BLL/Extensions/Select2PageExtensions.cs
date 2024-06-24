using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;

namespace CEC.SRV.BLL.Extensions
{
	public static class Select2PageExtensions
	{
        public static PageRequest ToPageRequest(this Select2Request request)
        {
            return new PageRequest
            {
                PageSize = request.pageLimit,
                PageNumber = request.page,
                FilterGroups = new List<FilterGroup>()
            };
        }
        public static PageRequest ToPageRequest(this Select2Request request, 
			string searchPropName, ComparisonOperator searchOperator)
		{
			return new PageRequest
			{
				PageSize = request.pageLimit,
				PageNumber = request.page,
				FilterGroups = string.IsNullOrWhiteSpace(request.q)
					? new List<FilterGroup>()
					: new List<FilterGroup>
						{
							new FilterGroup
							{
								Filters = new List<Filter>
								{
									new Filter
									{
										Operator = searchOperator,
										Property = searchPropName,
										Value = GetFilterWithDiacritics(request.q)
									}
								}
							}
						}
			};
		}

		private static string GetFilterWithDiacritics(string filter)
		{
			return filter.Aggregate(new StringBuilder(), (sbCurrent, chNext) =>
				("Ss".Contains(chNext) ? sbCurrent.Append("[sș]") : (
				 "Tt".Contains(chNext) ? sbCurrent.Append("[tț]") : (
				 "Aa".Contains(chNext) ? sbCurrent.Append("[aăâ]") : (
				 "Ii".Contains(chNext) ? sbCurrent.Append("[iî]") : sbCurrent.Append(chNext)))))).ToString();
		}
	}
}
