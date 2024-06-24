using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CEC.SAISE.EDayModule.App_Start;
using Amdaris;

namespace CEC.SAISE.EDayModule
{
    public class MvcApplication : System.Web.HttpApplication
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
