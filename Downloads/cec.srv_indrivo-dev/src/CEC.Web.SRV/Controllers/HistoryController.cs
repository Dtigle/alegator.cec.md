using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.History;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Controllers
{
    public class HistoryController : BaseController
    {
        private readonly IAuditerBll _auditerBll;

        public HistoryController(IAuditerBll auditerBll)
        {
            _auditerBll = auditerBll;
        }

        public ActionResult ManagerTypeHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListManagerTypeHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<LookupHistoryGridModel>();

            var data = _auditerBll.Get<ManagerType>(pageRequest, id);

            return data.ToJqGridJsonResult<ManagerType, LookupHistoryGridModel>();
        }

        public ActionResult ConflictShareReasonHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListConflictShareReasonHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<LookupHistoryGridModel>();

            var data = _auditerBll.Get<ConflictShareReasonTypes>(pageRequest, id);

            return data.ToJqGridJsonResult<ConflictShareReasonTypes, LookupHistoryGridModel>();
        }


        public ActionResult GenderHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListGenderHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<LookupHistoryGridModel>();

            var data = _auditerBll.Get<Gender>(pageRequest, id);

            return data.ToJqGridJsonResult<Gender, LookupHistoryGridModel>();
        }

        public ActionResult ElectionTypeHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListElectionTypeHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<LookupHistoryGridModel>();

            var data = _auditerBll.Get<ElectionType>(pageRequest, id);

            return data.ToJqGridJsonResult<ElectionType, LookupHistoryGridModel>();
        }

        public ActionResult ElectionHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListElectionHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<ElectionHistoryGridModel>();

            var data = _auditerBll.Get<Election>(pageRequest, id);

            return data.ToJqGridJsonResult<Election, ElectionHistoryGridModel>();
        }

        public ActionResult StreetTypeHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListStreetTypeHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<LookupHistoryGridModel>();

            var data = _auditerBll.Get<StreetType>(pageRequest, id);

            return data.ToJqGridJsonResult<StreetType, LookupHistoryGridModel>();
        }

        public ActionResult PersonStatusHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListPersonStatusHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<PersonStatusHistoryGridModel>();

            var data = _auditerBll.Get<PersonStatusType>(pageRequest, id);

            return data.ToJqGridJsonResult<PersonStatusType, PersonStatusHistoryGridModel>();
        }

        public ActionResult DocumentTypeHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListDocumentTypeHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<DocumentTypeHistoryGridModel>();

            var data = _auditerBll.Get<DocumentType>(pageRequest, id);

            return data.ToJqGridJsonResult<DocumentType, DocumentTypeHistoryGridModel>();
        }

        public ActionResult RegionTypeHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListRegionTypeHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<RegionTypeHistoryGridModel>();

            var data = _auditerBll.Get<RegionType>(pageRequest, id);

            return data.ToJqGridJsonResult<RegionType, RegionTypeHistoryGridModel>();
        }

        public ActionResult RegionHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListRegionHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<RegionHistoryGridModel>();

            var data = _auditerBll.Get<Region>(pageRequest, id);

            return data.ToJqGridJsonResult<Region, RegionHistoryGridModel>();
        }

        public ActionResult StreetHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListStreetHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<StreetsHistoryGridModel>();

            var data = _auditerBll.Get<Street>(pageRequest, id);

            return data.ToJqGridJsonResult<Street, StreetsHistoryGridModel>();
        }

        public ActionResult PollingStationHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListPollingStationHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<PollingStationHistoryGridModel>();

            var data = _auditerBll.Get<PollingStation>(pageRequest, id);

            return data.ToJqGridJsonResult<PollingStation, PollingStationHistoryGridModel>();
        }

        public ActionResult AddressHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListAddressHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<AddressHistoryGridModel>();

            var data = _auditerBll.Get<Address>(pageRequest, id);

            return data.ToJqGridJsonResult<Address, AddressHistoryGridModel>();
        }

        public ActionResult PersonAddressTypeHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListPersonAddressTypeHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<LookupHistoryGridModel>();

            var data = _auditerBll.Get<PersonAddressType>(pageRequest, id);

            return data.ToJqGridJsonResult<PersonAddressType, LookupHistoryGridModel>();
        }

        public ActionResult ConfigurationSettingHistory(long id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public JqGridJsonResult ListConfigurationSettingHistoryAjax(JqGridRequest request, long id)
        {
            var pageRequest = request.ToPageRequest<ConfigurationSettingHistoryGridModel>();

            var data = _auditerBll.Get<ConfigurationSetting>(pageRequest, id);

            return data.ToJqGridJsonResult<ConfigurationSetting, ConfigurationSettingHistoryGridModel>();
        }
    }
}