using System.Collections.Generic;
using System.Reflection;
using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Audit;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NLogProvider;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.NHibernateMappings;
using Microsoft.Practices.Unity;

namespace CEC.QuartzServer.Core
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
            var config = NHibernateConfig.Initialize<IdentityUser, ThreadLocalContextProvider>(new QuartzSecurityContext(), mappingAssemblies);
            config.IntegrateWithAudit(mappingAssemblies, Schemas.Audit);

            var sessionFactory = config.BuildSessionFactory();

            container.RegisterInstance(sessionFactory);

            container.RegisterType<ILogger, NLogLogger>(new TransientLifetimeManager());

            container.RegisterType<IRepository, Repository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISRVRepository, SrvRepository>(new ContainerControlledLifetimeManager());

            container.RegisterType<IBll, Bll>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILookupBll, LookupBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAddressBll, AddressBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPollingStationBll, PollingStationBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IVotersBll, VotersBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPrintBll, PrintBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImportBll, ImportBll>(new TransientLifetimeManager());
            container.RegisterType<IImportStatisticsBll, ImportStatisticsBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationSettingManager, ConfigurationSettingManager>(new ContainerControlledLifetimeManager());

            container.RegisterType<ILookupCacheDataProvider, QuartzLookupCacheDataProvider>(new TransientLifetimeManager());
            
            Amdaris.DependencyResolver.SetResolver(new Amdaris.UnityProvider.UnityDependencyResolver(container));
        }
    }
}
