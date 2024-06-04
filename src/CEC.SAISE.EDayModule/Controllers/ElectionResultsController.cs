using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Amdaris;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models;
using CEC.SAISE.EDayModule.Models.ElectionResults;
using CEC.SAISE.EDayModule.Models.Voting;
using CEC.SRV.BLL.Dto;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.ElectionResultView)]
    public class ElectionResultsController : BaseDataController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IElectionResultsBll _resultsBll;
        private readonly IUserBll _userBll;
        private readonly IVotingBll _votingBll;
        private readonly ILogger _logger;
        private readonly IAuditEvents _auditEvents;




        public ElectionResultsController(IConfigurationBll configurationBll, IElectionResultsBll resultsBll, IUserBll userBll, IVotingBll votingBll, ILogger logger, IAuditEvents auditEvents)
        {
            _configurationBll = configurationBll;
            _resultsBll = resultsBll;
            _userBll = userBll;
            _votingBll = votingBll;
            _logger = logger;
            _auditEvents = auditEvents;
        }

        // GET: ElectionResults
        public async Task<ActionResult> Index()
        {

            var userData = await _userBll.GetCurrentUserData();
            if (userData.CircumscriptionAcces)
            {
                return View("_NotImplementForUC");
            }
            var userDataModel = Mapper.Map<UserDataModel>(userData);
            var model = new ElectionResultsModel
            {
                UserData = userDataModel,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> RetrieveBallotPaper(DelimitationData delimitationData)
        {
            var userData = await _userBll.GetCurrentUserData();
            if (!userData.IsAdmin)
            {
                var aps = await _votingBll.GetAssignedPollingStationAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
                if (!aps.IsElectionResultEnabled)
                {
                    _logger.Info(string.Format("ElectionResults is disabled. userId: {0}", User.Identity.GetUserId<long>()));
                    return new HttpStatusCodeResult(423);
                }
            }

            var ballotPaper = await _resultsBll.GetBallotPaperAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
            var result = Mapper.Map<BallotPaperModel>(ballotPaper);

            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Generare proces verbal", loger);
            }
            catch (Exception e)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitResults(DelimitationData delimitation, BallotPaperDataModel model)
        {
            var aps = await _votingBll.GetAssignedPollingStationAsync(delimitation.ElectionId, delimitation.PollingStationId);
            if (!aps.IsElectionResultEnabled)
            {
                _logger.Info(string.Format("ElectionResults submition is disabled. userId: {0}", User.Identity.GetUserId<long>()));
                return new HttpStatusCodeResult(423);
            }

            if (aps == null)
            {
                return new HttpStatusCodeResult(400, string.Format("Assigned Polling Station was not found by ElectionId:{0} and PollingStationId:{1}", delimitation.ElectionId, delimitation.PollingStationId));
            }

            var ballotPaper = await _resultsBll.GetBallotPaperAsync(model.BallotPaperId);
            if (ballotPaper != null)
            {
                if (ballotPaper.Status != BallotPaperStatus.New)
                {
                    _logger.Info(string.Format("BallotPaper was already submited. userId: {0}", User.Identity.GetUserId<long>()));
                    return new HttpStatusCodeResult(423);
                }
            }
            else
            {
                return new HttpStatusCodeResult(400, string.Format("BallotPaper was not found by Id:{0}", model.BallotPaperId));
            }
            var ballotPaperDto = Mapper.Map<BallotPaperDataDto>(model);
            var result = await _resultsBll.SaveUpdateResults(ballotPaperDto, BallotPaperStatus.WaitingForApproval);

            return Json(result);
        }

        [HttpPost]
        [PermissionRequired(SaisePermissions.AllowElectionResultsVerification)]
        public async Task<ActionResult> ConfirmResults(DelimitationData delimitation, BallotPaperDataModel model)
        {
            var ballotPaperDto = Mapper.Map<BallotPaperDataDto>(model);
            var result = await _resultsBll.SaveUpdateResults(ballotPaperDto, BallotPaperStatus.Approved);
            if (result.Success)
            {
                try
                {
                    string loger = LoggerUtil.GetIpAddress();
                    var user = _userBll.GetById(User.Identity.GetUserId<long>());

                    await _auditEvents.InsertEvents(AuditEventTypeDto.BallotPaperAproved.GetEnumDescription(), user, "Aprobare proces verbal", loger);
                }
                catch
                {
                    //
                }
            }

            return Json(result);
        }
    }
}