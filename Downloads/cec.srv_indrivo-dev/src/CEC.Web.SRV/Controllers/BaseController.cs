using System.Collections.Generic;
using System.IO;
using System.Web.SessionState;
using Amdaris.NHibernateProvider.Web;
using CEC.Web.SRV.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Amdaris.Domain;
using Amdaris.NHibernateProvider.Identity;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Properties;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.AspNet.Identity;
using NHibernate;

namespace CEC.Web.SRV.Controllers
{
    [Authorize]
    [NHibernateSession]
    [SessionState(SessionStateBehavior.Disabled)]
    public abstract class BaseController : Controller
    {
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
            CheckUserIsBlocked();
            return base.BeginExecuteCore(callback, state);
        }

        public ActionResult GetFile(string fn)
        {
            var exportFileInfo = GridExporter.GetFile(fn);

            return File(exportFileInfo.FileStream, exportFileInfo.MimeType, fn);
        }

        private void CheckUserIsBlocked()
        {
            var sessionFactory = IoC.GetSessionFactory();
            if (sessionFactory != null)
            {
                var repository = new SrvRepository(sessionFactory);
                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                    var status = repository.Query<SRVIdentityUser>().FirstOrDefault(x => x.UserName == User.Identity.Name);
                    if (status?.IsBlocked == true) HttpContext.GetOwinContext().Authentication.SignOut();
                }
            }
        }

        protected ActionResult ExportGridData(JqGridRequest request, ExportType exportType,
            string dataSetName, Type gridModelType, Func<JqGridRequest, JqGridJsonResult> readDataAction)
        {
            using (var exporter = new ExcelExporter(dataSetName, gridModelType))
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

        protected ActionResult ExportGridData(JqGridRequest request, ExportType exportType, long? regionId, long? pollingStationId,
			string dataSetName, Type gridModelType, Func<JqGridRequest, long?, long?, JqGridJsonResult> readDataAction)
        {
            using (var exporter = new ExcelExporter(dataSetName, gridModelType))
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
                    var gridResult = readDataAction(request, regionId, pollingStationId);
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