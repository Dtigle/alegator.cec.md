using Amdaris;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain.TemplateManager;
using CEC.SAISE.EDayModule.Helpers;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models;
using CEC.SAISE.EDayModule.Models.Document;
using CEC.SAISE.EDayModule.Models.Voting;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.DocumentsView)]
    public class DocumentsController : BaseDataController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IUserBll _userBll;
        private readonly IVotingBll _votingBll;
        private readonly ILogger _logger;
        private readonly IAuditEvents _auditEvents;
        private readonly IDocumentsBll _documentsBll;

        public DocumentsController(IConfigurationBll configurationBll
            , IUserBll userBll, IVotingBll votingBll, ILogger logger
            , IAuditEvents auditEvents, IDocumentsBll documentsBll, IMinIoBll minIo)


        {
            _configurationBll = configurationBll;
            _userBll = userBll;
            _votingBll = votingBll;
            _logger = logger;
            _auditEvents = auditEvents;
            _documentsBll = documentsBll;
        }

        // GET: Documents
        public async Task<ActionResult> Index()
        {
            var userData = await _userBll.GetCurrentUserData();
            //if (userData.CircumscriptionAcces)
            //{
            //    return View("_NotImplementForUC");
            //}
            var userDataModel = Mapper.Map<UserDataModel>(userData);
            var model = new DocumentModel
            {
                UserData = userDataModel,
            };

            return View(model);
        }

        public async Task<ActionResult> RetrieveParameterValues(DelimitationData delimitationData, long templateNameId)
        {
            var userData = await _userBll.GetCurrentUserData();
            if (!userData.IsAdmin)
            {
                var aps = await _votingBll.GetAssignedPollingStationAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
                if (!aps.IsElectionResultEnabled)
                {
                    _logger.Info($"Documents is disabled. userId: {User.Identity.GetUserId<long>()}");
                    return new HttpStatusCodeResult(423);
                }
            }

            var document = await _documentsBll.GetDocumentAsync(delimitationData.ElectionId, delimitationData.PollingStationId, templateNameId);
            Dictionary<long, string> dictionary = new Dictionary<long, string>();
            if (document == null)
            {
                var parametersList = await _documentsBll.ListTemplateParameters(templateNameId);
                dictionary = parametersList.ToDictionary(
                            rp => rp.ParameterId,
                            rp => string.Empty
                        );
            }
            else
            {
                var parametersList = await _documentsBll.ListDocumentParameters(document.DocumentId);
                dictionary = parametersList.ToDictionary(
                            rp => rp.ReportParameterId,
                            rp => rp.ValueContent
                        );
            }

            switch (templateNameId)
            {
                case 1:
                    return View("YourViewName", dictionary); // replace "YourViewName" with the name of the view you want to display
                default: throw new ArgumentException("Invalid TemplateNameId");
            }

        }


        [HttpPost]
        public async Task<ActionResult> RetrieveDocument(DelimitationData delimitationData, long templateNameId)
        {
            //TO DO Check if method is necessary
            //var userData = await _userBll.GetCurrentUserData();
            //if (!userData.IsAdmin)
            //{
            //    var aps = await _votingBll.GetAssignedPollingStationAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
            //    //if (!aps.IsElectionResultEnabled)
            //    //{
            //    //    _logger.Info(string.Format("Documents is disabled. userId: {0}", User.Identity.GetUserId<long>()));
            //    //    return new HttpStatusCodeResult(423);
            //    //}
            //}

            var document = await _documentsBll.GetDocumentAsync(delimitationData.ElectionId, delimitationData.PollingStationId, templateNameId);

            var result = Mapper.Map<DocumentDataModel>(document);

            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Generare document", loger);
            }
            catch (Exception e)
            {

            }

            return Json(result);
        }


        [HttpPost]
        public async Task<ActionResult> SubmitResults(DelimitationData delimitationData, long templateNameId, DocumentDataModel model)
        {
            var aps = await _votingBll.GetAssignedPollingStationAsync(delimitationData.ElectionId, delimitationData.PollingStationId);
            if (!aps.IsElectionResultEnabled)
            {
                _logger.Info(string.Format("Document submition is disabled. userId: {0}", User.Identity.GetUserId<long>()));
                return new HttpStatusCodeResult(423);
            }

            if (aps == null)
            {
                return new HttpStatusCodeResult(400, string.Format("Assigned Polling Station was not found by ElectionId:{0} and PollingStationId:{1}", delimitationData.ElectionId, delimitationData.PollingStationId));
            }

            var document = await _documentsBll.GetDocumentAsync(delimitationData.ElectionId, delimitationData.PollingStationId, templateNameId);
            if (document != null)
            {
                if (document.Status != (int)DocumentStatus.New)
                {
                    _logger.Info(string.Format("Document was already submitted. userId: {0}", User.Identity.GetUserId<long>()));
                    return new HttpStatusCodeResult(423);
                }
            }

            var documentDto = Mapper.Map<DocumentDto>(model);
            documentDto.ElectionRoundId = aps.ElectionRound.Id;
            documentDto.PollingStationId = aps.PollingStation.Id;

            var result = await _documentsBll.SaveUpdateDocument(documentDto, templateNameId, DocumentStatus.Approved);

            if (result.Success)
            {
                try
                {

                    string loger = LoggerUtil.GetIpAddress();
                    var user = _userBll.GetById(User.Identity.GetUserId<long>());

                    await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Salvare document", loger);

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error occurred while saving or updating the document.");

                    // Return a meaningful error response to the client without exposing internal details
                    return new HttpStatusCodeResult(500, "An internal server error occurred. Please contact support.");
                }
            }
            return Json(result);
        }


        [HttpPost]
        //TO DO Create permissions specific to DocumentManagement
        [PermissionRequired(SaisePermissions.AllowElectionResultsVerification)]
        public async Task<ActionResult> ConfirmDocument(DelimitationData delimitation, DocumentDataModel model)
        {
            long templateNameId = 1;
            var documentDto = Mapper.Map<DocumentDto>(model);
            var result = await _documentsBll.SaveUpdateDocument(documentDto, templateNameId, DocumentStatus.Approved);
            if (result.Success)
            {
                try
                {
                    string loger = LoggerUtil.GetIpAddress();
                    var user = _userBll.GetById(User.Identity.GetUserId<long>());
                    //TO DO use DOcument Name when inserting event.
                    await _auditEvents.InsertEvents(AuditEventTypeDto.BallotPaperAproved.GetEnumDescription(), user, "Aprobare document", loger);
                }
                catch
                {
                    //
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> RetrieveCECEDocument(CircumscriptionDelimitation delimitationData, long templateNameId)
        {

            var document = await _documentsBll.GetCircumscriptionDocumentAsync(delimitationData.ElectionId, delimitationData.CircumscriptionId, templateNameId);

            var result = Mapper.Map<DocumentDataModel>(document);

            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Generare document", loger);
            }
            catch (Exception e)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Final Report CECE

        [HttpPost]
        public async Task<ActionResult> GetUnconfirmedPollingStations(CircumscriptionDelimitation delimitationData, long templateNameId)
        {

            var result = await _documentsBll.GetUnconfirmedPollingStations(delimitationData.ElectionId, delimitationData.CircumscriptionId, templateNameId);
            if (result.Any())
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Generare document", loger);
            }
            catch (Exception e)
            {

            }
            return Json(new List<UnconfirmedPollingStationsDto>(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> RetrieveFinalReportData(CircumscriptionDelimitation delimitationData, long templateNameId)
        {

            var document = await _documentsBll.GetFinalReportDataAsync(delimitationData.ElectionId, delimitationData.CircumscriptionId, templateNameId);

            var result = Mapper.Map<DocumentDataModel>(document);

            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Retribuire document", loger);
            }
            catch (Exception e)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitFinalReportCECEResults(CircumscriptionDelimitation delimitationData, long templateNameId, DocumentDataModel model)
        {

            //TO DO - Uncomment after testing
            //var resultUnconfirmedPollingStations = await _documentsBll.GetUnconfirmedPollingStations(delimitationData.ElectionId, delimitationData.CircumscriptionId, templateNameId);

            //if (resultUnconfirmedPollingStations.Any())
            //{
            //    return new HttpStatusCodeResult(423, "Document cant be submitted, some polling stations have not submitted the Final Report");
            //}

            var document = await _documentsBll.GetCECEDocumentAsync(delimitationData.ElectionId, delimitationData.CircumscriptionId, templateNameId);
            if (document != null)
            {
                if (document.StatusId == (int)DocumentStatus.Approved)
                {
                    _logger.Info(string.Format("Document was already submitted. userId: {0}", User.Identity.GetUserId<long>()));
                    return new HttpStatusCodeResult(423, "Document was already submitted");
                }
            }
            //else
            //{
            //    return new HttpStatusCodeResult(400, string.Format("Document was not found by Id:{0}", model.DocumentId));
            //}

            var documentDto = Mapper.Map<DocumentDto>(model);
            documentDto.AssignedCircumscriptionId = delimitationData.CircumscriptionId;
            var electionRoundId = await _documentsBll.GetElectionRoundIdByElection(delimitationData.ElectionId);
            documentDto.ElectionRoundId = electionRoundId;

            var result = await _documentsBll.SaveUpdateDocument(documentDto, templateNameId, DocumentStatus.Approved);

            if (result.Success)
            {
                try
                {

                    string loger = LoggerUtil.GetIpAddress();
                    var user = _userBll.GetById(User.Identity.GetUserId<long>());

                    await _auditEvents.InsertEvents(AuditEventTypeDto.GenerateBallotPaper.GetEnumDescription(), user, "Salvare document", loger);

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error occurred while saving or updating the document.");

                    // Return a meaningful error response to the client without exposing internal details
                    return new HttpStatusCodeResult(500, "An internal server error occurred. Please contact support.");
                }
            }
            return Json(result);
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> GetBallotPaperDocumentAsync(long electionId, long templateNameId, long? pollingStationId, long? circumscriptionId)
        {
            var result = await _documentsBll.GetBallotPaperDocumentAsync(electionId, templateNameId, pollingStationId, circumscriptionId);
            if (result == null)
            {
                return Json(new { Success = false, Message = "No such document found with specified parameters" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(int? templateNameId)
        {
            var printTemplateMapping = PrintTemplateNameMappings.GetMappings();

            ViewBag.TemplateNameId = templateNameId;
            ViewBag.PrintTemplateMapping = printTemplateMapping;

            return View();
        }
    }
}