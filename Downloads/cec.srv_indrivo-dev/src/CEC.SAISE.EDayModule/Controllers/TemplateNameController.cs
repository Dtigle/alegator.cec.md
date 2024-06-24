using Amdaris;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain.TemplateManager;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.TemplateName;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{
        [PermissionRequired(SaisePermissions.DocumentsView)]
        public class TemplateNameController : BaseDataController
        {
            private readonly ITemplateNameBLL _templateNameBll;
            private readonly ILogger _logger;
            private readonly IAuditEvents _auditEvents;

            public TemplateNameController(ITemplateNameBLL templateNameBll, ILogger logger, IAuditEvents auditEvents)
            {
                _templateNameBll = templateNameBll;
                _logger = logger;
                _auditEvents = auditEvents;
            }

            // GET: TemplateName
            public async Task<ActionResult> Index()
            {
                var templateNames = await _templateNameBll.GetAllTemplateNamesAsync();
                var model = new TemplateNameViewModel
                {
                    TemplateNames = templateNames,
                };

                return View(model);
            }

            [HttpPost]
            public async Task<ActionResult> RetrieveTemplateName(long id)
            {
                var templateName = await _templateNameBll.GetTemplateNameAsync(id);
                if (templateName == null)
                {
                    _logger.Info($"TemplateName with ID: {id} was not found.");
                    return new HttpStatusCodeResult(404);
                }

                var result = Mapper.Map<TemplateNameModel>(templateName);

                // Sample audit log
                try
                {
                    string loger = LoggerUtil.GetIpAddress();
                    var user = User.Identity.GetUserId<long>();

                    //await _auditEvents.InsertEvents("TemplateNameRetrieved", user., "TemplateName retrieved", loger);
                }
                catch (Exception e)
                {
                    // Logging of error or handling
                }

                return Json(result);
            }

            [HttpPost]
            public async Task<ActionResult> SubmitTemplateName(TemplateNameDto model)
            {
                var result = _templateNameBll.AddOrUpdateTemplateNameAsync(model);
                return Json(result);
            }

            [HttpPost]
            public async Task<ActionResult> DeleteTemplateName(long id)
            {
                var success = await _templateNameBll.DeleteTemplateNameAsync(id);

                if (!success)
                {
                    _logger.Info($"Failed to delete TemplateName with ID: {id}.");
                    return new HttpStatusCodeResult(500, "Failed to delete TemplateName.");
                }

                // Sample audit log
                try
                {
                    string loger = LoggerUtil.GetIpAddress();
                    var user = User.Identity.GetUserId<long>();

                    //await _auditEvents.InsertEvents("TemplateNameDeleted", user, "TemplateName deleted", loger);
                }
                catch (Exception e)
                {
                    // Logging of error or handling
                }

                return Json(new { Success = true });
            }
        }
    }