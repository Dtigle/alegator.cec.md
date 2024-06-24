using Amdaris;
using Amdaris.NLogProvider;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace CEC.Web.Results.Api
{
    public class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
             
            container.RegisterType<ILogger, NLogLogger>(new TransientLifetimeManager());

            DependencyResolver.SetResolver(new Amdaris.UnityProvider.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}