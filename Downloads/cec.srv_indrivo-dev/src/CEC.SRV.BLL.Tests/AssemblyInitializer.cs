using System.Collections.Generic;
using System.Reflection;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Audit;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Test;
using CEC.SRV.Domain.NHibernateMappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.BLL.Tests
{
    [TestClass]
    public class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var mappingAssemblies = new List<Assembly>{
                    typeof(UserRepository<>).Assembly,
                    typeof(SRVEntityMap<>).Assembly
                };

            var cfg = NHibernateConfig.Initialize<IdentityUser, ThreadLocalContextProvider>(
                new NullSecurityContext<IdentityUser>(), mappingAssemblies);
            
            cfg.IntegrateWithAudit(mappingAssemblies, Schemas.Audit);

            BaseRepositoryTests.InitializeNHibernateConfig(cfg);
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            BaseRepositoryTests.UnloadNHibernate();
        }
    }
}