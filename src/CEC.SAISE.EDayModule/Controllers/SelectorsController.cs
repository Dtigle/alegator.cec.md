using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SRV.BLL.Dto;

namespace CEC.SAISE.EDayModule.Controllers
{
    public class SelectorsController : BaseDataController
    {
        private readonly IUserBll _userBll;

        public SelectorsController(IUserBll userBll)
        {
            _userBll = userBll;
        }

        public async Task<ActionResult> SelectElections(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Comments", ComparisonOperator.Contains);
            var data = await _userBll.GetAccessibleElectionsAsync(pageRequest);

            var response =
                new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(),
                    data.Total, data.PageSize);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectCircumscription(Select2Request request, long? electionId)
        {
            var pageRequest = request.ToPageRequest("NameRo", ComparisonOperator.Contains);
            var data = _userBll.GetAccessibleCircumscriptions(pageRequest, electionId);

            var response =
                new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(),
                    data.Total, data.PageSize);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectRegions(Select2Request request, long electionId, long? circumscriptionId)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _userBll.GetAccessibleRegions(pageRequest, electionId, circumscriptionId);

            var response =
                new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.Name).ToList(),
                    data.Total, data.PageSize);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectPollingStations(Select2Request request, long electionId, long? circumscriptionId, long? regionId)
        {
            var pageRequest = request.ToPageRequest("FullName", ComparisonOperator.Contains);
            var data = _userBll.GetAccessiblePollingStations(pageRequest, electionId, circumscriptionId, regionId);

            var response =
                new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.FullName).ToList(),
                    data.Total, data.PageSize);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}