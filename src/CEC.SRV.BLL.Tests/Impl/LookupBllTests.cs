using System;
using System.Linq;
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
    public class LookupBllTests : BaseBllTests
    {
        private LookupBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<LookupBll>();
        }

        #region Update

        [TestMethod]
        public void UpdateStreetById_has_correct_logic()
        {
            UpdateTest(GetStreet);
        }

        [TestMethod]
        public void UpdateRegionById_has_correct_logic()
        {
            UpdateTest(GetRegion);
        }

        [TestMethod]
        public void UpdateRegionTypeById_has_correct_logic()
        {
            UpdateTest(GetRegionType);
        }

        [TestMethod]
        public void UpdateStreetTypeById_has_correct_logic()
        {
            UpdateTest(GetStreetType);
        }

        [TestMethod]
        public void UpdatePersonAddressTypeById_has_correct_logic()
        {
            UpdateTest(GetPersonAddressType);
        }

        [TestMethod]
        public void UpdateManagerTypeById_has_correct_logic()
        {
            UpdateTest(GetManagerType);
        }

        [TestMethod]
        public void UpdateGenderById_has_correct_logic()
        {
            UpdateTest(GetGender);
        }

        [TestMethod]
        public void UpdateDocumentTypeById_has_correct_logic()
        {
            UpdateTest(GetDocumentType);
        }

        [TestMethod]
        public void UpdateElectionTypeById_has_correct_logic()
        {
            UpdateTest(GetElectionType);
        }

        [TestMethod]
        public void UpdatePersonStatusTypeById_has_correct_logic()
        {
            UpdateTest(GetPersonStatusType);
        }

        private void UpdateTest<T>(Func<T> newObjBuilder) where T : Lookup
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(newObjBuilder);

            var id = entity.Id;
            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.Update<T>, id, name, description);

            // Assert

            var newEntity = GetFirstObjectFromDbTable<T>(x => x.Id == id);

            Assert.IsNotNull(newEntity);
            Assert.AreEqual(id, newEntity.Id);
            Assert.AreEqual(name, newEntity.Name);
            Assert.AreEqual(description, newEntity.Description);
        }

        #endregion Update

        #region Add

        [TestMethod]
        public void AddRegionTypeById_has_correct_logic()
        {
            AddTest<RegionType>();
        }

        [TestMethod]
        public void AddStreetTypeById_has_correct_logic()
        {
            AddTest<StreetType>();
        }

        [TestMethod]
        public void AddPersonAddressTypeById_has_correct_logic()
        {
            AddTest<PersonAddressType>();
        }

        [TestMethod]
        public void AddManagerTypeById_has_correct_logic()
        {
            AddTest<ManagerType>();
        }

        [TestMethod]
        public void AddGenderById_has_correct_logic()
        {
            AddTest<Gender>();
        }

        [TestMethod]
        public void AddDocumentTypeById_has_correct_logic()
        {
            AddTest<DocumentType>();
        }

        [TestMethod]
        public void AddElectionTypeById_has_correct_logic()
        {
            AddTest<ElectionType>();
        }

        [TestMethod]
        public void AddPersonStatusTypeById_has_correct_logic()
        {
            AddTest<PersonStatusType>();
        }

        private void AddTest<T>() where T : Lookup, new()
        {
            // Arrange

            SetAdministratorRole();

            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.Add<T>, name, description);

            // Assert

            var entity = GetLastCreatedObject<T>();

            Assert.IsNotNull(entity);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(description, entity.Description);
        }

        #endregion Add

        #region SaveOrUpdateWithNullId

        [TestMethod]
        public void SaveOrUpdateWithNullIdRegionTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<RegionType>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdStreetTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<StreetType>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdPersonAddressTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<PersonAddressType>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdManagerTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<ManagerType>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdGenderById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<Gender>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdDocumentTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<DocumentType>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdElectionTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<ElectionType>();
        }

        [TestMethod]
        public void SaveOrUpdateWithNullIdPersonStatusTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNullIdTest<PersonStatusType>();
        }

        private void SaveOrUpdateWithNullIdTest<T>() where T : Lookup, new()
        {
            // Arrange

            SetAdministratorRole();

            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdate<T>, (long?) null, name, description);

            // Assert

            var entity = GetLastCreatedObject<T>();

            Assert.IsNotNull(entity);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(description, entity.Description);
        }

        #endregion SaveOrUpdateWithNullId

        #region SaveOrUpdateWithNotNullId

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdRegionTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetRegionType);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdStreetTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetStreetType);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdPersonAddressTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetPersonAddressType);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdManagerTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetManagerType);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdGenderById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetGender);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdDocumentTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetDocumentType);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdElectionTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetElectionType);
        }

        [TestMethod]
        public void SaveOrUpdateWithNotNullIdPersonStatusTypeById_has_correct_logic()
        {
            SaveOrUpdateWithNotNullIdTest(GetPersonStatusType);
        }

        private void SaveOrUpdateWithNotNullIdTest<T>(Func<T> newObjBuilder) where T : Lookup, new()
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(newObjBuilder);

            long? id = entity.Id;
            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdate<T>, id, name, description);

            // Assert

            var newEntity = GetFirstObjectFromDbTable<T>(x => x.Id == id);

            Assert.IsNotNull(newEntity);
            Assert.AreEqual(id, newEntity.Id);
            Assert.AreEqual(name, newEntity.Name);
            Assert.AreEqual(description, newEntity.Description);
        }

        #endregion SaveOrUpdateWithNotNullId

        #region SaveOrUpdateDocType

        [TestMethod]
        public void SaveOrUpdateNonPrimaryDocTypeByNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdateDocType, (long?) null, name, description, false);

            // Assert

            var documentType = GetLastCreatedObject<DocumentType>();

            Assert.IsNotNull(documentType);
            Assert.AreEqual(name, documentType.Name);
            Assert.AreEqual(description, documentType.Description);
            Assert.IsFalse(documentType.IsPrimary);
        }

        [TestMethod]
        public void SaveOrUpdatePrimaryDocTypeByNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdateDocType, (long?) null, name, description, true);

            // Assert

            var documentType = GetLastCreatedObject<DocumentType>();

            Assert.IsNotNull(documentType);
            Assert.AreEqual(name, documentType.Name);
            Assert.AreEqual(description, documentType.Description);
            Assert.IsTrue(documentType.IsPrimary);

            GetAllObjectsFromDbTable<DocumentType>(x => x.Id != documentType.Id)
                .ForEach(y => Assert.IsFalse(y.IsPrimary));
        }

        [TestMethod]
        public void SaveOrUpdateNonPrimaryDocTypeByNotNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var docType = GetFirstObjectFromDbTable(GetDocumentType);

            long? id = docType.Id;
            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdateDocType, id, name, description, false);

            // Assert

            var documentType = GetFirstObjectFromDbTable<DocumentType>(x => x.Id == id);

            Assert.IsNotNull(documentType);
            Assert.AreEqual(id, documentType.Id);
            Assert.AreEqual(name, documentType.Name);
            Assert.AreEqual(description, documentType.Description);
            Assert.IsFalse(documentType.IsPrimary);
        }

        [TestMethod]
        public void SaveOrUpdatePrimaryDocTypeByNotNullPrimaryId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var docType = GetFirstObjectFromDbTable(x => x.IsPrimary, GetDocumentType);

            long? id = docType.Id;
            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdateDocType, id, name, description, true);

            // Assert

            var documentType = GetFirstObjectFromDbTable<DocumentType>(x => x.Id == id);

            Assert.IsNotNull(documentType);
            Assert.AreEqual(id, documentType.Id);
            Assert.AreEqual(name, documentType.Name);
            Assert.AreEqual(description, documentType.Description);
            Assert.IsTrue(documentType.IsPrimary);
        }

        [TestMethod]
        public void SaveOrUpdatePrimaryDocTypeByNotNullNonPrimaryId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var docType = GetFirstObjectFromDbTable(x => !x.IsPrimary, GetNonPrimaryDocumentType);

            long? id = docType.Id;
            const string name = "name";
            const string description = "description";

            // Act

            SafeExec(_bll.SaveOrUpdateDocType, id, name, description, true);

            // Assert

            var documentType = GetFirstObjectFromDbTable<DocumentType>(x => x.Id == id);

            Assert.IsNotNull(documentType);
            Assert.AreEqual(id, documentType.Id);
            Assert.AreEqual(name, documentType.Name);
            Assert.AreEqual(description, documentType.Description);
            Assert.IsTrue(documentType.IsPrimary);

            GetAllObjectsFromDbTable<DocumentType>(x => x.Id != documentType.Id)
                .ForEach(y => Assert.IsFalse(y.IsPrimary));
        }

        #endregion SaveOrUpdateDocType

        #region SaveOrUpdatePersonStatus

        [TestMethod]
        public void SaveOrUpdatePersonStatusByNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            const string name = "name";
            const string description = "description";
            const bool isExcludable = false;

            // Act

            SafeExec(_bll.SaveOrUpdatePersonStatus, (long?) null, name, description, isExcludable);

            // Assert

            var status = GetLastCreatedObject<PersonStatusType>();

            Assert.IsNotNull(status);
            Assert.AreEqual(name, status.Name);
            Assert.AreEqual(description, status.Description);
            Assert.AreEqual(isExcludable, status.IsExcludable);
        }

        [TestMethod]
        public void SaveOrUpdatePersonStatusByNotNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var status = GetFirstObjectFromDbTable(GetPersonStatusType);

            long? id = status.Id;
            const string name = "name";
            const string description = "description";
            const bool isExcludable = true;

            // Act

            SafeExec(_bll.SaveOrUpdatePersonStatus, id, name, description, isExcludable);

            // Assert

            var newStatus = GetFirstObjectFromDbTable<PersonStatusType>(x => x.Id == id);

            Assert.IsNotNull(newStatus);
            Assert.AreEqual(id, newStatus.Id);
            Assert.AreEqual(name, newStatus.Name);
            Assert.AreEqual(description, newStatus.Description);
            Assert.AreEqual(isExcludable, newStatus.IsExcludable);
        }

        #endregion SaveOrUpdatePersonStatus

        #region SaveOrUpdateRegionType

        [TestMethod]
        public void SaveOrUpdateRegionTypeByNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            const string name = "name";
            const string description = "description";
            const byte rank = 0;

            // Act

            SafeExec(_bll.SaveOrUpdateRegionType, (long?) null, name, description, rank);

            // Assert

            var regionType = GetLastCreatedObject<RegionType>();

            Assert.IsNotNull(regionType);
            Assert.AreEqual(name, regionType.Name);
            Assert.AreEqual(description, regionType.Description);
            Assert.AreEqual(rank, regionType.Rank);
        }

        [TestMethod]
        public void SaveOrUpdateRegionTypeByNotNullId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(GetRegionType);

            long? id = regionType.Id;
            const string name = "name";
            const string description = "description";
            const byte rank = 1;

            // Act

            SafeExec(_bll.SaveOrUpdateRegionType, id, name, description, rank);

            // Assert

            var newRegionType = GetFirstObjectFromDbTable<RegionType>(x => x.Id == id);

            Assert.IsNotNull(newRegionType);
            Assert.AreEqual(id, newRegionType.Id);
            Assert.AreEqual(name, newRegionType.Name);
            Assert.AreEqual(description, newRegionType.Description);
            Assert.AreEqual(rank, newRegionType.Rank);
        }

        #endregion SaveOrUpdateRegionType

        [TestMethod]
        public void UpdatePersonStatus_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var status = GetFirstObjectFromDbTable(GetPersonStatusType);

            var id = status.Id;
            const string name = "name";
            const string description = "description";
            const bool isExcludable = true;

            // Act

            SafeExec(_bll.UpdatePersonStatus, id, name, description, isExcludable);

            // Assert

            var newStatus = GetFirstObjectFromDbTable<PersonStatusType>(x => x.Id == id);

            Assert.IsNotNull(newStatus);
            Assert.AreEqual(id, newStatus.Id);
            Assert.AreEqual(name, newStatus.Name);
            Assert.AreEqual(description, newStatus.Description);
            Assert.AreEqual(isExcludable, newStatus.IsExcludable);
        }

        #region DeleteRegion

        [TestMethod]
        public void DeleteRegionWithSubregions_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            var regionId = region.Parent.Id;

            // Act

            SafeExec(_bll.DeleteRegion, regionId, true, false, "Error_SubRegionsChildren", MUI.Error_SubRegionsChildren);
            // Assert

            var newRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);

            Assert.IsNotNull(newRegion);
            Assert.IsNull(newRegion.Deleted);
        }

        [TestMethod]
        public void DeleteRegionWithStreets_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => x.Region != null, GetStreet);
            var regionId = street.Region.Id;

            // Act

            SafeExec(_bll.DeleteRegion, regionId, true, false, "Error_RegionsHasStreet", MUI.Error_RegionsHasStreet);

            // Assert

            var newRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);

            Assert.IsNotNull(newRegion);
            Assert.IsNull(newRegion.Deleted);
        }

        [TestMethod]
        public void DeleteRegionWithPollingStations_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(GetPollingStationWithoutStreets, true);
            var regionId = pollingStation.Region.Id;

            // Act

            SafeExec(_bll.DeleteRegion, regionId, true, false, "Error_RegionsHasPollingStation",
                MUI.Error_RegionsHasPollingStation);

            // Assert

            var newRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);

            Assert.IsNotNull(newRegion);
            Assert.IsNull(newRegion.Deleted);
        }

        [TestMethod]
        public void DeleteRegion_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion, true);
            var regionId = region.Id;

            // Act

            SafeExec(_bll.DeleteRegion, regionId);

            // Assert

            var newRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);

            Assert.IsNotNull(newRegion);
            Assert.IsNotNull(newRegion.Deleted);
        }

        #endregion DeleteRegion

        #region GetStreets

        [TestMethod]
        public void GetStreetsByAccessibleRegion_returns_correct_result()
        {
            // Arrange
            
            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(GetStreet);
            var regionId = street.Region.Id;
            var expectedStreets = GetAllIdsFromDbTable<Street>(x => x.Region.Id == regionId);

            // Act and Assert

            Assert.IsTrue(_bll.IsRegionAccessibleToCurrentUser(regionId));
            ActAndAssertAllPages(_bll.GetStreets, regionId, x => x.Id, expectedStreets, true, false);
        }

        [TestMethod]
        public void GetStreetsByNonAccessibleRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetRegistratorRole();

            // Act & Assert

            ActAndAssertAllPages(_bll.GetStreets, 1L, x => x.Id, new List<long>());
            Assert.IsFalse(_bll.IsRegionAccessibleToCurrentUser(1L));
        }

        #endregion GetStreets

        [TestMethod]
        public void GetCircumscriptions_returns_correct_page()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var expectedCircumscriptions =
                GetAllIdsFromDbTable<Region>(
                    x => (x.Parent != null) && (x.Parent.Parent == null) && (x.Deleted == null));

            // Act and Assert

            ActAndAssertAllPages(_bll.GetCircumscriptions, expectedCircumscriptions);
        }

        [TestMethod]
        public void GetCircumscriptions_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var expectedCircumscriptions =
                GetAllObjectsFromDbTable<Region>(
                    x => (x.Parent != null) && (x.Parent.Parent == null) && (x.Deleted == null));

            // Act

            var circumscriptions = SafeExecFunc(_bll.GetCircumscriptions);

            // Assert

            Assert.IsNotNull(circumscriptions);
            //AssertListsAreEqual(expectedCircumscriptions, circumscriptions.ToList());
        }

        [TestMethod]
        public void GetRegions_returns_correct_page()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            var parentRegionId = region.Parent.Id;

            var expectedRegions =
                GetAllIdsFromDbTable<Region>(x => (x.Parent != null) && (x.Parent.Id == parentRegionId));

            // Act and Assert

            ActAndAssertAllPages(_bll.GetRegions, parentRegionId, x => x.Id, expectedRegions);
        }

        [TestMethod]
        public void GetRegionsGrid_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var expectedRegions = GetAllIdsFromDbTable<Region>();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetRegionsGrid, x => x.Id, expectedRegions);
        }

        [TestMethod]
        public void RemoveOtherPrimaryDocs_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.IsPrimary, GetDocumentType);

            // Act

            SafeExec("RemoveOtherPrimaryDocs");

            // Assert

            var count = GetAllObjectsFromDbTable<DocumentType>(x => x.IsPrimary).Count;
            Assert.AreEqual(0, count);
        }

        #region CreateUpdateStreet

        [TestMethod]
        public void CreateUpdateStreetByZeroId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var streetType = GetFirstObjectFromDbTable(GetStreetType);
            var region = GetFirstObjectFromDbTable(x => x.HasStreets, GetRegion);

            const string name = "name";
            const string description = "description";
            var streetTypeId = streetType.Id;
            var regionId = region.Id;

            // Act

            SafeExec(_bll.CreateUpdateStreet, 0L, name, description, regionId, streetTypeId, (long?) null, (long?) null);

            // Assert

            var street = GetLastCreatedObject<Street>();

            Assert.IsNotNull(street);
            Assert.AreEqual(name, street.Name);
            Assert.AreEqual(description, street.Description);

            Assert.IsNotNull(street.Region);
            Assert.AreEqual(regionId, street.Region.Id);

            Assert.IsNotNull(street.StreetType);
            Assert.AreEqual(streetType.Id, street.StreetType.Id);

            Assert.IsNull(street.RopId);
            Assert.IsNull(street.SaiseId);
        }

        [TestMethod]
        public void CreateUpdateStreetByNonZeroId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var oldStreet = GetFirstObjectFromDbTable(GetStreet);
            var streetType = GetFirstObjectFromDbTable(GetStreetType);
            var region = GetFirstObjectFromDbTable(x => x.HasStreets, GetRegion);

            var id = oldStreet.Id;
            const string name = "name";
            const string description = "description";
            var streetTypeId = streetType.Id;
            var regionId = region.Id;

            // Act

            SafeExec(_bll.CreateUpdateStreet, id, name, description, regionId, streetTypeId, (long?) null, (long?) null);

            // Assert

            var street = GetFirstObjectFromDbTable<Street>(x => x.Id == id);

            Assert.IsNotNull(street);
            Assert.AreEqual(id, street.Id);
            Assert.AreEqual(name, street.Name);
            Assert.AreEqual(description, street.Description);

            Assert.IsNotNull(street.StreetType);
            Assert.AreEqual(streetType.Id, street.StreetType.Id);

            Assert.IsNull(street.RopId);
            Assert.IsNull(street.SaiseId);
        }

        #endregion CreateUpdateStreet

        #region CreateUpdateRegion

        [TestMethod]
        public void CreateUpdateRegionByZeroIdAndNullParent_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(GetRegionType);

            const string name = "name";
            const string description = "description";
            var regionTypeId = regionType.Id;
            const bool hasStreets = true;

            // Act

            SafeExec(_bll.CreateUpdateRegion, 0L, name, description, (long?) null, regionTypeId, hasStreets,
                (long?) null, (long?) null, false, true);
        }

        [TestMethod]
        public void CreateUpdateRegionByZeroIdAndNotNullParent_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(GetRegionType);
            var parentRegion = GetFirstObjectFromDbTable(GetRegion);

            const string name = "name";
            const string description = "description";
            long? parentId = parentRegion.Id;
            var regionTypeId = regionType.Id;
            const bool hasStreets = true;

            // Act

            SafeExec(_bll.CreateUpdateRegion, 0L, name, description, parentId, regionTypeId, hasStreets, (long?) null,
                (long?) null);

            // Assert

            var region = GetLastCreatedObject<Region>();

            Assert.IsNotNull(region);
            Assert.AreEqual(name, region.Name);
            Assert.AreEqual(description, region.Description);

            Assert.IsNotNull(region.Parent);
            Assert.AreEqual(parentId, region.Parent.Id);

            Assert.IsNotNull(region.RegionType);
            Assert.AreEqual(regionTypeId, region.RegionType.Id);

            Assert.AreEqual(hasStreets, region.HasStreets);
            Assert.IsNull(region.SaiseId);
            Assert.IsNull(region.RegistruId);
        }

        [TestMethod]
        public void CreateUpdateRegionByNonZeroIdAndNonValidParent_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(GetRegionType);
            var parentRegion = GetFirstObjectFromDbTable(x => x.RegionType.Rank > 0, GetRegionWithRank1);

            var region = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 0, GetRegionWithRank0);

            var id = region.Id;
            const string name = "name";
            const string description = "description";
            long? parentId = parentRegion.Id;
            var regionTypeId = regionType.Id;

            // Act

            SafeExec(_bll.CreateUpdateRegion, id, name, description, parentId, regionTypeId, true, (long?) null,
                (long?) null, true);
        }

        [TestMethod]
        public void CreateUpdateRegionByNonZeroIdAndNonValidRegionType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(x => x.Rank == 0, GetRegionType);
            var parentRegion = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 1, GetRegionWithRank1);
            var region = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 2, GetRegionWithRank2);

            var id = region.Id;
            const string name = "name";
            const string description = "description";
            long? parentId = parentRegion.Id;
            var regionTypeId = regionType.Id;

            // Act

            SafeExec(_bll.CreateUpdateRegion, id, name, description, parentId, regionTypeId, true, (long?) null,
                (long?) null, true);
        }

        [TestMethod]
        public void CreateUpdateRegionByNonZeroIdAndValidParentAndRegionType_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(x => x.Rank == 1, GetRegionTypeWithRank1);
            var parentRegion = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 1, GetRegionWithRank1);
            var region = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 2, GetRegionWithRank2);

            var id = region.Id;
            const string name = "name";
            const string description = "description";
            long? parentId = parentRegion.Id;
            var regionTypeId = regionType.Id;

            // Act

            SafeExec(_bll.CreateUpdateRegion, id, name, description, parentId, regionTypeId, true, (long?) null,
                (long?) null);

            // Assert

            var newRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == id);

            Assert.IsNotNull(newRegion);
            Assert.AreEqual(name, newRegion.Name);
            Assert.AreEqual(description, newRegion.Description);

            Assert.IsNotNull(newRegion.Parent);
            Assert.AreEqual(parentId, newRegion.Parent.Id);

            Assert.IsNotNull(newRegion.RegionType);
            Assert.AreEqual(regionTypeId, newRegion.RegionType.Id);

            Assert.IsTrue(newRegion.HasStreets);
            Assert.IsNull(newRegion.SaiseId);
            Assert.IsNull(newRegion.RegistruId);
        }

        [TestMethod]
        public void CreateUpdateRegionByNonZeroIdAndNullParent_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var regionType = GetFirstObjectFromDbTable(x => x.Rank == 1, GetRegionTypeWithRank1);
            var region = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 2, GetRegionWithRank2);

            var id = region.Id;
            const string name = "name";
            const string description = "description";
            var regionTypeId = regionType.Id;
            const bool hasStreets = true;

            // Act

            SafeExec(_bll.CreateUpdateRegion, id, name, description, (long?) null, regionTypeId, hasStreets,
                (long?) null, (long?) null, false, true);
        }

        #endregion CreateUpdateRegion

        #region UpdateAdministrativeInfo

        [TestMethod]
        public void UpdateAdministrativeInfoByZeroId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var managerType = GetFirstObjectFromDbTable(GetManagerType);
            var region = GetFirstObjectFromDbTable(GetRegion);

            const string name = "name";
            const string surname = "surname";
            var regionId = region.Id;
            var managerTypeId = managerType.Id;

            // Act

            SafeExec(_bll.UpdateAdministrativeInfo, 0L, name, surname, regionId, managerTypeId);

            // Assert

            var publicAdministration = GetLastCreatedObject<PublicAdministration>();

            Assert.IsNotNull(publicAdministration);
            Assert.AreEqual(name, publicAdministration.Name);
            Assert.AreEqual(surname, publicAdministration.Surname);

            Assert.IsNotNull(publicAdministration.Region);
            Assert.AreEqual(regionId, publicAdministration.Region.Id);

            Assert.IsNotNull(publicAdministration.ManagerType);
            Assert.AreEqual(managerTypeId, publicAdministration.ManagerType.Id);
        }

        [TestMethod]
        public void UpdateAdministrativeInfoByNonZeroId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var managerType = GetFirstObjectFromDbTable(GetManagerType);
            var region = GetFirstObjectFromDbTable(GetRegion);

            var oldPublicAdministration = GetFirstObjectFromDbTable(GetPublicAdministration);
            var id = oldPublicAdministration.Id;

            const string name = "name";
            const string surname = "surname";
            var regionId = region.Id;
            var managerTypeId = managerType.Id;

            // Act

            SafeExec(_bll.UpdateAdministrativeInfo, id, name, surname, regionId, managerTypeId);

            // Assert

            var publicAdministration = GetFirstObjectFromDbTable<PublicAdministration>(x => x.Id == id);

            Assert.IsNotNull(publicAdministration);
            Assert.AreEqual(id, publicAdministration.Id);
            Assert.AreEqual(name, publicAdministration.Name);
            Assert.AreEqual(surname, publicAdministration.Surname);

            Assert.IsNotNull(publicAdministration.Region);
            Assert.AreEqual(regionId, publicAdministration.Region.Id);

            Assert.IsNotNull(publicAdministration.ManagerType);
            Assert.AreEqual(managerTypeId, publicAdministration.ManagerType.Id);
        }

        #endregion UpdateAdministrativeInfo

        #region IsUnique

        [TestMethod]
        public void RegionIsUnique_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            var id = region.Id;
            var name = region.Name;
            var parentId = region.Parent.Id;
            var regionTypeId = region.RegionType.Id;

            // Act

            var result = SafeExecFunc(_bll.IsUnique, id, name, parentId, regionTypeId);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegionIsNotUnique_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            const long id = 0;
            var name = region.Name;
            var parentId = region.Parent.Id;
            var regionTypeId = region.RegionType.Id;

            // Act

            var result = SafeExecFunc(_bll.IsUnique, id, name, parentId, regionTypeId);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StreetIsUnique_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.StreetType != null), GetStreet);
            var id = street.Id;
            var regionId = street.Region.Id;
            var name = street.Name;
            var streetTypeId = street.StreetType.Id;

            // Act

            var result = SafeExecFunc(_bll.IsUnique, id, regionId, name, streetTypeId);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StreetIsNotUnique_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.StreetType != null), GetStreet);
            const long id = 0;
            var regionId = street.Region.Id;
            var name = street.Name;
            var streetTypeId = street.StreetType.Id;

            // Act

            var result = SafeExecFunc(_bll.IsUnique, id, regionId, name, streetTypeId);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsUniqueRegionTypeById_has_correct_logic()
        {
            IsUniqueTest(GetRegionType);
        }

        [TestMethod]
        public void IsUniqueStreetTypeById_has_correct_logic()
        {
            IsUniqueTest(GetStreetType);
        }

        [TestMethod]
        public void IsUniquePersonIsUniqueressTypeById_has_correct_logic()
        {
            IsUniqueTest(GetPersonAddressType);
        }

        [TestMethod]
        public void IsUniqueManagerTypeById_has_correct_logic()
        {
            IsUniqueTest(GetManagerType);
        }

        [TestMethod]
        public void IsUniqueGenderById_has_correct_logic()
        {
            IsUniqueTest(GetGender);
        }

        [TestMethod]
        public void IsUniqueDocumentTypeById_has_correct_logic()
        {
            IsUniqueTest(GetDocumentType);
        }

        [TestMethod]
        public void IsUniqueElectionTypeById_has_correct_logic()
        {
            IsUniqueTest(GetElectionType);
        }

        [TestMethod]
        public void IsUniquePersonStatusTypeById_has_correct_logic()
        {
            IsUniqueTest(GetPersonStatusType);
        }

        [TestMethod]
        public void IsNotUniqueRegionTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetRegionType);
        }

        [TestMethod]
        public void IsNotUniqueStreetTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetStreetType);
        }

        [TestMethod]
        public void IsNotUniquePersonIsNotUniqueressTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetPersonAddressType);
        }

        [TestMethod]
        public void IsNotUniqueManagerTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetManagerType);
        }

        [TestMethod]
        public void IsNotUniqueGenderById_has_correct_logic()
        {
            IsNotUniqueTest(GetGender);
        }

        [TestMethod]
        public void IsNotUniqueDocumentTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetDocumentType);
        }

        [TestMethod]
        public void IsNotUniqueElectionTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetElectionType);
        }

        [TestMethod]
        public void IsNotUniquePersonStatusTypeById_has_correct_logic()
        {
            IsNotUniqueTest(GetPersonStatusType);
        }

        public void IsUniqueTest<T>(Func<T> newObjBuilder) where T : Lookup, new()
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(newObjBuilder);
            long? id = entity.Id;
            var name = entity.Name;

            // Act

            var result = SafeExecFunc(_bll.IsUnique<T>, id, name);

            // Assert

            Assert.IsTrue(result);
        }

        public void IsNotUniqueTest<T>(Func<T> newObjBuilder) where T : Lookup, new()
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(newObjBuilder);
            long? id = 0;
            var name = entity.Name;

            // Act

            var result = SafeExecFunc(_bll.IsUnique<T>, id, name);

            // Assert

            Assert.IsFalse(result);
        }

        #endregion IsUnique

        [TestMethod]
        public void GetRegionsByParentIdAndFilter_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            var parentId = region.Parent.Id;
            const string regionNameFilter = "a";

            var expectedRegions =
                GetAllObjectsFromDbTable<Region>(
                    x =>
                        (x.Parent != null) && (x.Parent.Id == parentId) &&
                        x.Name.ToLower().Contains(regionNameFilter.ToLower()));

            // Act

            var regions = SafeExecFunc(_bll.GetRegionsByParentIdAndFilter, parentId, regionNameFilter);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetRegionsOfLevel1ByFilter_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            const string regionNameFilter = "a";
            var regionsOfLevel0 = GetAllObjectsFromDbTable<Region>(x => x.Parent == null);

            var expectedRegions = new List<Region>();

            regionsOfLevel0.ForEach(x => expectedRegions.AddRange(
                GetAllObjectsFromDbTable<Region>(
                    y =>
                        (y.Parent != null) && (y.Parent.Id == x.Id) &&
                        y.Name.ToLower().Contains(regionNameFilter.ToLower()))));

            // Act

            var regions = SafeExecFunc(_bll.GetRegionsOfLevel1ByFilter, regionNameFilter);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        #region Hierarhically

        [TestMethod]
        public void GetPollingStationsHierarhicallyByRegion_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.Parent != null) && (x.Parent.Parent != null),
                GetRegionWithParent);
            var regionId = region.Id;

            var expectedPollingStations = GetExpectedPollingStationsHierarhicallyByRegion(regionId);

            // Act

            var pollingStations = SafeExecFunc(_bll.GetPollingStationsHierarhicallyByRegion, regionId);

            // Assert

            Assert.IsNotNull(pollingStations);
            AssertListsAreEqual(expectedPollingStations, pollingStations.ToList());
        }

        [TestMethod]
        public void GetStreetsHierarhicallyByRegionAndFilter_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.Parent != null) && (x.Parent.Parent != null),
                GetRegionWithParent);
            var regionId = region.Id;
            const string streetNameFilter = "a";

            var expectedStreets = GetExpectedStreetsHierarhicallyByRegionAndFilter(regionId, streetNameFilter);

            // Act

            var streets = SafeExecFunc(_bll.GetStreetsHierarhicallyByRegionAndFilter, regionId, streetNameFilter);

            // Assert

            Assert.IsNotNull(streets);
            AssertListsAreEqual(expectedStreets, streets.ToList());
        }

        private List<PollingStation> GetExpectedPollingStationsHierarhicallyByRegion(long regionId)
        {
            var pollingStations =
                GetAllObjectsFromDbTable<PollingStation>(
                    x => (x.Region != null) && (x.Region.Id == regionId) && (x.Deleted == null));

            var childRegions =
                GetAllObjectsFromDbTable<Region>(x => (x.Parent != null) && (x.Parent.Id == regionId));

            var childPollingStations = new List<PollingStation>();
            childRegions.ForEach(
                x => childPollingStations.AddRange(GetExpectedPollingStationsHierarhicallyByRegion(x.Id)));
            pollingStations.AddRange(childPollingStations);

            return pollingStations;
        }

        private List<Street> GetExpectedStreetsHierarhicallyByRegionAndFilter(long regionId, string streetNameFilter)
        {
            var streets =
                GetAllObjectsFromDbTable<Street>(
                    x =>
                        (x.Region != null) && (x.Region.Id == regionId) &&
                        x.Name.ToLower().Contains(streetNameFilter.ToLower()));

            var childRegions =
                GetAllObjectsFromDbTable<Region>(x => (x.Parent != null) && (x.Parent.Id == regionId));

            var childStreets = new List<Street>();
            childRegions.ForEach(
                x => childStreets.AddRange(GetExpectedStreetsHierarhicallyByRegionAndFilter(x.Id, streetNameFilter)));
            streets.AddRange(childStreets);

            return streets;
        }

        #endregion Hierarhically

        #region VerificationRegion

        [TestMethod]
        public void VerificationRegionWithoutStreets_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets);
            var id = region.Id;

            // Act

            SafeExec(_bll.VerificationRegion, id, true, false, "Lookups_RegionNotAcceptStreets",
                MUI.Lookups_RegionNotAcceptStreets);
        }

        [TestMethod]
        public void VerificationDeletedRegion_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Deleted != null, GetDeletedRegionWithStreets);
            var id = region.Id;

            // Act

            SafeExec(_bll.VerificationRegion, id, true, false, "Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
        }

        [TestMethod]
        public void VerificationValidRegion_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted == null), GetRegion);
            var id = region.Id;

            // Act

            SafeExec(_bll.VerificationRegion, id);
        }

        #endregion VerificationRegion

        #region UpdateCircumscription

        [TestMethod]
        public void UpdateCircumscriptionWithNullCircumscriptionNumber_has_correct_logic()
        {
            UpdateCircumscriptionTest(null);
        }

        [TestMethod]
        public void UpdateCircumscriptionWithNotNullCircumscriptionNumber_has_correct_logic()
        {
            UpdateCircumscriptionTest(1);
        }

        private void UpdateCircumscriptionTest(int? circumscriptionNumber)
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var id = region.Id;

            // Act

            SafeExec(_bll.UpdateCircumscription, id, circumscriptionNumber);

            // Assert

            var newRegion = GetFirstObjectFromDbTable<Region>(x => x.Id == id);

            Assert.IsNotNull(newRegion);
            Assert.AreEqual(id, newRegion.Id);
            Assert.AreEqual(circumscriptionNumber, newRegion.Circumscription);
        }

        #endregion UpdateCircumscription

        #region UniqueValidationCircumscription

        [TestMethod]
        public void UniqueValidationCircumscription_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.Parent != null) && (x.Parent.Parent == null),
                GetRegionWithParent);
            var id = region.Id;
            var circumscription = region.Circumscription;

            // Act

            var result = SafeExecFunc(_bll.UniqueValidationCircumscription, id, circumscription);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NonUniqueValidationCircumscription_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.Parent != null) && (x.Parent.Parent == null),
                GetRegionWithParent);
            const long id = 0;
            var circumscription = region.Circumscription;

            // Act

            var result = SafeExecFunc(_bll.UniqueValidationCircumscription, id, circumscription);

            // Assert

            Assert.IsFalse(result);
        }

        #endregion UniqueValidationCircumscription

        #region DeleteStreet

        [TestMethod]
        public void DeleteStreetWithAddresses_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Deleted == null), GetAddress);
            var streetId = address.Street.Id;

            // Act

            SafeExec(_bll.DeleteStreet, streetId, true, false, "Street_NotPermittedDeleting",
                MUI.Street_NotPermittedDeleting);


            // Assert

            var street = GetFirstObjectFromDbTable<Street>(x => x.Id == streetId);

            Assert.IsNotNull(street);
            Assert.IsNull(street.Deleted);
        }

        [TestMethod]
        public void DeleteStreetWithoutAddresses_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var oldStreet = GetFirstObjectFromDbTable(GetStreet, true);
            var streetId = oldStreet.Id;

            // Act

            SafeExec(_bll.DeleteStreet, streetId);

            // Assert

            var street = GetLastDeletedObject<Street>();

            Assert.IsNotNull(street);
            Assert.AreEqual(streetId, street.Id);
            Assert.IsNotNull(street.Deleted);
        }

        #endregion DeleteStreet

        #region VerificationIfHasReference

        #region VerificationIfManagerTypeHasReference

        [TestMethod]
        public void
            VerificationIfManagerTypeHasReferenceByExistingPublicAdministrationWithGivenManagerType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var publicAdministration = GetFirstObjectFromDbTable(x => (x.ManagerType != null) && (x.Deleted == null),
                GetPublicAdministration);
            var managerTypeId = publicAdministration.ManagerType.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfManagerTypeHasReference, managerTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, publicAdministration.ManagerType.GetObjectType(),
                    publicAdministration.GetObjectType()));
        }

        [TestMethod]
        public void
            VerificationIfManagerTypeHasReferenceByNonExistingPublicAdministrationWithGivenManagerType_has_correct_logic
            ()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfManagerTypeHasReference, -1L);
        }

        #endregion VerificationIfManagerTypeHasReference

        #region VerificationIfPersonStatusHasReference

        [TestMethod]
        public void VerificationIfPersonStatusHasReferenceByExistingPersonStatusWithGivenStatusType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var personStatus = GetFirstObjectFromDbTable(x => (x.StatusType != null) && (x.Deleted == null),
                GetPersonStatus);
            var statusTypeId = personStatus.StatusType.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfPersonStatusHasReference, statusTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, personStatus.StatusType.GetObjectType(),
                    personStatus.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfPersonStatusHasReferenceByNonExistingPersonStatusWithGivenStatusType_has_correct_logic
            ()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfPersonStatusHasReference, -1L);
        }

        #endregion VerificationIfPersonStatusHasReference

        #region VerificationIfGenderHasReference

        [TestMethod]
        public void VerificationIfGenderHasReferenceByExistingAdditionalUserInfoWithGivenGender_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var additionalUserInfo = GetFirstObjectFromDbTable(x => (x.Gender != null) && (x.Deleted == null),
                GetAdditionalUserInfo);
            var genderId = additionalUserInfo.Gender.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfGenderHasReference, genderId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, additionalUserInfo.Gender.GetObjectType(),
                    additionalUserInfo.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfGenderHasReferenceByExistingPersonWithGivenGender_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var person = GetFirstObjectFromDbTable(x => (x.Gender != null) && (x.Deleted == null), GetPerson);
            var genderId = person.Gender.Id;

            var additionalUserInfos =
                GetAllObjectsFromDbTable<AdditionalUserInfo>(x => (x.Gender.Id == genderId) && (x.Deleted == null));
            additionalUserInfos.ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfGenderHasReference, genderId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, person.Gender.GetObjectType(), person.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfGenderHasReferenceByNonExistingReference_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfGenderHasReference, -1L);
        }

        #endregion VerificationIfGenderHasReference

        #region VerificationIfDocTypeHasReference

        [TestMethod]
        public void VerificationIfDocTypeHasReferenceByExistingPersonWithGivenDocType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var person =
                GetFirstObjectFromDbTable(
                    x => (x.Document != null) && (x.Document.Type != null) && (x.Deleted == null), GetPerson);
            var docTypeId = person.Document.Type.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfDocTypeHasReference, docTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, person.Document.Type.GetObjectType(), person.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfDocTypeHasReferenceByNonExistingPersonWithGivenDocType_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfDocTypeHasReference, -1L);
        }

        #endregion VerificationIfDocTypeHasReference

        #region VerificationIfElectionTypeHasReference

        [TestMethod]
        public void VerificationIfElectionTypeHasReferenceByExistingElectionWithGivenElectionType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var election = GetFirstObjectFromDbTable(x => (x.ElectionType != null) && (x.Deleted == null), GetElection);
            var electionTypeId = election.ElectionType.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfElectionTypeHasReference, electionTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, election.ElectionType.GetObjectType(), election.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfElectionTypeHasReferenceByNonExistingElectionWithGivenElectionType_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfElectionTypeHasReference, -1L);
        }

        #endregion VerificationIfElectionTypeHasReference

        #region VerificationIfPersonAddressTypeHasReference

        [TestMethod]
        public void
            VerificationIfPersonAddressTypeHasReferenceByExistingPersonAddressWithGivenPersonAddressType_throws_an_exception
            ()
        {
            // Arrange

            SetAdministratorRole();

            var personAddress = GetFirstObjectFromDbTable(x => (x.PersonAddressType != null) && (x.Deleted == null),
                GetPersonAddress);
            var personAddressTypeId = personAddress.PersonAddressType.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfPersonAddressTypeHasReference, personAddressTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, personAddress.PersonAddressType.GetObjectType(),
                    personAddress.GetObjectType()));
        }

        [TestMethod]
        public void
            VerificationIfPersonAddressTypeHasReferenceByNonExistingPersonAddressWithGivenPersonAddressType_has_correct_logic
            ()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfPersonAddressTypeHasReference, -1L);
        }

        #endregion VerificationIfPersonAddressTypeHasReference

        #region VerificationIfRegionTypeHasReference

        [TestMethod]
        public void VerificationIfRegionTypeHasReferenceByExistingRegionWithGivenRegionType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.RegionType != null) && (x.Deleted == null), GetRegion);
            var regionTypeId = region.RegionType.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfRegionTypeHasReference, regionTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, region.RegionType.GetObjectType(), region.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfRegionTypeHasReferenceByNonExistingRegionWithGivenRegionType_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfRegionTypeHasReference, -1L);
        }

        #endregion VerificationIfRegionTypeHasReference

        #region VerificationIfRegionHasReference

        [TestMethod]
        public void VerificationIfRegionHasReferenceByExistingRsaUserWithGivenRegion_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var rsaUser = GetFirstObjectFromDbTable(x => x.Region != null, GetRsaUser);
            var regionId = rsaUser.Region.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfRegionHasReference, regionId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, rsaUser.Region.GetObjectType(), rsaUser.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfRegionHasReferenceByExistingStreetWithGivenRegion_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.Deleted == null), GetStreet);
            var regionId = street.Region.Id;

            var rsaUsers = GetAllObjectsFromDbTable<RsaUser>(x => x.Region.Id == regionId);
            rsaUsers.ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfRegionHasReference, regionId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, street.Region.GetObjectType(), street.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfRegionHasReferenceByExistingPollingStationWithGivenRegion_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var pollingStation = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.Deleted == null),
                GetPollingStation);
            var regionId = pollingStation.Region.Id;

            var rsaUsers = GetAllObjectsFromDbTable<RsaUser>(x => x.Region.Id == regionId);
            rsaUsers.ForEach(Repository.Delete);

            var streets = GetAllObjectsFromDbTable<Street>(x => (x.Region.Id == regionId) && (x.Deleted == null));
            streets.ForEach(Repository.Delete);


            // Act & Assert

            SafeExec(_bll.VerificationIfRegionHasReference, regionId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, pollingStation.Region.GetObjectType(),
                    pollingStation.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfRegionHasReferenceByExistingPublicAdministrationWithGivenRegion_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var publicAdministration = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.Deleted == null),
                GetPublicAdministration);
            var regionId = publicAdministration.Region.Id;

            var rsaUsers = GetAllObjectsFromDbTable<RsaUser>(x => x.Region.Id == regionId);
            rsaUsers.ForEach(Repository.Delete);

            var streets = GetAllObjectsFromDbTable<Street>(x => (x.Region.Id == regionId) && (x.Deleted == null));
            streets.ForEach(Repository.Delete);

            var pollingStations =
                GetAllObjectsFromDbTable<PollingStation>(x => (x.Region.Id == regionId) && (x.Deleted == null));
            pollingStations.ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfRegionHasReference, regionId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, publicAdministration.Region.GetObjectType(),
                    publicAdministration.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfRegionHasReferenceByNonExistingReference_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfRegionHasReference, -1L);
        }

        #endregion VerificationIfRegionHasReference

        #region VerificationIfStreetTypeHasReference

        [TestMethod]
        public void VerificationIfStreetTypeHasReferenceByExistingStreetWithGivenStreetType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var street = GetFirstObjectFromDbTable(x => (x.StreetType != null) && (x.Deleted == null), GetStreet);
            var streetTypeId = street.StreetType.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfStreetTypeHasReference, streetTypeId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, street.StreetType.GetObjectType(), street.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfStreetTypeHasReferenceByNonExistingStreetWithGivenStreetType_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfStreetTypeHasReference, -1L);
        }

        #endregion VerificationIfStreetTypeHasReference

        #region VerificationIfStreetHasReference

        [TestMethod]
        public void VerificationIfStreetHasReferenceByExistingStreetWithGivenStreetType_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.Deleted == null), GetAddress);
            var streetId = address.Street.Id;

            // Act & Assert

            SafeExec(_bll.VerificationIfStreetHasReference, streetId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, address.Street.GetObjectType(), address.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfStreetHasReferenceByNonExistingStreetWithGivenStreetType_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            // Act & Assert

            SafeExec(_bll.VerificationIfStreetHasReference, -1L);
        }

        #endregion VerificationIfStreetHasReference

        #endregion VerificationIfHasReference

        [TestMethod]
        public void GetAvailableRegions_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var expResult =
                GetAllIdsFromDbTable<RegionWithFullyQualifiedName>(
                    x => (x.Region != null) && (x.Region.Deleted == null));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetAvailableRegions, expResult);
        }

        [TestMethod]
        public void GetLinkedRegionsByRegionIdThatNotExists_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var expResult = new List<LinkedRegionsFullName>();

            // Act

            var result = SafeExecFunc(_bll.GetLinkedRegionsByRegionId, -1L);

            // Assert

            Assert.IsNotNull(result);
            AssertListsAreEqual(expResult, result.ToList());
        }

        [TestMethod]
        public void GetLinkedRegionsByRegionIdThatExists_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var linkedRegion = GetFirstObjectFromDbTable(x => (x.Deleted == null) && x.Regions.Any(), GetLinkedRegion);
            var region = linkedRegion.Regions.FirstOrDefault();
            var regionId = (region != null) ? region.Id : -1;

            var expLinkedRegion =
                GetFirstObjectFromDbTable<LinkedRegion>(x => (x.Deleted == null) && x.Regions.Any(y => y.Id == regionId));
            var expResult = GetAllObjectsFromDbTable<LinkedRegionsFullName>(x => x.LinkedRegion.Id == expLinkedRegion.Id);

            // Act

            var result = SafeExecFunc(_bll.GetLinkedRegionsByRegionId, regionId);

            // Assert

            Assert.IsNotNull(result);
            AssertListsAreEqual(expResult, result.ToList());
        }

        [TestMethod]
        public void SaveLinkedRegionsThatExistsByMore1LinkedRegions_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var linkedRegion = GetFirstObjectFromDbTable(x => (x.Deleted == null) && x.Regions.Any(), GetLinkedRegion);
            var region = linkedRegion.Regions.FirstOrDefault();
            var regionId = (region != null) ? region.Id : -1;

            var region1 = GetFirstObjectFromDbTable(x => x.Id != regionId, GetRegionWithParent);
            var regionId1 = region1.Id;

            var region2 = GetFirstObjectFromDbTable(x => (x.Id != regionId) && (x.Id != regionId1),
                GetRegionWithRegistruId);
            var regionId2 = region2.Id;

            var linkedRegionIds = new[] {regionId1, regionId2};

            // Act

            SafeExec(_bll.SaveLinkedRegions, regionId, linkedRegionIds);

            // Assert

            var newLinkedRegion = GetLastCreatedObject<LinkedRegion>();

            Assert.IsNotNull(newLinkedRegion);
            Assert.IsNotNull(newLinkedRegion.Regions);
            AssertObjectListsAreEqual(linkedRegionIds.ToList(), newLinkedRegion.Regions.Select(x => x.Id).ToList());

            var checkLinkedRegion = GetFirstObjectFromDbTable<LinkedRegion>(x => (x.Deleted == null) && x.Regions.Select(y => y.Id).Contains(regionId));
            Assert.IsNull(checkLinkedRegion);
        }

        [TestMethod]
        public void SaveLinkedRegionsThatExistsByLessThan2LinkedRegions_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var linkedRegion = GetFirstObjectFromDbTable(x => (x.Deleted == null) && x.Regions.Any(), GetLinkedRegion);
            var region = linkedRegion.Regions.FirstOrDefault();
            var regionId = (region != null) ? region.Id : -1;

            var region1 = GetFirstObjectFromDbTable(x => x.Id != regionId, GetRegionWithParent);
            var regionId1 = region1.Id;
            
            var linkedRegionIds = new[] { regionId1};

            // Act

            SafeExec(_bll.SaveLinkedRegions, regionId, linkedRegionIds);

            // Assert

            var checkLinkedRegion = GetFirstObjectFromDbTable<LinkedRegion>(x => (x.Deleted == null) && x.Regions.Select(y => y.Id).Contains(regionId));
            Assert.IsNull(checkLinkedRegion);
        }

        [TestMethod]
        public void SaveLinkedRegionsThatNotExistsByMore1LinkedRegions_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var region1 = GetFirstObjectFromDbTable(GetRegionWithParent);
            var regionId1 = region1.Id;

            var region2 = GetFirstObjectFromDbTable(x => x.Id != regionId1, GetRegionWithRegistruId);
            var regionId2 = region2.Id;

            var linkedRegionIds = new[] { regionId1, regionId2 };

            // Act

            SafeExec(_bll.SaveLinkedRegions, -1L, linkedRegionIds);

            // Assert

            var newLinkedRegion = GetLastCreatedObject<LinkedRegion>();

            Assert.IsNotNull(newLinkedRegion);
            Assert.IsNotNull(newLinkedRegion.Regions);
            AssertObjectListsAreEqual(linkedRegionIds.ToList(), newLinkedRegion.Regions.Select(x => x.Id).ToList());
        }

        [TestMethod]
        public void SaveLinkedRegionsThatNotExistsByLessThan2LinkedRegions_has_correct_logic()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var region1 = GetFirstObjectFromDbTable(GetRegionWithParent);
            var regionId1 = region1.Id;

            var linkedRegionIds = new[] { regionId1};

            // Act

            SafeExec(_bll.SaveLinkedRegions, -1L, linkedRegionIds);
        }

        [TestMethod]
        public void GetFullyQualifiedName_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, LookupBll>();

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var regionId = region.Id;

            var expResult = GetFirstObjectFromDbTable<RegionWithFullyQualifiedName>(x => x.Region.Id == regionId);

            //  Act

            var result = SafeExecFunc(_bll.GetFullyQualifiedName, regionId);

            // Assert

            Assert.AreSame(expResult, result);

        }

        [TestMethod]
        public void Read_ConfiguratinSetting()
        {
            //arrange

            //act

            //asert
        }
    }
}