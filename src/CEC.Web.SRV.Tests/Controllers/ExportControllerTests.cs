using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Export;
using CEC.Web.SRV.Models.Voters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using AutoMapper;
using Moq;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.SRV.BLL;
using System.Globalization;
using CEC.Web.SRV.Resources;
using Amdaris.Domain;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class ExportControllerTests : BaseControllerTests
    {
        private readonly Mock<IPollingStationBll> _pollingStationBll;
        private readonly Mock<IPrintBll> _printBll;
        private readonly Mock<IExporterBll> _exporterBll;
        private readonly ExportController _controller;

        public ExportControllerTests()
        {
            _pollingStationBll = new Mock<IPollingStationBll>();
            _printBll = new Mock<IPrintBll>();
            _exporterBll = new Mock<IExporterBll>();
            _controller = new ExportController(_pollingStationBll.Object, _printBll.Object, _exporterBll.Object);
            BaseController = _controller;

            Mapper.Initialize(arg =>
            {
                arg.AddProfile<ExportPollingStationsProfile>();
                arg.AddProfile<PrintSessionProfile>();
                arg.AddProfile<SaiseExporterProfile>();
                arg.AddProfile<SaiseExporterStageProfile>();
            });
        }

        [TestMethod]
        public void ExportList_WithoutPrintSessionsInProgress_has_correct_model()
        {
            // Arrange

            _printBll.Setup(x => x.GetPrintSessionByStatus(PrintStatus.InProgress)).Returns(new List<PrintSession>());
            _printBll.Setup(x => x.GetPrintSessionByStatus(PrintStatus.Pending)).Returns(new List<PrintSession> {new PrintSession()});

            // Act

            var result = _controller.ExportList() as ViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as ListExportingModel;

            Assert.IsNotNull(model);
            Assert.IsFalse(model.IsProgress);
            Assert.AreEqual(1, model.TotalPollingStationInPending);

            _printBll.Verify(x => x.GetPrintSessionByStatus(PrintStatus.InProgress), Times.Once);
            _printBll.Verify(x => x.GetPrintSessionByStatus(PrintStatus.Pending), Times.Once);
        }

        [TestMethod]
        public void ExportList_WithPrintSessionsInProgress_has_correct_model()
        {
            // Arrange

            var printSessions = GetPrintSessions();
            var printSession = printSessions.FirstOrDefault();
            var exportPollingStations = GetExportPollingStations();

            _printBll.Setup(x => x.GetPrintSessionByStatus(PrintStatus.InProgress)).Returns(printSessions);
            _printBll.Setup(x => x.GetExportPollingStationsByPrintSession(It.IsAny<long>())).Returns(exportPollingStations);
            

            // Act

            var result = _controller.ExportList() as ViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as ListExportingModel;

            Assert.IsNotNull(model);
            Assert.IsTrue(model.IsProgress);
            Assert.AreEqual(exportPollingStations.Count, model.TotalPollingStationForExporting);
            
            Assert.IsNotNull(printSession);
            Assert.IsNotNull(model.ElectionInfo);
            Assert.AreEqual(printSession.Election.Id, model.ElectionInfo.ElectionId);
            Assert.AreEqual(printSession.Election.NameRo, model.ElectionInfo.ElectionTypeName);

            Assert.AreEqual(printSession.Id, model.PrintSessionId);
            
            if (printSession.StartDate.HasValue)
            {
                Assert.AreEqual(printSession.StartDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture), model.StartDate);
            }
            else
            {
                Assert.AreEqual(string.Empty, model.StartDate);
            }


            _printBll.Verify(x => x.GetPrintSessionByStatus(PrintStatus.InProgress), Times.Once);
            _printBll.Verify(x => x.GetExportPollingStationsByPrintSession(It.IsAny<long>()), Times.Once);
        }

        [TestMethod]
        public void HistoryExportList_returns_correct_view()
        {
            // Act

            var result = _controller.HistoryExportList() as ViewResult;

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HistoryExportRsaToSaise_returns_correct_view()
        {
            // Act

            var result = _controller.HistoryExportRsaToSaise() as ViewResult;

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ExportRsaToSaise_WithoutActiveSaiseExporter_has_correct_model()
        {
            // Arrange

            _exporterBll.Setup(x => x.GetActiveSaiseExporter()).Returns((SaiseExporter)null);

            // Act

            var result = _controller.ExportRsaToSaise() as ViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as ExportRsaToSaiseModel;

            Assert.IsNotNull(model);
            Assert.IsTrue(model.ExportAll.HasValue);
            Assert.IsFalse(model.ExportAll.Value);
            
            _exporterBll.Verify(x => x.GetActiveSaiseExporter(), Times.Once);
        }

        [TestMethod]
        public void ExportRsaToSaise_WithActiveSaiseExporter_has_correct_model()
        {
            // Arrange

            var saiseExporter = GetSaiseExporter();
            _exporterBll.Setup(x => x.GetActiveSaiseExporter()).Returns(saiseExporter);
            
            // Act

            var result = _controller.ExportRsaToSaise() as ViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as ExportRsaToSaiseModel;

            Assert.IsNotNull(model);
            Assert.IsTrue(model.IsProgress);

            Assert.IsNotNull(saiseExporter);
            Assert.IsNotNull(model.ElectionInfo);
            //Assert.AreEqual(saiseExporter.Election.Id, model.ElectionInfo.ElectionId);
            //Assert.AreEqual(saiseExporter.Election.NameRo, model.ElectionInfo.ElectionTypeName);

            Assert.AreEqual(saiseExporter.Id, model.SaiseExporterId);

            _exporterBll.Verify(x => x.GetActiveSaiseExporter(), Times.Once);
        }

        [TestMethod]
        public void ExportRsaToSaise_ByNonValidModel_has_correct_model()
        {
            // Arrange

            _controller.ModelState.AddModelError("", "error");

            var expModel = GetExportRsaToSaiseModel();

            // Act

            var result = _controller.ExportRsaToSaise(expModel) as ViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);

            var model = result.Model as ExportRsaToSaiseModel;

            Assert.IsNotNull(model);
            Assert.AreSame(expModel, model);

        }

        [TestMethod]
        public void ExportRsaToSaise_ByValidModelAndZeroElectionId_has_correct_model()
        {
            // Arrange
            
            var expModel = GetExportRsaToSaiseModel();
            expModel.ElectionInfo.ElectionId = 0;

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.ExportRsaToSaise(expModel) as ViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);

            Assert.IsTrue(_controller.ModelState["ElectionId"].Errors.Count > 0);

            var model = result.Model as ExportRsaToSaiseModel;

            Assert.IsNotNull(model);
            Assert.AreSame(expModel, model);

        }

        [TestMethod]
        public void ExportRsaToSaise_ByValidModelAndNonZeroElectionId_returns_correct_view()
        {
            // Arrange

            var expModel = GetExportRsaToSaiseModel();
            expModel.ElectionInfo.ElectionId = 1;

            _exporterBll.Setup(x => x.CreateSaiseExporter(expModel.ElectionInfo.ElectionId, expModel.ExportAll.Value));

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.ExportRsaToSaise(expModel) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);

            _exporterBll.Verify(x => x.CreateSaiseExporter(expModel.ElectionInfo.ElectionId, expModel.ExportAll.Value), Times.Once);
        }

        [TestMethod]
        public void CancelPrintSession_WithoutPrintSessionsInProgress_has_correct_logic()
        {
            // Arrange

            _printBll.Setup(x => x.GetPrintSessionByStatus(PrintStatus.InProgress)).Returns(new List<PrintSession>());

            // Act

            _controller.CancelPrintSession();

            // Assert

            _printBll.Verify(x => x.CancelPrintSession(It.IsAny<long>()), Times.Never);
        }

        [TestMethod]
        public void CancelPrintSession_WithPrintSessionsInProgress_has_correct_logic()
        {
            // Arrange

            var printSessions = GetPrintSessions();
            var printSession = printSessions.FirstOrDefault();
            _printBll.Setup(x => x.GetPrintSessionByStatus(PrintStatus.InProgress)).Returns(printSessions);
            _printBll.Setup(x => x.CancelPrintSession(printSession.Id));

            // Act

            _controller.CancelPrintSession();

            // Assert

            _printBll.Verify(x => x.CancelPrintSession(printSession.Id), Times.Once);
        }

        [TestMethod]
        public void GetPollingStationbyRegions_returns_correct_json()
        {
            // Arrange

            var request = new Select2Request
            {
                page = 1,
                pageLimit = 1,
                q = string.Empty
            };

            var regionIds = new long[] {1, 2, 3};
            var expItems = GetPollingStations();

            _pollingStationBll.Setup(x => x.GetPollingStationsByRegions(It.IsAny<PageRequest>(), regionIds)).Returns(
                new PageResponse<PollingStation>
                {
                    Items = expItems,
                    PageSize = 1,
                    StartIndex = 1,
                    Total = 1
                });

            // Act

            var result = _controller.GetPollingStationbyRegions(request, regionIds) as JsonResult;

            // Assert

            Assert.IsNotNull(result);

            var data = result.Data as Select2PagedResponse;
            Assert.IsNotNull(data);
            AssertListsAreEqual(data.Items, expItems, x => x.Id, x => string.Format("{0} - {1}", x.FullNumber, x.PollingStationAddress != null ? x.GetFullAddress() : MUI.FilterForVoters_PollingStation_MissingAddress));

            _pollingStationBll.Verify(x => x.GetPollingStationsByRegions(It.IsAny<PageRequest>(), regionIds), Times.Once);
        }

        [TestMethod]
        public void ListPollingStationAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();

            long? printSessionId = 1;
            var expItems = GetExportPollingStations();

            _printBll.Setup(x => x.GetExportPollingStations(It.IsAny<PageRequest>(), printSessionId)).Returns(GetPageResponse(expItems));

            // Act

            var result = _controller.ListPollingStationAjax(request, printSessionId) as JqGridJsonResult;

            // Assert
            
            AssertJqGridJsonResult(result, expItems);
            _printBll.Verify(x => x.GetExportPollingStations(It.IsAny<PageRequest>(), printSessionId), Times.Once);
        }

        [TestMethod]
        public void ListHistoryPollingStationAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();

            long? printSessionId = 1;
            var expItems = GetExportPollingStations();

            _printBll.Setup(x => x.GetHistoryExportPollingStations(It.IsAny<PageRequest>(), printSessionId)).Returns(GetPageResponse(expItems));

            // Act

            var result = _controller.ListHistoryPollingStationAjax(request, printSessionId) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, expItems);
            _printBll.Verify(x => x.GetHistoryExportPollingStations(It.IsAny<PageRequest>(), printSessionId), Times.Once);
        }

        [TestMethod]
        public void ListPrintSessionsAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            var expItems = GetPrintSessions();

            _printBll.Setup(x => x.GetExportPrintSessions(It.IsAny<PageRequest>())).Returns(GetPageResponse(expItems));

            // Act

            var result = _controller.ListPrintSessionsAjax(request) as JqGridJsonResult;

            // Assert

           AssertJqGridJsonResult(result, expItems);
            _printBll.Verify(x => x.GetExportPrintSessions(It.IsAny<PageRequest>()), Times.Once);
        }

        [TestMethod]
        public void ListSaiseExporterAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            var expItems = GetSaiseExporters();

            _printBll.Setup(x => x.GetSaiseExporter(It.IsAny<PageRequest>())).Returns(GetPageResponse(expItems));

            // Act

            var result = _controller.ListSaiseExporterAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, expItems);
            _printBll.Verify(x => x.GetSaiseExporter(It.IsAny<PageRequest>()), Times.Once);
        }

        [TestMethod]
        public void ListSaiseExporterStageAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            long? saiseExporterId = 1;
            var expItems = GetSaiseExporterStages();

            _exporterBll.Setup(x => x.GetSaiseExporter(It.IsAny<PageRequest>(), saiseExporterId)).Returns(GetPageResponse(expItems));

            // Act

            var result = _controller.ListSaiseExporterStageAjax(request, saiseExporterId) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, expItems);
            _exporterBll.Verify(x => x.GetSaiseExporter(It.IsAny<PageRequest>(), saiseExporterId), Times.Once);
        }

        [TestMethod]
        public void ListHistorySaiseExporterStageAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            long? saiseExporterId = 1;
            var expItems = GetSaiseExporterStages();

            _exporterBll.Setup(x => x.GetHistorySaiseExporter(It.IsAny<PageRequest>(), saiseExporterId)).Returns(GetPageResponse(expItems));

            // Act

            var result = _controller.ListHistorySaiseExporterStageAjax(request, saiseExporterId) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, expItems);
            _exporterBll.Verify(x => x.GetHistorySaiseExporter(It.IsAny<PageRequest>(), saiseExporterId), Times.Once);
        }

        [TestMethod]
        public void CreatePrintSession_ByExistingPollingStations_has_correct_logic()
        {
            // Arrange

            const long electionId = 1L;
            var regionIds = new long[] {1};
            var pollingStationsId = new long[] {1};

            _printBll.Setup(x => x.CreatePrintSession(electionId, pollingStationsId));

            // Act

            _controller.CreatePrintSession(electionId, regionIds, pollingStationsId);

            // Assert

            _printBll.Verify(x => x.CreatePrintSession(electionId, pollingStationsId), Times.Once);
        }

        [TestMethod]
        public void CreatePrintSession_ByNonExistingPollingStations_has_correct_logic()
        {
            // Arrange

            const long electionId = 1L;
            var regionIds = new long[] { 1 };
            var pollingStationsId = new long[0];
            var pollingStations = GetPollingStations();
            var psIds = pollingStations.Select(x => x.Id);

            _pollingStationBll.Setup(x => x.GetPollingStationsByRegion(regionIds)).Returns(pollingStations);
            _printBll.Setup(x => x.CreatePrintSession(electionId, psIds));

            // Act

            _controller.CreatePrintSession(electionId, regionIds, pollingStationsId);

            // Assert

            _printBll.Verify(x => x.CreatePrintSession(electionId, psIds), Times.Once);
            _pollingStationBll.Verify(x => x.GetPollingStationsByRegion(regionIds), Times.Once);
        }

        [TestMethod]
        public void CreatePrintSession_ByNullPollingStations_has_correct_logic()
        {
            // Arrange

            const long electionId = 1L;
            var regionIds = new long[] { 1 };
            var pollingStations = GetPollingStations();
            var psIds = pollingStations.Select(x => x.Id);

            _pollingStationBll.Setup(x => x.GetPollingStationsByRegion(regionIds)).Returns(pollingStations);
            _printBll.Setup(x => x.CreatePrintSession(electionId, psIds));

            // Act

            _controller.CreatePrintSession(electionId, regionIds, null);

            // Assert

            _printBll.Verify(x => x.CreatePrintSession(electionId, psIds), Times.Once);
            _pollingStationBll.Verify(x => x.GetPollingStationsByRegion(regionIds), Times.Once);
        }

        [TestMethod]
        public void SelectPrintStatus_returns_correct_result()
        {
            // Act

            var result = _controller.SelectPrintStatus() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as List<SelectListItem>;

            Assert.IsNotNull(model);
            AssertListsAreEqual(model, Enum.GetValues(typeof(PrintStatus)).Cast<PrintStatus>().ToList(), x => x.GetEnumDescription(), x => ((int)x).ToString());
        }

        [TestMethod]
        public void SelectStageStatus_returns_correct_result()
        {
            // Act

            var result = _controller.SelectStageStatus() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as List<SelectListItem>;

            Assert.IsNotNull(model);
            AssertListsAreEqual(model, Enum.GetValues(typeof(SaiseExporterStageStatus)).Cast<SaiseExporterStageStatus>().ToList(), x => x.GetEnumDescription(), x => ((int)x).ToString());
        }
        
        [TestMethod]
        public void SelectExporterStatus_returns_correct_result()
        {
            // Act

            var result = _controller.SelectExporterStatus() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as List<SelectListItem>;

            Assert.IsNotNull(model);
            AssertListsAreEqual(model, Enum.GetValues(typeof(SaiseExporterStatus)).Cast<SaiseExporterStatus>().ToList(), x => x.GetEnumDescription(), x => ((int)x).ToString());
        }

        [TestMethod]
        public void GetProgressOfSaiseExporter_returns_correct_result()
        {
            // Arrange

            const long saiseExporterId = 1L;

            const int progress = 1;
            const bool error = true;

            _exporterBll.Setup(x => x.GetProgressOfSaiseExporter(saiseExporterId)).Returns(progress);
            _exporterBll.Setup(x => x.GetFailedMessageOfSaiseExporter(saiseExporterId)).Returns(error);

            // Act

            var result = _controller.GetProgressOfSaiseExporter(saiseExporterId) as JsonResult;

            // Assert

            Assert.IsNotNull(result);

            var data = result.Data;

            Assert.IsNotNull(data);
            Assert.AreEqual(progress, data.GetType().GetProperty("Progres").GetValue(data));
            Assert.AreEqual(error, data.GetType().GetProperty("Error").GetValue(data));

            _exporterBll.Verify(x => x.GetProgressOfSaiseExporter(saiseExporterId), Times.Once);
            _exporterBll.Verify(x => x.GetFailedMessageOfSaiseExporter(saiseExporterId), Times.Once);
        }

        [TestMethod]
        public void GetProgressOfPrintSession_returns_correct_result()
        {
            // Arrange

            const long printSessionId = 1L;
            var exportPollingStations = GetExportPollingStations();
            exportPollingStations.ForEach(x => x.Status = PrintStatus.Failed);

            var pollingStationFinished = exportPollingStations.Count(x => x.Status == PrintStatus.Finished);
            var totalPollingStationForExporting = exportPollingStations.Count();
            var progress = pollingStationFinished * 100 / totalPollingStationForExporting;

            _printBll.Setup(x => x.GetExportPollingStationsByPrintSession(printSessionId)).Returns(exportPollingStations);

            // Act

            var result = _controller.GetProgressOfPrintSession(printSessionId) as JsonResult;

            // Assert

            Assert.IsNotNull(result);

            var data = result.Data;
            Assert.IsTrue((bool)data.GetType().GetProperty("Error").GetValue(data));
            Assert.AreEqual(progress, data.GetType().GetProperty("Progres").GetValue(data));
            Assert.AreEqual(pollingStationFinished, data.GetType().GetProperty("PollingStationFinished").GetValue(data));

            _printBll.Verify(x => x.GetExportPollingStationsByPrintSession(printSessionId), Times.Once);
        }

        private static void AssertJqGridJsonResult<T>(JsonResult result, List<T> expItems) where T: Entity
        {
            Assert.IsNotNull(result);

            var data = result.Data as JqGridResponse;
            Assert.IsNotNull(data);
            AssertListsAreEqual(data.Records, expItems);
        }

        private static List<PrintSession> GetPrintSessions()
        {
            return new List<PrintSession>
            {
                GetPrintSession()
            };
        }

        private static PrintSession GetPrintSession()
        {
            return new PrintSession(GetElection(), GetPollingStations())
            {
                Status = PrintStatus.InProgress,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
        }

        private static List<PollingStation> GetPollingStations()
        {
            return new List<PollingStation> { GetPollingStation()};
        }

        private static PollingStation GetPollingStation()
        {
            var pollingStation = new PollingStation(GetRegion())
            {
                ContactInfo = "info",
                GeoLocation = GetGeoLocation(),
                Location = "Chisinau",
                Number = "123",
                SaiseId = 123,
                SubNumber = "A"
            };

            pollingStation.PollingStationAddress = GetAddress(pollingStation);

            return pollingStation;
        }

        private static Election GetElection()
        {
            return new Election
            {
                //AcceptAbroadDeclaration = true,
                //Comments = "comments",
                //ElectionDate = new DateTime(2014, 11, 30),
                //ElectionType = GetElectionType(),
                //SaiseId = 123
            };
        }

        private static ElectionType GetElectionType()
        {
            return new ElectionType
            {
                Description = "description",
                Name = "name"
            };
        }

        private static Region GetRegion()
        {
            var region = new Region(GetRegionType())
            {
                Circumscription = 1,
                Description = "description",
                GeoLocation = GetGeoLocation(),
                HasStreets = true,
                Name = "Chisinau",
                RegistruId = 123,
                SaiseId = 321,
                StatisticCode = 222,
                StatisticIdentifier = 111
            };

            region.PublicAdministration = GetPublicAdministration(region);

            return region;
        }

        private static RegionType GetRegionType()
        {
            return new RegionType
            {
                Description = "description",
                Name = "name",
                Rank = 1
            };
        }

        private static GeoLocation GetGeoLocation()
        {
            return new GeoLocation
            {
                Latitude = 20,
                Longitude = 20
            };
        }

        private static PublicAdministration GetPublicAdministration(Region region)
        {
            return new PublicAdministration(region, GetManagerType())
            {
                Name = "name",
                Surname = "surname"
            };
        }

        private static ManagerType GetManagerType()
        {
            return new ManagerType
            {
                Name = "manager",
                Description = "description"
            };
        }

        private static Address GetAddress(PollingStation pollingStation)
        {
            return new Address
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = GetGeoLocation(),
                HouseNumber = 12,
                PollingStation = pollingStation,
                Street = GetStreet(),
                Suffix = "A"
            };
        }

        private static Street GetStreet()
        {
            return new Street(GetRegion(), GetStreetType(), "street")
            {
                RopId = 11,
                SaiseId = 123
            };
        }

        private static StreetType GetStreetType()
        {
            return new StreetType
            {
                Name = "str",
                Description = "str"
            };
        }

        private static ExportPollingStation GetExportPollingStation()
        {
            return new ExportPollingStation(GetPrintSession(), GetPollingStation())
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Status = PrintStatus.InProgress,
                StatusMessage = string.Empty
            };
        }

        private static List<ExportPollingStation> GetExportPollingStations()
        {
            return new List<ExportPollingStation>
            {
                GetExportPollingStation()
            };
        }

        private static SaiseExporter GetSaiseExporter()
        {
            return new SaiseExporter(1)
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                ErrorMessage = string.Empty,
                ExportAllVoters = true
            };
        }

        private static List<SaiseExporter> GetSaiseExporters()
        {
            return new List<SaiseExporter>()
            {
                GetSaiseExporter()
            };
        }

        private static SaiseExporterStage GetSaiseExporterStage()
        {
            return new SaiseExporterStage(GetSaiseExporter());
        }

        private static List<SaiseExporterStage> GetSaiseExporterStages()
        {
            return new List<SaiseExporterStage>()
            {
                GetSaiseExporterStage()
            };
        }

        private static ExportRsaToSaiseModel GetExportRsaToSaiseModel()
        {
            return new ExportRsaToSaiseModel
            {
                ElectionInfo = new ElectionModel
                {
                    ElectionDate = DateTime.Now,
                    ElectionId = 1,
                    ElectionTypeName = "name"
                },
                ExportAll = true,
                IsProgress = true,
                SaiseExporterId = 1
            };
        }

        private static void AssertListsAreEqual<T>(IEnumerable<Select2Item> list1, List<T> list2, Func<T, long> idFunc, Func<T, string> textFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.text) && string.Equals(idFunc(x), item.id))));
        }

        private static void AssertListsAreEqual<T>(IEnumerable<JqGridRecord> list1, List<T> list2) where T : Entity
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(x.Id.ToString(), item.Id))));
        }

        private static void AssertListsAreEqual<T>(IEnumerable<SelectListItem> list1, List<T> list2, Func<T, string> textFunc, Func<T, string> valueFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.Text) && string.Equals(valueFunc(x), item.Value))));
        }

        private static JqGridRequest GetJqGridRequest()
        {
            return new JqGridRequest
            {
                PageIndex = 1,
                PagesCount = 1,
                RecordsCount = 1,
                Searching = false
            };
        }

        private static PageResponse<T> GetPageResponse<T>(IList<T> items) where T: class
        {
            return new PageResponse<T>
            {
                Items = items,
                PageSize = 1,
                StartIndex = 1,
                Total = 1
            };
        }
    }
}

