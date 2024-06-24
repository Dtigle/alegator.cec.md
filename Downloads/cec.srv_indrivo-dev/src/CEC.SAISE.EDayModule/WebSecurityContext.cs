using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using System.Web;
using CEC.SAISE.Domain;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.EDayModule
{
    public class WebSecurityContext : IUserContext<SystemUser>
    {
        public SystemUser GetCurrentUser()
        {
            var userId = HttpContext.Current.GetOwinContext().Authentication.User.Identity.GetUserId<long>();
            var repository = DependencyResolver.Current.Resolve<IRepository>();
            return repository.LoadProxy<SystemUser, long>(userId);
        }
    }
}