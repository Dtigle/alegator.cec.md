using System;
using System.Collections.Generic;
using System.Reflection;
using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Audit;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Web;
using Amdaris.NLogProvider;
using Amdaris.UnityProvider;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.NHibernateMappings;
using Microsoft.Practices.Unity;

namespace CEC.Web.SRV.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            var mappingAssemblies = new List<Assembly> { 
                typeof(UserRepository<>).Assembly, 
                typeof(SRVEntityMap<>).Assembly
            };
            var config = NHibernateConfig.Initialize<IdentityUser, WebContextProvider>(new WebSecurityContext(), mappingAssemblies);
            config.IntegrateWithAudit(mappingAssemblies, Schemas.Audit);

            var sessionFactory = config.BuildSessionFactory();
            
            container.RegisterInstance(sessionFactory);

            container.RegisterType<ILogger, NLogLogger>(new TransientLifetimeManager());

            container.RegisterType<IRepository, Repository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISRVRepository, SrvRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuditerRepository, AuditerRepository>(new ContainerControlledLifetimeManager());


            container.RegisterType<IBll, Bll>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILookupBll, LookupBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAddressBll, AddressBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPollingStationBll, PollingStationBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IVotersBll, VotersBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuditerBll, AuditerBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IElectionBll, ElectionBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<INotificationBll, NotificationBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IStatisticsBll, StatisticsBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IConflictBll, ConflictBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IPrintBll, PrintBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IExporterBll, ExporterBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImportBll, ImportBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImportStatisticsBll, ImportStatisticsBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IJobsScheduler, JobsScheduler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationSettingBll, ConfigurationSettingBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IInteropBll, InteropBll>(new ContainerControlledLifetimeManager());

            container.RegisterType<ILookupCacheDataProvider, WebLookupCacheDataProvider>();

            container.RegisterType(typeof(IService<>), typeof(Service<>), new ContainerControlledLifetimeManager());
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
