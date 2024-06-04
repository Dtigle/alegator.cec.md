using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;

namespace CEC.QuartzServer.Core
{
    public class QuartzSecurityContext : IUserContext<IdentityUser>
    {
        public IdentityUser GetCurrentUser()
        {
            var repository = DependencyResolver.Current.Resolve<IRepository>();
            return repository.LoadProxy<IdentityUser, string>("1");
        }
    }

    
}
