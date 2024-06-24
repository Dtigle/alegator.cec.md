using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.ViewItem;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Conflict;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.Web.SRV.Models.Lookup;
using CEC.SRV.Domain.Constants;

namespace CEC.Web.SRV.Controllers
{
    //TODO: Ref#1 Uncomment this class and fix the errors

    public enum ConflictGridType
    {
        AllConflicts,
        FromMyRegion,
        SharedWithMyRegion,
        SharedByMyRegion
    }

    [Authorize]
    public class ConflictController : BaseController
    {
        private readonly IConflictBll _bll;
        private readonly IVotersBll _voterBll;
        private readonly IAddressBll _addressBll;
        private readonly IImportBll _importBll;

        public ConflictController(IConflictBll bll, IVotersBll voterBll, IAddressBll addressBll, IImportBll importBll)
        {
            _bll = bll;
            _voterBll = voterBll;
            _addressBll = addressBll;
            _importBll = importBll;
        }

        [Authorize(Roles = Transactions.Conflict)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGridList(ConflictStatusCode statusCode)
        {
            switch (statusCode)
            {
                case ConflictStatusCode.StatusConflict:
                    return PartialView("_StatusConflictList");
                case ConflictStatusCode.StreetConflict:
                case ConflictStatusCode.AddressFatalConflict:
                case ConflictStatusCode.AddressConflict:
                    return PartialView("_AddressConflictList");
                case ConflictStatusCode.PollingStationConflict:
                    return PartialView("_AddressNoStreetConflictList");
                case ConflictStatusCode.RegionConflict:
                    return PartialView("_RegionConflictList");
                case ConflictStatusCode.LocalityConflict:
                    return PartialView("_LocalityConflictList");
                case ConflictStatusCode.StreetZeroConflict:
                    return PartialView("_StreetConflictList");
                default:
                    return null;
            }
        }

        #region Status Conflict


        public ActionResult StatusListConflictAjax(JqGridRequest request, ConflictGridType? gridType)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();

            switch (gridType)
            {
                case ConflictGridType.AllConflicts:
                    return GetAllConflictsList(pageRequest, ConflictStatusCode.StatusConflict);
                case ConflictGridType.FromMyRegion:
                    return GetAllConflictsListFromMyRegion(pageRequest, ConflictStatusCode.StatusConflict);
                case ConflictGridType.SharedWithMyRegion:
                    return GetConflictsSharedWithMyRegion(pageRequest, ConflictStatusCode.StatusConflict);
                case ConflictGridType.SharedByMyRegion:
                    return GetConflictsSharedByMyRegions(pageRequest, ConflictStatusCode.StatusConflict);
                default:
                    throw new ArgumentException("Unknown argument gridType " + gridType);
            }
        }

