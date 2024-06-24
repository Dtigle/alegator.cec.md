using System.Net;
using System.Web.Mvc;
using Amdaris;

namespace CEC.Web.SRV.Infrastructure
{
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Amdaris.DependencyResolver.Current.Resolve<ILogger>().Error(filterContext.Exception);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { Message = filterContext.Exception.GetLocalizedMessage() }
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}