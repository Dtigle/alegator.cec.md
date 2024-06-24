using CEC.Web.Results.Api.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace CEC.Web.Results.Api
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add("Images", new Route("images/{*file}", new ImageRouteHandler()));

            routes.MapRoute(
                name: "Api",
                url: "api/{controller}/{id}",
                defaults: new { controller = "LiveResults", id = UrlParameter.Optional }
            );
             
        }
    }
}