        //TODO: Investigate logic taking in account that rspModificationDataId was changed to rspRegistrationDataId
        [HttpPost]
        public ActionResult ResolveStatusConflict(long conflictId, bool keepRSVVersion = false)
        {
            var conflictRegistrationData = GetConflictRegistrationData(conflictId);
            var conflictData = conflictRegistrationData.RspModificationData;

            var person = _bll.GetPerson(conflictData.Idnp);
            var regionId = person.EligibleAddress?.Address.Street.Region.Id ?? -1;

            if (keepRSVVersion)
            {
                conflictData.Comments = conflictData.Comments == null
                    ? string.Format(MUI.Conflict_RejectRspStatus_comment)
                    : string.Format("{0} {1}", conflictData.Comments, MUI.Conflict_RejectRspStatus_comment);
                _importBll.RejectRspStatus(conflictData);

                var notificationMessage =
                    string.Format(MUI.Notification_RspModificationData_StatusModification_Rejected, conflictData.Id,
                        conflictData.Idnp);
                _bll.WriteNotification(ConflictStatusCode.StatusConflict, conflictData, notificationMessage, regionId);
            }
            else
            {
                conflictData.Comments = conflictData.Comments == null
                    ? string.Format(MUI.Conflict_AcceptRspStatus_comment)
                    : string.Format("{0} {1}", conflictData.Comments, MUI.Conflict_AcceptRspStatus_comment);
                _importBll.AcceptRspStatus(conflictData);

                var notificationMessage =
                    string.Format(MUI.Notification_RspModificationData_StatusModification_Accepted, conflictData.Id,
                        conflictData.Idnp);
                _bll.WriteNotification(ConflictStatusCode.StatusConflict, conflictData, notificationMessage, regionId);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Address Conflict


        public ActionResult AddressListConflictAjax(JqGridRequest request, ConflictGridType? gridType)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();
            pageRequest.FilterByConflict(true);

            var conflictStatusCodes = new[]
            {
                ConflictStatusCode.AddressConflict, ConflictStatusCode.StreetConflict,
                ConflictStatusCode.AddressFatalConflict
            };

            ActionResult result;
            switch (gridType)
            {
                case ConflictGridType.AllConflicts:
                    result = GetAllConflictsList(pageRequest, conflictStatusCodes);
                    break;
                case ConflictGridType.FromMyRegion:
                    result = GetAllConflictsListFromMyRegion(pageRequest, conflictStatusCodes);
                    break;
                case ConflictGridType.SharedWithMyRegion:
                    result = GetConflictsSharedWithMyRegion(pageRequest, conflictStatusCodes);
                    break;
                case ConflictGridType.SharedByMyRegion:
                    result = GetConflictsSharedByMyRegions(pageRequest, conflictStatusCodes);
                    break;
                default:
                    throw new ArgumentException("Unknown argument gridType " + gridType);
            }
            return result;
        }

        //TODO: Investigate logic taking in account that rspModificationDataId was changed to rspRegistrationDataId
        public ActionResult ResolveAddressConflict(long conflictId)
        {
            var conflict = GetConflictRegistrationData(conflictId);
            var region = _bll.GetRegionByAdministrativeCode(conflict.Administrativecode);
            if (region == null)
            {
                throw new SrvException("Conflict_CreateAddress_NoRegionExist_ErrorMessage",
                    MUI.Conflict_CreateAddress_NoRegionExist_ErrorMessage);
            }
            var model = new ResolveAddressConflictModel
            {
                RspId = conflict.Id,
                RspAddress = conflict.GetRegistrationAddress(),
                RegionId = region.Id
            };

            return PartialView("_ResolveAddressConflict", model);
        }

        public ActionResult LoadAddressGridList(long? regionId)
        {
            return PartialView("_AddressRSVList", regionId);
        }

        public ActionResult SelectConflictGender()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem {Value = "1", Text = "M"});
            list.Add(new SelectListItem {Value = "2", Text = "F"});

            return PartialView("_Select", list);
        }


        public ActionResult LoadIdenticalAddressGridList(long? conflictId)
        {
            return PartialView("_AddressIdenticalList", conflictId);
        }

        /// <summary>
        /// Show conflict share view
        /// </summary>
        /// <param name="conflictModifiedDataId">aka RspModificationDataId</param>
        /// <param name="conflictDataId">aka RspRegistrationData</param>
        /// <returns></returns>
        public ActionResult ShareConflictView(long conflictModifiedDataId, long conflictDataId)
        {
            RspConflictData conflict = _bll.GetConflictFromRspConflictData(conflictDataId);
            if (conflict == null)
            {
                throw new ArgumentException("RspConflictDataId not found, id=" + conflictDataId);
            }

            List<SelectListItem> regions =
                conflict.Shares
                    .Where(x => x.Deleted == null)
                    .Select(
                        x =>
                            new SelectListItem {Text = x.Destination.GetFullName(), Value = x.Destination.Id.ToString()})
                    .ToList();

            // ?
            string note = conflict.Shares.Where(x => x.Deleted == null).Select(x => x.Note).FirstOrDefault();
            long reasonId = conflict.Shares.Where(x => x.Deleted == null).Select(x => x.Reason.Id).FirstOrDefault();


            var view = PartialView("_ShareConflict", new ShareConflictModel
            {
                ConflictId = conflict.Id,
                AllocatedRegions = regions,
                OriginalRegions = regions,
                ConflictShareNote = note,
                ReasonId = reasonId
            });

            GetConflictShareReasons(reasonId);
            return view;
        }

        //List Regions Tree View for Conflict Sharing
        [HttpPost]
        public JqGridJsonResult ListTreeViewRegionsAjax(int? nodeid)
        {
            var data = _bll.GetAllLocalities(nodeid).OrderBy(x => x.RegionType.Name).ThenBy(x => x.Name);

            var response = new JqGridResponse();

            response.Records.AddRange(data.Select(item =>
                new JqGridAdjacencyTreeRecord<LocalitiesTreeView>(
                    Convert.ToString(item.Id),
                    Mapper.Map<Region, LocalitiesTreeView>(item),
                    item.Parent != null ? item.Level : 0,
                    item.Parent != null ? Convert.ToString(item.Parent.Id) : "")
                {
                    Leaf = item.Children.Count == 0,
                }));

            return new JqGridJsonResult() {Data = response};
        }

        //****

