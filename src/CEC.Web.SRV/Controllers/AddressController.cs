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
using CEC.Web.SRV.Models.Address;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CEC.Web.SRV.Controllers
{
    [Authorize]
    public class AddressController : BaseController
    {
        private readonly IAddressBll _bll;

        public AddressController(IAddressBll bll)
        {
            _bll = bll;
        }

	    [Authorize(Roles = Transactions.Lookup + "," + Transactions.LookupRegister)]
		public ActionResult Buildings()
		{
			return View();
		}

		public JqGridJsonResult ListAddressesBuildingsAjax(JqGridRequest request, long? regionId)
		{
			var pageRequest = request.ToPageRequest<AddressBuildingsGridModel>();

			var data = _bll.GetAddresses(pageRequest, regionId.GetValueOrDefault());

			return data.ToJqGridJsonResult<AddressDto, AddressBuildingsGridModel>();
		}

        [HttpPost]
        public ActionResult ExportAddresses(JqGridRequest request, ExportType exportType, long? regionId)
        {
            return ExportGridData(request, exportType, "Addresses", typeof (AddressBuildingsGridModel),
                x => ListAddressesBuildingsAjax(x, regionId));
        }

        [HttpGet]
		public ActionResult SaveBuildingAddress(long regionId, long? addressId)
		{
	        if (addressId.HasValue)
	        {
				_bll.VerificationIsDeletedSrv<Address>(addressId.Value);
	        }
            _bll.VerificationRegion(regionId, addressId);
			var model = new UpdateBuildingAddressModel();
			if (addressId.HasValue)
			{
				var address = _bll.Get<Address>(addressId.Value);
				model = Mapper.Map<Address, UpdateBuildingAddressModel>(address);
			}
			model.RegionId = regionId;
			return PartialView("_UpdateBuildingAddressPartial", model);
		}

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveBuildingAddress(UpdateBuildingAddressModel model)
        {
            if (model.PollingStationId <= 0)
            {
                ModelState.AddModelError("PollingStationId", MUI.Address_RequiredPollingStation);
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_UpdateBuildingAddressPartial", model);
            }

            _bll.SaveAddress(model.Id, model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId);
			return Content(Const.CloseWindowContent);
		}

        public ActionResult ChangePollingStation(long regionId)
        {
			_bll.VerificationRegionForChangePollingStation(regionId);
			_bll.VerificationIsDeletedSrv<Address>(regionId);

            var model = new ChangePollingStationForAddressModel {RegionId = regionId};
            return PartialView("_ChangePollingStation", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePollingStation(ChangePollingStationForAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_ChangePollingStation", model);
            }

            _bll.UpdatePollingStation(model.PollingStationId, model.Addresses);

            return Content(Const.CloseWindowContent);
        }

        [HttpPost]
        public void DeleteAddress(long addressId)
        {
			_bll.VerificationIfAddressHasReference(addressId);
            _bll.DeleteAddress(addressId);
        }

	    public ActionResult UpdateGeolocation(long id)
	    {			
			_bll.VerificationIsDeletedSrv<Address>(id);
			var address = _bll.Get<Address>(id);
			_bll.VerificationRegionForUpdateGeolocation(address.Street.Region.Id);

			var model = new EditGeolocationModel { Id = id, Address = address.GetFullAddress(showpollingStation: true)};
			return PartialView("_UpdateGeolocation", model);
		}

		[HttpPost]
		public ActionResult UpdateGeolocation(EditGeolocationModel model)
		{
			if (ModelState.IsValid)
			{
				_bll.UpdateLocation(model.Id, model.Geolatitude, model.Geolongitude);
				return Content(Const.CloseWindowContent);
			}
			return PartialView("_UpdateGeolocation", model);
		}

        public ActionResult GetAddressesBy(string term)
        {
            var streets = _bll.GetStreetsByFilter(term);
            return Json(streets.Select(x => new { StreetId = x.Id, StreetName = x.GetFullName()}).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectBuildingType()
        {
            var statuses = Enum.GetValues(typeof(BuildingTypes))
                .Cast<BuildingTypes>().Select(x => new SelectListItem
                {
                    Value = x.GetFilterValue().ToString(),
                    Text = EnumHelper.GetEnumDescription(x),
                }).ToList();

            return PartialView("_Select", statuses);
        }

		[HttpPost]
		public ActionResult GetPollingStationsByFilter(long? regionId)
		{
			var selectPollingStations = _bll.GetPollingStations(regionId);
			return Json(selectPollingStations.Select(x => x.Number).ToList(), JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        public void UnDeleteAddress(long id)
        {
			var address = _bll.Get<Address>(id);
			_bll.VerificationIsRegionDeleted(address.Street.Region.Id);
			_bll.VerificationIsStreetDeleted(address.Street.Id);
            _bll.UnDelete<Address>(id);
        }

		[HttpPost]
		public ActionResult GetLocation(long regionId, long adressId)
		{
			var region = _bll.Get<Region>(regionId);
			var address = _bll.Get<Address>(adressId);
			return Json(new
			{
				RegionGeoData = region.GeoLocation, 
				AdressGeoData = address.GeoLocation
			});
		}

		public JsonResult GetStreets(Select2Request request, long? regionId)
		{
			var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
			var data = _bll.SearchStreets(pageRequest, regionId);
			var response = new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(), data.Total, data.PageSize);
			return Json(response, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetStreetsName(long id)
		{
			var address = _bll.Get<Street>(id);
			return Json(address != null ? address.GetFullName() : string.Empty);
		}

        public long GetRegionByPollingStation(long pollingStationId)
        {
            var pollingStation = _bll.Get<PollingStation>(pollingStationId);
            return pollingStation.Region.Id;
        }

		public ActionResult AdjustmentAddresses(long addressId)
		{
			_bll.VerificationIsDeletedSrv<Address>(addressId);
			var address = _bll.Get<Address>(addressId);
			_bll.VerificationRegionForAdjustmentAddresses(address.Street.Region.Id);
			
			var model = new AdjustmentAddressesModel
			{
				BaseAddressInfo = new PersonAddressModel
				{
					AddressId = address.Id,
					FullAddress = address.GetFullAddress(true,true,true)
				}
			};

			var assignedRegions = _bll.GetAssignedRegions();
			if (assignedRegions.Count == 1)
			{
				model.RegionId = assignedRegions.Select(x=>x.Id).FirstOrDefault();
			}

			return PartialView( "_AdjustmentAddresses", model);
		}
		
		[HttpPost]
		public ActionResult AdjustmentAddresses(AdjustmentAddressesModel model)
		{
			if (model.RegionId == 0)
				{
					ModelState.AddModelError("RegionId", MUI.AdjustmentAddressesErrorRequired_RegionOfAddress);
					return PartialView("_AdjustmentAddresses", model);
				}
				if (model.AdjustmentAddressesId == 0)
				{
					ModelState.AddModelError("AdjustmentAddressesId", MUI.AdjustmentAddressesErrorRequired_Address);
					return PartialView("_AdjustmentAddresses", model);
				}
				if (model.AdjustmentAddressesId == model.BaseAddressInfo.AddressId)
				{
					ModelState.AddModelError("AdjustmentAddressesId", MUI.AdjustmentAddressesSameAddress);
					return PartialView("_AdjustmentAddresses", model);
				}
			
			_bll.AdjustmentAddresses(model.BaseAddressInfo.AddressId, model.AdjustmentAddressesId);
			return Content(Const.CloseWindowContent);
		}

        public ActionResult RspAddressMappings()
        {
            return View();
        }

        public JqGridJsonResult ListRspAddressMappingsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<RspAddressMappingGridModel>();

            var data = _bll.ListAddressMappings(pageRequest);

            return data.ToJqGridJsonResult<RspAddressMapping, RspAddressMappingGridModel>();
        }

        [HttpPost]
        public void DeleteRspAddressMappings(long addressMappingId)
        {
            _bll.DeleteAddressMapping(addressMappingId);
        }

        [HttpPost]
        public ActionResult ExportRspAddressMappings(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "RspAddressMappings", typeof (RspAddressMappingGridModel),
                ListRspAddressMappingsAjax);
        }
    }
}

