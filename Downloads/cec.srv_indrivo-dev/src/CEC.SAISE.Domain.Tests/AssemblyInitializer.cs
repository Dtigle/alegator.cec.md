using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using Amdaris.NHibernateProvider.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SAISE.Domain.Tests
{
	[TestClass]
	public class AssemblyInitializer
	{
		[AssemblyInitialize]
		public static void Initialize(TestContext context)
		{
			var mappingAssemblies = new List<Assembly> { 
                typeof(UserRepository<>).Assembly, 
                typeof(CEC.SAISE.Domain.NHibernateMappings.AutoPersistenceModelProvider).Assembly
            };

			var cfg = NHibernateConfig.Initialize<SystemUser, ThreadLocalContextProvider>(
				new NullSecurityContext<SystemUser>(), mappingAssemblies);
			
			BaseRepositoryTests.InitializeNHibernateConfig(cfg);

			ClaimsPrincipal.Current.AddIdentity(new ClaimsIdentity(new[]
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