        //Getting the conflict share nomenclature
        private void GetConflictShareReasons(long selectedReason)
        {
            var status = _bll.GetAll<ConflictShareReasonTypes>();
            ViewData["ReasonId"] = status.ToSelectListUnencrypted(selectedReason, false, MUI.SelectPrompt, x => x.Name,
                x => x.Id);
        }

        [HttpPost]
        public ActionResult CancelConflictShares(long conflictDataId)
        {
            _bll.CancelAllConflictShares(conflictDataId);
            return Content(Const.CloseWindowContent);
        }

        [HttpPost]
        public ActionResult ShareConflict(ShareConflictModel model)
        {
            BllResult result;

            foreach (var region in model.AllocatedRegions)
            {
//                bool wasAdded = true;
//                foreach (var originalRegion in model.OriginalRegions)
//                {
//                    if (Convert.ToInt64(region.Value) == Convert.ToInt64(originalRegion.Value))
//                    {
//                        wasAdded = false;
//                        break;
//                    }
//                }
//
//                if (wasAdded)
//                {
                result = _bll.ShareConflict(model.ConflictId, Convert.ToInt64(region.Value), model.ReasonId,
                    model.ConflictShareNote);

                if (result.StatusCode <= 0)
                {
                    ModelState.AddModelError("", result.StatusMessage);

                    GetConflictShareReasons(model.ReasonId);

                    return PartialView("_ShareConflict", model);
                }
                //                }
            }

            foreach (var region in model.OriginalRegions)
            {
                bool wasRemoved = true;
                foreach (var allocatedRegion in model.AllocatedRegions)
                {
                    if (Convert.ToInt64(region.Value) == Convert.ToInt64(allocatedRegion.Value))
                    {
                        wasRemoved = false;
                        break;
                    }
                }
                if (wasRemoved)
                {
                    result = _bll.CancelConflictShare(model.ConflictId, Convert.ToInt64(region.Value));
                    if (result.StatusCode <= 0)
                    {
                        ModelState.AddModelError("", result.StatusMessage);

                        GetConflictShareReasons(model.ReasonId);

                        return PartialView("_ShareConflict", model);
                    }
                }
            }

            return Content("$$Conflict actualizat cu succes");
        }

        //******

        public ActionResult ListAddressesAjax(JqGridRequest request, long? regionId)
        {
            var pageRequest = request.ToPageRequest<AddressRSVGridModel>();

            var data = _bll.GetAddresses(pageRequest, regionId);

            return data.ToJqGridJsonResult<AddressBaseDto, AddressRSVGridModel>();
        }

        public ActionResult ListIdenticalAddressesAjax(JqGridRequest request, long conflictId)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();
            pageRequest.PageNumber = 0;
            pageRequest.PageSize = 1000000;

