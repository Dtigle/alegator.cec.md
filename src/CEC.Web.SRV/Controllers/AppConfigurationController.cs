using System;
using System.Web.Mvc;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.AppConfiguration;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Controllers
{
	[Authorize(Roles = Transactions.AppConfiguration)]
	public class AppConfigurationController : BaseController
    {
        private readonly IConfigurationSettingBll _bll;

        public AppConfigurationController(IConfigurationSettingBll bll)
        {
            _bll = bll;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Transactions.AppConfiguration)]
		public ActionResult CreateUpdateConfigurationSetting(long id)
        {
            var model = new UpdateConfigurationSettingModel();
            if (id != 0)
            {
                var configurationSetting = _bll.Get<ConfigurationSetting>(id);
                model.Id = configurationSetting.Id;
                model.Name = configurationSetting.Name;
                model.Value = configurationSetting.Value;
                model.ApplicationName = configurationSetting.ApplicationName;
            }
            return PartialView("_CreateUpdateConfigurationSetting", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUpdateConfigurationSetting(UpdateConfigurationSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateUpdateConfigurationSetting", model);
            }
            if (model.Id == 0)
            {
                ModelState.AddModelError("", MUI.ActionNotSupported);
                return PartialView("_CreateUpdateConfigurationSetting", model);
            }
            
            _bll.UpdateValue(model.Id, model.Value);
            return Content(Const.CloseWindowContent);
        }

        public JqGridJsonResult ListConfigurationSettingsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ConfigurationSettingsGridModel>();

            var data = _bll.Get<ConfigurationSetting>(pageRequest);

            return data.ToJqGridJsonResult<ConfigurationSetting, ConfigurationSettingsGridModel>();
        }
        
        [HttpPost]
        public void DeleteConfigurationSetting(long configSettingId)
        {
            _bll.Delete<ConfigurationSetting>(configSettingId);
        }

        [HttpPost]
        public void UnDeleteConfigurationSetting(long id)
        {
            _bll.UnDelete<ConfigurationSetting>(id);
        }

        [HttpPost]
        public ActionResult ExportConfigurationSettings(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "ConfigurationSettings", typeof(ConfigurationSettingsGridModel), ListConfigurationSettingsAjax);
        }

        public ActionResult GetConfigurationSettingById(long configSettingId)
        {
            var configurationSetting = _bll.Get<ConfigurationSetting>(configSettingId);
            return Json(new {configurationSetting.Name,configurationSetting.Value,configurationSetting.ApplicationName}, JsonRequestBehavior.AllowGet);
        }

    }
}