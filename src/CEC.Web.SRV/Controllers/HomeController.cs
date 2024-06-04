using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;

namespace CEC.Web.SRV.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [KeepAliveOutputCache]
        public ActionResult KeepAlive()
        {
            return new ContentResult { Content = "OK", ContentType = "text/plain" };
        }
    }
}