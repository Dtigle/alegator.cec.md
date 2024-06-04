using Amdaris;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.EDaySync;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.TransferDataToSSRS)]
    public class TransferController : BaseDataController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IElectionResultsBll _resultsBll;
        private readonly IUserBll _userBll;
        private readonly IVotingBll _votingBll;
        private readonly ILogger _logger;
        private readonly IAuditEvents _auditEvents;
        private readonly ISaiseRepository _repository;

        public TransferController(IConfigurationBll configurationBll, IElectionResultsBll resultsBll, IUserBll userBll, IVotingBll votingBll, ISaiseRepository repository, ILogger logger, IAuditEvents auditEvents)
        {
            _configurationBll = configurationBll;
            _resultsBll = resultsBll;
            _userBll = userBll;
            _votingBll = votingBll;
            _repository = repository;
            _logger = logger;
            _auditEvents = auditEvents;
        }

        // GET: Transfer
        public ActionResult Index()
        {
            string resultMessage = string.Empty;
            var eday = _repository.Query<ElectionDay>().FirstOrDefault();
            short statusCode = 0;
            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());
                _auditEvents.InsertEvents(AuditEventTypeDto.Transfer.GetEnumDescription(), user, "Accsarea Transfer Date", loger);
            }
            catch
            {
                //
            }
            var model = new TransferEDayModel() { };
            var LinkedServerExists = _resultsBll.CheckLinkedServerExists();

            ViewBag.LinkedServerExists = LinkedServerExists;
            if (LinkedServerExists && eday.StartDateToReportDb.HasValue)
            {
                try
                {
                    if (!eday.EndDateToReportDb.HasValue)
                    {
                        resultMessage = string.Format(@"Procesul de transfer este în derulare. Vă rugăm așteptați!
                        <br /> Data începere: {0}
                        <br /> Data finalizare: {1}", eday.StartDateToReportDb.HasValue ? eday.StartDateToReportDb.Value.ToString("dd.MM.yyyy HH:mm:ss") : "-", eday.EndDateToReportDb.HasValue ? eday.EndDateToReportDb.Value.ToString("dd.MM.yyyy mm:ss") : "-");
                        statusCode = 1;
                    }
                    else
                    {
                        resultMessage = "Procesul de transfer a fost finisat cu succes!";
                        statusCode = 0;
                    }
                }
                catch (Exception ex)
                {
                    resultMessage = ex.Message;
                    statusCode = 2;
                }
            }

            ViewBag.Eday = eday;

            ViewBag.ExecutionsStatus = resultMessage;
            return View(model);
        }

        [HttpPost]
        public ActionResult ExecuteTransferEDayDatabase(TransferEDayModel model)
        {
            bool LinkedServerExists = false;
            if (ModelState.IsValid)
            {
                using (var uw = new NhUnitOfWork())
                {
                    var t = new Task(() => _resultsBll.TransferEDayData(model.TargetHost, model.TargetUserName, model.TargetPassword));
                    t.Start();
                    uw.Complete();
                }

                using (var uw = new NhUnitOfWork())
                {
                    Thread.Sleep(3000);
                    LinkedServerExists = _resultsBll.CheckLinkedServerExists();
                    uw.Complete();
                }

                return new JsonResult
                {
                    Data = new
                    {
                        ExecStatus = LinkedServerExists ? 1 : 2,
                        ExecMessage = LinkedServerExists ? "Procesul de transfer date a fost initializat!" : "Eroare de conectare la serverul bazei de date de raportare!"
                    }
                };
            }

            return new JsonResult
            {
                Data = new
                {
                    ExecStatus = Status.Error,
                    ExecMessage = "<br />" + string.Join("<br />", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                }
            };
        }

        public ActionResult GetCurrentStatus()
        {
            string resultMessage = string.Empty;
            var eday = _repository.Query<ElectionDay>().FirstOrDefault();
            short statusCode = 0;
            try
            {
                if (eday.StartDateToReportDb.HasValue)
                {
                    if (!eday.EndDateToReportDb.HasValue)
                    {
                        resultMessage = string.Format(@"Procesul de transfer este în derulare. Vă rugăm așteptați!
                        <br /> Data începere: {0}
                        <br /> Data finalizare: {1}", eday.StartDateToReportDb.HasValue? eday.StartDateToReportDb.Value.ToString("dd.MM.yyyy HH:mm:ss"):"-", eday.EndDateToReportDb.HasValue ? eday.EndDateToReportDb.Value.ToString("dd.MM.yyyy mm:ss") : "-");
                        statusCode = 1;
                    }
                    else
                    {
                        resultMessage = "Procesul de transfer a fost finisat cu succes!";
                        statusCode = 0;
                    }
                }
                else
                {
                    statusCode = 3;
                }
            }
            catch (Exception ex)
            {
                resultMessage = ex.Message;
                statusCode = 2;
            }

            return new JsonResult
            {
                Data = new
                {
                    Message = resultMessage,
                    Code = statusCode
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet

            };
        }


        public JqGridJsonResult GetDataTransferStages(JqGridRequest request)
        {
            PageResponse<EDayStageGridModel> pageResp = new PageResponse<EDayStageGridModel>();
            var eday = _repository.Query<ElectionDay>().FirstOrDefault();
            if (eday.StartDateToReportDb.HasValue)
            {
                var t = _resultsBll.GetDataTransferStages();
                if (t != null)
                {
                    pageResp.Items = t.Select(k => new EDayStageGridModel
                    {
                        Description = k.TableName,
                        Statistics = k.Percent + "%",
                        Status = k.Percent < 100 && k.Percent > 0 ? "In execuție" : (k.Percent == 0 ? "In asteptare" : "Finisat")
                    }).ToList();

                }
                else
                {
                    pageResp.Items = new List<EDayStageGridModel>();
                }
            }
            else
            {
                pageResp.Items = new List<EDayStageGridModel>();
            }
            return pageResp.ToJqGridJsonResult<EDayStageGridModel, EDayStageGridModel>();
        }

    }
}