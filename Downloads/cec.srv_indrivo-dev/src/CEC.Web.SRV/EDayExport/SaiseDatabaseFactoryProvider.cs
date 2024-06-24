using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using NHibernate;
using SAISE.Domain.NHibernateMappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CEC.Web.SRV.EDayExport
{
    public static class SaiseDatabaseFactoryProvider
    {
        public static ISessionFactory GetFactory(string connectionString)
        {
            var mappingAssemblies = new List<Assembly> {
                    typeof(UserRepository<>).Assembly,
                    typeof(SaiseElectionMap).Assembly
                };
            var config = NHibernateConfig.Initialize<IdentityUser, ThreadLocalContextProvider>(new WebSecurityContext(), mappingAssemblies);
            config.AddProperties(new Dictionary<string, string> { { "connection.connection_string", connectionString } });
            config.Properties.Remove("connection.connection_string_name");

            var sessionFactory = config.BuildSessionFactory();

            return sessionFactory;
        }
    }
}