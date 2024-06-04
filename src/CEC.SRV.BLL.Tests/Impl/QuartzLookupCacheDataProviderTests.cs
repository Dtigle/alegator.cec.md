using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class QuartzLookupCacheDataProviderTests : BaseTests<QuartzLookupCacheDataProvider, ISRVRepository>
    {
        [TestMethod]
        public void RegionCache_is_not_null()
        {   
            // Act

            var result = Bll.RegionCache;
            
            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RegionCache));
        }

        [TestMethod]
        public void PollingStationCache_is_not_null()
        {
            // Act

            var result = Bll.PollingStationCache;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PollingStationCache));
        }

        [TestMethod]
        public void StreetTypeCache_is_not_null()
        {
            // Act

            var result = Bll.StreetTypeCache;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(StreetTypeCache));
        }

        [TestMethod]
        public void StreetTypeCodeCache_is_not_null()
        {
            // Act

            var result = Bll.StreetTypeCodeCache;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(StreetTypeCodeCache));
        }

        [TestMethod]
        public void LinkedRegionCache_is_not_null()
        {
            // Act

            var result = Bll.LinkedRegionCache;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(LinkedRegionCache));
        }
    }
}