            var data = _bll.GetConflictListByConflictAddress(pageRequest, conflictId);

            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();
        }

        [HttpPost]
        public ActionResult MapAddressConflict(long conflictId, long addressId, long[] applyToConflicts)
        {
            _importBll.ResolveByMappingAddress(conflictId, addressId, applyToConflicts);

            return Content(Const.CloseWindowContent);
        }

        [Authorize(Roles = Transactions.Conflict)]
        [HttpGet]
        public ActionResult CreateNewAddress(long conflictId, long regionId)
        {
            var conflictData = GetConflictRegistrationData(conflictId);

            var rspStreet = _bll.Get<StreetTypeCode>(conflictData.StreetCode);
            //TODO: refactoring required as ropId for streets may become Obsolete. Retrieval by StreetName may be more appropriate.
            //var rsvStreet = _bll.GetStreetByRopId(rspStreet.Id);

            //
            // GreenSoft: Fix, strada apartine unei regiuni, se prelua o strada ce apartinea altei regiuni
            //
            var rsvStreet = _bll.GetStreetByRopId(rspStreet.Id, regionId);

            if (rsvStreet == null)
            {
                //
                // GreenSoft: Se incearca preluarea dupa denumire, sint cazuri cand exista adresa adaugata de utilizator, fara RopId completat
                //
                rsvStreet = _bll.GetStreetNameAndRegionId(rspStreet.Name, regionId);
            }

            var model = new CreateAddressModel()
            {
                RspId = conflictId,
                RegionId = regionId,
                HouseNumber = conflictData.HouseNumber,
                Suffix = conflictData.GetHouseSuffix()
            };
            if (rsvStreet == null)
            {
                model.Street = rspStreet.Name;
            }
            else
            {
                model.Street = rsvStreet.GetFullName();
                model.StreetId = rsvStreet.Id;
            }

            SetPollingStationViewData(regionId);
            return PartialView("_CreateNewAddressPartial", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateNewAddress(CreateAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                SetPollingStationViewData(model.RegionId);
                return PartialView("_CreateNewAddressPartial", model);
            }
            var conflictAddressData = GetConflictRegistrationData(model.RspId);
            var rspData = conflictAddressData.RspModificationData;
            if (model.StreetId == null)
            {
                var rspStreet = _bll.Get<StreetTypeCode>(conflictAddressData.StreetCode);
                model.StreetId = _bll.CreateStreet(rspStreet.Name, null, model.RegionId, StreetType.RegularStreet,
                    rspStreet.Id, null);
            }

            var adress = _bll.SaveAddress((long) model.StreetId, model.HouseNumber, model.Suffix,
                BuildingTypes.Undefined, model.PollingStationId);
            rspData.Comments = rspData.Comments == null
                ? string.Format(MUI.Conflict_AcceptRspAddress_comment)
                : string.Format("{0} {1}", rspData.Comments, MUI.Conflict_AcceptRspAddress_comment);

            var conflictedRegistration = rspData.Registrations.FirstOrDefault(x => x.IsInConflict) ??
                                         rspData.Registrations.FirstOrDefault();

            _importBll.MapAddress(rspData, adress, conflictedRegistration.ApartmentNumber.GetValueOrDefault(),
                conflictedRegistration.ApartmentSuffix, true);

            return Content(Const.CloseWindowContent);
        }

        public ActionResult RejectAddressConflict(List<long> conflictIds, ConflictStatusCode conflictStatus)
        {
            var model = new RejectConflict {ConflictIds = conflictIds, ConflictStatus = conflictStatus};
            return PartialView(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RejectAddressConflictSave(RejectConflict model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("RejectAddressConflict", model);
            }
            foreach (var conflictId in model.ConflictIds)
            {
                var conflictRegistrationData = GetConflictRegistrationData(conflictId);
                var conflictData = conflictRegistrationData.RspModificationData;

                var person = _bll.GetPerson(conflictData.Idnp);
                var regionId = person.EligibleAddress?.Address.Street.Region.Id ?? -1;

                var userName = _bll.GetUserName();
                conflictData.Comments =
                    string.Format("{0} {1}", conflictData.Comments,
                        string.Format(MUI.Conflict_RejectRspAddress_comment, userName, model.Comment)).Truncate(255);

                var notificationMessage = string.Format(MUI.Notification_Conflict_RejectResolve, conflictData.Id,
                    conflictData.Idnp, model.Comment);
                if (model.ConflictStatus == ConflictStatusCode.AddressConflict)
                {
                    _importBll.RejectRspAddress(conflictData);
                    _bll.WriteNotification(ConflictStatusCode.AddressConflict, conflictData, notificationMessage,
                        regionId);
                    return Content(Const.CloseWindowContent);
                }

                _importBll.RejectRspPollingStation(conflictData);
                _bll.WriteNotification(ConflictStatusCode.PollingStationConflict, conflictData, notificationMessage,
                    regionId);
            }

            return Content(Const.CloseWindowContent);
        }

        #endregion

        #region Address without Street Conflict

        public ActionResult AddressNoStreetListConflictAjax(JqGridRequest request, ConflictGridType? gridType)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();
            pageRequest.FilterByConflict(true);

            switch (gridType)
            {
                case ConflictGridType.AllConflicts:
                    return GetAllConflictsList(pageRequest, ConflictStatusCode.PollingStationConflict);
                case ConflictGridType.FromMyRegion:
                    return GetAllConflictsListFromMyRegion(pageRequest, ConflictStatusCode.PollingStationConflict);
                case ConflictGridType.SharedWithMyRegion:
                    return GetConflictsSharedWithMyRegion(pageRequest, ConflictStatusCode.PollingStationConflict);
                case ConflictGridType.SharedByMyRegion:
                    return GetConflictsSharedByMyRegions(pageRequest, ConflictStatusCode.PollingStationConflict);
                default:
                    throw new ArgumentException("Unknown argument gridType " + gridType);
            }
        }

        //TODO: Investigate logic taking in account that rspModificationDataId was changed to rspRegistrationDataId
        public ActionResult ResolveAddressNoStreetConflict(List<long> conflictIds)
        {
            var conflict = GetConflictRegistrationData(conflictIds[0]);
            var region = _bll.GetRegionByAdministrativeCode(conflict.Administrativecode);
            if (region == null)
            {
                throw new SrvException("Conflict_CreateAddress_NoRegionExist_ErrorMessage",
                    MUI.Conflict_CreateAddress_NoRegionExist_ErrorMessage);
            }
            var model = new ResolveAddressNoStreetConflictModel
            {
                RspIds = conflictIds,
                RegionId = region.Id
            };
            SetPollingStationViewData(region.Id);
            return PartialView("_ResolveAddressNoStreetConflict", model);
        }

        //TODO: Investigate logic taking in account that rspModificationDataId was changed to rspRegistrationDataId
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MapAddressNoStreetConflict(ResolveAddressNoStreetConflictModel model)
        {
            if (!ModelState.IsValid)
            {
                SetPollingStationViewData(model.RegionId);
                return PartialView("_ResolveAddressNoStreetConflict", model);
            }
            var personIds = _bll.GetPersonIdbyRspIds(model.RspIds);
            _voterBll.ChangePollingStation(model.NewPollingStation.PStationId, personIds);

            foreach (var conflictId in model.RspIds)
            {
                var conflictRegistration = GetConflictRegistrationData(conflictId);
                var conflict = conflictRegistration.RspModificationData;
                conflict.Comments = conflict.Comments == null
                    ? string.Format(MUI.Conflict_MapAddress_comment)
                    : string.Format("{0} {1}", conflict.Comments, MUI.Conflict_MapAddress_comment);
                _importBll.AssignRspPollingStation(conflict);
            }
            return Content(Const.CloseWindowContent);
        }

        #endregion

        #region Region Conflict

        public ActionResult RegionListConflictAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();
            pageRequest.FilterByConflict(true);

            var dataAdmin = _bll.GetConflictListForAdmin2(pageRequest, ConflictStatusCode.RegionConflict);
            return dataAdmin.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        public ActionResult ResolveRegionConflict(long conflictId)
        {
            var message = string.Format(MUI.Conflict_RegionConflict_RetryMessage);
            _bll.UpdateStatusToRetry(conflictId, message, ConflictStatusCode.RegionConflict);
            return RedirectToAction("Index");
        }

        #endregion

        #region Street Conflict

        public ActionResult StreetListConflictAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();
            pageRequest.FilterByConflict(true);

            var dataAdmin = _bll.GetConflictListForAdmin2(pageRequest, ConflictStatusCode.StreetZeroConflict);
            return dataAdmin.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        public ActionResult ResolveStreetConflict(long conflictId)
        {
            const string message = "reprocesare";
            _bll.UpdateStatusToRetry(conflictId, message, ConflictStatusCode.StreetZeroConflict);
            return RedirectToAction("Index");
        }

        #endregion

        #region Localities Conflict

        public ActionResult LocalityConflictAjax(JqGridRequest request, ConflictGridType? gridType)
        {
            var pageRequest = request.ToPageRequest<ConflictGridModel>();
            pageRequest.FilterByConflict(true);

            switch (gridType)
            {
                //todo:
                case ConflictGridType.AllConflicts:
                    return GetAllConflictsForLinkedRegions(pageRequest);
                //todo:
                case ConflictGridType.FromMyRegion:
                    return GetAllConflictsForLinkedRegions(pageRequest);
                case ConflictGridType.SharedWithMyRegion:
                    return GetConflictsSharedWithMyRegionForLinkedRegions(pageRequest);
                case ConflictGridType.SharedByMyRegion:
                    return GetConflictsSharedByRegionForLinkedRegions(pageRequest);
                default:
                    throw new ArgumentException("Unknown argument gridType " + gridType);
            }
        }

        public ActionResult ResolveLocalityConflict(long conflictId)
        {
            var conflict = GetConflictRegistrationData(conflictId);
            var region = _bll.GetRegionByAdministrativeCode(conflict.Administrativecode);
            if (region == null)
            {
                throw new SrvException("Conflict_CreateAddress_NoRegionExist_ErrorMessage",
                    MUI.Conflict_CreateAddress_NoRegionExist_ErrorMessage);
            }
            var model = new ResolveLocalityConflictModel
            {
                RspId = conflictId,
            };

            return PartialView("_ResolveLocalityConflict", model);
        }

        [HttpPost]
        public ActionResult ChangeAddress(long conflictId, long addressId)
        {
            var personId = _bll.GetPersonIdByConflictId(conflictId);
            _bll.ChangeAddress(addressId, personId);

            _importBll.AcceptRsvLocality(conflictId);
            return Content(Const.CloseWindowContent);
        }

        [HttpPost]
        public ActionResult ChangePollingStation(long conflictId, long pollingStationId)
        {
            var personId = _bll.GetPersonIdByConflictId(conflictId);
            _voterBll.UpdatePollingStation(personId, pollingStationId);

            _importBll.AcceptRsvLocality(conflictId);
            return Content(Const.CloseWindowContent);
        }

        public JsonResult GetUserRegions(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _bll.GetUserRegions(pageRequest);
            var response =
                new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(),
                    data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckRegionForStreets(long regionId)
        {
            return Json(_bll.Get<Region>(regionId).HasStreets);
        }

        public ActionResult LoadPollingStationsList()
        {
            return PartialView("_PollingStationList");
        }

        #endregion

        public ActionResult GetViewConflict(long conflictId, ConflictStatusCode conflictType)
        {
            var conflictRegistrationData = GetConflictRegistrationData(conflictId);

            ActionResult result = null;

            if (conflictType.HasFlag(ConflictStatusCode.StatusConflict))
            {
                var voter = _bll.GetVoter(conflictRegistrationData.RspModificationData.Idnp);
                if (voter == null)
                {
                    throw new SrvException("Conflict_NoPersonExistInRSV_ErrorMessage",
                        MUI.Conflict_NoPersonExistInRSV_ErrorMessage);
                }
                var model = new ViewStatusConflictModel
                {
                    VoterData = Mapper.Map<VoterConflictDataDto, VoterConflictModel>(voter),
                    PeopleData =
                        Mapper.Map<RspModificationData, PeopleConflictModel>(
                            conflictRegistrationData.RspModificationData)
                };

                //
                // GreenSoft: Id-ul conflictului este rspRegistrationDataId
                //
                model.PeopleData.Id = conflictRegistrationData.Id;

                model.ConflictShares =
                    Mapper.Map<IList<ConflictShare>, IList<ConflictShareViewModel>>(_bll.GetAllConflictShares(conflictId));

                model.PeopleData.Address = conflictRegistrationData.GetRegistrationAddress();
                model.PeopleData.Region = conflictRegistrationData.Region;
                model.PeopleData.Locality = conflictRegistrationData.Locality;
                model.PeopleData.AdministrativeCode = (int) conflictRegistrationData.Administrativecode;
                result = PartialView("_ViewStatusConflict", model);
            }
            if (conflictType.HasFlag(ConflictStatusCode.AddressConflict))
            {
                var statusMessage = string.Empty;
                switch (conflictRegistrationData.RspModificationData.StatusConflictCode)
                {
                    case ConflictStatusCode.AddressConflict:
                        statusMessage = string.Format(MUI.Conflict_RegionConflict_StreetBlocNuExista,
                            conflictRegistrationData.StreetCode, conflictRegistrationData.Administrativecode,
                            conflictRegistrationData.HouseNumber,
                            conflictRegistrationData.GetHouseSuffix(), conflictRegistrationData.StreetName,
                            conflictRegistrationData.Region);
                        break;
                    case ConflictStatusCode.StreetConflict:
                        statusMessage = string.Format(MUI.Conflict_RegionConflict_StreetCodeInRegion,
                            conflictRegistrationData.StreetCode, conflictRegistrationData.Region,
                            conflictRegistrationData.Administrativecode, conflictRegistrationData.StreetName);
                        break;
                    case ConflictStatusCode.AddressFatalConflict:
                        statusMessage = MUI.Conflict_MissingNoResidenceRegionError;
                        break;
                }

                var model = new ViewAddressConflictModel
                {
                    IdRSP = conflictRegistrationData.Id,
                    StatusMessage = statusMessage,
                    Address = conflictRegistrationData.GetRegistrationAddress()
                };
                model.ConflictShares =
                    Mapper.Map<IList<ConflictShare>, IList<ConflictShareViewModel>>(_bll.GetAllConflictShares(conflictId));
                result = PartialView("_ViewAddressConflict", model);
            }
            if (conflictType.HasFlag(ConflictStatusCode.PollingStationConflict))
            {
                var model = new ViewAddressConflictModel
                {
                    IdRSP = conflictRegistrationData.Id,
                    StatusMessage =
                        string.Format(MUI.Conflict_PollingStationAmbigous, conflictRegistrationData.Locality,
                            conflictRegistrationData.Administrativecode, conflictRegistrationData.Region)
                };
                model.ConflictShares =
                    Mapper.Map<IList<ConflictShare>, IList<ConflictShareViewModel>>(_bll.GetAllConflictShares(conflictId));
                result = PartialView("_ViewAddressNoStreetConflict", model);
            }
            if (conflictType.HasFlag(ConflictStatusCode.RegionConflict))
            {
                var model = new RegionConflictModel()
                {
                    IdRSP = conflictRegistrationData.Id,
                    StatusMessage =
                        string.Format(MUI.Conflict_RegoinConflict_RegionNuExista,
                            conflictRegistrationData.Administrativecode),
                    RegistruId = conflictRegistrationData.Administrativecode
                };
                model.ConflictShares =
                    Mapper.Map<IList<ConflictShare>, IList<ConflictShareViewModel>>(_bll.GetAllConflictShares(conflictId));
                result = PartialView("_ViewRegionConflict", model);
            }
            if (conflictType.HasFlag(ConflictStatusCode.StreetZeroConflict))
            {
                var model = new ViewAddressConflictModel()
                {
                    StatusMessage = string.Format(MUI.Conflict_RegionConflict_StreetCodeInRegion,
                        conflictRegistrationData.StreetCode, conflictRegistrationData.Region,
                        conflictRegistrationData.Administrativecode, conflictRegistrationData.StreetName)
                };
                model.ConflictShares =
                    Mapper.Map<IList<ConflictShare>, IList<ConflictShareViewModel>>(_bll.GetAllConflictShares(conflictId));
                result = PartialView("_ViewStreetConflict", model);

            }
            if (conflictType.HasFlag(ConflictStatusCode.LocalityConflict))
            {
                var model = new ViewLocalityConflictModel
                {
                    IdRSP = conflictRegistrationData.Id,
                    StatusMessage =
                        string.Format(MUI.Conflict_RegoinConflict, conflictRegistrationData.Administrativecode)
                };
                model.ConflictShares =
                    Mapper.Map<IList<ConflictShare>, IList<ConflictShareViewModel>>(_bll.GetAllConflictShares(conflictId));
                result = PartialView("_ViewLocalityConflict", model);
            }

            return result;
        }

        private void SetPollingStationViewData(long regionId)
        {
            var pollingStations = _addressBll.GetPollingStations(regionId).Where(x => x.Deleted == null);
            ViewData["PollingStationId"] = pollingStations
                .OrderBy(x => x.Region.GetCircumscription().GetValueOrDefault()).ThenBy(x => x.Number)
                .ToSelectListUnencrypted(0, false, null, x => x.FullNumber, x => x.Id);
        }

        public ActionResult SelectSource()
        {
            var statuses = Enum.GetValues(typeof (SourceEnum))
                .Cast<SourceEnum>().Select(x => new SelectListItem
                {
                    Value = x.GetFilterValue().ToString(),
                    Text = EnumHelper.GetEnumDescription(x),
                }).ToList();

            return PartialView("_Select", statuses);
        }

        public ActionResult SelectStatus()
        {
            var statuses = Enum.GetValues(typeof (RawDataStatus))
                .Cast<RawDataStatus>().Select(x => new SelectListItem
                {
                    Value = x.GetFilterValue().ToString(),
                    Text = EnumHelper.GetEnumDescription(x),
                }).ToList();

            return PartialView("_Select", statuses);
        }



        //TODO: Investigate logic taking in account that rspModificationDataId was changed to rspRegistrationDataId
        private RspRegistrationData GetConflictRegistrationData(long registrationId)
        {
            return _bll.Get<RspRegistrationData>(registrationId);
        }


        private ActionResult GetAllConflictsList(PageRequest request, ConflictStatusCode conflictStatusCode)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                // var dataAdmin = _bll.GetConflictListForAdmin(request, conflictStatusCode);
                // return dataAdmin.ToJqGridJsonResult<RspConflictDataAdmin, ConflictGridModel>();

                var dataAdmin = _bll.GetConflictListForAdmin2(request, conflictStatusCode);
                return dataAdmin.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
            }
//            var data = _bll.GetAllConflicts2(request, new [] { conflictStatusCode});
//            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();

            var data = _bll.GetAllConflicts2(request, new[] {conflictStatusCode});
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetAllConflictsList(PageRequest request, ConflictStatusCode[] conflictStatusCodes)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
//                var dataAdmin = _bll.GetConflictListForAdmin(request, conflictStatusCodes);
//                return dataAdmin.ToJqGridJsonResult<RspConflictDataAdmin, ConflictGridModel>();

                var dataAdmin = _bll.GetConflictListForAdmin2(request, conflictStatusCodes);
                return dataAdmin.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
            }
