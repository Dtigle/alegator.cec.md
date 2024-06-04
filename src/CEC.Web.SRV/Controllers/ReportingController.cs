using Amdaris.Domain;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Web;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Reporting;
using CEC.Web.SRV.Properties;
using CEC.Web.SRV.Resources;
using SoftTehnica.MvcReportViewer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using CEC.Web.SRV.Infrastructure.Logger;
using CEC.Web.SRV.LoggingService;
using System.Security.Claims;


namespace CEC.Web.SRV.Controllers
{
    [Authorize]
    [NHibernateSession]
    [SessionState(SessionStateBehavior.Disabled)]
    public class ReportingController : ReportController
    {
        private readonly IElectionBll _electionBll;
        private readonly IPollingStationBll _pollingStationBll;
        private readonly ILookupBll _lookupBll;

        private readonly ReportServiceReference.ReportServiceClient _reportService;
        private readonly ElectionsServiceReference.ElectionsServiceClient _electionService;
        MessageHeader _messageHeader;


        protected override ICredentials NetworkCredentials
        {
            get
            {
                return new NetworkCredential(ConfigurationManager.AppSettings["MvcReportViewer.Username"], ConfigurationManager.AppSettings["MvcReportViewer.Password"]);
            }
        }

        protected override string ReportServerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"];
            }
        }

        public ReportingController(IElectionBll electionBll,
            IPollingStationBll pollingStationBll, ILookupBll lookupBll)
        {
            _electionBll = electionBll;
            _pollingStationBll = pollingStationBll;
            _lookupBll = lookupBll;
            _reportService = new ReportServiceReference.ReportServiceClient();
            _electionService = new ElectionsServiceReference.ElectionsServiceClient();
            _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures           
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            ViewBag.CurrentLanguage = cultureName.ToUpper();
            ViewBag.CurrentUserName = this.User.Identity.Name;
            ViewBag.UserIsAdmin = this.User.IsInRole("Administrator");

            return base.BeginExecuteCore(callback, state);
        }

        public ActionResult ListPrinting()
        {
            var model = new ListPrintingModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ListPrinting(ListPrintingModel model)
        {
            if (model.ElectionId == 0)
            {
                ModelState.AddModelError("ElectionId", MUI.ListPrintingModel_FieldRequired);
                return View(model);
            }

            if (model.PollingStationId == 0)
            {
                ModelState.AddModelError("PollingStationId", MUI.ListPrintingModel_FieldRequired);
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var election = _electionBll.Get<Election>(model.ElectionId);
                var pollingStation = _electionBll.Get<PollingStation>(model.PollingStationId);
                var pubAdmin = pollingStation.Region.PublicAdministration;

                model.ReportPath = Settings.Default.RS_SRV_VotersList_ReportPath;
                model.ElectionDate = election.ElectionRounds.LastOrDefault().ElectionDate;
                model.ElectionTitleRO = election.ElectionType.Name;
                model.ElectionTitleRU = election.ElectionType.Name;
                model.PollingStationNr = pollingStation.FullNumber;
                model.RegionName = pollingStation.Region.GetFullName();
                model.ManagerTypeName = pubAdmin != null ? pubAdmin.ManagerType.Name : MUI.MissingData;
                model.ManagerName = pubAdmin != null ? pubAdmin.GetManagerFullName() : MUI.MissingData;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult GetPollingStationbyRegions(Select2Request request, long[] regionsId)
        {
            var pageRequest = request.ToPageRequest("FullNumber", ComparisonOperator.Contains);
            var data = _pollingStationBll.GetPollingStationsByRegions(pageRequest, regionsId);
            var response = new Select2PagedResponse(
                data.Items.ToSelectSelect2List(
                    x => x.Id,
                    x => string.Format("{0} - {1}", x.FullNumber, x.PollingStationAddress != null
                                                                    ? x.GetFullAddress()
                                                                    : MUI.FilterForVoters_PollingStation_MissingAddress)).ToList(),
                    data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCircumscriptions(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _lookupBll.GetCircumscriptions(pageRequest);
            var response = new Select2PagedResponse(
                data.Items.ToSelectSelect2List(
                    x => x.Id,
                    x => string.Format("{0} - {1}", x.NameRo, x.Number)).ToList(),
                    data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPollingStationbyCircumscriptions(Select2Request request, long[] circumscriptionsId)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            using (new OperationContextScope(_electionService.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                ElectionsServiceReference.ElectionPollingStationsRequest req = new ElectionsServiceReference.ElectionPollingStationsRequest();
                req.ElectionCircumscriptionsId = circumscriptionsId;
                req.PageNumber = pageRequest.PageNumber;
                req.PageSize = pageRequest.PageSize;

                var ps = _electionService.GetElectionPollingStations(req);

                var response = new Select2PagedResponse(
                    ps.Items.ToSelectSelect2List(
                        x => x.Id,
                        x => string.Format("{0} - {1}", x.Name, x.Number)).ToList(),
                        ps.Total, ps.PageSize);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCircumscriptionsFromAdmin(Select2Request request, long electionRoundId)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);

            using (new OperationContextScope(_electionService.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                ElectionsServiceReference.ElectionCircumscriptionsRequest req = new ElectionsServiceReference.ElectionCircumscriptionsRequest();
                req.ElectionRoundId = electionRoundId;
                req.PageNumber = pageRequest.PageNumber;
                req.PageSize = pageRequest.PageSize;
                req.ExpandPollingStations = false;
                var circumscriptions = _electionService.GetElectionCircumscriptions(req);

                var response = new Select2PagedResponse(
                    circumscriptions.Items.ToSelectSelect2List(
                        x => x.Id,
                        x => string.Format("{0} - {1}", x.NameRo, x.Number)).ToList(),
                        circumscriptions.Total, circumscriptions.PageSize);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PollingStationsBorders()
        {
            var model = new PollingStationBordersModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PollingStationsBorders(PollingStationBordersModel model)
        {
            if (model.RegionId == 0)
            {
                ModelState.AddModelError("PollingStationId", MUI.PSBordersModel_RegionRequired);
                return View(model);
            }

            if (ModelState.IsValid)
            {
                model.ReportPath = Settings.Default.RS_SRV_PSBordersReport;
            }
            return View(model);
        }


        public ActionResult Index()
        {
            List<SelectListItem> reportsList = new List<SelectListItem>();

            using (new OperationContextScope(_reportService.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                var role = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
                var reports = _reportService.GetAppReports(role.Value);
                if (reports != null && reports.Reports.Length > 0)
                {
                    reportsList = reports.Reports.Select(s => new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Path
                    }).ToList();

                    reportsList.Insert(0, new SelectListItem { Text = "-", Value = "" });
                }
            }

            ViewData["Reports"] = reportsList;

            return View();
        }

        public FileContentResult GetReportImage(string originalPath)
        {
            var rawUrl = this.Request.RawUrl.UrlDecode();
            var startIndex = rawUrl.IndexOf(originalPath);
            if (startIndex > -1)
            {
                originalPath = rawUrl.Substring(startIndex);
            }

            var client = new System.Net.WebClient();
            client.Credentials = this.NetworkCredentials;

            var imageData = client.DownloadData(originalPath);

            return new FileContentResult(imageData, "image/png");
        }


        public ActionResult ViewReport(string path)
        {
            var model = this.GetReportViewerModel(Request);
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var reportName = string.Empty;

                    using (new OperationContextScope(_reportService.InnerChannel))
                    {
                        var role = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
                        OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                        var reports = _reportService.GetAppReports(role.Value);

                        if (reports != null && reports.Reports.Length > 0)
                        {
                            reportName = reports.Reports.FirstOrDefault(x => x.Path == path)?.Name;
                        }
                    }
                    LoggerUtils logEvent = new LoggerUtils();
                    logEvent.LogEvent(LogLevel.Information, Events.Reports.Value, Events.Reports.Description, new Dictionary<string, string>
                    {
                        { Events.Reports.Attributes.Report, reportName},
                    });
                }
                catch (Exception ex)
                { // 
                }
                model.Credentials = this.NetworkCredentials;
                model.AjaxLoadInitialReport = false;
                model.ReportPath = path;
                model.ViewMode = ReportViewModes.Export;

                //model.DataSourceDefinition = "/Data Sources/CEC_SRV";
                //model.ConnectString = "Data Source=192.168.1.84;Initial Catalog=CEC.SRV";
                //model.UserName = "sa";
                //model.Password = "Cecadmin2018";

                try
                {

                    var definedParameters = ReportServiceHelpers.GetReportParameters(model, false);
                    foreach (var definedParameter in definedParameters)
                    {
                        Dictionary<string, object> configs = new Dictionary<string, object>()
                        {
                            { "label",definedParameter.Prompt}
                        };
                        model.AddParameter(new ReportParameter(definedParameter.Name, string.Empty, configs));
                    }
                }
                catch(Exception ex)
                {

                }


            }
            return PartialView("_ViewReport", model);
        }

        public JsonResult GetEntities(Select2Request request, string type)
        {

            dynamic data = null;
            Select2PagedResponse response = null;
            switch (type)
            {
                case "regionId":
                    var pageRequestRegion = request.ToPageRequest("Name", ComparisonOperator.Contains);
                    data = _lookupBll.SearchEntity<Region>(pageRequestRegion);
                    response = new Select2PagedResponse((data as PageResponse<Region>).Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(), data.Total, data.PageSize);
                    return Json(response, JsonRequestBehavior.AllowGet);
                case "pollingStationId":
                    var pageRequestPollingStation = request.ToPageRequest("Location", ComparisonOperator.Contains);
                    data = _lookupBll.SearchEntity<PollingStation>(pageRequestPollingStation);
                    response = new Select2PagedResponse((data as PageResponse<PollingStation>).Items.ToSelectSelect2List(x => x.Id, x => x.Location).ToList(), data.Total, data.PageSize);
                    return Json(response, JsonRequestBehavior.AllowGet);
                default:
                    return Json(string.Empty);
            }
        }

        public JsonResult GetEntityName(long id, string type)
        {

            Entity entity = null;

            switch (type)
            {
                case "regionId":
                    entity = _lookupBll.Get<Region>(id);
                    return Json(entity != null ? (entity as Region).GetFullName() : string.Empty);
                case "pollingStationId":
                    entity = _lookupBll.Get<PollingStation>(id);
                    return Json(entity != null ? (entity as PollingStation).Location : string.Empty);
                default:
                    return Json(string.Empty);
            }
        }
    }
}