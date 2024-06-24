using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using NHibernate.Envers;
using Amdaris.NHibernateProvider;
using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Criteria;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class AuditerBllTests : BaseTests<AuditerBll, IRepository>
    {
        private readonly AuditQueryCreator _auditQueryCreator;
        private readonly AuditId _auditId;

        public AuditerBllTests()
        {
            _auditQueryCreator = SessionFactory.OpenSession().Auditer().CreateQuery();
            _auditId = AuditEntity.Id();
        }

        [TestInitialize]
        public new void Startup()
        {
            IsMockedRepository = false;
            var repository = new AuditerRepository(SessionFactory);
            Bll = new AuditerBll(repository);
        }
        
        #region Get

        [TestMethod]
        public void GetAddress_returns_correct_result()
        {
            GetTest<Address>();
        }

        [TestMethod]
        public void GetStreet_returns_correct_result()
        {
            GetTest<Street>();
        }

        [TestMethod]
        public void GetRegion_returns_correct_result()
        {
            GetTest<Region>();
        }

        [TestMethod]
        public void GetRegionType_returns_correct_result()
        {
            GetTest<RegionType>();
        }

        [TestMethod]
        public void GetStreetType_returns_correct_result()
        {
            GetTest<StreetType>();
        }

        [TestMethod]
        public void GetPollingStation_returns_correct_result()
        {
            GetTest<PollingStation>();
        }

        [TestMethod]
        public void GetPersonAddress_returns_correct_result()
        {
            GetTest<PersonAddress>();
        }

        [TestMethod]
        public void GetPerson_returns_correct_result()
        {
            GetTest<Person>();
        }

        [TestMethod]
        public void GetPersonAddressType_returns_correct_result()
        {
            GetTest<PersonAddressType>();
        }

        [TestMethod]
        public void GetPublicAdministration_returns_correct_result()
        {
            GetTest<PublicAdministration>();
        }

        [TestMethod]
        public void GetManagerType_returns_correct_result()
        {
            GetTest<ManagerType>();
        }

        [TestMethod]
        public void GetGender_returns_correct_result()
        {
            GetTest<Gender>();
        }

        [TestMethod]
        public void GetDocumentType_returns_correct_result()
        {
            GetTest<DocumentType>();
        }

        #endregion Get

        #region Common Tests

        public void GetTest<T>() where T : AuditedEntity<IdentityUser>
        {
            // Arrange

            const long entityId = 1;
            var expectedList = _auditQueryCreator.ForRevisionsOf<T>(true).Add(_auditId.Eq(entityId)).Results().Select(x => x.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(Bll.Get<T>, entityId, expectedList);
        }

        #endregion Common Tests
    }
}