//            var data = _bll.GetAllConflicts(request, conflictStatusCodes);
//            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();

            var data = _bll.GetAllConflicts2(request, conflictStatusCodes);
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetAllConflictsListFromMyRegion(PageRequest request, ConflictStatusCode conflictStatusCode)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
//                var dataAdmin = _bll.GetConflictListForAdmin(request, conflictStatusCode);
//                return dataAdmin.ToJqGridJsonResult<RspConflictDataAdmin, ConflictGridModel>();
                var dataAdmin = _bll.GetConflictListForAdmin2(request, conflictStatusCode);
                return dataAdmin.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
            }
            //            var data = _bll.GetConflictList(request, conflictStatusCode);
            //            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();

            var data = _bll.GetConflictList2(request, conflictStatusCode);
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetAllConflictsListFromMyRegion(PageRequest request,
            ConflictStatusCode[] conflictStatusCodes)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
//                var dataAdmin = _bll.GetConflictListForAdmin(request, conflictStatusCodes);
//                return dataAdmin.ToJqGridJsonResult<RspConflictDataAdmin, ConflictGridModel>();
                var dataAdmin = _bll.GetConflictListForAdmin2(request, conflictStatusCodes);
                return dataAdmin.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
            }
//            var data = _bll.GetConflictList(request, conflictStatusCodes);
//            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();
            var data = _bll.GetConflictList2(request, conflictStatusCodes);
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }


        private ActionResult GetConflictsSharedByMyRegions(PageRequest request, ConflictStatusCode conflictStatusCode)
        {
            var data = _bll.GetConflictSharedByMyRegionsList(request, new ConflictStatusCode[] {conflictStatusCode});
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetConflictsSharedByMyRegions(PageRequest request, ConflictStatusCode[] conflictStatusCodes)
        {
            var data = _bll.GetConflictSharedByMyRegionsList(request, conflictStatusCodes);
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetConflictsSharedWithMyRegion(PageRequest request, ConflictStatusCode conflictStatusCode)
        {
            var data = _bll.GetConflictSharedWithMyRegionsList(request, new ConflictStatusCode[] {conflictStatusCode});
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetConflictsSharedWithMyRegion(PageRequest request,
            ConflictStatusCode[] conflictStatusCodes)
        {
            var data = _bll.GetConflictSharedWithMyRegionsList(request, conflictStatusCodes);
            return data.ToJqGridJsonResult<ConflictViewItem, ConflictGridModel>();
        }

        private ActionResult GetAllConflictsForLinkedRegions(PageRequest request)
        {
            var data = _bll.GetConflictListForLinkedRegions(request, ConflictStatusCode.LocalityConflict);
            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();
        }

        private ActionResult GetConflictsSharedWithMyRegionForLinkedRegions(PageRequest request)
        {
            var data = _bll.GetConflictListSharedWithMyRegionsListForLinkedRegions(request,
                new[] {ConflictStatusCode.LocalityConflict});
            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();
        }

        private ActionResult GetConflictsSharedByRegionForLinkedRegions(PageRequest request)
        {
            var data = _bll.GetConflictListSharedByMyRegionsListForLinkedRegions(request,
                new[] {ConflictStatusCode.LocalityConflict});
            return data.ToJqGridJsonResult<RspConflictData, ConflictGridModel>();
        }


        [HttpPost]
        public ActionResult GetConflictCount()
        {
            long count = 0;
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                count = _bll.GetAllConflictCountForAdmin(
                    new[]
                    {
                        ConflictStatusCode.StatusConflict,
                        ConflictStatusCode.AddressConflict,
                        ConflictStatusCode.StreetConflict,
                        ConflictStatusCode.AddressFatalConflict,
                        ConflictStatusCode.PollingStationConflict,
                        ConflictStatusCode.RegionConflict,
                        ConflictStatusCode.LocalityConflict,
                    });
            }
            else
            {
                count = _bll.GetAllConflictCount(
                    new[]
                    {
                        ConflictStatusCode.StatusConflict,
                        ConflictStatusCode.AddressConflict,
                        ConflictStatusCode.StreetConflict,
                        ConflictStatusCode.AddressFatalConflict,
                        ConflictStatusCode.PollingStationConflict
                    });
            }
            return Json(count);
        }

        #region AddressesWithoutPollingStation
        public ActionResult LoadAddressesWithoutPollingStatioGridList()
        {
            return PartialView("_AddressesWithoutPollingStationList");
        }

        public ActionResult AddressesWithoutPollingStationListAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<AddressWithoutPollingStationGridModel>();

            var data = _bll.GetAddressesWithoutPollingStation(pageRequest);

            return data.ToJqGridJsonResult<AddressWithoutPollingStation, AddressWithoutPollingStationGridModel>();
        }

        #endregion
    }
}