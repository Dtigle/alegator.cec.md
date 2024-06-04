using System.Web;
using System.Web.Mvc;
using CEC.SAISE.EDayModule.Infrastructure;

namespace CEC.SAISE.EDayModule
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
			filters.Add(new AuthorizeAttribute());
        }
    }
}
