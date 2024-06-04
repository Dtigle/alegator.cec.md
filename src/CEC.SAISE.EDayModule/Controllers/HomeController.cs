using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Models.Home;
using CEC.SAISE.EDayModule.Models.Voting;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Models;

namespace CEC.SAISE.EDayModule.Controllers
{
    public class HomeController : BaseDataController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IUserBll _userBll;
        private readonly IVotingBll _votingBll;
        private const string _docsFolder = "~/Content/docs";
        private const string _reportsFolder = _docsFolder + "/reports";
        private const string _exercisesFolder = _docsFolder + "/exercises";

        public HomeController(IConfigurationBll configurationBll, IUserBll userBll, IVotingBll votingBll)
        {
            _configurationBll = configurationBll;
            _userBll = userBll;
            _votingBll = votingBll;
        }

        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return AdminIndex();
            }

            if (User.Identity.HasAnyPermission(SaisePermissions.PoliticalPartyView, SaisePermissions.StatisticsView, SaisePermissions.VoterCertificateCreate))
            {
                return await CandidatesRegistratorIndex();
            }

            if (User.Identity.HasPermission(SaisePermissions.VoterView))
            {
                return await PollingOfficerIndex();
            }

            return View();
        }

        public ActionResult AdminIndex()
        {
            return View("_AdminIndex");
        }

        public async Task<ActionResult> CandidatesRegistratorIndex()
        {
            var userData = await _userBll.GetCurrentUserData();
            var model = Mapper.Map<UserDataModel>(userData);

            return View("_CandidatesRegistratorIndex", model);
        }

        public async Task<ActionResult> PollingOfficerIndex()
        {
            var userData = await _userBll.GetCurrentUserData();
            StatisticsDto statistics = null;
            statistics = await _votingBll.GetStatisticsAsync(userData.AssignedElection.Id, userData.AssignedPollingStation.Id);

            var statisticsModel = Mapper.Map<PollingStationStatisticsModel>(statistics);

            var model = new DashboardModel
            {
                UserData = Mapper.Map<UserDataModel>(userData),
                PollingStationStatistics = statisticsModel,
                PSOpenningStartTime = _configurationBll.GetPSOpenningTime(),
                PSTournoutsStartTime = _configurationBll.GetPSTurnoutsTime(),
                PSElectionResultsStartTime = _configurationBll.GetPSElectionResultsTime(),
                ShowExercises = System.IO.Directory.Exists(Server.MapPath(_exercisesFolder))
            };

            return View("_PollingOfficerIndex", model);
        }

        public async Task<ActionResult> GetAccessibleReports()
        {
            var userData = await _userBll.GetCurrentUserData();
            var accessibleElections = await _userBll.GetAccessibleElections();
            var accessibleReports = new List<ReportLinkModel>();

            foreach (var election in accessibleElections)
            {
                if (election.Type.Id != ElectionType.Local_ConsilieriLocal &&
                    election.Type.Id != ElectionType.Local_PrimarLocal &&
                    userData.AssignedRegion.Id > 0)
                {
                    var reportLink = new ReportLinkModel
                    {
                        Title = string.Format("Raport de totalizare pentru {0}, circumscripția: {1}", election.Comments,
                                userData.AssignedRegion.Name),
                        Path = string.Format("{0}/{1}/{2}.pdf", _reportsFolder, election.Id, userData.AssignedRegion.Id)
                    };
                    reportLink.IsAvailable = System.IO.File.Exists(Server.MapPath(reportLink.Path));
                    accessibleReports.Add(reportLink);
                }
            }

            return PartialView("_AccessibleReports", accessibleReports.Where(x => x.IsAvailable));
        }

        public async Task<ActionResult> GetTestExercises()
        {
            var userData = await _userBll.GetCurrentUserData();
            var accessibleElections = await _userBll.GetAccessibleElections();
            var accessibleExercises = new List<ExerciseLinkModel>();

            foreach (var election in accessibleElections)
            {
                var exerciseLink = new ExerciseLinkModel
                {
                    Election = new ValueNameModel(userData.AssignedElection),
                    Region = new ValueNameModel(userData.AssignedRegion),
                    PollingStation = new ValueNameModel(userData.AssignedPollingStation),
                    Path = string.Format("{0}/{1}/{2}.html", _exercisesFolder, election.Id, userData.AssignedPollingStation.Id)
                };
                exerciseLink.IsAvailable = System.IO.File.Exists(Server.MapPath(exerciseLink.Path));
                accessibleExercises.Add(exerciseLink);
            }

            return PartialView("_ExercisesLinks", accessibleExercises.Where(x => x.IsAvailable));
        }
    }
}