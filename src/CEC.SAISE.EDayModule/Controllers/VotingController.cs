using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Amdaris;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.Voting;
using Microsoft.AspNet.Identity;
using NLog;
using LogLevel = CEC.SAISE.EDayModule.LoggingServices.LogLevel;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.VoterView)]
    public class VotingController : BaseDataController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IUserBll _userBll;
        private readonly IVotingBll _votingBll;
        private readonly ILogger _logger;
        private readonly IAuditEvents _auditEvents;

        public VotingController(IConfigurationBll configurationBll, IUserBll userBll, IVotingBll votingBll, ILogger logger, IAuditEvents auditEvents)
        {
            _configurationBll = configurationBll;
            _userBll = userBll;
            _votingBll = votingBll;
            _logger = logger;
            _auditEvents = auditEvents;
        }

        // GET: Voting
        public async Task<ActionResult> Index()
        {
            var userIsAdmin = User.IsInRole("Administrator");
            if (userIsAdmin)
            {
                return View("NotImplementedForAdmin");
            }

            var userData = await _userBll.GetCurrentUserData();

            if (userData.CircumscriptionAcces)
            {
                return View("_NotImplementForUC");
            }

            var aps = await _votingBll.GetAssignedPollingStationAsync(userData.AssignedElection.Id, userData.AssignedPollingStation.Id);

            if (aps == null)
            {
                _logger.Info(string.Format("AssignedPollingStation not found: userId: {0}", User.Identity.GetUserId<long>()));
                return View("Error");
            }

            if (!aps.IsTurnoutEnabled)
            {
                return View("_TurnoutNotEnabled");
            }

            var statistics = await _votingBll.GetStatisticsAsync(userData.AssignedElection.Id, userData.AssignedPollingStation.Id);

            if (!userIsAdmin && !statistics.IsOpen)
            {
                return RedirectToAction("OpenPollingStation");
            }


            var statisticsModel = Mapper.Map<PollingStationStatisticsModel>(statistics);

            var model = new InitialDataModel
            {
                UserData = Mapper.Map<UserDataModel>(userData),
                PollingStationStatistics = statisticsModel
            };

            return View(model);
        }

        public async Task<ActionResult> OpenPollingStation()
        {
            var userIsAdmin = User.IsInRole("Administrator");
            if (userIsAdmin)
            {
                return View("NotImplementedForAdmin");
            }

            var psOpenningTime = _configurationBll.GetPSOpenningTime();
            if ((DateTime.Now.TimeOfDay < psOpenningTime) && !_configurationBll.DebugModeEnabled())
            {
                return View("FunctionalityTimeNotAchieved", psOpenningTime);
            }

            var userData = await _userBll.GetCurrentUserData();
            if (userData.CircumscriptionAcces)
            {
                return View("_NotImplementForUC");
            }
            var openingData = await _votingBll.GetOpeningDataAsync(userData.AssignedElection.Id, userData.AssignedPollingStation.Id);

            var model = new OpeningModel
            {
                UserData = Mapper.Map<UserDataModel>(userData),
                OpeningData = openingData
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> OpenPollingStation(long assignedPollingStationId, int openingVoters)
        {
            try
            {
                var userData = await _userBll.GetCurrentUserData();

                var aps = await _votingBll.GetAssignedPollingStationAsync(
                    userData.AssignedElection.Id,
                    userData.AssignedPollingStation.Id);

                if (!aps.IsOpeningEnabled)
                {
                    _logger.Info(string.Format("OpenPollingStation is disabled. userId: {0}", User.Identity.GetUserId<long>()));
                    return new HttpStatusCodeResult(423);
                }
                string loger = LoggerUtil.GetIpAddress();
                var updateResult = await _votingBll.OpenPollingStationAsync(assignedPollingStationId, openingVoters, loger);
                var openingData = await _votingBll.GetOpeningDataAsync(assignedPollingStationId);

                var result = new OpeningUpdateModel
                {
                    OpeningData = openingData,
                    Result = updateResult
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    string.Format("Input data: userId: {0}, assignedPollingStationId = {1}, openingVoters = {2}",
                        User.Identity.GetUserId<long>(), assignedPollingStationId, openingVoters));
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SearchVoter(string idnp)
        {
            try
            {
                var userData = await _userBll.GetCurrentUserData();

                var aps = await _votingBll.GetAssignedPollingStationAsync(
                    userData.AssignedElection.Id,
                    userData.AssignedPollingStation.Id);

                if (!aps.IsTurnoutEnabled)
                {
                    return new HttpStatusCodeResult(423);
                }
                var loger = LoggerUtil.GetIpAddress();

                var searchResult = await _votingBll.SearchVoterAsync(idnp, loger);

                if (searchResult.Status == VoterSearchStatus.NotAssigned)
                {

                    var data = new VoterAssignementData
                    {
                        Circumscription = userData.AssignedCircumscription,
                        PollingStation = userData.AssignedPollingStation,
                    };
                    data.HidePollingLabel = true;
                    data.HideCircumscriptionLabel = true;
                    var result = _votingBll.CheckUserRegionValidation(long.Parse(idnp), userData.AssignedPollingStation.Id);
                    if (result == VoterUtanStatus.Success || result == VoterUtanStatus.WrongUtan)
                    {

                        if (result == VoterUtanStatus.Success)
                        {
                            data.IsSameCircumscription = true;
                        }
                        data.HideCircumscriptionLabel = false;
                    }
                    searchResult.VoterData.Assignement = data;
                }


                if (searchResult.VoterData != null && searchResult.VoterData.HasVoted)
                {
                    try
                    {
                        var logEvent = new LoggerUtil();
                        var sender = "Eday";
                        var subject = "Alerta de vot multiplu";
                        var message =
                            $"Incercare de votare multipla :  Numele/Prenumele : {searchResult.VoterData.FirstName} {searchResult.VoterData.LastName} , Idnp : {searchResult.VoterData.Idnp} , Act de identitate : {searchResult.VoterData.DocumentNumber} ,  Depistat la sectia de votare: {searchResult.VoterData.CompletPolingStationAdress} .   Data : {DateTime.Now}  ";
                        var allowService = ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"];
                        if (allowService == "1")
                        {
                            logEvent.MLogEvent(LogLevel.Information, AuditEventTypeDto.SearchIdnp.GetEnumDescription(), message, new Dictionary<string, string>
                            {
                                {"Person", idnp },
                                {"Status", "Found"}
                            });
                            MNotifyUtils.Mnotify(sender, message, subject);
                        }
                    }
                    catch
                    {
                        //
                    }

                }
                if (searchResult.VoterData != null && searchResult.VoterData.DateOfBirth.HasValue)
                {
                    searchResult.VoterData.DateOfBirth = DateTime.SpecifyKind(searchResult.VoterData.DateOfBirth.Value, DateTimeKind.Utc);
                }

                await _votingBll.LogSearchEventAsync(idnp, searchResult, loger);

                return Json(searchResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format("Input data: userId: {0}, idnp = {1}", User.Identity.GetUserId<long>(), idnp));
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateVoter(UpdateVoterModel updateData)
        {
            try
            {
                var updateResult = await _votingBll.SaveUpdateVoterAsync(Mapper.Map<VoterUpdateData>(updateData));

                var result = Mapper.Map<UpdateVoterResultModel>(updateResult);
                return Json(result);
            }
            catch (Exception ex)
            {
                string msg;
                if (updateData == null)
                {
                    msg = "updateData is null";
                }
                else
                {
                    msg = string.Format(
                            "Input data: userId = {0}, updateData.AssignedVoterId = {1}, updateData.AssignedVoterStatus = {2}, " +
                            "updateData.ElectionId = {3}, updateData.VoterId = {4}, updateData.VoterStatus = {5}",
                            User.Identity.GetUserId<long>(), updateData.AssignedVoterId, updateData.AssignedVoterStatus,
                            updateData.ElectionId, updateData.VoterId, updateData.VoterStatus);
                }

                _logger.Error(ex, msg);
                throw;
            }
        }
    }
}