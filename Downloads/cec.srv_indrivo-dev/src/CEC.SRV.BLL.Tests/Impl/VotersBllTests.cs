using System.Linq;
using CEC.SRV.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using System.Collections.Generic;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.BLL.Dto;
using CEC.Web.SRV.Resources;
using CEC.SRV.BLL.Repositories;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class VotersBllTests : BaseBllTests
    {
        private VotersBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<VotersBll>();
        }

        [TestMethod]
        public void Get_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expPersons = GetAllIdsFromDbTable<Person>(x => x.Addresses.FirstOrDefault(y => y.IsEligible) != null);

            // Act & Assert

            ActAndAssertAllPages(_bll.Get, expPersons);
        }

        [TestMethod]
        public void GetIdnp_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var person = GetFirstObjectFromDbTable(x =>
                (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null),
                GetPerson);

            var idnp = person.Idnp;

            var expectedPersons = GetAllIdsFromDbTable<Person>(x => 
                (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null) &&
                (x.Idnp == idnp));

            // Act and Assert

            ActAndAssertAllPages(_bll.GetIdnp, idnp, expectedPersons);
        }

        [TestMethod]
        public void GetByFiltersByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);

            long? regionId = region.Id;
            long? pollingStationId = pollingStation.Id;

            //PersonFullAddress personFullAddress = GetFirstObjectFromDbTable<PersonFullAddress>(x =>
            //    (x.PollingStation != null) && (x.Person != null) && (x.Region != null) &&
            //    (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null), GetPersonFullAddress);

            //long? regionId = personFullAddress.Region.Id;
            //long? pollingStationId = personFullAddress.PollingStation.Id;
            
            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.PollingStation != null) && (x.PollingStation.Id == pollingStationId.Value) &&
                     (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null) &&
                     (x.Region != null) &&
                     (( (x.Region.Parent != null) && (x.Region.Parent.Id == regionId.Value)) || 
                        (x.Region.Id == regionId.Value))).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, regionId, pollingStationId, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersWithNullRegionIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            long? pollingStationId = pollingStation.Id;
            
            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.PollingStation != null) && (x.PollingStation.Id == pollingStationId.Value) &&
                     (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null)).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, (long?)null, pollingStationId, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersWithNullPollingStationIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            long? regionId = region.Id;

            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null) &&
                     (x.Region != null) &&
                     (((x.Region.Parent != null) && (x.Region.Parent.Id == regionId.Value)) ||
                        (x.Region.Id == regionId.Value))).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, regionId, (long?)null, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersWithNullRegionIdAndPollingStationIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null)).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, (long?)null, (long?)null, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);

            long? regionId = region.Id;
            long? pollingStationId = pollingStation.Id;
            
            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.AssignedUser != null) && (x.AssignedUser.IdentityUser != null) &&
                     (x.AssignedUser.IdentityUser.Id == SecurityHelper.GetLoggedUserId()) &&
                     (x.PollingStation != null) && (x.PollingStation.Id == pollingStationId.Value) &&
                     (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null) &&
                     (x.Region != null) &&
                     (((x.Region.Parent != null) && (x.Region.Parent.Id == regionId.Value)) ||
                        (x.Region.Id == regionId.Value))).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, regionId, pollingStationId, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersWithNullRegionIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            long? pollingStationId = pollingStation.Id;

            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.AssignedUser != null) && (x.AssignedUser.IdentityUser != null) &&
                     (x.AssignedUser.IdentityUser.Id == SecurityHelper.GetLoggedUserId()) &&
                     (x.PollingStation != null) && (x.PollingStation.Id == pollingStationId.Value) &&
                     (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null)).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, (long?)null, pollingStationId, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersWithNullPollingStationIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            long? regionId = region.Id;
            
            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.AssignedUser != null) && (x.AssignedUser.IdentityUser != null) &&
                     (x.AssignedUser.IdentityUser.Id == SecurityHelper.GetLoggedUserId()) &&
                     (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null) &&
                     (x.Region != null) &&
                     (((x.Region.Parent != null) && (x.Region.Parent.Id == regionId.Value)) ||
                        (x.Region.Id == regionId.Value))).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, regionId, (long?)null, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByFiltersWithNullRegionIdAndPollingStationIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var expectedPersons = GetAllObjectsFromDbTable<PersonFullAddress>(
                x => (x.AssignedUser != null) && (x.AssignedUser.IdentityUser != null) &&
                     (x.AssignedUser.IdentityUser.Id == SecurityHelper.GetLoggedUserId()) &&
                     (x.Person != null) && (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent) != null)).Select(x => x.Person.Id).ToList();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetByFilters, (long?)null, (long?)null, x => x.Id, expectedPersons, true, false);
        }

        [TestMethod]
        public void GetByIdnp_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expectedPerson = GetFirstObjectFromDbTable(GetPerson);
            var idnp = expectedPerson.Idnp;

            // Act
            
            var person = SafeExecFunc(_bll.GetByIdnp, idnp);
            
            // Assert

            Assert.IsNotNull(person);
            Assert.AreSame(expectedPerson, person);

        }

        [TestMethod]
        public void GetVotersCountByNullAddress_returns_correct_result()
        {
            // Arrange
            
            SetAdministratorRole();
            
            // Act
            
            var result = SafeExecFunc(_bll.GetVotersCount, (PersonAddress)null);

            // Assert

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetVotersCountByNullAddressAddress_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            // Act

            var result = SafeExecFunc(_bll.GetVotersCount, new PersonAddress());

            // Assert

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetVotersCountByValidPersonAddress_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => x.Address != null, GetPersonAddress);

            var expResult = (int)GetDbTableCount<PersonAddress>(x =>
                (x.ApNumber == address.ApNumber) && (x.ApSuffix == address.ApSuffix) &&
                (x.Address != null) && (x.Address.Id == address.Address.Id) &&
                (x.Person.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null));

            // Act

            var result = SafeExecFunc(_bll.GetVotersCount, address);

            // Assert

            Assert.AreEqual(expResult, result);
        }

        [TestMethod]
        public void UpdateStatusForPersonWithEligibleAddress_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();
            
            var person = GetFirstObjectFromDbTable(x => x.EligibleAddress != null, GetPersonWithEligibleAddress);
            var personStatusType = GetFirstObjectFromDbTable(GetPersonStatusType);
            
            var personId = person.Id;
            var statusId = personStatusType.Id;
            const string confirmation = "confirmation";

            // Act

            SafeExec(_bll.UpdateStatus, personId, statusId, confirmation);

            // Assert

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == personId);
            var newPersonStatus = newPerson.PersonStatuses.FirstOrDefault(x => x.IsCurrent);

            Assert.IsNotNull(newPerson);
            Assert.IsNotNull(newPersonStatus);
            
            Assert.AreSame(personStatusType, newPersonStatus.StatusType);

            var notification = GetLastCreatedObject<Notification>();
            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_PersonStatus_Change, person.Idnp, confirmation), notification.Message);
            Assert.AreEqual(personId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.Update, notification.Event.EventType);

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                new UsersWithAccessToRegionCriterion(person.EligibleAddress.Address.Street.Region.Id));

            AssertListsAreEqual(users, notification.Receivers.ToList(), x=> x.Id, x => x.User.Id);

        }

        [TestMethod]
        public void SearchRegionByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expRegions = GetAllIdsFromDbTable<Region>();

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchRegion, expRegions);
        }

        [TestMethod]
        public void SearchRegionByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var expRegions = GetAllIdsFromDbTableWithUdfPropertyIn<Region>(x => x.Id, UdfRegionsCriterion());

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchRegion, expRegions);
        }

        [TestMethod]
        public void SearchActiveElections_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expElections = GetAllIdsFromDbTable<Election>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchActiveElections, expElections);
        }

        [TestMethod]
        public void SearchPollingStationsByNullRegionIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expPollingStations = GetAllIdsFromDbTable<PollingStation>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchPollingStations, (long?)null, expPollingStations);
        }

        [TestMethod]
        public void SearchPollingStationsByNullRegionIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var expPollingStations = GetAllIdsFromDbTableWithUdfPropertyIn<PollingStation>(x => x.Deleted == null, x => x.Region.Id, UdfRegionsCriterion());

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchPollingStations, (long?)null, expPollingStations);
        }

        [TestMethod]
        public void SearchPollingStationsByNotNullAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => (x.Deleted == null) && (x.Region != null), GetPollingStation);

            long? regionId = pollingStation.Region.Id;

            var expPollingStations = GetAllIdsFromDbTableWithUdfPropertyIn<PollingStation>(x => x.Deleted == null, x => x.Region.Id, new RegionChildsFilterCriterion(regionId.Value));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchPollingStations, regionId, expPollingStations);
        }

        [TestMethod]
        public void SearchPollingStationsByNotNullNonAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => (x.Deleted == null) && (x.Region != null), GetPollingStation);

            long? regionId = pollingStation.Region.Id;

            var expPollingStations = GetAllIdsFromDbTable<PollingStation>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchPollingStations, regionId, expPollingStations);
        }

        [TestMethod]
        public void SearchAddressByNullRegionIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var expAddresses = GetAllIdsFromDbTable<Address>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchAddress, (long?)null, expAddresses);
        }

        [TestMethod]
        public void SearchAddressByNullRegionIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var expAddresses = GetAllIdsFromDbTable<Address>(x => (x.Deleted == null) && (RegionIsAccessibleForCurrentUser(x.Street.Region.Id)));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchAddress, (long?)null, expAddresses);
        }

        [TestMethod]
        public void SearchAddressByNotNullRegionIdByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Street.Region != null), GetAddress);
            long? regionId = address.Street.Region.Id;
            var expAddresses = GetAllIdsFromDbTable<Address>(x => (x.Deleted == null) && (x.Street.Region.Id == regionId.Value));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchAddress, regionId, expAddresses);
        }

        [TestMethod]
        public void SearchAddressByNotNullRegionIdByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Street.Region != null), GetAddress);
            long? regionId = address.Street.Region.Id;

            var expAddresses = GetAllIdsFromDbTable<Address>(x => (x.Deleted == null) && (x.Street != null) && (x.Street.Region != null) && 
                (x.Street.Region.Id == regionId.Value) && RegionIsAccessibleForCurrentUser(x.Street.Region.Id));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchAddress, regionId, expAddresses);
        }

        [TestMethod]
        public void GetStayStatementsByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetStayStatement);
            var expectedStayStatements = GetAllIdsFromDbTable<StayStatement>();
            
            // Act & Assert

            ActAndAssertAllPages(_bll.GetStayStatements, expectedStayStatements);
        }

        [TestMethod]
        public void GetStayStatementsByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var expectedStayStatements = GetAllIdsFromDbTable<StayStatement>(x =>
                    (x.BaseAddress != null) && (x.DeclaredStayAddress != null) &&
                    (GetFirstObjectFromDbTable<PersonFullAddress>(y => 
                        (y.AssignedUser != null) && (y.AssignedUser.IdentityUser != null) &&
                        ((y.Id == x.BaseAddress.Id) || (y.Id == x.DeclaredStayAddress.Id)) &&
                        (y.AssignedUser.IdentityUser.Id == SecurityHelper.GetLoggedUserId())
                    ) != null)
                );

            // Act & Assert

            ActAndAssertAllPages(_bll.GetStayStatements, expectedStayStatements);
        }

        [TestMethod]
        public void CreateStayStatement_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.Id == 3, GetPersonAddressType, 3L);

            var person = GetFirstObjectFromDbTable(GetPersonWithEligibleAddress);
            var address = GetFirstObjectFromDbTable(GetAddress);
            var election = GetFirstObjectFromDbTable(GetElection);
            var oldPersonEligibleAddress = person.EligibleAddress;
            
            var personId = person.Id;
            var addressId = address.Id;
            var electionId = election.Id;
            const long id = 0;
            int? apNumber = 30;
            const string apSuffix = "S";
            
            // Act

            var stayStatementId = SafeExecFunc(_bll.CreateStayStatement, id, personId, addressId, apNumber, apSuffix, electionId);

            // Assert

            var stayStatement = GetLastCreatedObject<StayStatement>();

            Assert.AreEqual(stayStatementId, stayStatement.Id);
            Assert.AreSame(person, stayStatement.Person);
            Assert.AreSame(oldPersonEligibleAddress, stayStatement.BaseAddress);

            Assert.IsNotNull(stayStatement.DeclaredStayAddress);
            Assert.AreSame(person, stayStatement.DeclaredStayAddress.Person);
            Assert.AreSame(address, stayStatement.DeclaredStayAddress.Address);
            Assert.IsTrue(stayStatement.DeclaredStayAddress.IsEligible);
            Assert.AreEqual(apNumber, stayStatement.DeclaredStayAddress.ApNumber);
            Assert.AreEqual(apSuffix, stayStatement.DeclaredStayAddress.ApSuffix);

            Assert.AreSame(election, stayStatement.ElectionInstance);

            var notification = GetLastCreatedObject<Notification>();
            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_StayStatement_Creation, stayStatement.Id, stayStatement.Person.Idnp), notification.Message);
            Assert.AreEqual(stayStatementId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.New, notification.Event.EventType);

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                new UsersWithAccessToRegionCriterion(stayStatement.BaseAddress.Address.Street.Region.Id)).Union(
                    GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                        new UsersWithAccessToRegionCriterion(stayStatement.DeclaredStayAddress.Address.Street.Region.Id)))
                .Distinct().ToList();
            
            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        [TestMethod]
        public void CreateStayStatementByPollingStation_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            const long streetTypeId = StreetType.UnknownStreetType;
            GetFirstObjectFromDbTable(x => x.Id == streetTypeId, GetStreetType, streetTypeId);
            GetFirstObjectFromDbTable(x => x.Id == 3, GetPersonAddressType, 3L);

            var person = GetFirstObjectFromDbTable(GetPersonWithEligibleAddress);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var election = GetFirstObjectFromDbTable(GetElection);
            var oldPersonEligibleAddress = person.EligibleAddress;

            var personId = person.Id;
            var pollingStationId = pollingStation.Id;
            var electionId = election.Id;

            // Act

            var stayStatementId = SafeExecFunc(_bll.CreateStayStatement, personId, pollingStationId, electionId);

            // Assert

            var stayStatement = GetLastCreatedObject<StayStatement>();

            Assert.AreEqual(stayStatementId, stayStatement.Id);
            Assert.AreSame(person, stayStatement.Person);
            Assert.AreSame(oldPersonEligibleAddress, stayStatement.BaseAddress);

            Assert.IsNotNull(stayStatement.DeclaredStayAddress);
            Assert.AreSame(person, stayStatement.DeclaredStayAddress.Person);
            Assert.IsTrue(stayStatement.DeclaredStayAddress.IsEligible);
         
            Assert.AreSame(election, stayStatement.ElectionInstance);

            var notification = GetLastCreatedObject<Notification>();
            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_StayStatement_Creation, stayStatement.Id, stayStatement.Person.Idnp), notification.Message);
            Assert.AreEqual(stayStatementId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.New, notification.Event.EventType);

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                new UsersWithAccessToRegionCriterion(stayStatement.BaseAddress.Address.Street.Region.Id)).Union(
                    GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                        new UsersWithAccessToRegionCriterion(stayStatement.DeclaredStayAddress.Address.Street.Region.Id)))
                .Distinct().ToList();

            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        [TestMethod]
        public void ElectionUniqueStayStatement_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var stayStatement = GetFirstObjectFromDbTable(x => (x.Person != null) && (x.ElectionInstance != null), GetStayStatement);

            var personId = stayStatement.Person.Id;
            var electionId = stayStatement.ElectionInstance.Id;

            var expectedResult = GetAllObjectsFromDbTable<StayStatement>(x =>
                x.Person.Id == personId && x.ElectionInstance.Id == electionId &&
                x.DeclaredStayAddress.IsEligible).Any();

            // Act

            var result = SafeExecFunc(_bll.ElectionUniqueStayStatement, personId, electionId);
            
            // Assert

            Assert.AreEqual(expectedResult, result);

        }

        [TestMethod]
        public void CancelStayStatementByRegistratorWithoutAccess_throws_an_exception()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var users = this.GetAllObjectsFromDbTable<SRVIdentityUser>();
            users.ForEach(x => Repository.Delete(x));

            var stayStatement = GetFirstObjectFromDbTable(GetStayStatement);
            var stayStatementId = stayStatement.Id;
            
            // Act

            SafeExec(_bll.CancelStayStatement, stayStatementId, true, false, "StayStatement_CancellationDenied_Message", MUI.StayStatement_CancellationDenied_Message);
        }

        [TestMethod]
        public void CancelStayStatementByRegistratorWithAccess_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();
            
            var stayStatement = GetFirstObjectFromDbTable(GetStayStatement);
            var region = stayStatement.BaseAddress.Address.Street.Region;
            AddRegionToCurrentUser(region);
            var stayStatementId = stayStatement.Id;

            // Act

            SafeExec(_bll.CancelStayStatement, stayStatementId);
            
            // Assert
            
            // var delStayStatement = GetLastDeletedObject<StayStatement>();
            // Assert.IsNotNull(delStayStatement);
            // Assert.AreEqual(stayStatementId, delStayStatement.Id);

            var delStayStatement = GetFirstObjectFromDbTable<StayStatement>(x => x.Id == stayStatementId);
            Assert.IsNull(delStayStatement);
            
            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Person.Id == stayStatement.Person.Id) && (x.PersonAddressType != null) && (x.PersonAddressType.Id == 1))
                .ForEach(x => Assert.IsTrue(x.IsEligible));

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Person.Id == stayStatement.Person.Id) && (x.PersonAddressType != null) && (x.PersonAddressType.Id == 3))
                .ForEach(x => Assert.IsFalse(x.IsEligible));


            #region Notification Assert

            var notification = GetLastCreatedObject<Notification>();

            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_StayStatement_Cancellation, stayStatement.Id, stayStatement.Person.Idnp), notification.Message);
            Assert.AreEqual(stayStatementId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.Delete, notification.Event.EventType);
            
            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                new UsersWithAccessToRegionCriterion(region.Id));

            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);

            #endregion Notification Assert
        }

        [TestMethod]
        public void CancelStayStatementByAdministrator_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var stayStatement = GetFirstObjectFromDbTable(GetStayStatement);
            var stayStatementId = stayStatement.Id;

            // Act

            SafeExec(_bll.CancelStayStatement, stayStatementId);

            // Assert

            var delStayStatement = GetLastDeletedObject<StayStatement>();

            Assert.IsNotNull(delStayStatement);
            Assert.AreEqual(stayStatementId, delStayStatement.Id);

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Person.Id == stayStatement.Person.Id) && (x.PersonAddressType != null) && (x.PersonAddressType.Id == 1))
            .ForEach(x => Assert.IsTrue(x.IsEligible));

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Person.Id == stayStatement.Person.Id) && (x.PersonAddressType != null) && (x.PersonAddressType.Id == 3))
                .ForEach(x => Assert.IsFalse(x.IsEligible));

            #region Notification Assert

            var notification = GetLastCreatedObject<Notification>();
            
            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_StayStatement_Cancellation, stayStatement.Id, stayStatement.Person.Idnp), notification.Message);
            Assert.AreEqual(stayStatementId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.Delete, notification.Event.EventType);

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                new UsersWithAccessToRegionCriterion(stayStatement.BaseAddress.Address.Street.Region.Id));

            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);

            #endregion Notification Assert
        }

        [TestMethod]
        public void UpdateAddressByNullPollingStation_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var person = GetFirstObjectFromDbTable(GetPersonWithEligibleAddressAndNullPollingStation);
            var address = GetFirstObjectFromDbTable(x => x.PollingStation == null, GetAddressWithNullPollingStation);

            var personId = person.Id;
            var addressId = address.Id;
            int? apNumber = 1;
            const string apSufix = "A";

            // Act

            SafeExec(_bll.UpdateAddress, personId, addressId, apNumber, apSufix, true, false, "Voters_ChangeAddressError_NoPollingStation", MUI.Voters_ChangeAddressError_NoPollingStation);
        }

        [TestMethod]
        public void UpdateAddressByNotNullPollingStation_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();
            
            var person = GetFirstObjectFromDbTable(GetPersonWithEligibleAddress);
            var address = GetFirstObjectFromDbTable(x => x.PollingStation != null, GetAddress);

            var oldEligibleAddress = person.EligibleAddress.GetFullPersonAddress(true);
            var regionOfBaseAddressId = person.EligibleAddress.Address.Street.Region.Id;
            var personId = person.Id;
            var addressId = address.Id;
            int? apNumber = 2;
            const string apSufix = "B";

            // Act

            SafeExec(_bll.UpdateAddress, personId, addressId, apNumber, apSufix);
            
            // Assert

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == personId);

            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(apNumber, newPerson.EligibleAddress.ApNumber);
            Assert.AreEqual(apSufix, newPerson.EligibleAddress.ApSuffix);

            #region Notification Assert

            var notification = GetLastCreatedObject<Notification>();

            var regionOfDeclaredAddressId = newPerson.EligibleAddress.Address.Street.Region.Id;

            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_PersonAddress_Change, person.Idnp, 
                    oldEligibleAddress, 
                    newPerson.EligibleAddress.GetFullPersonAddress(true)), 
                notification.Message);
            Assert.AreEqual(personId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.Update, notification.Event.EventType);

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
              new UsersWithAccessToRegionCriterion(regionOfBaseAddressId)).Union(
                  GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                      new UsersWithAccessToRegionCriterion(regionOfDeclaredAddressId)))
              .Distinct().ToList();
            
            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);

            #endregion Notification Assert
        }

        [TestMethod]
        public void GetPersonWithEligibleResidenceWithoutStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();
            
            var expectedPerson = GetFirstObjectFromDbTable(x => 
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (!x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (!x.EligibleAddress.IsEligible),
                GetPersonWithNonEligibleEligibleAddressWithoutStreets);

            var personId = expectedPerson.Id;

            // Act
            
            SafeExecFunc(_bll.GetPersonWithEligibleResidence, personId, true, false, "ChangeAddress_NotPermitted", MUI.ChangeAddress_NotPermitted);
        }

        [TestMethod]
        public void GetPersonWithEligibleResidenceWithStreets_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expectedPerson = GetFirstObjectFromDbTable(x =>
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (x.EligibleAddress.IsEligible) && (x.EligibleAddress.PersonAddressType.Id != 3),
                GetPersonWithEligibleAddress);

            var personId = expectedPerson.Id;
            
            // Act
            
            var person = SafeExecFunc(_bll.GetPersonWithEligibleResidence, personId);

            // Assert

            Assert.AreSame(expectedPerson, person);
        }

        [TestMethod]
        public void GetElection_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.Deleted == null, GetElection);
            var expElections = GetAllObjectsFromDbTable<Election>(x => x.Deleted == null);

            // Act

            var elections = SafeExecFunc(_bll.GetElection);

            // Assert

            AssertListsAreEqual(expElections, elections.ToList());
        }

        [TestMethod]
        public void VerificationSameAddressTrueCase_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
            
            var personAddress = GetFirstObjectFromDbTable(x => x.Address != null, GetPersonAddress);
            var address = personAddress.Address;
            
            var personAddressId = personAddress.Id;
            var declaredAddressId = address.Id;
            
            // Act

            var result = SafeExecFunc(_bll.VerificationSameAddress, personAddressId, declaredAddressId);
            
            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerificationSameAddressFalseCase_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => x.Address != null, GetNonEligiblePersonAddressWithoutStreets);
            var address = GetFirstObjectFromDbTable(x => x.Street.Region.Id != personAddress.Address.Street.Region.Id, GetAddress);

            var personAddressId = personAddress.Id;
            var declaredAddressId = address.Id;

            // Act

            var result = SafeExecFunc(_bll.VerificationSameAddress, personAddressId, declaredAddressId);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerificationSameRegionTrueCase_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => x.Address != null, GetPersonAddress);
            
            var personAddressId = personAddress.Id;
            var regionId = personAddress.Address.Street.Region.Id;
            
            // Act

            var result = SafeExecFunc(_bll.VerificationSameRegion, personAddressId, regionId);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerificationSameRegionFalseCase_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => x.Address != null, GetNonEligiblePersonAddressWithoutStreets);
            var region = GetFirstObjectFromDbTable(x => x.Id != personAddress.Address.Street.Region.Id, GetRegion);

            var personAddressId = personAddress.Id;
            var regionId = region.Id;
            
            // Act

            var result = SafeExecFunc(_bll.VerificationSameRegion, personAddressId, regionId);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SaveAbroadVoterRegistration_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var person = GetFirstObjectFromDbTable(GetPerson);
            var personId = person.Id;
            const string abroadAddress = "abroad address";
            const string residenceAddress = "residence address"; 
            const string abroadAddresCountry = "abroad address country"; 
            const double abroadAddressLat = 10;
            const double abroadAddressLong = 30;
            const string email = "a.a@a.com";
            const string ipAddress = "192.168.111.111";

            // Act

            var abroadVrId = SafeExecFunc(_bll.SaveAbroadVoterRegistration, personId, abroadAddress, residenceAddress, abroadAddresCountry, abroadAddressLat, abroadAddressLong, email, ipAddress);

            // Assert

            var avr = GetFirstObjectFromDbTable<AbroadVoterRegistration>(x => x.Id == abroadVrId);

            Assert.IsNotNull(avr);
            Assert.AreEqual(abroadVrId, avr.Id);
            Assert.AreEqual(abroadAddress, avr.AbroadAddress);
            Assert.IsNotNull(avr.Person);
            Assert.AreEqual(personId, avr.Person.Id);
            Assert.AreEqual(residenceAddress, avr.ResidenceAddress);
            Assert.AreEqual(abroadAddresCountry, avr.AbroadAddressCountry);
            Assert.AreEqual(abroadAddressLat, avr.AbroadAddressLat);
            Assert.AreEqual(abroadAddressLong, avr.AbroadAddressLong);
            Assert.AreEqual(email, avr.Email);
            Assert.AreEqual(ipAddress, avr.IpAddress);
        }

        [TestMethod]
        public void GetAbroadVotersAddress_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expAvrs = GetAllObjectsFromDbTable<AbroadVoterRegistration>();

            // Act

            var avrs = SafeExecFunc(_bll.GetAbroadVotersAddress);

            // Assert

            AssertListsAreEqual(expAvrs, avrs);
        }

        [TestMethod]
        public void GetRegionOfVotes_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();
            
            GetFirstObjectFromDbTable(GetAbroadVoterRegistration);
            var expCsgDtos = GetAllObjectsFromDbTable<AbroadVoterRegistration>().Select(x => x.AbroadAddressCountry).Distinct()
                .Select(
                    x =>
                        new CountryStatisticGroupedDto()
                        {
                            Country = x,
                            Count = (int) GetDbTableCount<AbroadVoterRegistration>(y => y.AbroadAddressCountry == x)
                        }).ToList();

            // Act

            var csgDtos = SafeExecFunc(_bll.GetRegionOfVotes, (string)null);

            // Assert
            
            Assert.IsNotNull(csgDtos);
            Assert.AreEqual(expCsgDtos.Count, csgDtos.Count);
            Assert.IsTrue(expCsgDtos.All(x => csgDtos.Any(y => (x.Country == y.Country) && (x.Count == y.Count))));
        }

        [TestMethod]
        public void GetAbroadVotersTimeline_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();
            
            GetFirstObjectFromDbTable(GetAbroadVoterRegistration);
            
            var expCsgDtos = GetAllObjectsFromDbTable<AbroadVoterRegistration>().Select(x => x.Created.Date).Distinct()
                .Select(
                    x =>
                        new StatisticTimelineDto()
                        {
                            Date = x,
                            Count = (int)GetDbTableCount<AbroadVoterRegistration>(y => y.Created.Date == x)
                        }).ToList();
            
            // Act

            var csgDtos = SafeExecFunc(_bll.GetAbroadVotersTimeline);

            // Assert
            
            Assert.AreEqual(expCsgDtos.Count, csgDtos.Count);
            Assert.IsTrue(expCsgDtos.All(x => csgDtos.Any(y => (x.Date == y.Date) && (x.Count == y.Count))));
        }

        [TestMethod]
        public void IsRegisteredToElectionTrueCase_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();
            
            var avr = GetFirstObjectFromDbTable(x => x.Person != null, GetAbroadVoterRegistration);
            var personId = avr.Person.Id;
            
            // Act

            var result = SafeExecFunc(_bll.IsRegisteredToElection, personId);
            
            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsRegisteredToElectionFalseCase_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            // Act

            var result = SafeExecFunc(_bll.IsRegisteredToElection, -1L);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdatePollingStation_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            const long streetTypeId = StreetType.UnknownStreetType;
            GetFirstObjectFromDbTable(x => x.Id == streetTypeId, GetStreetType, streetTypeId);

            var person = GetFirstObjectFromDbTable(GetPersonWithEligibleAddress);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);

            var oldPollingStation = person.EligibleAddress.Address.PollingStation;
            var oldPollingStationNumber = oldPollingStation != null ? oldPollingStation.FullNumber : "[N/A]";
            
            var personId = person.Id;
            var pollingStationId = pollingStation.Id;

            // Act

            SafeExec(_bll.UpdatePollingStation, personId, pollingStationId);

            // Assert

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == personId);

            Assert.IsNotNull(newPerson.EligibleAddress);
            Assert.IsNotNull(newPerson.EligibleAddress.Address);
            Assert.AreSame(pollingStation, newPerson.EligibleAddress.Address.PollingStation);

            #region Notification Assert

            var notification = GetLastCreatedObject<Notification>();
            
            Assert.IsNotNull(notification);
            Assert.AreEqual(string.Format(MUI.Notification_PersonPollingStation_Change, person.Idnp,
                    oldPollingStationNumber,
                    newPerson.EligibleAddress.Address.PollingStation.FullNumber),
                notification.Message);
            Assert.AreEqual(personId, notification.Event.EntityId);
            Assert.AreEqual(EventTypes.Update, notification.Event.EventType);

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
              new UsersWithAccessToRegionCriterion(pollingStation.Region.Id));

            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);

            #endregion Notification Assert
        }

        [TestMethod]
        public void ChangePollingStation_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            var streetTypeId = StreetType.UnknownStreetType;
            GetFirstObjectFromDbTable(x => x.Id == streetTypeId, GetStreetType, streetTypeId);

            var person = GetFirstObjectFromDbTable(GetPersonWithEligibleAddress);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);

            var oldPollingStation = person.EligibleAddress.Address.PollingStation;
            var oldPollingStationNumber = oldPollingStation != null ? oldPollingStation.FullNumber : "[N/A]";

            var peopleIds = new List<long>() { person.Id };
            var pollingStationId = pollingStation.Id;

            var users = GetAllObjectsFromDbTableWithUdfPropertyIn<SRVIdentityUser>(x => x.Id,
                 new UsersWithAccessToRegionCriterion(pollingStation.Region.Id));

            // Act

            SafeExec(_bll.ChangePollingStation, pollingStationId, peopleIds);

            // Assert
            
            peopleIds.ForEach(personId =>
                {
                    var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == personId);

                    Assert.IsNotNull(newPerson);
                    Assert.IsNotNull(newPerson.EligibleAddress);
                    Assert.IsNotNull(newPerson.EligibleAddress.Address);
                    Assert.AreSame(pollingStation, newPerson.EligibleAddress.Address.PollingStation);

                    #region Notification Assert

                    var entityTypeMsg =
                        MUI.ResourceManager.GetString(((INotificationEntity) newPerson).GetNotificationType());

                    var notification = GetFirstObjectFromDbTable<Notification>(x => 
                        (x.Event.EntityId == personId) && 
                        (x.Event.EntityType.Contains(entityTypeMsg ?? "")));

                    Assert.IsNotNull(notification);
                    Assert.AreEqual(string.Format(MUI.Notification_PersonPollingStation_Change, newPerson.Idnp,
                            oldPollingStationNumber,
                            newPerson.EligibleAddress.Address.PollingStation.FullNumber),
                        notification.Message);
                    Assert.AreEqual(personId, notification.Event.EntityId);
                    Assert.AreEqual(EventTypes.Update, notification.Event.EventType);
                    
                    AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);

                    #endregion Notification Assert
                }
            );
        }

        [TestMethod]
        public void GetRegionPollingStationsByPersonByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPollingStation);
            var exPollingStations = GetAllObjectsFromDbTable<PollingStation>();

            // Act

            var pollingStations = SafeExecFunc(_bll.GetRegionPollingStationsByPerson);
            
            // Assert

            AssertListsAreEqual(exPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetRegionPollingStationsByPersonByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();
            
            GetFirstObjectFromDbTable(GetPollingStation);
            var exPollingStations = GetAllObjectsFromDbTableWithUdfPropertyIn<PollingStation>(x => x.Region.Id, UdfRegionsCriterion());

            // Act

            var pollingStations = SafeExecFunc(_bll.GetRegionPollingStationsByPerson);

            // Assert

            AssertListsAreEqual(exPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void VerificationSameRegionWithoutStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var person = GetFirstObjectFromDbTable(x =>
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (!x.EligibleAddress.IsEligible),
                GetPersonWithNonEligibleEligibleAddressWithStreets);

            long[] peopleIds = { person.Id };
            
            // Act

            SafeExec(_bll.VerificationSameRegion, peopleIds, true, false, "ChangePollingStation_NotPermitted", MUI.ChangePollingStation_NotPermitted);
        }

        [TestMethod]
        public void VerificationSameRegionForVariousRegions_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();
            
            var person1 = GetFirstObjectFromDbTable(x =>
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (!x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (x.EligibleAddress.IsEligible) && (x.EligibleAddress.PersonAddressType.Id != 3),
                GetPersonWithEligibleEligibleAddressWithoutStreets);

            var person2 = GetFirstObjectFromDbTable(x =>
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (!x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (x.EligibleAddress.IsEligible) && (x.EligibleAddress.PersonAddressType.Id != 3) && (x.Id != person1.Id),
                GetPersonWithEligibleEligibleAddressWithoutStreets2);

            long[] peopleIds = { person1.Id, person2.Id};

            // Act

            SafeExec(_bll.VerificationSameRegion, peopleIds, true, false, "ChangeVotersData_NotPermitted", MUI.ChangeVotersData_NotPermitted);
        }

        [TestMethod]
        public void VerificationSameRegionForSameRegions_does_not_throw_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var person1 = GetFirstObjectFromDbTable(x =>
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (!x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (x.EligibleAddress.IsEligible) && (x.EligibleAddress.PersonAddressType.Id != 3),
                GetPersonWithEligibleEligibleAddressWithoutStreets);

            var person2 = GetFirstObjectFromDbTable(x =>
                    (x.EligibleAddress != null) && (x.EligibleAddress.Address != null) && (x.EligibleAddress.Address.Street != null) &&
                    (x.EligibleAddress.Address.Street.Region != null) && (!x.EligibleAddress.Address.Street.Region.HasStreets) &&
                    (x.EligibleAddress.IsEligible) && (x.EligibleAddress.PersonAddressType.Id != 3) && (x.Id != person1.Id),
                GetPersonWithEligibleEligibleAddressWithoutStreets2);
            person2.SetEligibleAddress(person1.EligibleAddress);
            Repository.SaveOrUpdate(person2);

            long[] peopleIds = { person1.Id, person2.Id };

            // Act

            SafeExec(_bll.VerificationSameRegion, peopleIds);
        }

        [TestMethod]
        public void GetMixedStreetsRegionStreetsTypeByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.HasStreets, GetRegion);
            GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);

            // Act

            var type = SafeExecFunc(_bll.GetRegionStreetsType);

            // Assert

            Assert.IsNotNull(type);
            Assert.AreEqual(RegionStreetsType.Mixed, type);
        }

        [TestMethod]
        public void GetWithoutStreetsRegionStreetsTypeByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);

            GetAllObjectsFromDbTable<Region>(x => x.HasStreets).ForEach(x =>
            {
                x.HasStreets = false;
                Repository.SaveOrUpdate(x);
                Session.Flush();
            });

            // Act

            var type = SafeExecFunc(_bll.GetRegionStreetsType);

            // Assert

            Assert.IsNotNull(type);
            Assert.AreEqual(RegionStreetsType.WithoutStreets, type);
        }

        [TestMethod]
        public void GetWithStreetsRegionStreetsTypeByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.HasStreets, GetRegion);

            GetAllObjectsFromDbTable<Region>(x => !x.HasStreets).ForEach(x =>
            {
                x.HasStreets = true;
                Repository.SaveOrUpdate(x);
                Session.Flush();
            });

            // Act

            var type = SafeExecFunc(_bll.GetRegionStreetsType);

            // Assert

            Assert.IsNotNull(type);
            Assert.AreEqual(RegionStreetsType.WithStreets, type);
        }

        [TestMethod]
        public void GetMixedStreetsRegionStreetsTypeByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var region1 = GetFirstObjectFromDbTable(x => x.HasStreets, GetRegion);
            var region2 = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);

            AddRegionToCurrentUser(region1);
            AddRegionToCurrentUser(region2);

            // Act

            var type = SafeExecFunc(_bll.GetRegionStreetsType);

            // Assert

            Assert.IsNotNull(type);
            Assert.AreEqual(RegionStreetsType.Mixed, type);
        }

        [TestMethod]
        public void GetWithoutStreetsRegionStreetsTypeByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);

            GetAllObjectsFromDbTable<Region>(x => x.HasStreets).ForEach(x =>
            {
                x.HasStreets = false;
                Repository.SaveOrUpdate(x);
                Session.Flush();
            });

            AddRegionToCurrentUser(region);

            // Act

            var type = SafeExecFunc(_bll.GetRegionStreetsType);

            // Assert

            Assert.IsNotNull(type);
            Assert.AreEqual(RegionStreetsType.WithoutStreets, type);
        }

        [TestMethod]
        public void GetWithStreetsRegionStreetsTypeByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, VotersBll>();

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets, GetRegion);

            AddRegionToCurrentUser(region);

            GetAllObjectsFromDbTable<Region>(x => !x.HasStreets).ForEach(x =>
            {
                x.HasStreets = true;
                Repository.SaveOrUpdate(x);
                Session.Flush();
            });

            // Act

            var type = SafeExecFunc(_bll.GetRegionStreetsType);

            // Assert

            Assert.IsNotNull(type);
            Assert.AreEqual(RegionStreetsType.WithStreets, type);
        }
    }

}
