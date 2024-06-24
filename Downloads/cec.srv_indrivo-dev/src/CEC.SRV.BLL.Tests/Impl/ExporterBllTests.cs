using System.Linq;
using CEC.SRV.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.Web.SRV.Resources;
using CEC.SRV.BLL.Repositories;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class ExporterBllTests : BaseBllTests
    {
        private ExporterBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<ExporterBll>();
        }

        [TestMethod]
        public void CreateSaiseExporterByNonExistingElection_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            // Act

            SafeExec(_bll.CreateSaiseExporter, -1L, false, false, true);
        }

        [TestMethod]
        public void CreateSaiseExporterByExistingElectionWithoutSaiseId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var election = GetFirstObjectFromDbTable(x => x.ElectionRounds != null, GetElection);
            var electionId = election.Id;

            // Act

            SafeExec(_bll.CreateSaiseExporter, electionId, false, false, true);
        }

        [TestMethod]
        public void CreateSaiseExporterByExistingElectionAndAlreadyExistingDataToExport_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var se = GetFirstObjectFromDbTable(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress, GetSaiseExporter);
            var election = se.ElectionDayId;
            var electionId = election;

            // Act

            SafeExec(_bll.CreateSaiseExporter, electionId, false, true, false, "SaiseExporter_ExistForProcessing", MUI.SaiseExporter_ExistForProcessing);
        }

        [TestMethod]
        public void CreateSaiseExporterByExistingElectionWithSaiseId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            GetAllObjectsFromDbTable<SaiseExporter>(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress)
                .ForEach(x =>
                {
                    x.Status = SaiseExporterStatus.Failed;
                    Repository.SaveOrUpdate(x);
                });

            var election = GetFirstObjectFromDbTable(x => x.ElectionRounds != null, GetElectionWithSaiseId);
            var electionId = election.Id;
            const bool exportAllVoters = false;

            // Act

            SafeExec(_bll.CreateSaiseExporter, electionId, exportAllVoters);

            // Assert

            var exporter = GetLastCreatedObject<SaiseExporter>();

            Assert.AreEqual(exportAllVoters, exporter.ExportAllVoters);
            Assert.AreEqual(SaiseExporterStatus.New, exporter.Status);
            //Assert.AreSame(election, exporter.Election);
            Assert.IsNotNull(exporter.Stages);
            Assert.IsTrue(exporter.Stages.Any());
        }

        [TestMethod]
        public void GetUnProcessedSaiseExporter_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress, GetSaiseExporter);

            var expSaiseExporters =
                GetAllObjectsFromDbTable<SaiseExporter>(
                    x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress);

            // Act

            var saiseExporters = SafeExecFunc(_bll.GetUnProcessedSaiseExporter);

            // Assert

            AssertListsAreEqual(expSaiseExporters, saiseExporters.ToList());
        }

        [TestMethod]
        public void GetActiveSaiseExporter_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expSaiseExporter = GetFirstObjectFromDbTable(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress, GetSaiseExporter);

            // Act

            var saiseExporter = SafeExecFunc(_bll.GetActiveSaiseExporter);

            // Assert

            Assert.AreSame(expSaiseExporter, saiseExporter);
        }

        [TestMethod]
        public void GetProgressOfSaiseExporter_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ExporterBll>();

            SetAdministratorRole();

            var exporter = GetFirstObjectFromDbTable(GetSaiseExporter);
            var saiseExporterId = exporter.Id;
            var totalStages = exporter.Stages.Count();
            var finishedStages = exporter.Stages.Count(x => x.Status == SaiseExporterStageStatus.Done);

            Assert.IsTrue(totalStages > 0);

            var expResult = finishedStages * 100 / totalStages;

            // Act

            var result = SafeExecFunc(_bll.GetProgressOfSaiseExporter, saiseExporterId);

            // Assert

            Assert.AreEqual(expResult, result);
        }

        [TestMethod]
        public void GetSaiseExporterByNotNullId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ExporterBll>();

            SetAdministratorRole();

            var exporter = GetFirstObjectFromDbTable(GetSaiseExporter);

            long? saiseExporterId = exporter.Id;

            var stages =
                GetAllIdsFromDbTable<SaiseExporterStage>(
                    x => (x.SaiseExporter != null) && (x.SaiseExporter.Id == saiseExporterId));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetSaiseExporter, saiseExporterId, stages);
        }

        [TestMethod]
        public void GetSaiseExporterByNullId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ExporterBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress, GetSaiseExporter);

            var stages =
                GetAllIdsFromDbTable<SaiseExporterStage>(
                    x => x.SaiseExporter.Status == SaiseExporterStatus.New || x.SaiseExporter.Status == SaiseExporterStatus.InProgress);

            // Act & Assert

            ActAndAssertAllPages(_bll.GetSaiseExporter, (long?)null, stages);
        }

        [TestMethod]
        public void GetHistorySaiseExporterByNotNullId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ExporterBll>();

            SetAdministratorRole();

            var exporter = GetFirstObjectFromDbTable(GetSaiseExporter);
            long? saiseExporterId = exporter.Id;
            var stages = GetAllIdsFromDbTable<SaiseExporterStage>(x => (x.SaiseExporter != null) && (x.SaiseExporter.Id == saiseExporterId));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetHistorySaiseExporter, saiseExporterId, stages);
        }

        [TestMethod]
        public void GetHistorySaiseExporterByNullId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, ExporterBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetSaiseExporter);

            // Act & Assert

            ActAndAssertAllPages(_bll.GetHistorySaiseExporter, (long?)null, new List<long>());
        }

        [TestMethod]
        public void GetFailedMessageOfSaiseExporterTrueCase_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var saiseExporterStage = GetFirstObjectFromDbTable(x => (x.Status == SaiseExporterStageStatus.Failed) && (x.SaiseExporter != null), GetSaiseExporterStage);
            var saiseExporterId = saiseExporterStage.SaiseExporter.Id;

            // Act

            var result = SafeExecFunc(_bll.GetFailedMessageOfSaiseExporter, saiseExporterId);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetFailedMessageOfSaiseExporterFalseCase_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            // Act

            var result = SafeExecFunc(_bll.GetFailedMessageOfSaiseExporter, -1L);

            // Assert

            Assert.IsFalse(result);
        }
    }

}
