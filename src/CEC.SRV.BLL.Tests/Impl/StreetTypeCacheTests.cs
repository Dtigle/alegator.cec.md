using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class StreetTypeCacheTests : BaseTests<StreetTypeCache, ISRVRepository>
    {
        [TestInitialize]
        public void Startup2()
        {
            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetStreetType);
            Bll = CreateBll<StreetTypeCache>();
        }

        [TestMethod]
        public void GetAll_returns_correct_result()
        {
            // Arrange

            var expStreetTypes = GetAllObjectsFromDbTable<StreetType>();

            // Act

            var streetTypes = SafeExecFunc(Bll.GetAll);
            
            // Assert

            AssertListsAreEqual(expStreetTypes, streetTypes.ToList());
        }

        [TestMethod]
        public void GetAll_returns_only_from_cache()
        {
            // Arrange

            GetFirstObjectFromDbTable(GetStreetType2, true);
            var expCount = GetDbTableCount<StreetType>() - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetAll().Count(), expCount);
        }
    }
}
