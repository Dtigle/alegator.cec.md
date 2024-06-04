using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class LinkedRegionCacheTests : BaseTests<LinkedRegionCache, ISRVRepository>
    {
        private long _regionId;

        [TestInitialize]
        public void Startup2()
        {
            SetAdministratorRole();

            var linkedRegion = GetFirstObjectFromDbTable(x => (x.Regions != null) && x.Regions.Any(), GetLinkedRegion);
            var region = linkedRegion.Regions.FirstOrDefault();
            _regionId = (region != null) ? region.Id : -1;

            Bll = CreateBll<LinkedRegionCache>();
        }

        [TestMethod]
        public void GetAll_returns_correct_result()
        {
            // Arrange

            var expLinkedRegions = GetAllObjectsFromDbTable<LinkedRegion>();

            // Act

            var linkedRegions = SafeExecFunc(Bll.GetAll);
            
            // Assert

            AssertListsAreEqual(expLinkedRegions, linkedRegions.ToList());
        }

        [TestMethod]
        public void GetAll_returns_only_from_cache()
        {
            // Arrange

            GetFirstObjectFromDbTable(GetLinkedRegion, true);
            var expCount = GetDbTableCount<LinkedRegion>() - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetAll().Count(), expCount);
        }

        [TestMethod]
        public void GetByRegion_returns_correct_result()
        {
            // Arrange

            var expLinkedRegions = GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(r => r.Id == _regionId));

            // Act
            var linkedRegions = SafeExecFunc(Bll.GetByRegion, _regionId);

            // Assert

            AssertListsAreEqual(expLinkedRegions, linkedRegions.ToList());
        }

        [TestMethod]
        public void GetByRegion_returns_only_from_cache()
        {
            // Arrange

            var linkedRegion = GetFirstObjectFromDbTable(GetLinkedRegion, true);
            var region = linkedRegion.Regions.FirstOrDefault();
            var regionId = (region != null) ? region.Id : -1;

            var expCount = GetDbTableCount<LinkedRegion>(x => x.Regions.Any(r => r.Id == regionId)) - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetByRegion(regionId).Count(), expCount);
        }
    }
}
