using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Dto;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Infrastructure.Logger;
using CEC.Web.SRV.LoggingService;
using CEC.Web.SRV.Models.Export;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;


namespace CEC.Web.SRV.Controllers
{
    public class ExportController : BaseController
    {
        private readonly IPollingStationBll _pollingStationBll;
        private readonly IPrintBll _printBll;
        private readonly IExporterBll _exporterBll;
        private readonly ElectionsServiceReference.ElectionsServiceClient _electionService;
        MessageHeader _messageHeader;

        public ExportController(IPollingStationBll pollingStationBll, IPrintBll printBll, IExporterBll exporterBll)
        {
            _pollingStationBll = pollingStationBll;
            _printBll = printBll;
            _exporterBll = exporterBll;
            _electionService = new ElectionsServiceReference.ElectionsServiceClient();
            _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
        }

        [Authorize(Roles = Transactions.Export)]
        public ActionResult ExportList()
        {
            var printSession = _printBll.GetPrintSessionByStatus(PrintStatus.InProgress).FirstOrDefault();
            var model = new ListExportingModel();
            if (printSession == null)
            {
                model.IsProgress = false;
                model.TotalPollingStationInPending = _printBll.GetPrintSessionByStatus(PrintStatus.Pending).Count;
                model.ElectionRoundId = -1;

            }
            else
            {
                model.ElectionInfo = new ElectionModel
                {
                    ElectionId = printSession.Election.Id,
                    ElectionTypeName = printSession.Election.ElectionType.Name
                };
                var exportPollingStation = _printBll.GetExportPollingStationsByPrintSession(printSession.Id);
                model.PrintSessionId = printSession.Id;
                model.TotalPollingStationForExporting = exportPollingStation.Count();
                model.StartDate = printSession.StartDate.HasValue
                    ? printSession.StartDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                    : string.Empty;
                model.IsProgress = true;
            }
            return View(model);
        }

        [Authorize(Roles = Transactions.Export)]
        public ActionResult ExportElectionResult()
        {
            var printSession = _printBll.GetPrintSessionByStatus(PrintStatus.InProgress).FirstOrDefault();
            var model = new ListExportingModel();
            if (printSession == null)
            {
                model.IsProgress = false;
                model.TotalPollingStationInPending = _printBll.GetPrintSessionByStatus(PrintStatus.Pending).Count;
                model.ElectionRoundId = -1;

            }
            else
            {
                model.ElectionInfo = new ElectionModel
                {
                    ElectionId = printSession.Election.Id,
                    ElectionTypeName = printSession.Election.ElectionType.Name
                };
                var exportPollingStation = _printBll.GetExportPollingStationsByPrintSession(printSession.Id);
                model.PrintSessionId = printSession.Id;
                model.TotalPollingStationForExporting = exportPollingStation.Count();
                model.StartDate = printSession.StartDate.HasValue
                    ? printSession.StartDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                    : string.Empty;
                model.IsProgress = true;
            }
            return View(model);
        }

