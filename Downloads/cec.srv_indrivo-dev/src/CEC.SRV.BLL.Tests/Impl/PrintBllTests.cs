using System;
using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CEC.SRV.BLL.Dto;
using NHibernate.Linq;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.BLL.ReportService;
using CEC.SRV.BLL.Repositories;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class PrintBllTests : BaseBllTests
    {
        private PrintBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<PrintBll>();
        }

        [TestMethod]
        public void CreatePrintSession_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var election = GetFirstObjectFromDbTable(GetElection);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var electionId = election.Id;
            var pollingStationIds = new List<long>() {pollingStation.Id};

            // Act
            
            SafeExec(_bll.CreatePrintSession, electionId, pollingStationIds);

            // Assert

            var session = GetFirstObjectFromDbTable<PrintSession>(x =>
                (x.Election != null) && (x.ExportPollingStations != null) &&
                (x.Election.Id == electionId) && (x.ExportPollingStations.Count == pollingStationIds.Count) &&
                (pollingStationIds.All(z => x.ExportPollingStations.Select(y => y.PollingStation.Id).Contains(z))));

            Assert.IsNotNull(session);

            session.ExportPollingStations.ForEach(x =>
            {
                Assert.IsNotNull(x.PrintSession);
                Assert.AreEqual(session.Id, x.PrintSession.Id);
            });
        }

        [TestMethod]
        public void CancelPrintSession_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var printSession = GetFirstObjectFromDbTable(GetPrintSession);
            var printSessionId = printSession.Id;

            // Act
            
            SafeExec(_bll.CancelPrintSession, printSessionId);
            
            // Assert

            var newPrintSession = GetFirstObjectFromDbTable<PrintSession>(x => x.Id == printSessionId);
            var date = DateTimeOffset.Now.Date.Date;

            Assert.AreEqual(PrintStatus.Canceled, newPrintSession.Status);
            Assert.IsTrue(newPrintSession.EndDate.HasValue);
            Assert.AreEqual(date, newPrintSession.EndDate.Value.Date.Date);

            if (newPrintSession.ExportPollingStations != null)
            {
                newPrintSession.ExportPollingStations.ForEach(x => 
                    Assert.IsTrue((x.Status == PrintStatus.Finished) ||
                    ((x.Status == PrintStatus.Canceled) && (x.EndDate.HasValue) &&
                    (x.EndDate.Value.Date.Date == date))));
            }
        }

        [TestMethod]
        public void GetPrintSessionByStatus_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var session = GetFirstObjectFromDbTable(GetPrintSession);
            var status = session.Status;
            var expSessions = GetAllObjectsFromDbTable<PrintSession>(x => x.Status == status);

            // Act
            
            var sessions = SafeExecFunc(_bll.GetPrintSessionByStatus, status);

            // Assert

            AssertListsAreEqual(expSessions, sessions.ToList());
        }

        [TestMethod]
        public void GetExportPollingStationsByNotNullPrintSessionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            var exportPollingStation = GetFirstObjectFromDbTable(x => x.PrintSession != null,
                GetExportPollingStation);

            long? printSessionId = exportPollingStation.Id;

            var exportPollingStations =
                GetAllIdsFromDbTable<ExportPollingStation>(
                    x => (x.PrintSession != null) && (x.PrintSession.Id == printSessionId));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetExportPollingStations, printSessionId, exportPollingStations);
        }

        [TestMethod]
        public void GetExportPollingStationsByNullPrintSessionIdAndNonExistingPrintSessionsInProgress_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            var printSessionsInProgress =
                GetAllObjectsFromDbTable<PrintSession>(x => x.Status == PrintStatus.InProgress);
            printSessionsInProgress.ForEach(x =>
            {
                x.Status = PrintStatus.Pending;
                Repository.SaveOrUpdate(x);
            });
            
            // Act & Assert

            ActAndAssertAllPages(_bll.GetExportPollingStations, (long?)null, new List<long>());
        }

        [TestMethod]
        public void GetExportPollingStationsByNullPrintSessionIdAndExistingPrintSessionsInProgress_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(x => x.Status == PrintStatus.InProgress, GetPrintSession);
            var printSessionInProgress =
                GetFirstObjectFromDbTable<PrintSession>(x => x.Status == PrintStatus.InProgress);
            long? printSessionInProgressId = printSessionInProgress.Id;

            var exportPollingStations =
                GetAllIdsFromDbTable<ExportPollingStation>(
                    x => (x.PrintSession != null) && (x.PrintSession.Id == printSessionInProgressId));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetExportPollingStations, (long?)null, exportPollingStations);
        }

        [TestMethod]
        public void GetHistoryExportPollingStationsByNotNullPrintSessionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            var exportPollingStation = GetFirstObjectFromDbTable(x => x.PrintSession != null,
                GetExportPollingStation);

            long? printSessionId = exportPollingStation.Id;

            var exportPollingStations =
                GetAllIdsFromDbTable<ExportPollingStation>(
                    x => (x.PrintSession != null) && (x.PrintSession.Id == printSessionId));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetHistoryExportPollingStations, printSessionId, exportPollingStations);
        }

        [TestMethod]
        public void GetHistoryExportPollingStationsByNullPrintSessionId_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            // Act & Assert

            ActAndAssertAllPages(_bll.GetHistoryExportPollingStations, (long?)null, new List<long>());
        }

        [TestMethod]
        public void GetExportPrintSessions_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetPrintSession);
            var expPrintSessions = GetAllIdsFromDbTable<PrintSession>();

            // Act & Assert

            ActAndAssertAllPages(_bll.GetExportPrintSessions, expPrintSessions);
        }

        [TestMethod]
        public void GetSaiseExporter_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, PrintBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetSaiseExporter);
            var expSaiseExporters = GetAllIdsFromDbTable<SaiseExporter>();

            // Act & Assert

            ActAndAssertAllPages(_bll.GetSaiseExporter, expSaiseExporters);
        }

        [TestMethod]
        public void GetExportPollingStationsByPrintSession_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var exportPollingStation = GetFirstObjectFromDbTable(x => x.PrintSession != null, GetExportPollingStation);
            var printSessionId = exportPollingStation.PrintSession.Id;

            var expExportPollingStations = 
                GetAllObjectsFromDbTable<ExportPollingStation>(x => (x.PrintSession != null) && (x.PrintSession.Id == printSessionId));

            // Act

            var exportPollingStations = SafeExecFunc(_bll.GetExportPollingStationsByPrintSession, printSessionId);

            // Assert

            AssertListsAreEqual(expExportPollingStations, exportPollingStations.ToList());
        }

        [TestMethod]
        public void RequestStayStatementReport_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            bool ex;
            var parameters = GetSsrsPrintParameters();
            var stayStatement = GetFirstObjectFromDbTable(GetStayStatement);
            var stayStatementId = stayStatement.Id;
            var expReport = ExpectedReportStream(parameters, stayStatementId, out ex);

            // Act
            
            var report = SafeExecFunc(_bll.RequestStayStatementReport, parameters, stayStatementId, false, ex);

            // Assert

            if (report != null)
            {
                AssertObjectListsAreEqual(expReport, report.ToList());
            }
        }

        [TestMethod]
        public void ProcessPrintSession_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var session = GetFirstObjectFromDbTable(x => x.Status == PrintStatus.Pending, GetPendingPrintSession);
            var pendingSessionId = session.Id;

            var otherPrintSessions = GetAllObjectsFromDbTable<PrintSession>(x => (x.Status == PrintStatus.Pending) && (x.Id != pendingSessionId));
            otherPrintSessions.ForEach(x =>
            {
                x.Status = PrintStatus.InProgress;
                Repository.SaveOrUpdate(x);
            });
            
            var parameters = GetSsrsPrintParameters();
            var startDate = DateTimeOffset.Now.Date.Date;

            // Act

            SafeExec(_bll.ProcessPrintSession, parameters);

            // Assert

            var endDate = DateTimeOffset.Now.Date.Date;

            Assert.IsNotNull(session);
            Assert.IsTrue((session.Status == PrintStatus.Canceled) || (session.Status == PrintStatus.Failed) || (session.Status == PrintStatus.Finished));

            Assert.IsTrue(session.EndDate.HasValue);
            Assert.AreEqual(endDate, session.EndDate.Value.Date.Date);

            Assert.IsTrue(session.StartDate.HasValue);
            Assert.AreEqual(startDate, session.StartDate.Value.Date.Date);

            Assert.IsTrue((session.Status != PrintStatus.Canceled) || session.ExportPollingStations.Any(x => x.Status == PrintStatus.Canceled));

            Assert.IsTrue((session.Status != PrintStatus.Failed) || 
                session.ExportPollingStations.Any(x => 
                    (x.Status == PrintStatus.Failed) && 
                    x.EndDate.HasValue && (x.EndDate.Value.Date.Date == endDate)));

            Assert.IsTrue((session.Status != PrintStatus.Finished) ||
                session.ExportPollingStations.All(x => 
                    ((x.Status != PrintStatus.Pending) && (x.Status != PrintStatus.Canceled))));
        }

        private List<byte> ExpectedReportStream(SSRSPrintParameters parameters, long stayStatementId, out bool throwExists)
        {
            ReportExecutionService res = GetReportExecutionService(parameters, stayStatementId, out throwExists);
            return throwExists ? null: GetReport(res, out throwExists);
        }

        private ReportExecutionService GetReportExecutionService(SSRSPrintParameters ssrsParameters, long stayStatementId, out bool throwExists)
        {
            var ssrs = new ReportExecutionService()
            {
                Credentials = ssrsParameters.GetCredentials(),
                Url = ssrsParameters.ServerUrl,
                ExecutionHeaderValue = new ExecutionHeader()
            };

            try
            {
                ssrs.LoadReport(ssrsParameters.ReportName, null);
                ssrs.SetExecutionParameters(
                    new ParameterValue[] { new ParameterValue { Name = "stayStatementNumber", Value = stayStatementId.ToString() } }, "en-us");

                throwExists = false;
            }
            catch (Exception)
            {
                throwExists = true;
            }
            
            return ssrs;
        }

        private List<byte> GetReport(ReportExecutionService service, out bool throwExists)
        {
            List<byte> result = null;

            try
            {
                string mimeType;
                string extension;
                Warning[] warnings;
                string[] streamIDs;
                string encoding;
                result = service.Render("PDF", "<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>", out extension, out encoding, out mimeType, out warnings, out streamIDs).ToList();
                throwExists = false;
            }
            catch (Exception)
            {   
                throwExists = true;
            }

            return result;
        }

        private SSRSPrintParameters GetSsrsPrintParameters()
        {
            return new SSRSPrintParameters
            {
                ServerUrl = "http://localhost:80/ReportServer/ReportExecution2005.asmx",
                UserName = "",
                Password = "",
                ReportName = "/SRV/StayStatementForm",
                ExportPath = "d:\\RSA"
            };
        }
    }

}
