using System.Web;
using System.Web.Mvc;
using CEC.Web.Results.Api.Infrastructure;

namespace CEC.Web.Results.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
        }
    }
}
