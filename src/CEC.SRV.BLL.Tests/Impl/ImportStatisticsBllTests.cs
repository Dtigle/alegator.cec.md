using System;
using System.Linq;
using System.Reflection;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain.Importer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Amdaris.NHibernateProvider;
using NHibernate.Linq;
using Amdaris.Domain;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class ImportStatisticsBllTests : BaseTests<ImportStatisticsBll, IRepository>
    {
        [TestMethod]
        public void GetTotalBydate_returns_zero()
        {
            // Act

            var result = Bll.GetTotalBydate(DateTime.Now);
            
            // Assert

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IsNew_returns_correct_result()
        {
            // Act

            var result = SafeExecFunc<bool>("IsNew", new object[] {StatisticChanges.New}, (Type[])null, true);
           
            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsChangedResidence_returns_correct_result()
        {   
            // Act

            var result = SafeExecFunc<bool>("IsChangedResidence", new object[] {StatisticChanges.AddressUpdate}, (Type[])null, true);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsChangedStatus_returns_correct_result()
        {
            // Act

            var result = SafeExecFunc<bool>("IsChangedStatus", new object[] { StatisticChanges.StatusUpdate }, (Type[])null, true);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsUpdated_returns_correct_result()
        {
            // Act

            var result = SafeExecFunc<bool>("IsUpdated", new object[] { StatisticChanges.Update }, (Type[])null, true);
            
            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Add_has_correct_logic()
        {
            var region = GetFirstObjectFromDbTable(GetRegion);

            var regionId = region.Id;

            Enum.GetValues(typeof(StatisticChanges)).Cast<StatisticChanges>().ForEach(x => AddTest(regionId, x));
        }

        [TestMethod]
        public void Save_has_correct_logic()
        {
            // Arrange

            var region = GetFirstObjectFromDbTable(GetRegion);
            var regionId = region.Id;

            var expectedStatistic = SetStatistic(regionId);

            var statistics = new Dictionary<long, ImportStatistic>();
            statistics.Add(regionId, expectedStatistic);
            // Act

            SafeExec(Bll.Save, (statistics));

            // Assert

            var statistic = GetLastCreatedStatistic();

            Assert.IsNotNull(statistic);
            Assert.AreEqual(expectedStatistic.Total, statistic.Total);
            Assert.AreEqual(expectedStatistic.Error, statistic.Error);
            Assert.AreEqual(expectedStatistic.Conflicted, statistic.Conflicted);
            Assert.AreEqual(expectedStatistic.Updated, statistic.Updated);
            Assert.AreEqual(expectedStatistic.ChangedStatus, statistic.ChangedStatus);
            Assert.AreEqual(expectedStatistic.ResidenceChnaged, statistic.ResidenceChnaged);
            Assert.AreEqual(expectedStatistic.New, statistic.New);
        }

        private void AddTest(long regionId, StatisticChanges status)
        {
            // Arrange

            var statistic = GetStatistic(regionId);

            var expectedTotal = statistic.Total + 1;
            var expectedError = statistic.Error + Convert.ToInt32((status & StatisticChanges.Error) != 0);
            var expectedConflicted = statistic.Conflicted + Convert.ToInt32((status & StatisticChanges.Conflict) != 0);
            var expectedUpdated = statistic.Updated + Convert.ToInt32((status & StatisticChanges.Update) == StatisticChanges.Update && (status & StatisticChanges.New) != StatisticChanges.New);
            var expectedChangedStatus = statistic.ChangedStatus + Convert.ToInt32((status & StatisticChanges.StatusUpdate) == StatisticChanges.StatusUpdate && (status & StatisticChanges.New) != StatisticChanges.New);
            var expectedResidenceChanged = statistic.ResidenceChnaged + Convert.ToInt32((status & StatisticChanges.AddressUpdate) == StatisticChanges.AddressUpdate && (status & StatisticChanges.New) != StatisticChanges.New);
            var expectedNew = statistic.New + Convert.ToInt32((status & StatisticChanges.New) == StatisticChanges.New && (status & StatisticChanges.Conflict) == 0);


            // Act

            SafeExec((region, status1) => Bll.UpdateStatisticData(statistic, status1), regionId, status);

            // Assert

            statistic = GetStatistic(regionId);

            Assert.AreEqual(expectedTotal, statistic.Total);
            Assert.AreEqual(expectedError, statistic.Error);
            Assert.AreEqual(expectedConflicted, statistic.Conflicted);
            Assert.AreEqual(expectedUpdated, statistic.Updated);
            Assert.AreEqual(expectedChangedStatus, statistic.ChangedStatus);
            Assert.AreEqual(expectedResidenceChanged, statistic.ResidenceChnaged);
            Assert.AreEqual(expectedNew, statistic.New);
        }

        private ImportStatistic GetStatistic(long regionId)
        {
            var statistics = SafeGetField<Dictionary<long, ImportStatistic>>("_statistics");
            var statistic = (statistics != null) && statistics.ContainsKey(regionId) ? statistics[regionId] : new ImportStatistic();
            return statistic;
        }

        private ImportStatistic SetStatistic(long regionId)
        {
            var statistics = new Dictionary<long, ImportStatistic>(); //SafeGetField<Dictionary<long, ImportStatistic>>("_statistics");

            var statistic = new ImportStatistic()
            {
                Total = 20,
                ChangedStatus = 2,
                Conflicted = 3,
                New = 4,
                Error = 3,
                ResidenceChnaged = 2,
                Updated = 6
            };

            if (statistics.ContainsKey(regionId))
            {
                statistics[regionId] = statistic;
            }
            else
            {
                statistics.Add(regionId, statistic);
            }
            
            //SafeSetField("_statistic", statistic);

            return statistic;
        }

        private ImportStatistic GetLastCreatedStatistic()
        {
            if (IsMockedRepository)
            {
                return LastCreated<ImportStatistic>();
            }
            var query = Repository.Query<ImportStatistic>();
            return query.FirstOrDefault(x => x.Date == query.Max(y => y.Date));
        }
    }
}
