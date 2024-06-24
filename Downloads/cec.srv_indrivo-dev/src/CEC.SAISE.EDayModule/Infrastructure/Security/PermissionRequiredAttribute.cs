using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using CEC.SAISE.BLL.Helpers;

namespace CEC.SAISE.EDayModule.Infrastructure.Security
{
    public class PermissionRequiredAttribute : AuthorizeAttribute
    {
        public PermissionRequiredAttribute(params object[] anyOf)
        {
            AnyOf = anyOf.Cast<string>().ToArray();
        }

        protected string[] AnyOf { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var user = HttpContext.Current.User as ClaimsPrincipal;
            if (user == null)
            {
                base.HandleUnauthorizedRequest(filterContext);
                return;
            }

            if (user.Identity.HasAnyPermission(AnyOf))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}