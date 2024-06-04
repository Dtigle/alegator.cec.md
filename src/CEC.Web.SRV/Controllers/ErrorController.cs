using System.Web;
using System.Web.Mvc;

namespace CEC.Web.SRV.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult NotFound(string aspxerrorpath)
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            var exception = new HttpException(404, "NotFoundHttpException") {Source = aspxerrorpath};
            var model = new HandleErrorInfo(exception, "Error", "NotFound");

            return View("Error", model);
        }
    }
}