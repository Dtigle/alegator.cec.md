using Amdaris;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CEC.Web.Api.App_Start;

namespace CEC.Web.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Initialize();
        }

        protected void Application_Error(object sender, EventArgs args)
        {
            var httpException = Server.GetLastError() as HttpException;
            if (httpException != null)
            {
                Amdaris.DependencyResolver.Current.Resolve<ILogger>().Error(httpException);
            }
        }
    }
}
