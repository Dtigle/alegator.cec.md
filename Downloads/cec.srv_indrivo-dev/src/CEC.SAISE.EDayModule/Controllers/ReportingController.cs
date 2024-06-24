using Amdaris.NHibernateProvider.Web;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using SoftTehnica.MvcReportViewer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{
    [Authorize]
    [NHibernateSession]
    [PermissionRequired(SaisePermissions.ReportElectionResults)]
    public class ReportingController : ReportController
    {

        private readonly ReportServiceReference.ReportServiceClient _reportService;
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

        public ReportingController()
        {

            _reportService = new ReportServiceReference.ReportServiceClient();
            _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
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
                }
                catch (Exception ex)
                {
                }
                model.Credentials = this.NetworkCredentials;
                model.AjaxLoadInitialReport = false;
                model.ReportPath = path;
                model.ViewMode = ReportViewModes.Export;

                //var s = ConfigurationManager.ConnectionStrings["AmdarisConnectionString"];
                //var builder = new SqlConnectionStringBuilder(s.ToString());

                //model.DataSourceDefinition = "/Data Sources/EdayDatatSource";
                //model.ConnectString = string.Format("Data Source={0};Initial Catalog={1}", builder.DataSource, builder.InitialCatalog);
                //model.UserName = builder.UserID;
                //model.Password = builder.Password;


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
                catch (Exception ex)
                {

                }


            }
            return PartialView("_ViewReport", model);
        }


    }
}