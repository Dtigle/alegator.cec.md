using System.Linq;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Importer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using System.Collections.Generic;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class AddressBllTests : BaseBllTests
    {
        private AddressBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<AddressBll>();
        }
        
        [TestMethod]
        public void GetByNullRegionId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
         
            // Act and Assert

            ActAndAssertAllPages(_bll.Get, (long?)null, new List<long>(), false);
            }

        [TestMethod]
        public void GetByNotNullRegionIdAndAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();
            
            var address = GetFirstObjectFromDbTable(GetAddress);
            long? regionId = address.Street.Region.Id;
            var expectedAddresses = GetAllIdsFromDbTable<Address>(x => x.Street.Region.Id == regionId);

            // Act and Assert

            Assert.IsTrue(_bll.IsRegionAccessibleToCurrentUser(regionId.Value));
            ActAndAssertAllPages(_bll.Get, regionId, expectedAddresses);
        }

        [TestMethod]
        public void GetByNotNullRegionIdAndNonAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();
            
            SetRegistratorRole();

            var address = GetFirstObjectFromDbTable(GetAddress);
            long? regionId = address.Street.Region.Id;

            // Act and Assert

            ActAndAssertAllPages(_bll.Get, regionId, new List<long>(), false);
        }

        [TestMethod]
        public void GetStreetsByNullRegionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();
            
            SetRegistratorRole();

            var expectedStreets = GetAllObjectsFromDbTable<Street>();

           // Act
            
            var streets = SafeExecFunc(_bll.GetStreets, (long?)null);

            // Assert

            Assert.IsNotNull(streets);
            AssertListsAreEqual(expectedStreets, streets.ToList());
        }

        [TestMethod]
        public void GetStreetsByNotNullRegionIdAndRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();

            var street = GetFirstObjectFromDbTable(GetStreet);
            long? regionId = street.Region.Id;
            var expectedStreets = GetAllObjectsFromDbTableWithUdfPropertyIn<Street>(x => x.Region.Id == regionId.Value, x => x.Region.Id, UdfRegionsCriterion());
              
            // Act
            
            var streets = SafeExecFunc(_bll.GetStreets, regionId);

            // Assert

            Assert.IsFalse(SecurityHelper.LoggedUserIsInRole("Administrator"));
            Assert.IsNotNull(streets);
            AssertListsAreEqual(expectedStreets, streets.ToList());
        }

        [TestMethod]
        public void GetStreetsByNotNullRegionIdAndAdministrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(GetStreet);
            long? regionId = street.Region.Id;
            var expectedStreets = GetAllObjectsFromDbTable<Street>(x => x.Region.Id == regionId.Value);

            // Act

            var streets = SafeExecFunc(_bll.GetStreets, regionId);

            // Assert

            Assert.IsTrue(SecurityHelper.LoggedUserIsInRole("Administrator"));
            Assert.IsNotNull(streets);
            AssertListsAreEqual(expectedStreets, streets.ToList());
        }

        [TestMethod]
        public void GetStreetsByFilter_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();
            
            var street = GetFirstObjectFromDbTable(GetStreet);
            var term = string.IsNullOrEmpty(street.Name) ? "S" : street.Name.Substring((int) (street.Name.Length / 2));
            var expectedStreets = GetAllObjectsFromDbTable<Street>(x => x.Name.Contains(term));
            
            // Act

            var streets = SafeExecFunc(_bll.GetStreetsByFilter, term);

            // Assert
            
            Assert.IsNotNull(streets);
            AssertListsAreEqual(expectedStreets, streets.ToList());
        }

        [TestMethod]
        public void GetPollingStationsByNullRegionId_returns_correct_result()
        {
            // Act

            var result = SafeExecFunc(_bll.GetPollingStations, (long?)null);

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetPollingStationsByNotNullRegionIdAndAccessibleRegion_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            long? regionId = pollingStation.Region.Id;
            var expectedPollingStations = GetAllObjectsFromDbTable<PollingStation>(x => (x.Region.Id == regionId) && (x.Deleted == null));

            // Act

            var pollingStations = SafeExecFunc(_bll.GetPollingStations, regionId);

            // Assert

            Assert.IsTrue(_bll.IsRegionAccessibleToCurrentUser(regionId.Value));
            Assert.IsNotNull(pollingStations);
            AssertListsAreEqual(expectedPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetPollingStationsByNotNullRegionIdAndNonAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();

            long? regionId = 1;
            
            // Act

            var result = SafeExecFunc(_bll.GetPollingStations, regionId);

            // Assert

            Assert.IsFalse(_bll.IsRegionAccessibleToCurrentUser(regionId.Value));
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetPollingStationsByFilter_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var term = string.IsNullOrEmpty(pollingStation.Number) ? "1" : pollingStation.Number.Substring(0, (int)((pollingStation.Number.Length + 1)/ 2));
            var expectedPollingStations = GetAllObjectsFromDbTable<PollingStation>(x => x.Number.StartsWith(term) && (x.Deleted == null));

            // Act

            var pollingStations = SafeExecFunc(_bll.GetPollingStationsByFilter, term);

            // Assert

            Assert.IsNotNull(pollingStations);
            AssertListsAreEqual(expectedPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetAddressesByStreetId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();

            var address = GetFirstObjectFromDbTable(GetAddress);
            var streetId = address.Street.Id;
            var expectedAddresses = GetAllIdsFromDbTable<Address>(x => x.Street.Id == streetId);

            // Act

            var addresses = SafeExecFunc(_bll.GetAddressesByStreetId, streetId);

            // Assert

            Assert.IsNotNull(addresses);
            AssertObjectListsAreEqual(expectedAddresses, addresses.Select(x => x.Id).ToList());
        }

        [TestMethod]
        public void VerificationNotDeletedRegionWithStreets_returns_correct_result()
        {
            // Arrange
            
            SetAdministratorRole();

            var regionId = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted == null), GetRegion).Id;
            
            // Act

            SafeExec(_bll.VerificationRegion, regionId, (long?)null);
            
            // Assert

            VerificationIsRegionDeletedFalseCaseTest(regionId);
        }

        [TestMethod]
        public void VerificationDeletedRegionWithStreets_returns_correct_result()
        {
            // Arrange
            
            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted != null), GetDeletedRegionWithStreets);
            var regionId = region.Id;
            
            // Act

            SafeExec(_bll.VerificationRegion, regionId, (long?)null, true);
          
            // Assert

            VerificationIsRegionDeletedTrueCaseTest(regionId);
        }

        [TestMethod]
        public void VerificationRegionWithoutStreetsWithNullAddressId_returns_correct_result()
        {
            // Arrange
            
            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);
            var regionId = region.Id;
            var expectedRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);

            // Act

            SafeExec(_bll.VerificationRegion, regionId, (long?)null, true, false, "Address_RegionNotAcceptStreets", MUI.Address_RegionNotAcceptStreets);
          
            // Assert

            Assert.IsNotNull(expectedRegion);
            Assert.IsFalse(expectedRegion.HasStreets);
        }

        [TestMethod]
        public void VerificationRegionWithoutStreetsWithAddressId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);
            var regionId = region.Id;
            var expectedRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);

            // Act

            SafeExec(_bll.VerificationRegion, regionId, (long?)1, true, false, "Address_UpdateErrore_RegionHasNoStreets", MUI.Address_UpdateErrore_RegionHasNoStreets);

            // Assert

            Assert.IsFalse(expectedRegion.HasStreets);
        }

        [TestMethod]
        public void GetAddressesByAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(GetAddress);
            var regionId = address.Street.Region.Id;
            var expectedAddresses = GetAllIdsFromDbTable<Address>(x => x.Street.Region.Id == regionId);

            // Act and Assert

            Assert.IsTrue(_bll.IsRegionAccessibleToCurrentUser(regionId));
            ActAndAssertAllPages(_bll.GetAddresses, regionId, x => x.Id, expectedAddresses);
        }

        [TestMethod]
        public void GetAddressesByNonAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();
        
            // Act & assert
            
            ActAndAssertAllPages(_bll.GetAddresses, 1L, x => x.Id, new List<long>());
            Assert.IsFalse(_bll.IsRegionAccessibleToCurrentUser(1L));
        }

        [TestMethod]
        public void SaveExistingAddressWithOtherAddressId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            var address = GetFirstObjectFromDbTable(x => x.Street != null, GetAddress);

            // Arrange

            var addressId = address.Id + 1;
            var streetId = address.Street.Id;
            var houseNumber = address.HouseNumber;
            var suffix = address.Suffix;
            var buildingTypes = address.BuildingType;
            
            // Act
            
            SafeExec(_bll.SaveAddress, addressId, streetId, houseNumber, suffix, buildingTypes, (long?)null, true, false, "Create_UniqueError", MUI.Create_UniqueError);
        }

        [TestMethod]
        public void SaveExistingAddressWithTheSameAddressIdAndNullPollingStationId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(GetAddress);

            var addressId = address.Id;
            var streetId = GetFirstObjectFromDbTable(GetStreet).Id;
            const string suffix = "A";
            const BuildingTypes buildingTypes = BuildingTypes.Administrative;

            // Act
            
            SafeExec(_bll.SaveAddress, addressId, streetId, (int?)null, suffix, buildingTypes, (long?)null);
            
            // Assert

            var newAddress = GetFirstObjectFromDbTable<Address>(x => x.Id == addressId);
            Assert.AreEqual(addressId, newAddress.Id);
            Assert.IsNotNull(newAddress.Street);
            Assert.AreEqual(streetId, newAddress.Street.Id);
            Assert.IsNull(newAddress.HouseNumber);
            Assert.AreEqual(suffix, newAddress.Suffix);
            Assert.AreEqual(buildingTypes, newAddress.BuildingType);
            Assert.IsNull(newAddress.PollingStation);
        }
        
        [TestMethod]
        public void SaveExistingAddressWithTheSameAddressIdAndNotNullPollingStationId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(GetAddress);

            var addressId = address.Id;
            var streetId = GetFirstObjectFromDbTable(GetStreet).Id;
            int? houseNumber = 1;
            const string suffix = "A";
            const BuildingTypes buildingTypes = BuildingTypes.Administrative;
            long? pollingStationId = GetFirstObjectFromDbTable(GetPollingStation).Id;

            // Act
            
            SafeExec(_bll.SaveAddress, addressId, streetId, houseNumber, suffix, buildingTypes, pollingStationId);
            
            // Assert

            var newAddress = GetFirstObjectFromDbTable<Address>(x => x.Id == addressId);
            Assert.AreEqual(addressId, newAddress.Id);
            Assert.IsNotNull(newAddress.Street);
            Assert.AreEqual(streetId, newAddress.Street.Id);
            Assert.AreEqual(houseNumber, newAddress.HouseNumber);
            Assert.AreEqual(suffix, newAddress.Suffix);
            Assert.AreEqual(buildingTypes, newAddress.BuildingType);
            Assert.IsNotNull(newAddress.PollingStation);
            Assert.AreEqual(pollingStationId, newAddress.PollingStation.Id);
        }

        [TestMethod]
        public void SaveNewAddress_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            var streetId = GetFirstObjectFromDbTable(GetStreet).Id;
            int? houseNumber = 1;
            const string suffix = "A";
            const BuildingTypes buildingTypes = BuildingTypes.Administrative;
            long? pollingStationId = GetFirstObjectFromDbTable(GetPollingStation).Id;

            // Act

            SafeExec(_bll.SaveAddress, 0L, streetId, houseNumber, suffix, buildingTypes, pollingStationId);
            
            // Assert

            var newAddress = GetLastCreatedObject<Address>();

            Assert.IsNotNull(newAddress);
            Assert.IsNotNull(newAddress.Street);
            Assert.AreEqual(streetId, newAddress.Street.Id);
            Assert.AreEqual(houseNumber, newAddress.HouseNumber);
            Assert.AreEqual(suffix, newAddress.Suffix);
            Assert.AreEqual(buildingTypes, newAddress.BuildingType);
            Assert.IsNotNull(newAddress.PollingStation);
            Assert.AreEqual(pollingStationId, newAddress.PollingStation.Id);
        }

        [TestMethod]
        public void UpdateLocationForExistingAddress_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(GetAddress);

            const double latitude = 20;
            const double longitude = 30;

            // Act

            SafeExec(_bll.UpdateLocation, address.Id, latitude, longitude);
            
            // Assert

            var newAddress = GetFirstObjectFromDbTable<Address>(x => x.Id == address.Id);

            Assert.IsNotNull(newAddress.GeoLocation);
            Assert.AreEqual(latitude, newAddress.GeoLocation.Latitude);
            Assert.AreEqual(longitude, newAddress.GeoLocation.Longitude);
        }

        [TestMethod]
        public void DeleteAddressWithAssociatedPollingStations_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => (x.PollingStationAddress != null) && (x.Deleted == null), GetPollingStation);
            var addressId = pollingStation.PollingStationAddress.Id;

            // Act
            
            SafeExec(_bll.DeleteAddress, addressId, true, false, "Address_NotPermittedDeletingDueToPollingStation", MUI.Address_NotPermittedDeletingDueToPollingStation);
          
            // Assert

            AssertDeletedEntity<Address>(addressId, false);
        }

        [TestMethod]
        public void DeleteAddressWithoutPollingStationsButWithPersonAddresses_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address != null) && (x.Deleted == null), GetPersonAddress);
            var addressId = personAddress.Address.Id;

            var pollingStations = GetAllObjectsFromDbTable<PollingStation>(x => (x.PollingStationAddress != null) && (x.PollingStationAddress.Id == addressId) && (x.Deleted == null));
            pollingStations.ForEach(DeleteEntity);

            // Act

            SafeExec(_bll.DeleteAddress, addressId, true, false, "Address_NotPermittedDeletingDueToPerson", MUI.Address_NotPermittedDeletingDueToPerson);
           
            // Assert

            AssertDeletedEntity<Address>(addressId, false);
        }

        [TestMethod]
        public void DeleteAddressWithoutPollingStationsAndPersonAddresses_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            var address = GetFirstObjectFromDbTable(GetAddress);
            var addressId = address.Id;

            var pollingStations = GetAllObjectsFromDbTable<PollingStation>(x => (x.PollingStationAddress != null) && (x.PollingStationAddress.Id == addressId) && (x.Deleted == null));
            pollingStations.ForEach(DeleteEntity);

            var personAddresses = GetAllObjectsFromDbTable<PersonAddress>(x => x.Address.Id == addressId && x.Deleted == null);
            personAddresses.ForEach(DeleteEntity);
          
            // Act
            
            SafeExec(_bll.DeleteAddress, addressId);
            
            // Assert

            AssertDeletedEntity<Address>(addressId, true);
        }

        [TestMethod]
        public void UpdateNotExistingPollingStation_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            // Act
            
            SafeExec(_bll.UpdatePollingStation, -1L, (IEnumerable<long>)null, true, false, "", "PollingSttion not found.");
        }

        [TestMethod]
        public void UpdateExistingPollingStation_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var pollingStationId = pollingStation.Id;
            var address = GetFirstObjectFromDbTable(GetAddress);
            var addressIds = new[] {address.Id};

            // Act

            SafeExec(_bll.UpdatePollingStation, pollingStationId, addressIds);
            
            // Assert

            var newAddress = GetFirstObjectFromDbTable<Address>(x => x.Id == address.Id);

            Assert.IsNotNull(newAddress.PollingStation);
            Assert.AreEqual(pollingStationId, newAddress.PollingStation.Id);
        }

        [TestMethod]
        public void SearchStreetsByNullRegionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var expStreets = GetAllIdsFromDbTable<Street>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchStreets, (long?)null, expStreets);
        }

        [TestMethod]
        public void SearchStreetsByNotNullRegionIdAndAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.Deleted == null), GetStreet);
            long? regionId = street.Region.Id;

            var expStreets = GetAllIdsFromDbTable<Street>(x => (x.Deleted == null) && (x.Region != null) && (x.Region.Id == regionId.Value));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchStreets, regionId, expStreets);
        }

        [TestMethod]
        public void SearchStreetsByNotNullRegionIdAndRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.Deleted == null), GetStreet);
            long? regionId = street.Region.Id;
            AddRegionToCurrentUser(street.Region);

            var expStreets = GetAllIdsFromDbTable<Street>(x => (x.Deleted == null) && (x.Region != null) && (x.Region.Id == regionId.Value) && RegionIsAccessibleForCurrentUser(regionId.Value));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchStreets, regionId, expStreets);
        }

        [TestMethod]
        public void SearchStreetsForPublicByNullRegionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var expStreets = GetAllIdsFromDbTable<Street>(x => x.Deleted == null);

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchStreetsForPublic, (long?)null, expStreets);
        }

        [TestMethod]
        public void SearchStreetsForPublicByNotNullRegionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.Deleted == null), GetStreet);
            long? regionId = street.Region.Id;

            var expStreets = GetAllIdsFromDbTable<Street>(x => (x.Deleted == null) && (x.Region != null) && (x.Region.Id == regionId.Value));

            // Act & Assert

            ActAndAssertAllPages(_bll.SearchStreetsForPublic, regionId, expStreets);
        }

        [TestMethod]
        public void VerificationRegionForChangePollingStationByRegionWithoutStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForChangePollingStation, regionId, true, false, "Address_RegionNotAcceptChangePollingStation", MUI.Address_RegionNotAcceptChangePollingStation);
        }

        [TestMethod]
        public void VerificationRegionForChangePollingStationByNotDeletedRegionWithStreets_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted == null), GetRegion);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForChangePollingStation, regionId);
        }

        [TestMethod]
        public void VerificationRegionForChangePollingStationByDeletedRegionWithStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted != null), GetDeletedRegionWithStreets);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForChangePollingStation, regionId, true, false, "Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
        }

        [TestMethod]
        public void VerificationRegionForUpdateGeolocationByRegionWithoutStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForUpdateGeolocation, regionId, true, false, "Address_RegionNotAcceptUpdateGeolocation", MUI.Address_RegionNotAcceptUpdateGeolocation);
        }

        [TestMethod]
        public void VerificationRegionForUpdateGeolocationByNotDeletedRegionWithStreets_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted == null), GetRegion);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForUpdateGeolocation, regionId);
        }

        [TestMethod]
        public void VerificationRegionForUpdateGeolocationByDeletedRegionWithStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted != null), GetDeletedRegionWithStreets);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForUpdateGeolocation, regionId, true, false, "Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
        }

        [TestMethod]
        public void VerificationRegionForAdjustmentAddressesByRegionWithoutStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForAdjustmentAddresses, regionId, true, false, "Address_RegionNotAcceptAdjustmentAddresses", MUI.Address_RegionNotAcceptAdjustmentAddresses);
        }

        [TestMethod]
        public void VerificationRegionForAdjustmentAddressesByNotDeletedRegionWithStreets_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted == null), GetRegion);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForAdjustmentAddresses, regionId);
        }

        [TestMethod]
        public void VerificationRegionForAdjustmentAddressesByDeletedRegionWithStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted != null), GetDeletedRegionWithStreets);
            var regionId = region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationRegionForAdjustmentAddresses, regionId, true, false, "Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
        }

        [TestMethod]
        public void VerificationIsStreetDeletedByDeletedStreet_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => x.Deleted != null, GetDeletedStreet);
            var streetId = street.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIsStreetDeleted, streetId, true, false, "Address_UnDeleteAddressError", MUI.Address_UnDeleteAddressError);
        }

        [TestMethod]
        public void VerificationIsStreetDeletedByNotDeletedStreet_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => x.Deleted == null, GetStreet);
            var streetId = street.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIsStreetDeleted, streetId);
        }

        [TestMethod]
        public void AdjustmentAddresses_has_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var oldPersonAddress = GetFirstObjectFromDbTable(x => (x.Address != null) && (x.Address.Street != null), GetPersonAddress);
            var oldAddress = oldPersonAddress.Address;
            var oldAddressId = oldAddress.Id;
            var oldAddressStreetId = oldAddress.Street.Id;
            
            var newAddress = GetFirstObjectFromDbTable(x => x.Id != oldAddressId, GetAddressWithoutStreets);
            var newAddressId = newAddress.Id;
            
            var newPersonAddresses = GetAllObjectsFromDbTable<PersonAddress>(x => (x.Address != null) && (x.Address.Id == oldAddressId));

            GetFirstObjectFromDbTable(x => x.SrvAddressId == oldAddressId, () =>
            {
                var maddr = GetMappingAddress();
                maddr.SrvAddressId = oldAddressId;
                return maddr;
            });

            // Act

            SafeExec(_bll.AdjustmentAddresses, oldAddressId, newAddressId);

            // Assert

            oldAddress = GetLastDeletedObject<Address>();
            Assert.IsNotNull(oldAddress);
            Assert.AreEqual(oldAddressId, oldAddress.Id);
            
            newPersonAddresses.ForEach(x => Assert.AreSame(newAddress, x.Address));
            
            var mappingAddress = GetFirstObjectFromDbTable<MappingAddress>(x => x.SrvAddressId == oldAddressId);
            Assert.IsNull(mappingAddress);

            var oldAddressStreet = GetFirstObjectFromDbTable<Street>(x => x.Id == oldAddressStreetId);
            var addressOnTheOldStreet = GetFirstObjectFromDbTable<Address>(x => x.Street.Id == oldAddressStreetId);
            Assert.IsTrue((oldAddressStreet == null) || (oldAddressStreet.Deleted != null) || (addressOnTheOldStreet != null));
        }

        [TestMethod]
        public void VerificationIfAddressHasReferenceByExistingPersonWithGivenAddress_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address != null) && (x.Deleted == null), GetPersonAddress);
            var addressId = personAddress.Address.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfAddressHasReference, addressId, true, false, "",
                string.Format(MUI.HasReference_Error, personAddress.Address.GetObjectType(), personAddress.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfAddressHasReferenceByExistingPollingStationWithGivenAddress_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => (x.PollingStationAddress != null) && (x.Deleted == null), GetPollingStation);
            var addressId = pollingStation.PollingStationAddress.Id;

            GetAllObjectsFromDbTable<PersonAddress>(x => x.Address.Id == addressId && x.Deleted == null).ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfAddressHasReference, addressId, true, false, "",
                string.Format(MUI.HasReference_Error, pollingStation.PollingStationAddress.GetObjectType(), pollingStation.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfAddressHasReference_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfAddressHasReference, -1L);
        }

        [TestMethod]
        public void GetAssignedRegionsByAdmin_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetAdministratorRole();

            var expRegions = GetAllObjectsFromDbTable<Region>();

            // Act

            var regions = SafeExecFunc(_bll.GetAssignedRegions);
            
            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expRegions, regions.ToList());
        }

        [TestMethod]
        public void GetAssignedRegionsByRegistrator_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, AddressBll>();

            SetRegistratorRole();

            var expRegions = GetAllObjectsFromDbTableWithUdfPropertyIn<Region>(x => x.Id, UdfRegionsCriterion());

            // Act

            var regions = SafeExecFunc(_bll.GetAssignedRegions);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expRegions, regions.ToList());
        }
    }
}
