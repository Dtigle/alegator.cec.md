using System;
using System.Linq;
using System.Security.Claims;

namespace CEC.SRV.BLL
{
    public static class SecurityHelper
    {
        public static string GetLoggedUserId()
        {
            var principal = ClaimsPrincipal.Current;
            if (principal == null) throw new ArgumentNullException("principal");
            
            return principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }

        public static bool LoggedUserIsInRole(string roleName)
        {
            var principal = ClaimsPrincipal.Current;
            if (principal == null) throw new ArgumentNullException("principal");

            return principal.IsInRole(roleName);
        }
    }
}