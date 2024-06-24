using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;

namespace CEC.Web.Api
{
    public class WebApiSecurityContext : IUserContext<IdentityUser>
    {
        public IdentityUser GetCurrentUser()
        {

            return null;
        }
    }
}