using Amdaris;
using System.Net;
using System.Web.Mvc;

namespace CEC.Web.Results.Api.Infrastructure
{
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {

        public override void OnException(ExceptionContext filterContext)
        {
            Amdaris.DependencyResolver.Current.Resolve<ILogger>().Error(filterContext.Exception);

            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.Result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    Message = filterContext.Exception.Message
                }
            };
            filterContext.ExceptionHandled = true;

            base.OnException(filterContext);
        }

    }
}
