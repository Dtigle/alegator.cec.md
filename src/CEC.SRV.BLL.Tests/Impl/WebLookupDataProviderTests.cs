using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class WebLookupDataProviderTests : BaseTests<WebLookupCacheDataProvider, ISRVRepository>
    {
        private LookupBll _lookupBll;

        [TestInitialize]
        public void Startup2()
        {
            _lookupBll = new LookupBll(Repository);
            Bll = new WebLookupCacheDataProvider(_lookupBll);
        }

        [TestMethod]
        public void RegionCache_returns_correct_result()
        {
            // Act

            var result = Bll.RegionCache;

            // Assert

            Assert.AreSame(_lookupBll, result);
        }

        [TestMethod]
        public void PollingStationCache_returns_correct_result()
        {
            // Act

            var result = Bll.PollingStationCache;

            // Assert

            Assert.AreSame(_lookupBll, result);
        }

        [TestMethod]
        public void StreetTypeCache_returns_correct_result()
        {
            // Act

            var result = Bll.StreetTypeCache;

            // Assert

            Assert.AreSame(_lookupBll, result);
        }

        [TestMethod]
        public void StreetTypeCodeCache_returns_correct_result()
        {
            // Act

            var result = Bll.StreetTypeCodeCache;

            // Assert

            Assert.AreSame(_lookupBll, result);
        }

        [TestMethod]
        public void LinkedRegionCache_returns_correct_result()
        {
            // Act

            var result = Bll.LinkedRegionCache;

            // Assert

            Assert.AreSame(_lookupBll, result);
        }
    }
}
