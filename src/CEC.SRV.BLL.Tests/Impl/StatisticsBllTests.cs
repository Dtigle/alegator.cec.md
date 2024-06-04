using System;
using System.Linq;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using NHibernate.Linq;
using CEC.SRV.Domain.Importer;
using CEC.SRV.BLL.Repositories;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class StatisticsBllTests : BaseBllTests
    {
        private StatisticsBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<StatisticsBll>();
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeople,
                GetDbTableCount<Person>(x => x.Deleted == null));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeople,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x)));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithoutDeadsByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            var deadStatusType =
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Decedat"), GetPersonDeadStatusType, 2L);
            var deadStatusTypeId = deadStatusType.Id;

            GetFirstObjectFromDbTable(
                x => x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (y.StatusType.Id == deadStatusTypeId)) != null,
                GetDeadPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithoutDeads,
                GetDbTableCount<Person>(
                    x =>
                        (x.Deleted == null) &&
                        (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && y.StatusType.Id != deadStatusTypeId) !=
                         null)));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithoutDeadsByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetDeadPerson);

            var deadStatusType =
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Decedat"), GetPersonDeadStatusType);
            var deadStatusTypeId = deadStatusType.Id;

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithoutDeads,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && y.StatusType.Id != deadStatusTypeId) != null) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x)));
        }

        [TestMethod]
        public void GetTotalNumberOfVotersByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfVoters,
                GetDbTableCount<Person>(
                    x =>
                        (x.Deleted == null) &&
                        (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null)));
        }

        [TestMethod]
        public void GetTotalNumberOfVotersByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfVoters,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x)));
        }

        [TestMethod]
        public void GetTotalNumberOfStayStatementDeclarationsByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetStayStatement);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfStayStatementDeclarations,
                GetDbTableCount<StayStatement>(x => x.Deleted == null));
        }

        [TestMethod]
        public void GetTotalNumberOfStayStatementDeclarationsByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetStayStatement);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfStayStatementDeclarations,
                GetDbTableCount<StayStatement>(x =>
                    (x.Deleted == null) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x.Person)));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByGenderByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            Enum.GetValues(typeof (GenderTypes)).Cast<GenderTypes>().ForEach(gender =>
                ActAndAssertLongValue(
                    _bll.GetTotalNumberOfPeopleByGender,
                    gender,
                    GetDbTableCount<Person>(x =>
                        (x.Deleted == null) &&
                        (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                        (x.Gender != null) &&
                        (x.Gender.Id == (long) gender)
                        )));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByGenderByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            Enum.GetValues(typeof (GenderTypes)).Cast<GenderTypes>().ForEach(gender =>
                ActAndAssertLongValue(
                    _bll.GetTotalNumberOfPeopleByGender,
                    gender,
                    GetDbTableCount<Person>(x =>
                        (x.Deleted == null) &&
                        (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                        (x.Gender != null) &&
                        (x.Gender.Id == (long) gender) &&
                        EligiblePersonRegionIsAccessibleForCurrentUser(x)
                        )));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDoBMissingByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithDoBMissing,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    (x.DateOfBirth == null)
                    ));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDoBMissingByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithDoBMissing,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) && (x.PersonStatuses != null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    (x.DateOfBirth == null) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x)
                    ));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDocMissingByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithDocMissing,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    (x.Document == null)
                    ));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDocMissingByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithDocMissing,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    (x.Document == null) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x)
                    ));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDocExpiredByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithDocExpired,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    (x.Document != null) &&
                    (x.Document.ValidBy != null) &&
                    (x.Document.ValidBy < DateTime.UtcNow)
                    ));
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDocExpiredByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(
                _bll.GetTotalNumberOfPeopleWithDocExpired,
                GetDbTableCount<Person>(x =>
                    (x.Deleted == null) &&
                    (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                    (x.Document != null) &&
                    (x.Document.ValidBy != null) &&
                    (x.Document.ValidBy < DateTime.UtcNow) &&
                    EligiblePersonRegionIsAccessibleForCurrentUser(x)
                    ));
        }

        [TestMethod]
        public void GetNumberOfPeopleForAgeIntervalsByAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            var ageLimits = new int[] {0, 18, 41, 66, 76, 91, 1000};

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            Enumerable.Range(0, 6).ForEach(ageInterval =>
                ActAndAssertLongValue(
                    _bll.GetNumberOfPeopleForAgeIntervals,
                    ageInterval,
                    GetDbTableCount<Person>(x =>
                        (x.Deleted == null) &&
                        (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                        (x.DateOfBirth > DateTime.Now.Date.AddYears(-ageLimits[ageInterval + 1])) &&
                        (x.DateOfBirth <= DateTime.Now.Date.AddYears(-ageLimits[ageInterval]))
                        )));
        }

        [TestMethod]
        public void GetNumberOfPeopleForAgeIntervalsByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            var ageLimits = new int[] {0, 18, 41, 66, 76, 91, 1000};

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            Enumerable.Range(0, 6).ForEach(ageInterval =>
                ActAndAssertLongValue(
                    _bll.GetNumberOfPeopleForAgeIntervals,
                    ageInterval,
                    GetDbTableCount<Person>(x =>
                        (x.Deleted == null) &&
                        (x.PersonStatuses.FirstOrDefault(y => y.IsCurrent && (!y.StatusType.IsExcludable)) != null) &&
                        (x.DateOfBirth > DateTime.Now.Date.AddYears(-ageLimits[ageInterval + 1])) &&
                        (x.DateOfBirth <= DateTime.Now.Date.AddYears(-ageLimits[ageInterval])) &&
                        EligiblePersonRegionIsAccessibleForCurrentUser(x)
                        )));
        }

        [TestMethod]
        public void GetNewVotersByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetNewVoters, 0);
        }

        [TestMethod]
        public void GetNewDeadPeopleByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetNewDeadPeople, 0);
        }

        [TestMethod]
        public void GetPersonalDataChangesByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetPersonalDataChanges, 0);
        }

        [TestMethod]
        public void GetAddressesChangesByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetAddressesChanges, 0);
        }

        [TestMethod]
        public void GetImportSuccessfulByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetImportSuccessful, 0);
        }

        [TestMethod]
        public void GetImportFailedByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetImportFailed, 0);
        }

        [TestMethod]
        public void GetNewVotersByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetNewVoters, 0);
        }

        [TestMethod]
        public void GetNewDeadPeopleByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetNewDeadPeople, 0);
        }

        [TestMethod]
        public void GetPersonalDataChangesByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetPersonalDataChanges, 0);
        }

        [TestMethod]
        public void GetAddressesChangesByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetAddressesChanges, 0);
        }

        [TestMethod]
        public void GetImportSuccessfulByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetImportSuccessful, 0);
        }

        [TestMethod]
        public void GetImportFailedByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            GetFirstObjectFromDbTable(GetPerson);

            // Act and Assert

            ActAndAssertLongValue(_bll.GetImportFailed, 0);
        }

        [TestMethod]
        public void GetStatisticsForPollingStationByAdmin_returns_correct_result()
        {
            // Arrange
            
            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            var statistics = GetAllIdsFromDbTable<PollingStationStatistics>();

            // Act and Assert

            ActAndAssertAllPages(_bll.GetStatisticsForPollingStation, statistics);
        }

        [TestMethod]
        public void GetStatisticsForPollingStationByRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            var statistics = GetAllIdsFromDbTableWithUdfPropertyIn<PollingStationStatistics>(x => x.RegionId,
                UdfRegionsCriterion());

            // Act and Assert

            ActAndAssertAllPages(_bll.GetStatisticsForPollingStation, statistics);
        }

        [TestMethod]
        public void BaseQuery_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            var expPersons = GetAllObjectsFromDbTable<Person>(x =>
                (x.Deleted == null) &&
                x.PersonStatuses.Any(y => y.IsCurrent && (y.StatusType != null) && y.StatusType.IsExcludable));

            // Act

            var personsQuery = SafeExecFunc(_bll.BaseQuery);

            // Assert

            Assert.IsNotNull(personsQuery);
            AssertListsAreEqual(expPersons, personsQuery.List().ToList());
        }

        [TestMethod]
        public void RegisrtratorBaseQuery_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            var query = Repository.QueryOver<Person>();
            var expPersons = GetAllObjectsFromDbTable<Person>(x => x.Addresses.Any(y => y.IsEligible));

            // Act

            var personsQuery = SafeExecFunc(_bll.RegisrtratorBaseQuery, query);

            // Assert

            Assert.IsNotNull(personsQuery);
            AssertListsAreEqual(expPersons, personsQuery.List().ToList());
        }

        [TestMethod]
        public void ReturnLongTypeFromQuery_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            var query = Repository.QueryOver<Person>();
            var expResult = GetDbTableCount<Person>();

            // Act

            ActAndAssertLongValue(_bll.ReturnLongTypeFromQuery, query, expResult);
        }

        [TestMethod]
        public void GetImportStatisticsByNullRegionIdAndAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            var expDtos = GetAllObjectsFromDbTable<ImportStatistic>().Select(x => x.Date).Distinct().Select(x =>
                new ImportStatisticsGridDto
                {
                    ImportDateTime = x,
                    RegionCount = (int) GetDbTableCount<ImportStatistic>(y => y.Date == x)
                }).ToList();

            var pageRequest = GetPageRequest();
            pageRequest.PageSize = expDtos.Count + 10;

            // Act

            var dtosPage = SafeExecFunc(_bll.GetImportStatistics, pageRequest, (long?) null);

            // Assert

            Assert.IsNotNull(dtosPage);
            Assert.IsNotNull(dtosPage.Items);
            AssertObjectListsAreEqual(expDtos.Select(x => x.ImportDateTime).ToList(),
                dtosPage.Items.Select(x => x.ImportDateTime).ToList());
            AssertObjectListsAreEqual(expDtos.Select(x => x.RegionCount).ToList(),
                dtosPage.Items.Select(x => x.RegionCount).ToList());
        }

        [TestMethod]
        public void GetImportStatisticsByNullRegionIdAndRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            var expDtos =
                GetAllObjectsFromDbTable<ImportStatistic>(x => RegionIsAccessibleForCurrentUser(x.Region))
                    .Select(x => x.Date)
                    .Distinct()
                    .Select(x =>
                        new ImportStatisticsGridDto
                        {
                            ImportDateTime = x,
                            RegionCount =
                                (int)
                                    GetDbTableCount<ImportStatistic>(
                                        y => (y.Date == x) && RegionIsAccessibleForCurrentUser(y.Region))
                        }).ToList();

            var pageRequest = GetPageRequest();
            pageRequest.PageSize = expDtos.Count + 10;

            // Act

            var dtosPage = SafeExecFunc(_bll.GetImportStatistics, pageRequest, (long?) null);

            // Assert

            Assert.IsNotNull(dtosPage);
            Assert.IsNotNull(dtosPage.Items);
            AssertObjectListsAreEqual(expDtos.Select(x => x.ImportDateTime).ToList(),
                dtosPage.Items.Select(x => x.ImportDateTime).ToList());
            AssertObjectListsAreEqual(expDtos.Select(x => x.RegionCount).ToList(),
                dtosPage.Items.Select(x => x.RegionCount).ToList());
        }

        [TestMethod]
        public void GetImportStatisticsByNotNullRegionIdAndAdmin_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetAdministratorRole();

            var importStatistic = GetFirstObjectFromDbTable(GetImportStatistic);
            long? regionId = importStatistic.Region;

            var importStatistics = GetAllObjectsFromDbTableWithUdfPropertyIn<ImportStatistic>(x => x.Region,
                new RegionChildsFilterCriterion(regionId.Value));
            var expDtos = importStatistics.Select(x => x.Date).Distinct().Select(x =>
                new ImportStatisticsGridDto
                {
                    ImportDateTime = x,
                    RegionCount = importStatistics.Count(y => y.Date == x)
                }).ToList();

            var pageRequest = GetPageRequest();
            pageRequest.PageSize = expDtos.Count + 10;

            // Act

            var dtosPage = SafeExecFunc(_bll.GetImportStatistics, pageRequest, regionId);

            // Assert

            Assert.IsNotNull(dtosPage);
            Assert.IsNotNull(dtosPage.Items);
            AssertObjectListsAreEqual(expDtos.Select(x => x.ImportDateTime).ToList(),
                dtosPage.Items.Select(x => x.ImportDateTime).ToList());
            AssertObjectListsAreEqual(expDtos.Select(x => x.RegionCount).ToList(),
                dtosPage.Items.Select(x => x.RegionCount).ToList());
        }

        [TestMethod]
        public void GetImportStatisticsByNotNullRegionIdAndRegistrator_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            var importStatistic = GetFirstObjectFromDbTable(GetImportStatistic);
            long? regionId = importStatistic.Region;

            var region = GetFirstObjectFromDbTable<Region>(x => x.Id == regionId);
            AddRegionToCurrentUser(region);

            var importStatistics1 = GetAllObjectsFromDbTableWithUdfPropertyIn<ImportStatistic>(x => x.Region,
                new RegionChildsFilterCriterion(regionId.Value));
            var importStatistics2 = GetAllObjectsFromDbTableWithUdfPropertyIn<ImportStatistic>(x => x.Region,
                UdfRegionsCriterion());
            var importStatistics = importStatistics1.Intersect(importStatistics2).ToList();

            var expDtos = importStatistics.Select(x => x.Date).Distinct().Select(x =>
                new ImportStatisticsGridDto
                {
                    ImportDateTime = x,
                    RegionCount = importStatistics.Count(y => y.Date == x)
                }).ToList();

            var pageRequest = GetPageRequest();
            pageRequest.PageSize = expDtos.Count + 10;

            // Act

            var dtosPage = SafeExecFunc(_bll.GetImportStatistics, pageRequest, regionId);

            // Assert

            Assert.IsNotNull(dtosPage);
            Assert.IsNotNull(dtosPage.Items);
            AssertObjectListsAreEqual(expDtos.Select(x => x.ImportDateTime).ToList(),
                dtosPage.Items.Select(x => x.ImportDateTime).ToList());
            AssertObjectListsAreEqual(expDtos.Select(x => x.RegionCount).ToList(),
                dtosPage.Items.Select(x => x.RegionCount).ToList());
        }

        [TestMethod]
        public void GetImportStatisticsByDateTimeAndRegion_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, StatisticsBll>();

            SetRegistratorRole();

            var importStatistic = GetFirstObjectFromDbTable(GetImportStatistic);
            var regionId = importStatistic.Region;
            var importDateTime = importStatistic.Date;

            var importStatistics = GetAllObjectsFromDbTableWithUdfPropertyIn<ImportStatistic>(x => x.Date == importDateTime, x => x.Region, new RegionChildsFilterCriterion(regionId));

            var expDto = new ImportStatisticsDto
                {
                    New = importStatistics.Sum(x => x.New),
                    Conflicted = importStatistics.Sum(x => x.Conflicted),
                    Updated = importStatistics.Sum(x => x.Updated),
                    Error = importStatistics.Sum(x => x.Error),
                    Total = importStatistics.Sum(x => x.Total),
                    ResidenceChanged = importStatistics.Sum(x => x.ResidenceChnaged),
                    ChangedStatus = importStatistics.Sum(x => x.ChangedStatus)
                };
            
            // Act

            var dto = SafeExecFunc(_bll.GetImportStatistics, importDateTime, regionId);

            // Assert

            Assert.IsNotNull(dto);
            Assert.AreEqual(expDto.New, dto.New);
            Assert.AreEqual(expDto.Conflicted, dto.Conflicted);
            Assert.AreEqual(expDto.Updated, dto.Updated);
            Assert.AreEqual(expDto.Error, dto.Error);
            Assert.AreEqual(expDto.Total, dto.Total);
            Assert.AreEqual(expDto.ResidenceChanged, dto.ResidenceChanged);
            Assert.AreEqual(expDto.ChangedStatus, dto.ChangedStatus);
        }

    }
}
