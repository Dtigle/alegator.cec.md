using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain.Paging;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.SAISE.EDayModule.Infrastructure.Grids
{
    public static class PageRequestExtensions
    {
        public static PageResponse<T> GetDataPerPage<T>(IEnumerable<T> data, JqGridRequest request) where T : class
        {
            var skipCount = request.PageIndex * request.RecordsCount;
            var pageData = data.Skip(skipCount).Take(request.RecordsCount).ToList();
            return new PageResponse<T>
            {
                Items = pageData,
                PageSize = request.RecordsCount,
                StartIndex = request.PageIndex + 1,
                Total = data.Count()
            };
        }
    }
}