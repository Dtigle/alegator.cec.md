using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Test;
using CEC.QuartzServer.Core;
using CEC.SRV.Domain.NHibernateMappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class ConfigurationSettingManagerTests
    {
        private readonly ISessionFactory sessionFactory;

        public ConfigurationSettingManagerTests()
        {
            var mappingAssemblies = new List<Assembly> { 
                typeof(UserRepository<>).Assembly,
                typeof(SRVEntityMap<>).Assembly
            };
            var config = NHibernateConfig.Initialize<IdentityUser, ThreadLocalContextProvider>(new NullSecurityContext<IdentityUser>(), mappingAssemblies);

            sessionFactory = config.BuildSessionFactory();

            ClaimsPrincipal.Current.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"), 
                new Claim(ClaimTypes.Name, "System")
            }, "QuatzAuthentication"));

        }

        [TestMethod]
        public void GetSettingValue_Read()
        {
            //arrange
            var configurationSettingManager = new ConfigurationSettingManager(sessionFactory);

            //act
            var value = configurationSettingManager.Get("RspUser");

            //asert
            Assert.IsNotNull(value);
        }
    }
}