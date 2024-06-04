using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models;
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
        public PermissionManageController(IVotingBll votingBll, IPollingStationStageBll psStageBll, IAuditEvents auditEvents, IUserBll userBll, IElectionResultsBll resultsBll)
		{
            _psStageBll = psStageBll;
		    _auditEvents = auditEvents;
		    _userBll = userBll;
            _resultsBll = resultsBll;
            _votingBll = votingBll;
        }

		// GET: PermissionManage
		public ActionResult Index()
		{
		    try
		    {
		        string loger = LoggerUtil.GetIpAddress();
		        var user = _userBll.GetById(User.Identity.GetUserId<long>());

		        _auditEvents.InsertEvents(AuditEventTypeDto.FunctionalManagement.GetEnumDescription(), user, "Accsarea Gestionarea functionala", loger);
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
        public async Task<ActionResult> SubmitResults(DelimitationData delimitation, BallotPaperDataModel model)
        { 
            var ballotPaperDto = Mapper.Map<BallotPaperDataDto>(model);
            var result = await _resultsBll.SaveUpdateResults(ballotPaperDto, BallotPaperStatus.WaitingForApproval);

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
    }
}