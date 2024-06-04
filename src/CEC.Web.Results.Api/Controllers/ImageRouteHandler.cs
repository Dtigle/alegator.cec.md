using System.Web;
using System.Web.Routing;

namespace CEC.Web.Results.Api.Controllers
{
    public class ImageRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new PolitPartyLogoImageHandler();
        }
    }
}