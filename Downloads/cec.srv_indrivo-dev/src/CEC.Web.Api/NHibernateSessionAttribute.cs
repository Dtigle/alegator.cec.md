using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Amdaris.NHibernateProvider;

namespace CEC.Web.Api
{
    public class NHibernateSessionAttribute : ActionFilterAttribute
    {
        private const string NhUnitOfWorkKey = "NhUnitOfWork";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.Properties[NhUnitOfWorkKey] = new NhUnitOfWork();

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            var untiOfWork = (NhUnitOfWork)actionExecutedContext.ActionContext.Request.Properties[NhUnitOfWorkKey];

            if (actionExecutedContext.Exception != null)
            {
                untiOfWork.Dispose();
                return;
            }

            untiOfWork.Complete();
        }
    }
}