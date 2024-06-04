using Amdaris.Domain.Paging;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.EDayServiceReference;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.EDaySync;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Mvc;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.EDayModule.Infrastructure;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.AdjustDbEday)]
    public class SyncController : BaseDataController
    {

        private readonly ISaiseRepository _repository;
        private readonly IAuditEvents _auditEvents;
        private readonly IUserBll _userBll;
        private readonly MessageHeader _messageHeader;

        public SyncController(IUserBll userBll, IAuditEvents auditEvents, ISaiseRepository repository)
        {
            _userBll = userBll;
            _auditEvents = auditEvents;
            _repository = repository;
            _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
        }

        [HttpGet]
        public ActionResult Index()
        {
            EDayServiceClient _eDayService = new EDayServiceClient();
            using (new OperationContextScope(_eDayService.InnerChannel))
            {
                try
                {

                    var eday = _repository.Query<ElectionDay>().FirstOrDefault();
                    OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                    var response = _eDayService.GetEDayStages(eday.Id);
                    if (response.EDayExporter.Status == "New")
                    {
                        ViewBag.EDaySyncStatus = EDaySyncStatus.InProgress;
                    }
                    ViewBag.EDaySyncStatus = EDaySyncStatus.Done;
                }
                catch (Exception ex)
                {
                    ViewBag.EDaySyncStatus = EDaySyncStatus.MissingConnection;
                }
            }

            return View();
        }


        [HttpPost]
        public ActionResult Index(string t)
        {
            EDayResponse response;
            EDayServiceClient _eDayService = new EDayServiceClient();
            using (new OperationContextScope(_eDayService.InnerChannel))
            {
                var eday = _repository.Query<ElectionDay>().FirstOrDefault();
                OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                 response = _eDayService.SyncEDayData(eday.Id);
                ViewBag.EDaySyncStatus = EDaySyncStatus.InProgress;
            }

            try
            {
                if (response != null)
                {
                    string loger = LoggerUtil.GetIpAddress();
                    var user = _userBll.GetById(User.Identity.GetUserId<long>());
                    if (response.EDayExporter.Stages.Length > 0)
                    {
                        foreach (var item in response.EDayExporter.Stages)
                        {
                            switch (item.Type)
                            {
                                case "CandidateUpdate":
                                    _auditEvents.InsertEvents(AuditEventTypeDto.AdjustCandidate.GetEnumDescription(), user, response.EDayExporter.Id.ToString(), loger);
                                    break;
                                case "VoterUpdate":
                                    _auditEvents.InsertEvents(AuditEventTypeDto.AdjustVoters.GetEnumDescription(), user, response.EDayExporter.Id.ToString(), loger);
                                    break;
                            }
                        }
                    }

                }

            }
            finally
            {

            }
            return View();
        }

        public JqGridJsonResult GetAllEDayStages(JqGridRequest request)
        {
            var pageResp = new PageResponse<EDayStageGridModel>();
            pageResp.Items = new List<EDayStageGridModel>();

            var eDayService = new EDayServiceClient();

            using (new OperationContextScope(eDayService.InnerChannel))
            {
                try
                {
                    var eday = _repository.Query<ElectionDay>().FirstOrDefault();
                    OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                    if (eday != null)
                    {
                        var response = eDayService.GetEDayStages(eday.Id);


                        foreach (var item in response.EDayExporter.Stages.Where(s =>
                            s.Type == "CandidateUpdate" || s.Type == "VoterUpdate"))
                        {
                            var events = new AuditEvents();
                            switch (item.Type)
                            {
                                case "CandidateUpdate":
                                    events = _auditEvents.GetAuditEvents(response.EDayExporter.Id.ToString(), AuditEventTypeDto.AdjustCandidate.GetEnumDescription()).FirstOrDefault();
                                    break;
                                case "VoterUpdate":
                                    events = _auditEvents.GetAuditEvents(response.EDayExporter.Id.ToString(), AuditEventTypeDto.AdjustVoters.GetEnumDescription()).FirstOrDefault();
                                    break;
                            }

                            var gridItem = new EDayStageGridModel
                            {
                                Description = item.Description,
                                Statistics = string.IsNullOrEmpty(item.Statistics) ? "-" : item.Statistics + "%",
                                Status = item.Status == "InProgress"
                                    ? "In execuție"
                                    : (item.Status == "Pending" ? "In asteptare" : "Finisat")
                            };
                            if (events != null)
                            {
                                gridItem.Data = events.EditDate.ToString(CultureInfo.InvariantCulture);
                                gridItem.User = events.userId;
                                gridItem.IpAddress = events.userMachineIp;
                            };

                            pageResp.Items.Add(gridItem);
                        }
                    }
                }
                catch {
                    pageResp.Items = new List<EDayStageGridModel>();
                }
            }
            return pageResp.ToJqGridJsonResult<EDayStageGridModel, EDayStageGridModel>();
        }
    }
}