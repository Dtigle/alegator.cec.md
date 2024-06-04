using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class RegionCacheTests : BaseTests<RegionCache, ISRVRepository>
    {
        private long _registruId;

        [TestInitialize]
        public void Startup2()
        {
            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.RegistruId != null, GetRegionWithRegistruId);
            _registruId = region.RegistruId.HasValue ? region.RegistruId.Value : -1;

            Bll = CreateBll<RegionCache>();
        }

        [TestMethod]
        public void GetAll_returns_correct_result()
        {
            // Arrange

            var expRegions = GetAllObjectsFromDbTable<Region>();

            // Act

            var regions = SafeExecFunc(Bll.GetAll);
            
            // Assert

            AssertListsAreEqual(expRegions, regions.ToList());
        }

        [TestMethod]
        public void GetAll_returns_only_from_cache()
        {
            // Arrange

            GetFirstObjectFromDbTable(GetRegionWithoutStreets, true);
            var expCount = GetDbTableCount<Region>() - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetAll().Count(), expCount);
        }

        [TestMethod]
        public void GetByRegistruId_returns_correct_result()
        {
            // Arrange

            var expRegions = GetAllObjectsFromDbTable<Region>(x => x.RegistruId == _registruId);

            // Act
            var regions = SafeExecFunc(Bll.GetByRegistruId, _registruId);

            // Assert

            AssertListsAreEqual(expRegions, regions.ToList());
        }

        [TestMethod]
        public void GetByRegistruId_returns_only_from_cache()
        {
            // Arrange

            var region = GetFirstObjectFromDbTable(GetRegionWithRegistruId2, true);
            var registruId = region.RegistruId.HasValue ? region.RegistruId.Value : -1;

            var expCount = GetDbTableCount<Region>(x => x.RegistruId == registruId) - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetByRegistruId(registruId).Count(), expCount);
        }
    }
}
