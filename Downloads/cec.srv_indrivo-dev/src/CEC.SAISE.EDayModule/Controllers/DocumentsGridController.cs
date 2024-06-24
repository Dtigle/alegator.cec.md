using Amdaris.Domain.Paging;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.DocumentsGrid;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{
    [PermissionRequired(SaisePermissions.DocumentsGridView)]
    public class DocumentsGridController : BaseDataController
    {
        private readonly IDocumentsBll _documentsBll;
        private readonly IAuditEvents _auditEvents;
        private readonly IUserBll _userBll;
        public DocumentsGridController(IAuditEvents auditEvents, IUserBll userBll, IDocumentsBll documentsBll)
        {
            _auditEvents = auditEvents;
            _userBll = userBll;
            _documentsBll = documentsBll;
        }
        // GET: DocumentsGrid
        public ActionResult Index()
        {
            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                _auditEvents.InsertEvents(AuditEventTypeDto.FunctionalManagement.GetEnumDescription(), user, "Accesarea Gestionarea Documentelor Grid", loger);
            }
            catch
            {
                // 
            }
            return View();
        }

        public async Task<ActionResult> ListPollingStationDocumentAjax(JqGridRequest request, long electionId, long templateNameId, long circumscriptionId)
        {

            var pageRequest = request.ToPageRequest<PollingStationDocumentStageGridModel>();
            var user = await _userBll.GetCurrentUserData();
            if (user.IsAdmin || user.CircumscriptionAcces)
            {
                var data = await _documentsBll.GetPollingStationDocuments(pageRequest, electionId, templateNameId, circumscriptionId);
                return data.ToJqGridJsonResult<PollintStationDocumentStageDto, PollingStationDocumentStageGridModel>();
            }
            else
            {
                // Create an empty PageResponse
                var emptyResponse = new PageResponse<PollintStationDocumentStageDto>
                {
                    Items = new List<PollintStationDocumentStageDto>(), // No items
                    Total = 0, // Total count is 0
                    PageSize = pageRequest.PageSize,
                    StartIndex = pageRequest.PageNumber
                };

                // Convert the empty response to a JqGridJsonResult
                return emptyResponse.ToJqGridJsonResult<PollintStationDocumentStageDto, PollingStationDocumentStageGridModel>();
            }
        }

        [AllowAnonymous]
        public ActionResult GetBPStatuses()
        {
            var bpStatuses = BLL.Helpers.PoliticalPartyStatusExtension.GetValuesAsArray<BallotPaperStatus>();

            return PartialView("_Select", bpStatuses.ToSelectListUnencrypted(0, false, null, x => x.Value, x => x.Value.ToString()));
        }
    }
}