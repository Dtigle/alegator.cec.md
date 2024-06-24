using System;
using System.Collections.Generic;
using System.Reflection;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Web;
using Microsoft.Practices.Unity;
using Amdaris;
using Amdaris.Domain;
using Amdaris.NLogProvider;
using Amdaris.UnityProvider;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Impl;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SRV.Domain.NHibernateMappings;
using Amdaris.NHibernateProvider.Audit;

namespace CEC.SAISE.EDayModule.App_Start
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

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();
            var mappingAssemblies = new List<Assembly> { 
                typeof(UserRepository<>).Assembly, 
                typeof(CustomUserManager).Assembly,
                typeof(CEC.SAISE.Domain.NHibernateMappings.AutoPersistenceModelProvider).Assembly
            };
            var config = NHibernateConfig.Initialize<SystemUser, WebContextProvider>(new WebSecurityContext(), mappingAssemblies);
            config.IntegrateWithAudit(mappingAssemblies, Schemas.Audit);
            var sessionFactory = config.BuildSessionFactory();

			container.RegisterInstance(sessionFactory);

            container.RegisterType<ILogger, NLogLogger>(new TransientLifetimeManager());

            container.RegisterType<IRepository, Repository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISaiseRepository, SaiseRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserBll, UserBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IVotingBll, VotingBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IElectionResultsBll, ElectionResultsBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IPoliticalPartyBll, PoliticalPartyBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IConcurentsBll, ConcurentsBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<ILogoImageBll, LogoImageBll>(new ContainerControlledLifetimeManager());
			container.RegisterType<IConfigurationBll, ConfigurationBll>(new ContainerControlledLifetimeManager());
	        container.RegisterType<IPollingStationStageBll, PollingStationStageBll>(new ContainerControlledLifetimeManager());
	        container.RegisterType<IVoterCertificatBll, VoterCertificatBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuditEvents, AuditEventsBLL>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITemplateNameBLL, TemplateNameBLL>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDocumentsBll, DocumentsBll>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMinIoBll, MinIoBll>(new ContainerControlledLifetimeManager());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
