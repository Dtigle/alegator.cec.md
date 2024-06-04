using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Importer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using System.Collections.Generic;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using NHibernate.Linq;
using NHibernate.Criterion;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class ConflictBllTests : BaseBllTests
    {
        private ConflictBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<ConflictBll>();
        }

        [TestMethod]
        public void GetConflictList_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetRegistratorRole();
            
            // Act And Assert

            Enum.GetValues(typeof(ConflictStatusCode)).Cast<ConflictStatusCode>().ForEach(GetConflictListTest);
        }

        private void GetConflictListTest(ConflictStatusCode code)
        {
            // Arrange

            const ConflictStatusCode none = ConflictStatusCode.None;

            GetFirstObjectFromDbTable(x =>
                ((x.RspModificationData.StatusConflictCode & code) != none) &&
                ((x.RspModificationData.AcceptConflictCode & code) == none) &&
                ((x.RspModificationData.RejectConflictCode & code) == none), GetRspRegistrationData);

            var expConflicts = GetAllObjectsFromDbTable<RspConflictData>(x =>
                ((x.StatusConflictCode & code) != none) &&
                ((x.AcceptConflictCode & code) == none) &&
                ((x.RejectConflictCode & code) == none));
            expConflicts.ForEach(x => AddRegionToCurrentUser(x.SrvRegion));
            
            // Act

            ActAndAssertAllPages(_bll.GetConflictList, code, expConflicts.Select(x => x.Id).ToList());
        }

        [TestMethod]
        public void GetConflictListForLinkedRegions_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetRegistratorRole();

            // Act And Assert

            Enum.GetValues(typeof(ConflictStatusCode)).Cast<ConflictStatusCode>().ForEach(GetConflictListForLinkedRegionsTest);
        }

        private void GetConflictListForLinkedRegionsTest(ConflictStatusCode code)
        {
            // Arrange

            const ConflictStatusCode none = ConflictStatusCode.None;

            GetFirstObjectFromDbTable(x =>
                ((x.RspModificationData.StatusConflictCode & code) != none) &&
                ((x.RspModificationData.AcceptConflictCode & code) == none) &&
                ((x.RspModificationData.RejectConflictCode & code) == none), GetRspRegistrationData);

            var regionIds = new List<long>();
            var linkedRegions = GetAllObjectsFromDbTable<LinkedRegion>(
                x => (x.Deleted == null) && x.Regions.Any(y => RegionIsAccessibleForCurrentUser(y.Id)))
                .Select(x => x.Regions);
            linkedRegions.ForEach(x => regionIds.AddRange(x.Select(y => y.Id)));

            var expConflicts = GetAllObjectsFromDbTable<RspConflictData>(x =>
                ((x.StatusConflictCode & code) != none) &&
                ((x.AcceptConflictCode & code) == none) &&
                ((x.RejectConflictCode & code) == none) &&
                regionIds.Contains(x.SrvRegion.Id));
            expConflicts.ForEach(x => AddRegionToCurrentUser(x.SrvRegion));

            // Act

            ActAndAssertAllPages(_bll.GetConflictListForLinkedRegions, code, expConflicts.Select(x => x.Id).ToList());
        }

        [TestMethod]
        public void GetConflictListForAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetAdministratorRole();

            // Act And Assert

            Enum.GetValues(typeof(ConflictStatusCode)).Cast<ConflictStatusCode>().ForEach(GetConflictListForAdminTest);
        }

        private void GetConflictListForAdminTest(ConflictStatusCode code)
        {
            // Arrange

            GetFirstObjectFromDbTable(x =>
                ((x.RspModificationData.StatusConflictCode & code) > 0) &&
                ((x.RspModificationData.AcceptConflictCode & code) == 0), GetRspRegistrationData);
            
            var expConflicts = GetAllIdsFromDbTable<RspConflictDataAdmin>(x =>
                ((x.StatusConflictCode & code) > 0) &&
                ((x.AcceptConflictCode & code) == 0));

            // Act

            ActAndAssertAllPages(_bll.GetConflictListForAdmin, code, expConflicts);
        }

        [TestMethod]
        public void GetAddressesByNullRegionIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetAddress);

            var addresses = GetAllIdsFromDbTable<Address>();
            
            // Act and Assert

            ActAndAssertAllPages(_bll.GetAddresses, (long?)null, x => x.Id, addresses);
        }

        [TestMethod]
        public void GetAddressesByNullRegionIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetRegistratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Street.Region != null), GetAddress);
            AddRegionToCurrentUser(address.Street.Region);

            var addresses = new List<long>();
            GetCurrentUser().Regions.Select(y => y.Id).ForEach(x => addresses.AddRange(GetAllIdsFromDbTable<Address>(y => y.Street.Region.Id == x)));

            // Act and Assert

            ActAndAssertAllPages(_bll.GetAddresses, (long?)null, x => x.Id, addresses);
        }

        [TestMethod]
        public void GetAddressesByNotNullRegionIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Street.Region != null), GetAddress);
            long? regionId = address.Street.Region.Id;
            var addresses = GetAllIdsFromDbTable<Address>(x => (x.Street != null) && (x.Street.Region != null) && (x.Street.Region.Id == regionId));

            // Act and Assert

            ActAndAssertAllPages(_bll.GetAddresses, regionId, x => x.Id, addresses);
        }

        [TestMethod]
        public void GetAddressesByNotNullRegionIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetRegistratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Street.Region != null), GetAddress);
            AddRegionToCurrentUser(address.Street.Region);

            long? regionId = address.Street.Region.Id;

            var addresses = RegionIsAccessibleForCurrentUser(regionId.Value) ? 
                GetAllIdsFromDbTable<Address>(x => (x.Street != null) && (x.Street.Region != null) && (x.Street.Region.Id == regionId)) :
                new List<long>();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetAddresses, regionId, x => x.Id, addresses);
        }

        [TestMethod]
        public void GetVoter_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => 
                (x.Person != null) && (x.Person.Deleted == null) && (x.IsEligible) &&
                (x.Person.Gender != null) && (x.Person.Document != null) &&
                (x.Person.Document.Type != null) && (x.Person.PersonStatuses != null) &&
                (x.Person.PersonStatuses.Any(y => y.IsCurrent)), GetPersonAddress);

            var idnp = personAddress.Person.Idnp;

            var address = GetFirstObjectFromDbTable<PersonFullAddress>(x =>
                (x.Person != null) && (x.Person.Deleted == null) && (x.IsEligible) &&
                (x.Person.Gender != null) && (x.Person.Document != null) &&
                (x.Person.Document.Type != null) && (x.Person.PersonStatuses != null) &&
                (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null)); 
            
            // Act

            var dto = SafeExecFunc(_bll.GetVoter, idnp);

            // Assert

            Assert.IsNotNull(dto);
            
            Assert.AreEqual(address.Person.Id, dto.Id);
            Assert.AreEqual(address.Person.Idnp, dto.Idnp);
            Assert.AreEqual(address.Person.FirstName, dto.FirstName);
            Assert.AreEqual(address.Person.Surname, dto.Surname);
            Assert.AreEqual(address.Person.MiddleName, dto.MiddleName);
            Assert.AreEqual(address.Person.DateOfBirth, dto.DateOfBirth);
            Assert.AreEqual(address.Person.Gender.Name, dto.Gender);
            Assert.AreEqual(address.Person.Document.Seria, dto.DocSeria);
            Assert.AreEqual(address.Person.Document.Number, dto.DocNumber);
            Assert.AreEqual(address.Person.Document.IssuedBy, dto.DocIssueBy);
            Assert.AreEqual(address.Person.Document.IssuedDate, dto.DocIssueDate);
            Assert.AreEqual(address.Person.Document.ValidBy, dto.DocValidBy);
            Assert.AreEqual(address.Person.Document.Type.Name, dto.DocType);

            var currentStatus = address.Person.PersonStatuses.FirstOrDefault(x => x.IsCurrent);

            Assert.IsNotNull(currentStatus);
            Assert.AreEqual(currentStatus.StatusType.Name, dto.PersonStatus);

            Assert.AreEqual(address.FullAddress, dto.Address);
            Assert.AreEqual(address.Person.Comments, dto.Comments);
        }

        [TestMethod]
        public void GetRegionByRegistruIdLessThan1000000_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expRegion = GetFirstObjectFromDbTable(x => x.StatisticIdentifier.HasValue && (x.StatisticIdentifier.Value < 1000000), () =>
            {
                var reg = GetRegionWithRegistruId();
                reg.RegistruId = 1000000;
                reg.StatisticIdentifier = 1;
                return reg;
            });
            var registruId = expRegion.StatisticIdentifier.HasValue ? expRegion.StatisticIdentifier.Value : -1;
            
            // Act
            
            var region = SafeExecFunc(_bll.GetRegionByAdministrativeCode, registruId);
     
            // Assert

            Assert.AreSame(expRegion, region);
        }

        [TestMethod]
        public void GetRegionByRegistruIdGreaterThan999999_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expRegion = GetFirstObjectFromDbTable(x => x.RegistruId.HasValue && (x.RegistruId.Value >= 1000000),
                () =>
                {
                    var reg = GetRegionWithRegistruId();
                    reg.RegistruId = 1000000;
                    reg.StatisticIdentifier = 1;
                    return reg;
                });
            var registruId = expRegion.RegistruId.HasValue ? expRegion.RegistruId.Value : -1;

            // Act

            var region = SafeExecFunc(_bll.GetRegionByAdministrativeCode, registruId);

            // Assert

            Assert.AreSame(expRegion, region);
        }

        [TestMethod]
        public void GetStreetByRopId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
            
            var expStreet = GetFirstObjectFromDbTable(x => (x.RopId != null) && (x.Deleted == null), GetStreetWithRopId);
            var streetCodeId = expStreet.RopId.HasValue ? expStreet.RopId.Value : -1;

            // Act

            var street = SafeExecFunc(_bll.GetStreetByRopId, streetCodeId);

            // Assert

            Assert.AreSame(expStreet, street);
        }

        [TestMethod]
        public void CreateStreet_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            var region = GetFirstObjectFromDbTable(GetRegion);
            var streetType = GetFirstObjectFromDbTable(GetStreetType);

            const string name = "name";
            const string description = "description";
            var regionId = region.Id;
            var streetTypeId = streetType.Id;
            long? ropId = 1;
            long? saiseId = 2;
            
            // Act

            var streetId = SafeExecFunc(_bll.CreateStreet, name, description, regionId, streetTypeId, ropId, saiseId);

            // Assert

            var street = GetLastCreatedObject<Street>();

            Assert.AreEqual(streetId, street.Id);
            Assert.AreEqual(name, street.Name);
            Assert.AreEqual(description, street.Description);
            Assert.AreEqual(ropId, street.RopId);
            Assert.AreEqual(saiseId, street.SaiseId);

            Assert.IsNotNull(street.Region);
            Assert.AreEqual(regionId, street.Region.Id);

            Assert.IsNotNull(street.StreetType);
            Assert.AreEqual(streetTypeId, street.StreetType.Id);
        }

        [TestMethod]
        public void GetPersonIdbyRspIds_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
            
            GetFirstObjectFromDbTable(GetRspRegistrationData);

            var personByConflicts = GetAllObjectsFromDbTable<PersonByConflict>(x => x.Person != null);
            var conflictIds = personByConflicts.Select(x => x.Id).ToList();
            var expResult = personByConflicts.Select(x => x.Person.Id).ToList();

            // Act

            var result = SafeExecFunc(_bll.GetPersonIdbyRspIds, conflictIds);

            // Assert

            Assert.IsNotNull(result);
            AssertObjectListsAreEqual(expResult, result);
        }

        [TestMethod]
        public void GetPersonIdbyRspIdsByNullPerson_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetRspRegistrationData);

            var personByConflicts = GetAllObjectsFromDbTable<PersonByConflict>(x => x.Person != null);
            var conflictIds = personByConflicts.Select(x => x.Id).ToList();
            conflictIds.Add(-1);

            // Act & Assert

            SafeExecFunc(_bll.GetPersonIdbyRspIds, conflictIds, true, false, "Conflict_NoPersonExistInRSV_ErrorMessage", MUI.Conflict_NoPersonExistInRSV_ErrorMessage);
        }

        [TestMethod]
        public void GetPersonIdbyRspId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetRspRegistrationData);

            var personByConflict = GetFirstObjectFromDbTable(x => x.Person != null, GetPersonByConflict);
            var conflictId = personByConflict.Id;
            var expResult = personByConflict.Person.Id;

            // Act & Assert

            ActAndAssertLongValue(_bll.GetPersonIdByConflictId, conflictId, expResult);
        }

        [TestMethod]
        public void GetPersonIdbyRspIdByNullPerson_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
            
            // Act & Assert

            SafeExecFunc(_bll.GetPersonIdByConflictId, -1L, true, false, "Conflict_NoPersonExistInRSV_ErrorMessage", MUI.Conflict_NoPersonExistInRSV_ErrorMessage);
        }

        [TestMethod]
        public void WriteNotification_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var data = GetFirstObjectFromDbTable(GetRspRegistrationData);
            var users = GetAllObjectsFromDbTable<SRVIdentityUser>(x => x.Roles.FirstOrDefault(y => y.Name == RolesStrings.Administrator) != null);
            const string notificationMessage = "message";

            // Act & Assert

            Enum.GetValues(typeof(ConflictStatusCode)).Cast<ConflictStatusCode>().ForEach(x =>
                WriteNotificationTest(x, data.RspModificationData, notificationMessage, users));
        }

        [TestMethod]
        public void SaveExistingAddress_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var checkAddress = GetFirstObjectFromDbTable(x => x.Street != null, GetAddress);

            var streetId = checkAddress.Street.Id;
            var houseNumber = checkAddress.HouseNumber;
            var suffix = checkAddress.Suffix;
            var buildingTypes = checkAddress.BuildingType;

            // Act

            SafeExecFunc(_bll.SaveAddress, streetId, houseNumber, suffix, buildingTypes, (long?)null, true, false, "CreateAddress_AlreadyExist_Message", MUI.CreateAddress_AlreadyExist_Message);
        }

        [TestMethod]
        public void SaveNonExistingAddressByNullPollingStation_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(GetStreet, true);
            var streetId = street.Id;
            const string suffix = "A";
            const BuildingTypes buildingTypes = BuildingTypes.Administrative;

            // Act

            SafeExecFunc(_bll.SaveAddress, streetId, (int?)null, suffix, buildingTypes, (long?)null);

            // Assert

            var newAddress = GetLastCreatedObject<Address>();

            Assert.IsNotNull(newAddress);
            Assert.AreSame(street, newAddress.Street);
            Assert.IsNull(newAddress.HouseNumber);
            Assert.AreEqual(suffix, newAddress.Suffix);
            Assert.AreEqual(buildingTypes, newAddress.BuildingType);
            Assert.IsNull(newAddress.PollingStation);
        }

        [TestMethod]
        public void SaveNonExistingAddressByNotNullPollingStation_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(GetStreet, true);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);

            var streetId = street.Id;
            int? houseNumber = 1;
            const string suffix = "A";
            const BuildingTypes buildingTypes = BuildingTypes.Administrative;
            long? pollingStationId = pollingStation.Id;

            // Act

            SafeExecFunc(_bll.SaveAddress, streetId, houseNumber, suffix, buildingTypes, pollingStationId);

            // Assert

            var newAddress = GetLastCreatedObject<Address>();

            Assert.IsNotNull(newAddress);
            Assert.AreSame(street, newAddress.Street);
            Assert.AreEqual(houseNumber, newAddress.HouseNumber);
            Assert.AreEqual(suffix, newAddress.Suffix);
            Assert.AreEqual(buildingTypes, newAddress.BuildingType);
            Assert.AreSame(pollingStation, newAddress.PollingStation);
        }

        [TestMethod]
        public void UpdateStatusToRetry_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetAdministratorRole();

            var data = GetFirstObjectFromDbTable(GetRspRegistrationData);
            var conflictId = data.Id;
            const string message = "message";

            var registrationData =
                GetFirstObjectFromDbTable<RspRegistrationData>(
                    x => x.RspModificationData.Id == conflictId && x.IsInConflict);
            int administrativeCode = registrationData.Administrativecode;

            // Act and Assert

            Enum.GetValues(typeof(ConflictStatusCode)).Cast<ConflictStatusCode>().ForEach(x =>
                UpdateStatusToRetryTest(conflictId, message, x, administrativeCode));
        }
        
        [TestMethod]
        public void GetConflict_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
            
            var expData = GetFirstObjectFromDbTable(GetRspRegistrationData);
            var id = expData.Id;

            // Act
            
            var data = SafeExecFunc(_bll.GetConflict, id);

            // Assert

            Assert.AreSame(expData, data);
        }

        [TestMethod]
        public void GetUserName_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expUserName = GetCurrentUser().UserName;
            
            // Act

            var userName = SafeExecFunc(_bll.GetUserName);

            // Assert

            Assert.AreEqual(expUserName, userName);
        }

        [TestMethod]
        public void GetUserRegionsByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetAdministratorRole();

            var expRegions = GetAllIdsFromDbTable<Region>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.GetUserRegions, expRegions);
        }

        [TestMethod]
        public void GetUserRegionsByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ConflictBll>();

            SetRegistratorRole();

            var expRegions = GetAllIdsFromDbTableWithUdfPropertyIn<Region>(x => x.Deleted == null, x => x.Id, UdfRegionsCriterion());

            // Act & Assert

            ActAndAssertAllPages(_bll.GetUserRegions, expRegions);

        }

        [TestMethod]
        public void ChangeAddress_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var person = GetFirstObjectFromDbTable(x => (x.EligibleAddress != null) && (x.EligibleAddress.Address != null), GetPersonWithEligibleAddress);
            var personId = person.Id;
            var personEligibleAddressId = person.EligibleAddress.Address.Id;

            var address = GetFirstObjectFromDbTable(x => x.Id != personEligibleAddressId, GetAddressWithNullHouseNumber);
            var addressId = address.Id;
            
            // Act

            SafeExec(_bll.ChangeAddress, addressId, personId);

            // Assert

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == personId);

            Assert.IsNotNull(newPerson.EligibleAddress);
            Assert.IsNotNull(newPerson.EligibleAddress.Address);
            Assert.AreEqual(addressId, newPerson.EligibleAddress.Address.Id);
        }

        [TestMethod]
        public void GetConflictAddress_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var rspRegData = GetFirstObjectFromDbTable(x => (x.RspModificationData != null) && x.IsInConflict,
                () => new RspRegistrationData()
                {
                    RspModificationData = GetRspRegistrationData().RspModificationData,
                    IsInConflict = true
                });

            var conflictId = rspRegData.RspModificationData.Id;
            
            // Act

            var rrd = SafeExecFunc(_bll.GetConflictAddress, conflictId);

            // Assert

            Assert.IsNotNull(rrd);
            Assert.IsNotNull(rrd.RspModificationData);
            Assert.AreEqual(conflictId, rrd.RspModificationData.Id);
            Assert.IsTrue(rrd.IsInConflict);
        }

        private void UpdateStatusToRetryTest(long conflictId, string message, ConflictStatusCode conflictCode, int administrativeCode)
        {
            // Arrange

            var conflicts = GetAllObjectsFromDbTable<RspModificationData>(x =>
                ((x.StatusConflictCode & conflictCode) > 0) &&
                x.Registrations.Any(y => y.IsInConflict && (y.Administrativecode == administrativeCode)));

            // Act

            SafeExec(_bll.UpdateStatusToRetry, conflictId, message, conflictCode);

            // Assert

            conflicts.ForEach(x =>
            {
                var conflict = GetFirstObjectFromDbTable<RspModificationData>(y => y.Id == x.Id);
                Assert.IsNotNull(conflict);
                Assert.AreEqual(RawDataStatus.Retry, conflict.Status);
                Assert.IsTrue(conflict.StatusDate.HasValue);
                Assert.AreEqual(DateTimeOffset.Now.Date.Date, conflict.StatusDate.Value.Date.Date);
                Assert.AreEqual(message, conflict.StatusMessage);
            });
        }

        private void WriteNotificationTest(ConflictStatusCode conflictStatus, RspModificationData conflictData, string notificationMessage, List<SRVIdentityUser> expectedUsers)
        {
            // Act

            SafeExec(_bll.WriteNotification, conflictStatus, conflictData, notificationMessage);

            // Assert
           
            var notification = GetLastCreatedObject<Notification>();

            Assert.IsNotNull(notification);
            Assert.AreEqual(notificationMessage, notification.Message);
            Assert.AreEqual(conflictData.Id, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.Update, notification.Event.EventType);
            
            AssertListsAreEqual(expectedUsers, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }
    }

}
