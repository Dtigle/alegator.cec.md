using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using System.Web;
using Microsoft.AspNet.Identity;

namespace CEC.Web.SRV
{
    public class WebSecurityContext : IUserContext<IdentityUser>
    {
        public IdentityUser GetCurrentUser()
        {
            var userId = HttpContext.Current.GetOwinContext().Authentication.User.Identity.GetUserId();
            var repository = DependencyResolver.Current.Resolve<IRepository>();
            return repository.LoadProxy<IdentityUser, string>(userId);
        }
    }
}