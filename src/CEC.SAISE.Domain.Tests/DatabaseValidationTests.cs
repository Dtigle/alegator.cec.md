using System;
using System.Text;
using Amdaris.NHibernateProvider.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Tool.hbm2ddl;

namespace CEC.SAISE.Domain.Tests
{
	[TestClass]
	public class DatabaseValidationTests : BaseRepositoryTests
	{
		[TestMethod]
		public void GenerateUpdateSchema()
		{
			var updater = new SchemaUpdate(NHConfig);
			updater.Execute(Console.WriteLine, false);
		}

		[TestMethod]
		public void ValidateDatabaseSchema()
		{
			var sb = new StringBuilder();

			var updater = new SchemaUpdate(NHConfig);
			updater.Execute(s => sb.AppendLine(s), false);

			Assert.IsTrue(sb.Length == 0, sb.ToString());
		}

		protected override void LoadData()
		{
			
		}
	}
}
