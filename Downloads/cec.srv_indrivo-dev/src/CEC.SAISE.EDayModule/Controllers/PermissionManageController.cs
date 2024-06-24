using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models;
using CEC.SAISE.EDayModule.Models.DocumentsGrid;
using CEC.SAISE.EDayModule.Models.ElectionResults;
using CEC.SAISE.EDayModule.Models.PermissionManage;
using CEC.SAISE.EDayModule.Models.Voting;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(BLL.Helpers.SaisePermissions.ManageFunctionality)]

    public class PermissionManageController : BaseDataController
    {
        private readonly IPollingStationStageBll _psStageBll;
        private readonly IAuditEvents _auditEvents;
        private readonly IUserBll _userBll;
        private readonly IElectionResultsBll _resultsBll;
        private readonly IVotingBll _votingBll;
        private readonly IDocumentsBll _documentsBll;
        public PermissionManageController(IVotingBll votingBll, IPollingStationStageBll psStageBll, IAuditEvents auditEvents, IUserBll userBll, IElectionResultsBll resultsBll, IDocumentsBll documentsBll)

        {
            _psStageBll = psStageBll;
            _auditEvents = auditEvents;
            _userBll = userBll;
            _resultsBll = resultsBll;
            _votingBll = votingBll;
            _documentsBll = documentsBll;
        }

        // GET: PermissionManage
        public ActionResult Index()
        {
            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                _auditEvents.InsertEvents(AuditEventTypeDto.FunctionalManagement.GetEnumDescription(), user, "Accesarea Gestionarea functionala", loger);
            }
            catch
            {
                // 
            }
            return View();
        }

        public ActionResult ListPollingStationAjax(JqGridRequest request, long electionId)
        {
            var pageRequest = request.ToPageRequest<PollingStationStageEnablerGridModel>();

            var data = _psStageBll.GetPollingStation(pageRequest, electionId);

            return data.ToJqGridJsonResult<PollingStationStageEnablerDto, PollingStationStageEnablerGridModel>();
        }


        [HttpPost]
        public ActionResult ProcessOptions(OptionsToggleModel model)
        {
            _psStageBll.ProcessOptions(Mapper.Map<OptionsToggleDto>(model));

            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult GetBPStatuses()
        {
            var bpStatuses = PoliticalPartyStatusExtension.GetValuesAsArray<BallotPaperStatus>();

            return PartialView("_Select", bpStatuses.ToSelectListUnencrypted(0, false, null, x => x.Value, x => x.Key.ToString()));
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetPoolingStation(int id)
        {
            var result = _psStageBll.GetPollingStationId(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetBallotPapper(DelimitationData delimitationData)
        {
            var userData = await _userBll.GetCurrentUserData();
            var userDataModel = Mapper.Map<UserDataModel>(userData);
            if (!userData.IsAdmin)
            {
                var aps = await _votingBll.GetAssignedPollingStationAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
                if (!aps.IsElectionResultEnabled)
                {

                    return new HttpStatusCodeResult(423);
                }
            }

            var ballotPaper = await _resultsBll.GetBallotPaperAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
            if (ballotPaper.Status == 0)
                return Json("Procesul verbal nu a fost expediat pentru aprobare.");

            var result = Mapper.Map<BallotPaperModel>(ballotPaper);

            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());


                await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Generare proces verbal", loger);
            }
            catch
            {
                //
            }
            var model = new ElectionResultsModel
            {
                UserData = userDataModel,
                BallotPaper = result

            };
            return PartialView("_BallotPaper", model);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitResults(DelimitationData delimitation, BallotPaperDataModel model, long templateNameId)
        {
            var ballotPaperDto = Mapper.Map<BallotPaperDataDto>(model);
            var result = await _resultsBll.SaveUpdateResults(ballotPaperDto, BallotPaperStatus.WaitingForApproval, templateNameId);

            return Json(result);
        }

        [HttpPost]
        [Infrastructure.Security.PermissionRequired(BLL.Helpers.SaisePermissions.AllowElectionResultsVerification)]
        public JsonResult ConfirmResults(List<long> model)
        {
            if (model != null)
            {
                var update = _resultsBll.AproveBallotPaper(model);
                if (update)
                {
                    try
                    {
                        string loger = LoggerUtil.GetIpAddress();
                        var user = _userBll.GetById(User.Identity.GetUserId<long>());

                        _auditEvents.InsertEvents(AuditEventTypeDto.BallotPaperAproved.GetEnumDescription(), user, "Aprobare proces verbal", loger);
                    }
                    catch
                    {
                        //
                    }
                    return Json("Aprobat cu succes", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Sa produs o eroare mai incercați ", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Sa produs o eroare mai incercați ", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Infrastructure.Security.PermissionRequired(BLL.Helpers.SaisePermissions.AllowElectionResultsVerification)]
        public JsonResult ConfirmPollingStationActivityHours(UpdatePollingStationActivityModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { Status = "Error", Errors = errorList });
            }

            try
            {
                var updatePollingStationActivityDto = Mapper.Map<UpdatePollingStationActivityDto>(model);

                if (_psStageBll.UpdatePollingStationActivity(updatePollingStationActivityDto))
                {
                    return Json(new { Status = "Success", Message = "Orele de votare au fost confirmate cu succes." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string loger = LoggerUtil.GetIpAddress();
                    var user = _userBll.GetById(User.Identity.GetUserId<long>());

                    _auditEvents.InsertEvents(AuditEventTypeDto.FunctionalManagement.GetEnumDescription(), user, "Aprobare Orele de votare a esuat", loger);
                    return Json(new { Status = "Error", Message = "Sa produs o eroare mai incercați" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (ValidationException ex)
            {
                return Json(new { Status = "Error", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                _auditEvents.InsertEvents(AuditEventTypeDto.FunctionalManagement.GetEnumDescription(), user, $"Eroare la Aprobare Orele de votare: {ex.Message}", loger);
                return Json(new { Status = "Error", Message = "Sa produs o eroare mai incercați" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetElectionDurationList()
        {
            var electionDurationList = _psStageBll.GetElectionDurations();
            return Json(electionDurationList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Infrastructure.Security.PermissionRequired(BLL.Helpers.SaisePermissions.AllowElectionResultsVerification)]
        public JsonResult SetPollingStationCaptureSignature(List<long> ids, bool capture)
        {
            if (!ModelState.IsValid)
            {
                return Json(GetModelError(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                var result = _psStageBll.ActivateCaptureSignature(ids, capture);
                return Json(result ? GetSuccessResponse() : LogAndRespondError("Captarea semnaturii electronice activata/dezactivata cu succes"));

            }
            catch (Exception ex)
            {
                return LogAndRespondError($"Sa produs o eroare mai incercați: {ex.Message}");
            }
        }

        [HttpPost]
        [Infrastructure.Security.PermissionRequired(BLL.Helpers.SaisePermissions.AllowElectionResultsVerification)]
        public JsonResult SuspendPollingStationActivity(List<long> ids, bool suspend)
        {
            if (!ModelState.IsValid)
                return Json(GetModelError(), JsonRequestBehavior.AllowGet);

            try
            {
                var result = _psStageBll.SuspendPollingStationActivity(ids, suspend);
                return Json(result ? GetSuccessResponse() : LogAndRespondError("Suspendarea/Activarea sectiei de vot a esuat."));
            }
            catch (Exception ex)
            {
                return LogAndRespondError($"Sa produs o eroare mai incercați: {ex.Message}");
            }
        }

        private object GetModelError()
        {
            var errorList = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return new { Status = "Error", Errors = errorList };
        }

        private object GetSuccessResponse()
        {
            return new { Status = "Success", Message = "Operatie realizata cu succes." };
        }

        private JsonResult LogAndRespondError(string errorMessage)
        {
            LogError(errorMessage);
            return Json(new { Status = "Error", Message = "Sa produs o eroare mai incercați." }, JsonRequestBehavior.AllowGet);
        }

        private void LogError(string errorMessage)
        {
            string logger = LoggerUtil.GetIpAddress();
            var user = _userBll.GetById(User.Identity.GetUserId<long>());
            _auditEvents.InsertEvents(AuditEventTypeDto.FunctionalManagement.GetEnumDescription(), user, errorMessage, logger);
        }

    }
}