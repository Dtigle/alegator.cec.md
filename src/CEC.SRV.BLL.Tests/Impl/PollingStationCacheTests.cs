using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class PollingStationCacheTests : BaseTests<PollingStationCache, ISRVRepository>
    {
        private long _regionId;

        [TestInitialize]
        public void Startup2()
        {
            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => x.Region != null, GetPollingStation);
            _regionId = pollingStation.Region.Id;

            Bll = CreateBll<PollingStationCache>();
        }

        [TestMethod]
        public void GetAll_returns_correct_result()
        {
            // Arrange

            var expPollingStations = GetAllObjectsFromDbTable<PollingStation>();
            
            // Act

            var pollingStations = SafeExecFunc(Bll.GetAll);
            
            // Assert

            AssertListsAreEqual(expPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetAll_returns_only_from_cache()
        {
            // Arrange

            GetFirstObjectFromDbTable(GetPollingStationWithoutStreets, true);
            var expCount = GetDbTableCount<PollingStation>() - 1;
            
            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetAll().Count(), expCount);
        }

        [TestMethod]
        public void GetByRegion_returns_correct_result()
        {
            // Arrange

            var expPollingStations = GetAllObjectsFromDbTable<PollingStation>(x => (x.Region != null) && (x.Region.Id == _regionId));

            // Act
            
            var pollingStations = SafeExecFunc(Bll.GetByRegion, _regionId);
            
            // Assert

            AssertListsAreEqual(expPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetByRegion_returns_only_from_cache()
        {
            // Arrange

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStationWithoutStreets, true);
            var regionId = pollingStation.Region.Id;

            var expCount = GetDbTableCount<PollingStation>(x => (x.Region != null) && (x.Region.Id == regionId)) - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetByRegion(regionId).Count(), expCount);
        }
    }
}
