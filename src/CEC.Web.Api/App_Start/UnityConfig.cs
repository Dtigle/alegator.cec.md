using System.Collections.Generic;
using System.Reflection;
using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Web;
using Amdaris.NLogProvider;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.NHibernateMappings;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using System.Configuration;

namespace CEC.Web.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            var mappingAssemblies = new List<Assembly> { 
                typeof(UserRepository<>).Assembly,
                typeof(SRVEntityMap<>).Assembly
            };
            var config = NHibernateConfig.Initialize<IdentityUser, WebContextProvider>(new WebApiSecurityContext(), mappingAssemblies);

            var sessionFactory = config.BuildSessionFactory();

            container.RegisterInstance(sessionFactory);

            container.RegisterType<ILogger, NLogLogger>(new TransientLifetimeManager());

            container.RegisterType<IRepository, Repository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISRVRepository, WebApiSrvRepository>(new ContainerControlledLifetimeManager());
            

            container.RegisterType<IBll, Bll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IElectionBll, ElectionBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILookupBll, LookupBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAddressBll, AddressBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPollingStationBll, PollingStationBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IVotersBll, VotersBll>(new ContainerControlledLifetimeManager());

            container.RegisterType<IImportBll, ImportBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImportStatisticsBll, ImportStatisticsBll>(new ContainerControlledLifetimeManager());
            
            container.RegisterType<IEmailClientHelper, EmailClientHelper>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(ConfigurationManager.AppSettings["SMPT.username"], ConfigurationManager.AppSettings["SMPT.password"], ConfigurationManager.AppSettings["SMPT.host"], int.Parse(ConfigurationManager.AppSettings["SMPT.port"])));

            container.RegisterType(typeof(IService<>), typeof(Service<>), new ContainerControlledLifetimeManager());
            Amdaris.DependencyResolver.SetResolver(new Amdaris.UnityProvider.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}