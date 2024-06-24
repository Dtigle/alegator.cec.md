using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.PollingStation;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CEC.Web.SRV.Controllers
{
    [Authorize]
    [Authorize(Roles = Transactions.Lookup + "," + Transactions.LookupRegister)]
    public class PollingStationController : BaseController
    {
        private readonly IPollingStationBll _bll;

        public PollingStationController(IPollingStationBll bll)
        {
            _bll = bll;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateUpdatePollingStation(long regionId, long? pollingStationId)
        {
            if (pollingStationId.HasValue)
            {
                _bll.VerificationIsDeletedSrv<PollingStation>(pollingStationId.Value);
            }
            var region = _bll.Get<Region>(regionId);
            _bll.VerificationIsRegionDeleted(regionId);

            if (!region.HasStreets)
            {
                //var baseModel = new UpdatePollingStationBaseModel();
                //if (pollingStationId.HasValue)
                //{
                //    var pollingStation = _bll.Get<PollingStation>(pollingStationId.Value);
                //    baseModel = Mapper.Map<PollingStation, UpdatePollingStationBaseModel>(pollingStation);
                //}
                //baseModel.RegionId = regionId;
                //baseModel.CircumscriptionNumber = _bll.GetCircumscription(regionId);
                //return PartialView("_CreateUpdatePollingStationNoAddressPartial", baseModel);

                return PartialView("_HasNoStreetsMessage");
            }

            var model = new UpdatePollingStationModel();
            if (pollingStationId.HasValue)
            {
                var pollingStation = _bll.Get<PollingStation>(pollingStationId.Value);
                model = Mapper.Map<PollingStation, UpdatePollingStationModel>(pollingStation);
            }
            model.RegionId = regionId;
            model.CircumscriptionNumber = _bll.GetCircumscription(regionId);

            SetPollingStationViewData(model);
            return PartialView("_CreateUpdatePollingStationPartial", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateUpdatePollingStation(UpdatePollingStationModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    if (model.AddressId == 0)
            //    {
            //        SetPollingStationViewData(model);
            //        return PartialView("_CreateUpdatePollingStationPartial", model);
            //    }
            //    return PartialView("_CreateUpdatePollingStationNoAddressPartial", model);
            //}


            var pollingStation = _bll.Get<PollingStation>(model.Id);
            var existingModel = Mapper.Map<PollingStation, UpdatePollingStationModel>(pollingStation);



            // formating polling station Number
            var numberFormated = String.Join("/", existingModel.Number.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.PadLeft(3, '0')));

            _bll.CreateUpdatePollingStation(existingModel.Id, existingModel.RegionId, numberFormated, existingModel.Location,
                model.AddressId.GetValueOrDefault(), existingModel.ContactInfo, existingModel.SaiseId, existingModel.PollingStationType);

            ElectionsServiceReference.ElectionsServiceClient _electionService = new ElectionsServiceReference.ElectionsServiceClient();

            try
            {
                MessageHeader _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
                using (new OperationContextScope(_electionService.InnerChannel))
                {
                    string fullAddress = string.Empty;
                    if (model.AddressId.HasValue && model.AddressId.Value != 0)
                    {
                        var address = _bll.Get<Address>(model.AddressId.Value);
                        fullAddress = address != null ? address.GetFullAddress() : string.Empty;
                    }
                    OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                    _electionService.SetPollingStationAddress(model.Id, fullAddress);
                }
            }
            catch { }
            return Content(Const.CloseWindowContent);
        }


        [HttpPost]
        public ActionResult SynkAdminPollingStation()
        {
            try
            {
                ElectionsServiceReference.ElectionsServiceClient _electionService = new ElectionsServiceReference.ElectionsServiceClient();
                var ps = _bll.GetAll<PollingStation>();
                foreach (var p in ps)
                {
                    MessageHeader _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
                    using (new OperationContextScope(_electionService.InnerChannel))
                    {
                        string fullAddress = string.Empty;
                        if (p.PollingStationAddress != null)
                        {
                            var address = _bll.Get<Address>(p.PollingStationAddress.Id);
                            fullAddress = address != null ? address.GetFullAddress() : p.Location;
                            OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                            _electionService.SetPollingStationAddress(p.Id, fullAddress);
                        }

                    }
                }
            }
            catch { }
            return Content(Const.CloseWindowContent);
        }

        private void SetPollingStationViewData(UpdatePollingStationModel model)
        {
            var pollingStationTypes = SearchableColumnsHelper.GetPollingStationTypes();
            pollingStationTypes.Remove("");
            ViewData["PollingStationType"] = pollingStationTypes.ToSelectList(model.PollingStationType.ToString());

            var votersListOrderTypes = GetVotersListOrderTypes().Where(x => x.Deleted == null);
            ViewData["VotersListOrderType"] = votersListOrderTypes.ToSelectListUnencrypted(model.VotersListOrderType.HasValue ? model.VotersListOrderType.Value : -1, false, "Implicit", x => x.Description, x => x.Id);
        }

        public IList<VotersListOrderType> GetVotersListOrderTypes()
        {
            return _bll.GetAll<VotersListOrderType>();
        }


        public ActionResult SelectVotersListOrderTypes()
        {
            var items = _bll.GetAll<VotersListOrderType>(); ;

            return PartialView("_Select", items.ToSelectListUnencrypted(0, false, null, x => x.Description, x => x.Id));
        }

        [HttpPost]
        public void DeletePollingStation(long pollingStationId)
        {
            _bll.VerificationIfPollingStationHasReference(pollingStationId);
            _bll.DeletePollingStation(pollingStationId);
        }

        [HttpPost]
        public void UnDeletePollingStation(long id)
        {
            var pollingStation = _bll.Get<PollingStation>(id);
            _bll.VerificationIsRegionDeleted(pollingStation.Region.Id);
            _bll.UnDelete<PollingStation>(id);
        }

        public JqGridJsonResult ListPollingStationAjax(JqGridRequest request, long? regionId)
        {
            var pageRequest = request.ToPageRequest<PollingStationGridModel>();

            var data = _bll.GetPollingStation(pageRequest, regionId.GetValueOrDefault());

            return data.ToJqGridJsonResult<PollingStationDto, PollingStationGridModel>();
        }

        public JsonResult GetAddressName(long id)
        {
            var address = _bll.Get<Address>(id);
            return Json(address != null ? address.GetFullAddress() : string.Empty);
        }

        public JsonResult GetStreetName(long id)
        {
            var street = _bll.Get<Street>(id);
            return Json(street != null ? street.GetFullName() : string.Empty);
        }

        [HttpPost]
        public ActionResult ExportPollingStations(JqGridRequest request, ExportType exportType, long? regionId)
        {
            return ExportGridData(request, exportType, "PollingStations", typeof(PollingStationGridModel), x => ListPollingStationAjax(x, regionId));
        }

        [HttpGet]
        public ActionResult AddElectionNumberList()
        {
            return PartialView("_SortingVotersBy");
        }

        [HttpPost]
        public async Task<ActionResult> AddElectionNumberList(ElectionNumberListOrderBy model)
        {

            try
            {
                await _bll.AdddElectionNumberList(Mapper.Map<ElectionNumberListOrderByDto>(model));
            }
            catch (Exception e)
            {

            }
            return Content("Succes");
        }
        public JsonResult GetDateForOrder(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _bll.SearchOrderType(pageRequest);
            var response = new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.Name).ToList(),
                data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}