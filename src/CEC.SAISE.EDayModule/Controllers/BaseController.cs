using CEC.SAISE.EDayModule.Infrastructure.Export;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace CEC.SAISE.EDayModule.Controllers
{
    [Authorize]
    [SessionState(SessionStateBehavior.Disabled)]
    public class BaseController : Controller
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
        public ActionResult GetFile(string fn)
        {
            var exportFileInfo = GridExporter.GetFile(fn);

            return File(exportFileInfo.FileStream, exportFileInfo.MimeType, fn);
        }
        protected ActionResult ExportGridData(JqGridRequest request, ExportType exportType,
           string dataSetName, Type gridModelType, Func<JqGridRequest, JqGridJsonResult> readDataAction)
        {
            // using (var exporter = new ExcelExporter(dataSetName, gridModelType))
            using (var exporter = new CsvExporter(dataSetName, gridModelType))
            {
                // reset page index and number of records to be returned if we want full table
                if (exportType == ExportType.AllData)
                {
                    request.PageIndex = 0;
                    request.RecordsCount = 1000;
                }

                JqGridResponse gridResponse;

                do
                {
                    var gridResult = readDataAction(request);
                    gridResponse = gridResult.Data as JqGridResponse;

                    exporter.Export(gridResponse);

                    // increase PageIndex for a potential next read
                    request.PageIndex++;
                } while (gridResponse != null &&
                         exportType == ExportType.AllData &&
                         (gridResponse.PageIndex < gridResponse.TotalPagesCount));

                var url = Url.Action("GetFile", new { fn = exporter.FileName });

                return Json(url);
            }
        }

    }
}