        [Authorize(Roles = Transactions.Export)]
        public ActionResult HistoryExportList()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Export)]
        public ActionResult HistoryExportRsaToSaise()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Transactions.Export)]
        public ActionResult ExportRsaToSaise()
        {
            var saiseExporter = _exporterBll.GetActiveSaiseExporter();
            var model = new ExportRsaToSaiseModel();
            if (saiseExporter == null)
            {
                model.ExportAll = false;
            }
            else
            {
                model.SaiseExporterId = saiseExporter.Id;
                //model.ElectionInfo = new ElectionModel
                //{
                //	ElectionId = saiseExporter.Election.Id,
                //	ElectionTypeName = saiseExporter.Election.NameRo
                //};
                model.IsProgress = true;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ExportRsaToSaise(ExportRsaToSaiseModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ElectionInfo.ElectionId == 0)
            {
                ModelState.AddModelError("ElectionId", MUI.ListPrintingModel_FieldRequired);
                return View(model);
            }

            _exporterBll.CreateSaiseExporter(model.ElectionInfo.ElectionId, (bool)model.ExportAll);
            return RedirectToAction("ExportRsaToSaise");
        }

        [HttpPost]
        public void CancelPrintSession()
        {
            var printSession = _printBll.GetPrintSessionByStatus(PrintStatus.InProgress).FirstOrDefault();
            if (printSession != null)
            {
                _printBll.CancelPrintSession(printSession.Id);
            }
        }

        [HttpPost]
        public ActionResult GetPollingStationbyRegions(Select2Request request, long[] regionsId)
        {
            var pageRequest = request.ToPageRequest("FullNumber", ComparisonOperator.Contains);
            var data = _pollingStationBll.GetPollingStationsByRegions(pageRequest, regionsId);
            var response = new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => string.Format("{0} - {1}", x.FullNumber, x.PollingStationAddress != null ? x.GetFullAddress() : MUI.FilterForVoters_PollingStation_MissingAddress)).ToList(), data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListPollingStationAjax(JqGridRequest request, long? printSessionId)
        {
            var pageRequest = request.ToPageRequest<ExportPollingStationsGridModel>();

            var data = _printBll.GetExportPollingStations(pageRequest, printSessionId);

            return data.ToJqGridJsonResult<ExportPollingStation, ExportPollingStationsGridModel>();
        }

        public ActionResult ListHistoryPollingStationAjax(JqGridRequest request, long? printSessionId)
        {
            var pageRequest = request.ToPageRequest<ExportPollingStationsGridModel>();

            var data = _printBll.GetHistoryExportPollingStations(pageRequest, printSessionId);

            return data.ToJqGridJsonResult<ExportPollingStation, ExportPollingStationsGridModel>();
        }

        public ActionResult ListPrintSessionsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<PrintSessionsGridModel>();

            var data = _printBll.GetExportPrintSessions(pageRequest);

            return data.ToJqGridJsonResult<PrintSession, PrintSessionsGridModel>();
        }

        public ActionResult ListSaiseExporterAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<SaiseExporterGridModel>();

            var data = _printBll.GetSaiseExporter(pageRequest);

            return data.ToJqGridJsonResult<SaiseExporter, SaiseExporterGridModel>();
        }

        public ActionResult ListSaiseExporterStageAjax(JqGridRequest request, long? saiseExporterId)
        {
            var pageRequest = request.ToPageRequest<ExportRsaToSaiseGridModel>();

            var data = _exporterBll.GetSaiseExporter(pageRequest, saiseExporterId);

            return data.ToJqGridJsonResult<SaiseExporterStage, ExportRsaToSaiseGridModel>();
        }

        public ActionResult ListHistorySaiseExporterStageAjax(JqGridRequest request, long? saiseExporterId)
        {
            var pageRequest = request.ToPageRequest<ExportRsaToSaiseGridModel>();

            var data = _exporterBll.GetHistorySaiseExporter(pageRequest, saiseExporterId);

            return data.ToJqGridJsonResult<SaiseExporterStage, ExportRsaToSaiseGridModel>();
        }

        [HttpPost]
        public void CreatePrintSession(long electionId, long[] circumscriptionsId, long[] pollingStationsId)
        {

            using (new OperationContextScope(_electionService.InnerChannel))
            {
                List<ExportPollingStationDto> exportPollingStationsDto = new List<ExportPollingStationDto>();

                OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                ElectionsServiceReference.ElectionPollingStationsRequest req = new ElectionsServiceReference.ElectionPollingStationsRequest();
                req.ElectionCircumscriptionsId = circumscriptionsId;
                req.PageNumber = 1;
                req.PageSize = 10000;
                var listP = new List<long>();
                var ps = _electionService.GetElectionPollingStations(req);
                var electionroundId = ps.Items.First();
                if (pollingStationsId != null && pollingStationsId.Length > 0)
                {
                    
                    var psResult = ps.Items.Where(s => pollingStationsId.Contains(s.Id));
                    foreach (var item in psResult)
                    {
                        exportPollingStationsDto.Add(new ExportPollingStationDto
                        {
                            CircumscriptionId = item.CircumscriptionId,
                            ElectionRoundId = item.ElectionRoundId,
                            PollingStationId = item.PollingStationId,
                            NumberPerElection = string.IsNullOrEmpty(item.NumberPerElection) ? item.Number : item.NumberPerElection
                        });
                        listP.Add(item.PollingStationId);
                       
                    }

                    //try
                    //{
                    //    var elNr = _exporterBll.SetElectionListNr(listP, electionroundId.ElectionRoundId);
                    //}
                    //catch
                    //{

                    //    //
                    //}

                    _printBll.CreatePrintSession(electionId, exportPollingStationsDto);
                    
                }
                else
                {
                    foreach (var item in ps.Items)
                    {
                        exportPollingStationsDto.Add(new ExportPollingStationDto
                        {
                            CircumscriptionId = item.CircumscriptionId,
                            ElectionRoundId = item.ElectionRoundId,
                            PollingStationId = item.PollingStationId,
                            NumberPerElection = string.IsNullOrEmpty(item.NumberPerElection) ? item.Number : item.NumberPerElection
                        });
                        listP.Add(item.PollingStationId);
                    }
                    //try
                    //{
                    //    var elNr = _exporterBll.SetElectionListNr(listP, electionroundId.ElectionRoundId);
                    //}
                    //catch(Exception e)
                    //{

                    //    return;
                    //}


                    _printBll.CreatePrintSession(electionId, exportPollingStationsDto);

                    
                }


                try
                {
                    var regionIdLog = circumscriptionsId == null ? "All Circumscriptions" : string.Join(",", circumscriptionsId.ToArray());
                    var electionIdLog = electionId.ToString();

                    LoggerUtils logEvent = new LoggerUtils();
                    logEvent.LogEvent(LogLevel.Information, Events.VoterExport.Value, Events.VoterExport.Description, new Dictionary<string, string>
                {
                    { Events.VoterExport.Attributes.Region,  regionIdLog},
                    { Events.VoterExport.Attributes.Election,  electionIdLog}
                });
                }
                catch
                { // 
                }


               

            }


        }

        public ActionResult SelectPrintStatus()
        {
            var statuses = Enum.GetValues(typeof(PrintStatus))
                .Cast<PrintStatus>().Select(x => new SelectListItem
                {
                    Value = ((int)x).ToString(),
                    Text = EnumHelper.GetEnumDescription(x),
                }).ToList();

            return PartialView("_Select", statuses);
        }
        public ActionResult SelectStageStatus()
        {
            var statuses = Enum.GetValues(typeof(SaiseExporterStageStatus))
                .Cast<SaiseExporterStageStatus>().Select(x => new SelectListItem
                {
                    Value = ((int)x).ToString(),
                    Text = EnumHelper.GetEnumDescription(x),
                }).ToList();

            return PartialView("_Select", statuses);
        }

        public ActionResult SelectExporterStatus()
        {
            var statuses = Enum.GetValues(typeof(SaiseExporterStatus))
                .Cast<SaiseExporterStatus>().Select(x => new SelectListItem
                {
                    Value = ((int)x).ToString(),
                    Text = EnumHelper.GetEnumDescription(x),
                }).ToList();

            return PartialView("_Select", statuses);
        }

        [HttpPost]
        public ActionResult GetProgressOfSaiseExporter(long saiseExporterId)
        {
            var progres = _exporterBll.GetProgressOfSaiseExporter(saiseExporterId);
            var error = _exporterBll.GetFailedMessageOfSaiseExporter(saiseExporterId);
            return Json(new { Progres = progres, Error = error });
        }

        [HttpPost]
        public ActionResult GetProgressOfPrintSession(long printSessionId)
        {
            var exportPollingStation = _printBll.GetExportPollingStationsByPrintSession(printSessionId);
            var error = exportPollingStation.FirstOrDefault(x => x.Status == PrintStatus.Failed) != null;
            var pollingStationFinished = exportPollingStation.Count(x => x.Status == PrintStatus.Finished);
            var totalPollingStationForExporting = exportPollingStation.Count();
            var progress = pollingStationFinished * 100 / totalPollingStationForExporting;
            return Json(new { PollingStationFinished = pollingStationFinished, Progres = progress, Error = error });
        }

    }
}