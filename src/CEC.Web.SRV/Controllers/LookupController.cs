using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CEC.SRV.Domain.Constants;

namespace CEC.Web.SRV.Controllers
{
    public class LookupController : BaseController
    {
        private readonly ILookupBll _bll;
        private readonly IService<ILookupBll> _lookupService;

        public LookupController(ILookupBll bll, IService<ILookupBll> lookupService)
        {
            _bll = bll;
            _lookupService = lookupService;
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult StreetTypes()
        {
            return View();
        }
        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult StreetRspTypes()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult RegionTypes()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult ManagerTypes()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult PersonStatus()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult Genders()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup + "," + Transactions.LookupRegister)]
        public ActionResult Streets()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult DocumentTypes()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult ElectionTypes()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult RegionsTree()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult RegionsGrid()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult PersonAddressTypes()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Lookup)]
        public ActionResult Circumscriptions()
        {
            return View();
        }

        [Authorize(Roles = Transactions.Administrator)]
        public ActionResult ConflictShareReasons()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateUpdateManagerType(long? managerTypeId)
        {
            if (managerTypeId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<ManagerType>(managerTypeId.Value);
            }

            var model = new UpdateLookupModel();

            if (managerTypeId != null)
            {
                var managerType = _bll.Get<ManagerType>((long)managerTypeId);
                model = Mapper.Map<ManagerType, UpdateLookupModel>(managerType);
            }

            return PartialView("_UpdateManagerTypePartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdateManagerType(UpdateLookupModel model)
        {
            ValidateModel<ManagerType>(model.Id, model.Name);

            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdate<ManagerType>(model.Id, model.Name, model.Description);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateManagerTypePartial", model);
        }

        [HttpPost]
        public void DeleteManagerType(long id)
        {
            _bll.VerificationIfManagerTypeHasReference(id);
            _bll.Delete<ManagerType>(id);
        }

        [HttpPost]
        public void UnDeleteManagerType(long id)
        {
            _bll.UnDelete<ManagerType>(id);
        }

        [HttpGet]
        public ActionResult CreateUpdateStreetType(long? streetTypeId)
        {
            if (streetTypeId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<StreetType>(streetTypeId.Value);
            }

            var model = new UpdateLookupModel();

            if (streetTypeId != null)
            {
                var streetType = _bll.Get<StreetType>((long)streetTypeId);
                model = Mapper.Map<StreetType, UpdateLookupModel>(streetType);
            }

            return PartialView("_UpdateStreetTypePartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdateStreetType(UpdateLookupModel model)
        {
            ValidateModel<StreetType>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdate<StreetType>(model.Id, model.Name, model.Description);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateStreetTypePartial", model);
        }

        [HttpPost]
        public void DeleteStreetType(long id)
        {
            _bll.VerificationIfStreetTypeHasReference(id);
            _bll.Delete<StreetType>(id);
        }

        [HttpPost]
        public void UnDeleteStreetType(long id)
        {
            _bll.UnDelete<StreetType>(id);
        }

        [HttpGet]
        public ActionResult CreateUpdateRegionType(long? regionTypeId)
        {
            if (regionTypeId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<RegionType>(regionTypeId.Value);
            }
            var model = new UpdateRegionTypesModel();

            if (regionTypeId != null)
            {
                var regionType = _bll.Get<RegionType>((long)regionTypeId);
                model = Mapper.Map<RegionType, UpdateRegionTypesModel>(regionType);
            }

            return PartialView("_UpdateRegionTypePartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdateRegionType(UpdateRegionTypesModel model)
        {
            ValidateModel<RegionType>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdateRegionType(model.Id, model.Name, model.Description, (byte)model.Rank);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateRegionTypePartial", model);
        }

        [HttpPost]
        public void DeleteRegionType(long id)
        {
            _bll.VerificationIfRegionTypeHasReference(id);
            _bll.Delete<RegionType>(id);
        }

        [HttpPost]
        public void UnDeleteRegionType(long id)
        {
            _bll.UnDelete<RegionType>(id);
        }

        [HttpGet]
        public ActionResult CreateUpdatePersonStatus(long? personStatusId)
        {
            if (personStatusId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<PersonStatusType>(personStatusId.Value);
            }
            var model = new UpdatePersonStatusModel();

            if (personStatusId != null)
            {
                var personStatus = _bll.Get<PersonStatusType>((long)personStatusId);
                model = Mapper.Map<PersonStatusType, UpdatePersonStatusModel>(personStatus);
            }

            return PartialView("_UpdatePersonStatusPartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdatePersonStatus(UpdatePersonStatusModel model)
        {
            ValidateModel<PersonStatusType>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdatePersonStatus(model.Id, model.Name, model.Description, model.IsExcludable);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdatePersonStatusPartial", model);
        }

        [HttpPost]
        public void DeletePersonStatus(long id)
        {
            _bll.VerificationIfPersonStatusHasReference(id);
            _bll.Delete<PersonStatusType>(id);
        }

        [HttpPost]
        public void UnDeletePersonStatus(long id)
        {
            _bll.UnDelete<PersonStatusType>(id);
        }

        [HttpGet]
        public ActionResult CreateUpdateGender(long? genderId)
        {
            if (genderId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<Gender>(genderId.Value);
            }

            var model = new UpdateLookupModel();

            if (genderId != null)
            {
                var gender = _bll.Get<Gender>((long)genderId);
                model = Mapper.Map<Gender, UpdateLookupModel>(gender);
            }

            return PartialView("_UpdateGenderPartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdateGender(UpdateLookupModel model)
        {
            ValidateModel<Gender>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdate<Gender>(model.Id, model.Name, model.Description);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateGenderPartial", model);
        }

        [HttpPost]
        public void DeleteGender(long id)
        {
            _bll.VerificationIfGenderHasReference(id);
            _bll.Delete<Gender>(id);
        }

        [HttpPost]
        public void UnDeleteGender(long id)
        {
            _bll.UnDelete<Gender>(id);
        }

        [HttpGet]
        public ActionResult CreateUpdateStreet(long regionId, long? streetId)
        {
            if (streetId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<Street>(streetId.Value);
            }

            _bll.VerificationRegion(regionId);

            var model = new UpdateStreetModel { RegionId = regionId };
            if (streetId.HasValue)
            {
                var street = _bll.Get<Street>(streetId.Value);
                model = Mapper.Map<Street, UpdateStreetModel>(street);
            }

            SetStreetsViewData(model);
            return PartialView("_CreateStreetPartial", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateUpdateStreet(UpdateStreetModel model)
        {
            ValidateModel(model);

            if (!ModelState.IsValid)
            {
                SetStreetsViewData(model);
                return PartialView("_CreateStreetPartial", model);
            }

            _bll.CreateUpdateStreet(model.Id, model.Name, model.Description, model.RegionId, model.StreetTypeId,
                       model.RopId, model.SaiseId);
            return Content(Const.CloseWindowContent);
        }

        //CreateUpdatePersonAddressType
        [HttpGet]
        public ActionResult CreateUpdatePersonAddressType(long? personAddressTypeId)
        {
            if (personAddressTypeId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<PersonAddressType>(personAddressTypeId.Value);
            }
            var model = new UpdateLookupModel();

            if (personAddressTypeId != null)
            {
                var addressType = _bll.Get<PersonAddressType>((long)personAddressTypeId);
                model = Mapper.Map<PersonAddressType, UpdateLookupModel>(addressType);
            }

            return PartialView("_UpdateAddressTypePartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdatePersonAddressType(UpdateLookupModel model)
        {
            ValidateModel<PersonAddressType>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdate<PersonAddressType>(model.Id, model.Name, model.Description);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateAddressTypePartial", model);
        }

        private void SetStreetsViewData(UpdateStreetModel model)
        {
            var streetType = _bll.GetAll<StreetType>().Where(x => x.Deleted == null);
            ViewData["StreetTypeId"] = streetType.ToSelectListUnencrypted(model.StreetTypeId, false, null, x => x.Name, x => x.Id);
        }

        [HttpPost]
        public void DeleteStreet(long streetId)
        {
            _bll.VerificationIfStreetHasReference(streetId);
            _bll.DeleteStreet(streetId);
        }

        [HttpPost]
        public void UnDeleteStreet(long id)
        {
            var street = _bll.Get<Street>(id);
            _bll.VerificationIsRegionDeleted(street.Region.Id);
            _bll.UnDelete<Street>(id);
        }

        [HttpGet]
        public ActionResult CreateUpdateDocType(long? docTypeId)
        {
            if (docTypeId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<DocumentType>(docTypeId.Value);
            }

            var model = new UpdateDocumentTypesModel();

            if (docTypeId != null)
            {
                var docType = _bll.Get<DocumentType>((long)docTypeId);
                model = Mapper.Map<DocumentType, UpdateDocumentTypesModel>(docType);
            }

            return PartialView("_UpdateDocumentTypesPartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdateDocType(UpdateDocumentTypesModel model)
        {
            ValidateModel<DocumentType>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdateDocType(model.Id, model.Name, model.Description, model.IsPrimary);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateDocumentTypesPartial", model);
        }

        [HttpPost]
        public void DeleteDocType(long id)
        {
            _bll.VerificationIfDocTypeHasReference(id);
            _bll.Delete<DocumentType>(id);
        }

        [HttpPost]
        public void UnDeleteDocType(long id)
        {
            _bll.UnDelete<DocumentType>(id);
        }

        public JqGridJsonResult ListElectionTypesAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ElectionTypeGridModel>();

            var data = _bll.Get<ElectionType>(pageRequest);

            var result = data.ToJqGridJsonResult<ElectionType, ElectionTypeGridModel>();

            return result;
        }

        [HttpPost]
        public ActionResult ExportElectionTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "ElectionTypes", typeof(ElectionTypeGridModel), ListElectionTypesAjax);
        }

        [HttpGet]
        public ActionResult CreateUpdateElectionType(long? electionTypeId)
        {
            UpdateElectionTypeModel model = null;
            if (electionTypeId.HasValue)
            {
                var bean = _bll.Get<ElectionType>(electionTypeId.Value);
                model = Mapper.Map<ElectionType, UpdateElectionTypeModel>(bean);
            }
            else
            {
                model = new UpdateElectionTypeModel
                {
                    ElectionRoundsNo = 1
                };
            }

            SetElectionTypeViewData(model);

            return PartialView("_UpdateElectionTypePartial", model);
        }


        public void SetElectionTypeViewData(UpdateElectionTypeModel model)
        {
            GetCircumscriptionLists(model.CircumscriptionListId);
            ViewData["ElectionCompetitorType"] =
                EnumHelper.GetValues(typeof(ElectionCompetitorType))
                    .ToSelectList(model.ElectionCompetitorType.ToString());

            ViewData["ElectionArea"] =
                EnumHelper.GetValues(typeof(ElectionArea))
                    .ToSelectList(model.ElectionArea.ToString());

            ViewData["ElectionRoundsNo"] = new Dictionary<string, string>
            {
                {"1", "1"},
                {"2", "2"},
                {"3", "3"},
                {"4", "4"},
                {"5", "5"},
                {"6", "6"},
                {"7", "7"},
                {"8", "8"},
                {"9", "9"},
                {"10", "10"},

            }.ToSelectList(model.ElectionRoundsNo.ToString());
        }

        private void GetCircumscriptionLists(long selectedId)
        {
            ViewData["CircumscriptionListId"] = _bll.GetAll<CircumscriptionList>().ToSelectList(selectedId, false, x => x.Id, x => x.Name);
        }

        [HttpPost]
        public ActionResult CreateUpdateElectionType(UpdateElectionTypeModel model)
        {
            ValidateModel<ElectionType>(model.Id, model.Name);
            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdate<ElectionType>(model.Id, model.Name, model.Description);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateElectionTypePartial", model);
        }

        [HttpPost]
        public void DeleteElectionType(long id)
        {
            _bll.VerificationIfElectionTypeHasReference(id);
            _bll.Delete<ElectionType>(id);
        }

        [HttpPost]
        public void UnDeleteElectionType(long id)
        {
            _bll.UnDelete<ElectionType>(id);
        }

        public JqGridJsonResult ListPersonAddressTypesAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<LookupGridModel>();

            var data = _bll.Get<PersonAddressType>(pageRequest);

            return data.ToJqGridJsonResult<PersonAddressType, LookupGridModel>();
        }

        [HttpPost]
        public ActionResult ExportPersonAddressTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "PersonAddressTypes", typeof(LookupGridModel), ListPersonAddressTypesAjax);
        }

        [HttpPost]
        public void DeletePersonAddressType(long id)
        {
            _bll.VerificationIfPersonAddressTypeHasReference(id);
            _bll.Delete<PersonAddressType>(id);
        }

        [HttpPost]
        public void UnDeletePersonAddressType(long id)
        {
            _bll.UnDelete<PersonAddressType>(id);
        }

        public JqGridJsonResult ListStreetTypeAjax(JqGridRequest request)
        {

            var pageRequest = request.ToPageRequest<LookupGridModel>();
            var response = _lookupService.Exec(x => x.Get<StreetType>(pageRequest));

            var data = response.Model;

            return data.ToJqGridJsonResult<StreetType, LookupGridModel>();
        }

        public JqGridJsonResult ListStreetRspTypeAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ClassifierGridModel>();
            var response = _lookupService.Exec(x => x.Get<StreetTypeCode>(pageRequest));

            var data = response.Model;

            return data.ToJqGridJsonResult<StreetTypeCode, ClassifierGridModel>();
        }

        [HttpPost]
        public ActionResult ExportStreetTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "StreetTypes", typeof(LookupGridModel), ListStreetTypeAjax);
        }

        [HttpPost]
        public ActionResult ExportStreetRspTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "StreetRspTypes", typeof(ClassifierGridModel), ListStreetRspTypeAjax);
        }

        public JqGridJsonResult ListManagerTypesAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<LookupGridModel>();

            var data = _bll.Get<ManagerType>(pageRequest);

            return data.ToJqGridJsonResult<ManagerType, LookupGridModel>();
        }

        [HttpPost]
        public ActionResult ExportManagerTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "ManagerTypes", typeof(LookupGridModel), ListManagerTypesAjax);
        }

        public JqGridJsonResult ListCircumscriptionsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<CircumscriptionGridModel>();

            var data = _bll.GetCircumscriptions(pageRequest);

            return data.ToJqGridJsonResult<Circumscription, CircumscriptionGridModel>();
        }

        [HttpPost]
        public ActionResult ExportCircumscriptions(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "Circumscriptions", typeof(CircumscriptionGridModel), ListCircumscriptionsAjax);
        }

        public JqGridJsonResult ListGendersAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<LookupGridModel>();

            var data = _bll.Get<Gender>(pageRequest);

            return data.ToJqGridJsonResult<Gender, LookupGridModel>();
        }

        [HttpPost]
        public ActionResult ExportGenders(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "Genders", typeof(LookupGridModel), ListGendersAjax);
        }

        public JqGridJsonResult ListRegionTypesAjax(JqGridRequest request)
        {

            var pageRequest = request.ToPageRequest<RegionTypesGridModel>();

            var data = _bll.Get<RegionType>(pageRequest);

            return data.ToJqGridJsonResult<RegionType, RegionTypesGridModel>();
        }

        [HttpPost]
        public ActionResult ExportRegionTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "RegionTypes", typeof(RegionTypesGridModel), ListRegionTypesAjax);
        }

        public JqGridJsonResult ListPersonStatusAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<PersonStatusGridModel>();

            var data = _bll.Get<PersonStatusType>(pageRequest);

            return data.ToJqGridJsonResult<PersonStatusType, PersonStatusGridModel>();
        }

        [HttpPost]
        public ActionResult ExportPersonStatuses(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "PersonStatuses", typeof(PersonStatusGridModel), ListPersonStatusAjax);
        }

        public JqGridJsonResult ListDocTypesAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<DocumentTypeGridModel>();

            var data = _bll.Get<DocumentType>(pageRequest);

            return data.ToJqGridJsonResult<DocumentType, DocumentTypeGridModel>();
        }

        [HttpPost]
        public ActionResult ExportDocTypes(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "DocTypes", typeof(DocumentTypeGridModel), ListDocTypesAjax);
        }

        public JqGridJsonResult ListConflictShareReasonsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<LookupGridModel>();

            var data = _bll.Get<ConflictShareReasonTypes>(pageRequest);

            return data.ToJqGridJsonResult<ConflictShareReasonTypes, LookupGridModel>();
        }

        [HttpPost]
        public ActionResult ExportConflictShareReasons(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "ConflictShareReasons", typeof(LookupGridModel), ListConflictShareReasonsAjax);
        }

        [HttpGet]
        public ActionResult CreateUpdateConflictShareReason(long? conflictShareReasonId)
        {
            if (conflictShareReasonId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<ConflictShareReasonTypes>(conflictShareReasonId.Value);
            }

            var model = new UpdateLookupModel();

            if (conflictShareReasonId != null)
            {
                var conflictShareReason = _bll.Get<ConflictShareReasonTypes>((long)conflictShareReasonId);
                model = Mapper.Map<ConflictShareReasonTypes, UpdateLookupModel>(conflictShareReason);
            }
            return PartialView("_UpdateConflictShareReasonPartial", model);
        }

        [HttpPost]
        public ActionResult CreateUpdateConflictShareReason(UpdateLookupModel model)
        {
            ValidateModel<ConflictShareReasonTypes>(model.Id, model.Name);

            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdate<ConflictShareReasonTypes>(model.Id, model.Name, model.Description);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateConflictShareReasonPartial", model);
        }

        [HttpPost]
        public void DeleteConflictShareReason(long conflictShareReasonId)
        {
            _bll.VerificationIfConflictShareReasonHasReference(conflictShareReasonId);
            _bll.Delete<ConflictShareReasonTypes>(conflictShareReasonId);
        }


        [HttpPost]
        public void UnDeleteConflictShareReason(long id)
        {
            _bll.UnDelete<ConflictShareReasonTypes>(id);
        }


        [HttpPost]
        public JqGridJsonResult ListTreeViewRegionsAjax(int? nodeid)
        {
            var data = _bll.GetLocalities(nodeid).OrderBy(x => x.RegionType.Name).ThenBy(x => x.Name);

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

            return new JqGridJsonResult() { Data = response };
        }

        public JqGridJsonResult ListStreetsAjax(JqGridRequest request, long? regionId)
        {
            var pageRequest = request.ToPageRequest<StreetsGridModel>();

            var data = _bll.GetStreets(pageRequest, regionId.GetValueOrDefault());

            return data.ToJqGridJsonResult<StreetDto, StreetsGridModel>();
        }

        [HttpPost]
        public ActionResult ExportStreets(JqGridRequest request, ExportType exportType, long? regionId)
        {
            return ExportGridData(request, exportType, "Streets", typeof(StreetsGridModel), x => ListStreetsAjax(x, regionId));
        }

        public JqGridJsonResult ListRegionsTreeAjax(JqGridRequest request, long? parentId)
        {
            var pageRequest = request.ToPageRequest<RegionTreeViewModel>();

            var data = _bll.GetRegions(pageRequest, parentId.GetValueOrDefault());

            return data.ToJqGridJsonResult<RegionRow, RegionTreeViewModel>();
        }

        public JqGridJsonResult ListRegionsGridAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<RegionGridViewModel>();

            var data = _bll.GetRegionsGrid(pageRequest);

            return data.ToJqGridJsonResult<RegionRow, RegionGridViewModel>();
        }

        [HttpPost]
        public ActionResult ExportRegions(JqGridRequest request, ExportType exportType, long? parentId)
        {
            return ExportGridData(request, exportType, "Regions", typeof(RegionTreeViewModel), x => ListRegionsTreeAjax(x, parentId));
        }

        [HttpGet]
        public ActionResult CreateUpdateRegion(long parentId, long? regionId)
        {
            if (regionId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<Region>(regionId.Value);
            }

            _bll.VerificationIsRegionDeleted(parentId);

            var model = new UpdateRegionModel { Parent = parentId };
            if (regionId.HasValue)
            {
                var region = _bll.Get<Region>(regionId.Value);
                model = Mapper.Map<Region, UpdateRegionModel>(region);
            }

            SetRegionsViewData(model);
            return PartialView("_CreateRegionPartial", model);
        }

        private void SetRegionsViewData(UpdateRegionModel model)
        {
            var regionTypes = GetRegionTypes(model.Parent).Where(x => x.Deleted == null);
            var regions = _bll.GetAll<Region>();
            ViewData["Parent"] = regions
                .OrderBy(x => x.RegionType.Name).ThenBy(x => x.Name)
                .ToSelectListUnencrypted(model.Parent, false, null, x => x.GetFullName(), x => x.Id);
            ViewData["RegionType"] = regionTypes.ToSelectListUnencrypted(model.RegionType, false, null, x => x.Name, x => x.Id);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateUpdateRegion(UpdateRegionModel model)
        {
            ValidateModel(model);

            if (!ModelState.IsValid)
            {
                SetRegionsViewData(model);
                return PartialView("_CreateRegionPartial", model);
            }

            try
            {
                _bll.CreateUpdateRegion(model.Id, model.Name, model.Description, model.Parent, model.RegionType,
                    model.HasStreets, model.SaiseId, model.Cuatm);
            }
            catch (SrvException srvex)
            {
                ModelState.AddModelError("", srvex.Message);
                SetRegionsViewData(model);
                return PartialView("_CreateRegionPartial", model);
            }
            return Content(Const.CloseWindowContent);
        }

        [HttpPost]
        public void DeleteRegion(long id)
        {
            _bll.VerificationIfRegionHasReference(id);
            _bll.DeleteRegion(id);
        }

        [HttpPost]
        public void UnDeleteRegion(long id)
        {
            _bll.UnDelete<Region>(id);
        }

        //public ActionResult SelectParentRegion(long? regionId)
        //{
        //	var regions = _bll.GetAll<Region>();
        //	return PartialView("_Select", regions.ToSelectListUnencrypted(0, false, null, x => x.GetFullName(), x => x.Id));
        //}

        public ActionResult SelectRegionType(long? regionId)
        {
            var regionTypes = GetRegionTypes(regionId);

            return PartialView("_Select",
                regionTypes.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Id));
        }

        public IList<RegionType> GetRegionTypes(long? parentId)
        {
            return parentId == null ? _bll.GetAll<RegionType>() : _bll.GetRegionTypesByFilter((long)parentId);
        }

        public ActionResult SelectYesNo()
        {
            var items = new[]
            {
                new SelectListItem {Text = MUI.Yes, Value = bool.TrueString},
                new SelectListItem {Text = MUI.No, Value = bool.FalseString}
            };

            return PartialView("_Select", items);
        }

        public ActionResult SelectStreetType()
        {
            var streetTypes = _bll.GetAll<StreetType>();

            return PartialView("_Select", streetTypes.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Id));
        }

        public ActionResult SelectCircumscriptionList()
        {
            var streetTypes = _bll.GetAll<CircumscriptionList>();

            return PartialView("_Select", streetTypes.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Id));
        }

        [HttpGet]
        public ActionResult UpdateAdministrativeInfo(long regionId)
        {
            var publicAdministration = _bll.GetPublicAdministration(regionId);
            var model = publicAdministration != null
                ? Mapper.Map<PublicAdministration, UpdatePublicAdministrationModel>(publicAdministration)
                : new UpdatePublicAdministrationModel { RegionId = regionId };

            SetAdministrativeViewData(model);
            return PartialView("_PublicAdministrativeInfoPartial", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateAdministrativeInfo(UpdatePublicAdministrationModel model)
        {
            SetAdministrativeViewData(model);

            if (!ModelState.IsValid)
            {
                return PartialView("_PublicAdministrativeInfoPartial", model);
            }

            _bll.UpdateAdministrativeInfo(model.Id, model.Name, model.Surname, model.RegionId, model.ManagerTypeId);
            return Content(Const.CloseWindowContent);
        }

        private void SetAdministrativeViewData(UpdatePublicAdministrationModel model)
        {
            var managerType = _bll.GetAll<ManagerType>().Where(x => x.Deleted == null);
            ViewData["ManagerTypeId"] = managerType.ToSelectListUnencrypted(model.ManagerTypeId, false, null, x => x.Name, x => x.Id);
        }

        private void ValidateModel(UpdateRegionModel model)
        {
            var isUnique = _bll.IsUnique(model.Id, model.Name, model.Parent, model.RegionType);
            if (!isUnique)
            {
                ModelState.AddModelError("", MUI.RegionUniqueKey_ValidateMessage);
            }
        }

        private void ValidateModel(UpdateStreetModel model)
        {
            var isUnique = _bll.IsUnique(model.Id, model.RegionId, model.Name, model.StreetTypeId);
            if (!isUnique)
            {
                ModelState.AddModelError("", MUI.StreetUniqueKey_ValidateMessage);
            }
        }

        private void ValidateModel<T>(long? id, string name) where T : Lookup, new()
        {
            var isUnique = _bll.IsUnique<T>(id, name);
            if (!isUnique)
            {
                ModelState.AddModelError("", MUI.Create_UniqueError);
            }
        }

        [HttpGet]
        public ActionResult UpdateCircumscription(long circumscriptionId)
        {
            _bll.VerificationIsDeletedLookup<Region>(circumscriptionId);
            var model = new UpdateCircumscriptionModel();
            var circumscription = _bll.Get<Region>(circumscriptionId);
            model = Mapper.Map<Region, UpdateCircumscriptionModel>(circumscription);

            return PartialView("_UpdateCircumscription", model);
        }

        [HttpPost]
        public ActionResult UpdateCircumscription(UpdateCircumscriptionModel model)
        {
            if (!_bll.UniqueValidationCircumscription(model.Id, model.CircumscriptionNumber))
            {
                ModelState.AddModelError("", MUI.Lookups_CircumscriptionAlreadyExist);
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_UpdateCircumscription", model);
            }

            _bll.UpdateCircumscription(model.Id, model.CircumscriptionNumber);
            return Content(Const.CloseWindowContent);
        }

        //[HttpPost]
        //public ActionResult SaveGridConfig(string options)
        //{
        //    return null;
        //}

        [HttpGet]
        public ActionResult LinkRegions(long regionId)
        {
            var linkedRegions = _bll.GetLinkedRegionsByRegionId(regionId);
            var model = new LinkRegionsModel() { CurrentRegionId = regionId, LinkedRegions = new List<LinkedRegionsModel>() };
            foreach (var linkedRegion in linkedRegions)
            {
                model.LinkedRegions.Add(
                    new LinkedRegionsModel
                    {
                        LinkedRegionId = linkedRegion.Id,
                        FullRegionName = linkedRegion.FullyQualifiedName
                    });
            }
            if (linkedRegions.Count == 0)
            {
                model.LinkedRegions.Add(
                    new LinkedRegionsModel
                    {
                        LinkedRegionId = regionId,
                        FullRegionName = _bll.GetFullyQualifiedName(regionId) != null
                                        ? _bll.GetFullyQualifiedName(regionId).FullyQualifiedName
                                        : string.Empty
                    });
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult LinkRegions(long regionId, long[] linkedRegionIds)
        {
            if (linkedRegionIds.Count() > 0)
            {
                _bll.SaveLinkedRegions(regionId, linkedRegionIds);
            }
            return Content(Const.CloseWindowContent);
        }

        public JsonResult GetLinkedRegions(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("FullyQualifiedName", ComparisonOperator.Contains);
            var data = _bll.GetAvailableRegions(pageRequest);
            var response = new Select2PagedResponse(
                data.Items.ToSelectSelect2List(
                    x => x.Id,
                    x => x.FullyQualifiedName).ToList(),
                    data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}