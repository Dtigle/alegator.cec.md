using System.Collections.Generic;
using System.Reflection;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using NHibernate;
using SAISE.Domain.NHibernateMappings;

namespace CEC.QuartzServer.Core
{
    public static class SaiseDatabaseFactoryProvider
    {
        public static ISessionFactory GetFactory
        {
            get
            {
                var mappingAssemblies = new List<Assembly> { 
                    typeof(UserRepository<>).Assembly,
                    typeof(SaiseElectionMap).Assembly
                };
                var config = NHibernateConfig.Initialize<IdentityUser, ThreadLocalContextProvider>(new QuartzSecurityContext(), mappingAssemblies);
                config.SetProperty("connection.connection_string_name", "SaiseConnectionString");

                var sessionFactory = config.BuildSessionFactory();

                return sessionFactory;
            }
        }
    }
}