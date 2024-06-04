using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;

namespace CEC.SRV.BLL.Extensions
{
    public static class PageRequestExtensions
    {
        public static void FilterByConflict(this PageRequest pageRequest, bool filterValue)
        {
            if (pageRequest.FilterGroups.Count == 0)
            {
                pageRequest.FilterGroups.Add(new FilterGroup()
                {
                    Filters = new List<Filter>{new Filter{
                                Operator = ComparisonOperator.IsEqualTo,
                                Property = "IsInConflict",
                                Value = true
                            }}
                });
            }
            else
            {
                pageRequest.FilterGroups.First().Filters.Add(new Filter
                {
                    Operator = ComparisonOperator.IsEqualTo,
                    Property = "IsInConflict",
                    Value = true
                });
            }
        }
    }
}
