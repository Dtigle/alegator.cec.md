using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using System.Collections.Generic;
using CEC.Web.SRV.Resources;
using CEC.SRV.BLL.Repositories;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class PollingStationBllTests : BaseBllTests
    {
        private PollingStationBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<PollingStationBll>();
        }

        [TestMethod]
        public void GetPollingStationByAccessibleRegion_returns_correct_page()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => x.Region != null, GetPollingStation);
            var regionId = pollingStation.Region.Id;

            var expectedPollingStations =
                GetAllIdsFromDbTable<PollingStation>(x => (x.Region != null) && (x.Region.Id == regionId));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetPollingStation, regionId, x => x.Id, expectedPollingStations);
        }

        [TestMethod]
        public void GetPollingStationByNonAccessibleRegion_returns_correct_page()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetRegistratorRole();
            
            // Act & Assert

            ActAndAssertAllPages(_bll.GetPollingStation, (long)1, x => x.Id, new List<long>());
        }

        [TestMethod]
        public void GetPollingStationsByRegions_returns_correct_page()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Circumscription != null, GetRegion);

            var circumscription = region.Circumscription;
            var regionIds = new long[] {region.Id};

            var expectedPollingStations =
                GetAllIdsFromDbTable<PollingStation>(x => x.OwingCircumscription == circumscription);

            // Act & Assert

            ActAndAssertAllPages(_bll.GetPollingStationsByRegions, regionIds, expectedPollingStations);
        }

        [TestMethod]
        public void GetPollingStationAddressesByAccessibleRegion_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Street.Region != null), GetAddress);
            var regionId = address.Street.Region.Id;
            
            var expectedAddresses = GetAllObjectsFromDbTable<Address>(
                    x => (x.Street != null) && (x.Street.Region != null) && (x.Street.Region.Id == regionId));
            
            // Act

            var addresses = SafeExecFunc(_bll.GetPollingStationAddresses, regionId);

            // Assert

            Assert.IsNotNull(addresses);
            AssertListsAreEqual(expectedAddresses, addresses.ToList());
        }

        [TestMethod]
        public void GetPollingStationAddressesByNonAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetRegistratorRole();

            // Act

            var addresses = SafeExecFunc(_bll.GetPollingStationAddresses, 1L);

            // Assert

            Assert.IsNotNull(addresses);
            AssertListsAreEqual(new List<Address>(), addresses.ToList());
        }

        [TestMethod]
        public void GetAccessiblePollingStationsByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();
            
            GetFirstObjectFromDbTable(x => x.Deleted == null, GetPollingStation);

            var expectedPollingStations =
                GetAllObjectsFromDbTable<PollingStation>(x => x.Deleted == null);
            
            // Act

            var pollingStations = SafeExecFunc(_bll.GetAccessiblePollingStations);

            // Assert

            Assert.IsNotNull(pollingStations);
            AssertListsAreEqual(expectedPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetAccessiblePollingStationsByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(x => x.Deleted == null, GetPollingStation);

            var expectedPollingStations =
                GetAllObjectsFromDbTableWithUdfPropertyIn<PollingStation>(x => x.Deleted == null, x => x.Region.Id, UdfRegionsCriterion());

            // Act

            var pollingStations = SafeExecFunc(_bll.GetAccessiblePollingStations);

            // Assert

            Assert.IsNotNull(pollingStations);
            AssertListsAreEqual(expectedPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetPollingStationsByRegion_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Circumscription != null, GetRegion);

            var circumscription = region.Circumscription;
            var regionIds = new long[] { region.Id };

            var expectedPollingStations =
                GetAllObjectsFromDbTable<PollingStation>(x => x.OwingCircumscription == circumscription);

            // Act

            var pollingStations = SafeExecFunc(_bll.GetPollingStationsByRegion, regionIds);

            // Assert

            Assert.IsNotNull(pollingStations);
            AssertListsAreEqual(expectedPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetNotNullCircumscription_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => (x.Id == 1) && (x.Parent == null), GetRegion, 1L);
            
            var region = GetFirstObjectFromDbTable(x => (x.Parent != null) && (x.Parent.Parent == null) && (x.Circumscription != null), GetRegionWithParent);

            var expectedCircumscription = region.Circumscription;
            var regionId = region.Id;

            // Act

            var circumscription = SafeExecFunc(_bll.GetCircumscription, regionId);

            // Assert

            Assert.AreEqual(expectedCircumscription, circumscription);
        }

        [TestMethod]
        public void GetNullCircumscription_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Circumscription == null, GetRegionWithoutCircumscription);
            var regionId = region.Id;

            // Act

            var circumscription = SafeExecFunc(_bll.GetCircumscription, regionId);

            // Assert

            Assert.IsNull(circumscription);
        }

        [TestMethod]
        public void CreateUpdatePollingStationWithZeroIdAndZeroAddressId_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);

            const long id = 0;
            var regionId = region.Id;
            const string number = "12";
            const string location = "location";
            const long addressId = 0;
            const string contactInfo = "contact info";

            // Act
            
            SafeExec(_bll.CreateUpdatePollingStation, id, regionId, number, location, addressId, contactInfo, (long?)null, PollingStationTypes.Normal);
            
            // Assert

            var newPollingStation = GetLastCreatedObject<PollingStation>();
            var newNotification = GetLastCreatedObject<Notification>();

            Assert.AreEqual(number, newPollingStation.Number);
            Assert.AreEqual(location, newPollingStation.Location);
            Assert.IsNull(newPollingStation.PollingStationAddress);
            Assert.AreEqual(contactInfo, newPollingStation.ContactInfo);
            Assert.IsNull(newPollingStation.SaiseId);

            Assert.AreEqual(EventTypes.New, newNotification.Event.EventType);
            Assert.AreEqual(newPollingStation.Id, newNotification.Event.EntityId);
            Assert.AreEqual(string.Format(MUI.Notification_PollingStation_Create, newPollingStation.Number, newPollingStation.Region.GetFullName()), newNotification.Message);

            var expectedUsers =
                GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                    new UsersWithAccessToRegionCriterion(regionId));

            AssertListsAreEqual(expectedUsers, newNotification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        [TestMethod]
        public void CreateUpdatePollingStationWithZeroIdAndNonZeroAddressId_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var address = GetFirstObjectFromDbTable(GetAddress);

            const long id = 0;
            var regionId = region.Id;
            const string number = "12";
            const string location = "location";
            var addressId = address.Id;
            const string contactInfo = "contact info";

            // Act

            SafeExec(_bll.CreateUpdatePollingStation, id, regionId, number, location, addressId, contactInfo, (long?)null, PollingStationTypes.Normal);
            
            // Assert

            var newPollingStation = GetLastCreatedObject<PollingStation>();
            var newNotification = GetLastCreatedObject<Notification>();

            Assert.AreEqual(number, newPollingStation.Number);
            Assert.AreEqual(location, newPollingStation.Location);

            Assert.IsNotNull(newPollingStation.PollingStationAddress);
            Assert.AreSame(address, newPollingStation.PollingStationAddress);

            Assert.AreEqual(contactInfo, newPollingStation.ContactInfo);
            Assert.IsNull(newPollingStation.SaiseId);

            Assert.AreEqual(EventTypes.New, newNotification.Event.EventType);
            Assert.AreEqual(newPollingStation.Id, newNotification.Event.EntityId);
            Assert.AreEqual(string.Format(MUI.Notification_PollingStation_Create, newPollingStation.Number, newPollingStation.Region.GetFullName()), newNotification.Message);

            var expectedUsers =
                GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                    new UsersWithAccessToRegionCriterion(regionId));

            AssertListsAreEqual(expectedUsers, newNotification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        [TestMethod]
        public void CreateUpdatePollingStationWithNonZeroIdAndZeroAddressId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var id = pollingStation.Id;
            var regionId = region.Id;
            const string number = "12";
            const string location = "location";
            const long addressId = 0;
            const string contactInfo = "contact info";

            // Act

            SafeExec(_bll.CreateUpdatePollingStation, id, regionId, number, location, addressId, contactInfo, (long?)null, PollingStationTypes.Normal);

            // Assert

            var newPollingStation = GetFirstObjectFromDbTable<PollingStation>(x => x.Id == id);

            Assert.AreEqual(id, newPollingStation.Id);
            Assert.AreEqual(number, newPollingStation.Number);
            Assert.AreEqual(location, newPollingStation.Location);
            Assert.IsNull(newPollingStation.PollingStationAddress);
            Assert.AreEqual(contactInfo, newPollingStation.ContactInfo);
            Assert.IsNull(newPollingStation.SaiseId);
        }

        [TestMethod]
        public void CreateUpdatePollingStationWithNonZeroIdAndNonZeroAddressId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var address = GetFirstObjectFromDbTable(GetAddress);
            
            var id = pollingStation.Id;
            var regionId = region.Id;
            const string number = "12";
            const string location = "location";
            var addressId = address.Id;
            const string contactInfo = "contact info";

            // Act

            SafeExec(_bll.CreateUpdatePollingStation, id, regionId, number, location, addressId, contactInfo, (long?)null, PollingStationTypes.Normal);

            // Assert

            var newPollingStation = GetLastCreatedObject<PollingStation>();

            Assert.AreEqual(number, newPollingStation.Number);
            Assert.AreEqual(location, newPollingStation.Location);

            Assert.IsNotNull(newPollingStation.PollingStationAddress);
            Assert.AreSame(address, newPollingStation.PollingStationAddress);

            Assert.AreEqual(contactInfo, newPollingStation.ContactInfo);
            Assert.IsNull(newPollingStation.SaiseId);
        }

        [TestMethod]
        public void DeletePollingStationAssociatedToSomeAddresses_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.PollingStation != null) && (x.Deleted == null), GetAddress);
            var pollingStationId = address.PollingStation.Id;

            // Act
            
            SafeExec(_bll.DeletePollingStation, pollingStationId, true, false, "PollingStation_NotPermissionDeletePollingStation", MUI.PollingStation_NotPermissionDeletePollingStation);
        }

        [TestMethod]
        public void DeleteExistingPollingStationAssociatedToAnyAddress_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PollingStationBll>();

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStationWithoutStreets, true);
            var pollingStationId = pollingStation.Id;
            var regionId = pollingStation.Region.Id;

            // Act
            
            SafeExec(_bll.DeletePollingStation, pollingStationId);
            
            // Assert

            var deletedPollingStation = GetLastDeletedObject<PollingStation>();
            var newNotification = GetLastCreatedObject<Notification>();

            Assert.IsNotNull(deletedPollingStation);
            Assert.AreEqual(pollingStationId, deletedPollingStation.Id);

            var expectedUsers =
                GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                    new UsersWithAccessToRegionCriterion(regionId));

            AssertListsAreEqual(expectedUsers, newNotification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        [TestMethod]
        public void DeleteNotExistingPollingStation_throw_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var expectedCount = GetDbTableCount<PollingStation>();

            // Act

            SafeExec(_bll.DeletePollingStation, 0L, false, true);

            // Assert

            var count = GetDbTableCount<PollingStation>();
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void VerificationIfPollingStationHasReferenceByExistingExportPollingStationWithGivenPollingStation_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var exportPollingStation = GetFirstObjectFromDbTable(x => (x.PollingStation != null) && (x.Deleted == null), GetExportPollingStation);
            var pollingStationId = exportPollingStation.PollingStation.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfPollingStationHasReference, pollingStationId, true, false, string.Empty,
                string.Format(MUI.HasReference_Error, exportPollingStation.PollingStation.GetObjectType(), exportPollingStation.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfPollingStationHasReferenceByExistingAddressWithGivenPollingStation_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.PollingStation != null) && (x.Deleted == null), GetAddress);
            var pollingStationId = address.PollingStation.Id;

            var exportPollingStations = GetAllObjectsFromDbTable<ExportPollingStation>(x => (x.PollingStation.Id == pollingStationId) && (x.Deleted == null));
            exportPollingStations.ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfPollingStationHasReference, pollingStationId, true, false, string.Empty,
                string.Format(MUI.HasReference_Error, address.PollingStation.GetObjectType(), address.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfPollingStationHasReferenceByNotExistingReference_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfPollingStationHasReference, -1L);
        }
    }
}
