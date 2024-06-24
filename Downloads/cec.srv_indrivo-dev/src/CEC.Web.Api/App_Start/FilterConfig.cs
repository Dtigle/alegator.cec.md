using CEC.Web.Api.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace CEC.Web.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
        }
    }
}
