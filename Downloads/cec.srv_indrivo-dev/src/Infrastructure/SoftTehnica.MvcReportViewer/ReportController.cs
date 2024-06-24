using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SoftTehnica.MvcReportViewer
{
    public abstract class ReportController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var cultureName = "ro";

            //todo: add jquery.cookie and uncomment following code
            /*
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
			*/
            // Modify current thread's cultures           
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            /*
            ViewBag.CurrentLanguage = cultureName.ToUpper();
            */
            ViewBag.CurrentUserName = this.User.Identity.Name;
            ViewBag.UserIsAdmin = this.User.IsInRole("Administrator");

            return base.BeginExecuteCore(callback, state);
        }
        protected abstract System.Net.ICredentials NetworkCredentials { get; }
        protected abstract string ReportServerUrl { get; }

        /// <summary>
        /// This indicates whether or not to replace image urls from your report server to image urls on your local site to act as a proxy
        /// *useful if your report server is not accessible publicly*
        /// </summary>
        protected virtual bool UseCustomReportImagePath { get { return false; } }
        protected virtual bool AjaxLoadInitialReport { get { return true; } }
        protected virtual System.Text.Encoding Encoding { get { return System.Text.Encoding.ASCII; } }

        protected virtual string ReportImagePath
        {
            get
            {
                return "/Reporting/ReportImage/?originalPath={0}";
            }
        }

        protected virtual int? Timeout
        {
            get
            {
                return null;
            }
        }

        public JsonResult ViewReportPage(string reportPath, string HistoryId, int? page = 0)
        {
            var model = this.GetReportViewerModel(Request);
            model.HistoryId = HistoryId;
            model.ViewMode = ReportViewModes.View;
            model.ReportPath = reportPath;
            string content = string.Empty;
            var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0, page, page, true);
            if (contentData.ReportData != null)
            {
                content = HtmlHelperExtensions.BytesToStringConverted(contentData.ReportData);
            }
            if (model.UseCustomReportImagePath && model.ReportImagePath.HasValue())
            {
                content = ReportServiceHelpers.ReplaceImageUrls(model, content);
            }
            var missingParams = model.IsMissingAnyRequiredParameterValues(contentData.Parameters);
            if (missingParams != null && missingParams.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append("<div class='ReportViewerInformation'>Lipsesc parametrii obligatorii pentru a genera raportul: </br>");
                foreach (var missingParam in missingParams)
                {
                    sb.Append(missingParam.Value + "</br>");
                }
                sb.Append("</div>");
                content = sb.ToString();
            }



            var jsonResult = Json(
                new
                {
                    CurrentPage = contentData.CurrentPage,
                    Content = content,
                    TotalPages = contentData.TotalPages,
                    HistoryId = model.HistoryId,
                    togleKey = model.Parameters.FirstOrDefault(s => s.Key == "togleKey")?.Values != null ? string.Join("|", model.Parameters.FirstOrDefault(s => s.Key == "togleKey")?.Values) : string.Empty
                }
                , JsonRequestBehavior.AllowGet
            );

            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public FileResult ExportReport(string reportPath, string format)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.Export;
            model.ReportPath = reportPath;

            var extension = "";
            var mimeType = "";
            switch (format.ToUpper())
            {
                case "CSV":
                    format = "CSV";
                    extension = ".csv";
                    mimeType = "text/csv";
                    break;

                case "MHTML":
                    format = "MHTML";
                    extension = ".mht";
                    mimeType = "message/rfc822";
                    break;

                case "PDF":
                    format = "PDF";
                    extension = ".pdf";
                    mimeType = "application/pdf";
                    break;

                case "TIFF":
                    format = "IMAGE";
                    extension = ".tif";
                    mimeType = "image/tiff";
                    break;

                case "XML":
                    format = "XML";
                    extension = ".xml";
                    mimeType = "text/xml";
                    break;

                case "WORDOPENXML":
                    format = "WORDOPENXML";
                    extension = ".docx";
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;

                case "EXCELOPENXML":
                default:
                    format = "EXCELOPENXML";
                    extension = ".xlsx";
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
            }

            var contentData = ReportServiceHelpers.ExportReportToFormat(model, format, null, null, true);

            var filename = reportPath;
            if (filename.Contains("/"))
            {
                filename = filename.Substring(filename.LastIndexOf("/"));
                filename = filename.Replace("/", "");
            }

            filename = filename + extension;
            //contentData.MimeType
            return File(contentData.ReportData, mimeType, filename);
        }


        public JsonResult FindStringInReport(string reportPath, string searchText, int? page = 0)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.View;
            model.ReportPath = reportPath;

            return Json(ReportServiceHelpers.FindStringInReport(model, searchText, page).ToInt32(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReloadParameters(string reportPath)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.View;
            model.ReportPath = reportPath;

            return Json(SoftTehnica.MvcReportViewer.HtmlHelperExtensions.ParametersToHtmlString(null, model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintReport(string reportPath)
        {
            var model = this.GetReportViewerModel(Request);
            model.ViewMode = ReportViewModes.Print;
            model.ReportPath = reportPath;

            var contentData = ReportServiceHelpers.ExportReportToFormat(model, ReportFormats.Html4_0, null,null, true);
            var content = HtmlHelperExtensions.BytesToStringConverted(contentData.ReportData);
            content = ReportServiceHelpers.ReplaceImageUrls(model, content);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("	<body>");
            //sb.AppendLine($"		<img src='data:image/tiff;base64,{Convert.ToBase64String(contentData.ReportData)}' />");
            sb.AppendLine($"		{content}");
            sb.AppendLine("		<script type='text/javascript'>");
            sb.AppendLine("			(function() {");
            /*
			sb.AppendLine("				var beforePrint = function() {");
			sb.AppendLine("					console.log('Functionality to run before printing.');");
			sb.AppendLine("				};");
			*/
            sb.AppendLine("				var afterPrint = function() {");
            sb.AppendLine("					window.onfocus = function() { window.close(); };");
            sb.AppendLine("					window.onmousemove = function() { window.close(); };");
            sb.AppendLine("				};");

            sb.AppendLine("				if (window.matchMedia) {");
            sb.AppendLine("					var mediaQueryList = window.matchMedia('print');");
            sb.AppendLine("					mediaQueryList.addListener(function(mql) {");
            sb.AppendLine("						if (mql.matches) {");
            //sb.AppendLine("							beforePrint();");
            sb.AppendLine("						} else {");
            sb.AppendLine("							afterPrint();");
            sb.AppendLine("						}");
            sb.AppendLine("					});");
            sb.AppendLine("				}");

            //sb.AppendLine("				window.onbeforeprint = beforePrint;");
            sb.AppendLine("				window.onafterprint = afterPrint;");

            sb.AppendLine("			}());");
            sb.AppendLine("			window.print();");
            sb.AppendLine("		</script>");
            sb.AppendLine("	</body>");

            sb.AppendLine("<html>");

            return Content(sb.ToString());
        }

        public FileContentResult ReportImage(string originalPath)
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

        protected ReportViewerModel GetReportViewerModel(HttpRequestBase request)
        {
            var model = new ReportViewerModel();
            model.AjaxLoadInitialReport = this.AjaxLoadInitialReport;
            model.Credentials = this.NetworkCredentials;

            var enablePagingResult = _getRequestValue(request, "ReportViewerEnablePaging");
            if (enablePagingResult.HasValue())
            {
                model.EnablePaging = enablePagingResult.ToBoolean();
            }
            else
            {
                model.EnablePaging = true;
            }

            model.Encoding = this.Encoding;
            model.ServerUrl = this.ReportServerUrl;
            model.ReportImagePath = this.ReportImagePath;
            model.Timeout = this.Timeout;
            model.UseCustomReportImagePath = this.UseCustomReportImagePath;
            model.BuildParameters(Request);

            return model;
        }
        private string _getRequestValue(HttpRequestBase request, string key)
        {
            var values = request.QueryString.GetValues(key);
            if (values != null && values.Length > 0)
            {
                return values[0].ToSafeString();
            }

            try
            {
                if (request.Form != null && request.Form.Keys != null && request.Form[key] != null)
                {
                    return request.Form[key].ToSafeString();
                }
            }
            catch
            {
                //No need to throw errors, just no Form was passed in and it's unhappy about that
            }

            return String.Empty;
        }

    }
}