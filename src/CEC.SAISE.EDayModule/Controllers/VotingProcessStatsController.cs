using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Export;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.PermissionManage;
using CEC.SAISE.EDayModule.Models.VotingProcessStats;
using CEC.SAISE.EDayModule.Properties;

using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.StatisticsView, SaisePermissions.PoliticalPartyView)]
    public class VotingProcessStatsController : BaseDataController
    {
        private readonly IPollingStationStageBll _psStageBll;
        private readonly IAuditEvents _udiAuditEvents;
        private readonly IUserBll _userBll;

        public VotingProcessStatsController(IPollingStationStageBll psStageBll, IAuditEvents audtiAuditEvents ,IUserBll userBll)
        {
            _psStageBll = psStageBll;
            _udiAuditEvents = audtiAuditEvents;
            _userBll = userBll;
        }

        // GET: VotingProcessStats
        public ActionResult Index()
        {
            string loger = LoggerUtil.GetIpAddress();
            var user = _userBll.GetById(User.Identity.GetUserId<long>());

             _udiAuditEvents.InsertEvents(AuditEventTypeDto.Statistic.GetEnumDescription(), user, "Acesarea Statistici", loger);

            return View();
        }

        //public ActionResult ListVotingStatsAjax(JqGridRequest request, long electionId)
        //{
        //    var pageRequest = request.ToPageRequest<VotingProcessStatsGridModel>();

        //    var data = _psStageBll.GetVotingStatsForUser(pageRequest, electionId, Settings.Default.Turnout_ElectionId);

        //    return data.ToJqGridJsonResult<VotingProcessStatsDto, VotingProcessStatsGridModel>();
        //}
        public JqGridJsonResult ListVotingStatsAjax(JqGridRequest request)
        {
            long strName = Convert.ToInt64(Request["electionId"].ToString());
            
            var pageRequest = request.ToPageRequest<VotingProcessStatsGridModel>();

            var data = _psStageBll.GetVotingStatsForUser(pageRequest, strName, Settings.Default.Turnout_ElectionId);

            return data.ToJqGridJsonResult<VotingProcessStatsDto, VotingProcessStatsGridModel>();
        }



        [AllowAnonymous]
        public ActionResult GetBPStatuses()
        {
            var bpStatuses = Infrastructure.PoliticalPartyStatusExtension.GetValuesAsArray<BallotPaperStatus>();

            return PartialView("_Select", bpStatuses.ToSelectListUnencrypted(0, false, null, x => x.Value, x => x.Key.ToString()));
        }
        [HttpPost]
        public ActionResult ExportStatistic(JqGridRequest request, ExportType exportType ,long electionId)
        {
            return ExportGridData(request, exportType, "ExportStatistic", typeof(VotingProcessStatsGridModel), ListVotingStatsAjax);
        }
    }
}