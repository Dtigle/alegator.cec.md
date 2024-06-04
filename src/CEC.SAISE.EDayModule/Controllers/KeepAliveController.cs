using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.SAISE.EDayModule.Infrastructure;

namespace CEC.SAISE.EDayModule.Controllers
{
    public class KeepAliveController : BaseController
    {
        [KeepAliveOutputCache]
        public ActionResult Index()
        {
            return new ContentResult { Content = "OK", ContentType = "text/plain" };
        }
    }
}