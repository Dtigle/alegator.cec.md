using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Amdaris;
using CEC.Web.SRV.App_Start;

namespace CEC.Web.SRV
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Initialize();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpException = Server.GetLastError() as HttpException;
            if (httpException != null)
            {
                //Loging http exception here, 500 exceptions are logged in error handling attribute
                Amdaris.DependencyResolver.Current.Resolve<ILogger>().Error(httpException);
            }
        }
    }
}
