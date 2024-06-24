using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Audit;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Test;
using CEC.SRV.Domain.NHibernateMappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.Database.Tests
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

            ClaimsPrincipal.Current.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, "0"), 
                new Claim(ClaimTypes.Name, "test@user.com")
            }, "TestAuthentication"));

            
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            BaseRepositoryTests.UnloadNHibernate();
        }
    }
}