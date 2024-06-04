using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class StreetTypeCodeCacheTests : BaseTests<StreetTypeCodeCache, ISRVRepository>
    {
        private StreetTypeCode _streetTypeCode;
       
        [TestInitialize]
        public void Startup2()
        {
            SetAdministratorRole();

            _streetTypeCode = GetFirstObjectFromDbTable(GetStreetTypeCode);

            Bll = CreateBll<StreetTypeCodeCache>();
        }

        [TestMethod]
        public void GetAll_returns_correct_result()
        {
            // Arrange

            var expCodes = GetAllObjectsFromDbTable<StreetTypeCode>();

            // Act
            
            var codes = SafeExecFunc(Bll.GetAll);
            
            // Assert

            AssertListsAreEqual(expCodes, codes.ToList());
        }

        [TestMethod]
        public void GetAll_returns_only_from_cache()
        {
            // Arrange

            GetFirstObjectFromDbTable(GetStreetTypeCode2, true);
            var expCount = GetDbTableCount<StreetTypeCode>() - 1;

            // Act & Assert

            ActAndAssertLongValue(() => Bll.GetAll().Count(), expCount);
        }

        [TestMethod]
        public void GetByStreetTypeCode_returns_correct_result()
        {   
            // Act
            
            var code = SafeExecFunc(Bll.GetByStreetTypeCode, _streetTypeCode.Id);
        
            // Assert

            Assert.AreSame(_streetTypeCode, code);
        }

        [TestMethod]
        public void GetByStreetTypeCode_returns_only_from_cache()
        {
            // Arrange

            var newCode = GetFirstObjectFromDbTable(GetStreetTypeCode2, true);
            var id = newCode.Id;
            
            // Act
            
            var code = SafeExecFunc(Bll.GetByStreetTypeCode, id);

            // Assert

            Assert.IsNull(code);
            Assert.IsNotNull(newCode);
        }
    }
}
