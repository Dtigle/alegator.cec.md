using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;

namespace CEC.Web.SRV
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
        }
    }
}
