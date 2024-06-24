using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.BLL.Helpers
{
    public static class SecurityHelper
    {
        public static IPrincipal GetLoggedUser()
        {
            return ClaimsPrincipal.Current;
        }

        public static long GetLoggedUserId()
        {
            var principal = ClaimsPrincipal.Current;
            if (principal == null) throw new ArgumentNullException("principal");

            return principal.Identity.GetUserId<long>();
        }

        public static bool LoggedUserIsInRole(string roleName)
        {
            var principal = ClaimsPrincipal.Current;
            if (principal == null) throw new ArgumentNullException("principal");

            return principal.IsInRole(roleName);
        }

        public static bool HasPermission(this IIdentity identity, string permissionName)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            ClaimsIdentity identity1 = identity as ClaimsIdentity;
            return identity1 != null && identity1.HasClaim("permission", permissionName);
        }

        public static bool HasAnyPermission(this IIdentity identity, params string[] permissions)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            ClaimsIdentity identity1 = identity as ClaimsIdentity;
            return identity1 != null && permissions != null &&
                   identity1.Claims
                            .Where(x => x.Type == "permission")
                            .Select(x => x.Value)
                            .Intersect(permissions)
                            .Any();
        }
    }
}
