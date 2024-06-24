using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Interop;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Interop;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Transaction = CEC.SRV.Domain.Interop.Transaction;

namespace CEC.Web.SRV.Controllers
{
    [Authorize(Roles = CEC.SRV.Domain.Constants.Transactions.Interop)]
    public class InteropController: BaseController
    {

        private readonly IInteropBll _bll;

        public InteropController(IInteropBll bll)
        {
            _bll = bll;
        }

        public ActionResult DashBoard()
        {
            return View();
        }
        public ActionResult InteropSystems()
        {
            return View();
        }
        public ActionResult Institutions()
        {
            return View();
        }
        public JqGridJsonResult ListInteropSystemsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<InteropSystemGridModel>();

            var data = _bll.Get<InteropSystem>(pageRequest);

            return data.ToJqGridJsonResult<InteropSystem, InteropSystemGridModel>();
        }

        public JsonResult GetInteropSystems(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _bll.Get<InteropSystem>(pageRequest);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id, x => x.Description).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectInteropSystems()
        {
            var items = _bll.GetAll<InteropSystem>();

            return PartialView("_Select", items.ToSelectListUnencrypted(0, false, null, x => x.Description, x => x.Id));
        }

        public ActionResult SelectInstitutions()
        {
            var items = _bll.GetAll<Institution>();

            return PartialView("_Select", items.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Id));
        }


        public JsonResult GetInteropSystemName(long id)
        {
            var item = _bll.Get<InteropSystem>(id);
            return Json(item != null ? item.Description : string.Empty);
        }
        

        [HttpGet]
        public ActionResult CreateUpdateInteropSystem(long? itemId)
        {
            if (itemId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<InteropSystem>(itemId.Value);
            }

            var model = new UpdateInteropSystemModel();

            if (itemId != null)
            {
                var institution = _bll.Get<InteropSystem>(itemId.Value);
                model = Mapper.Map<InteropSystem, UpdateInteropSystemModel>(institution);
            }


            GetProcessingType(model.TransactionProcessingType);
            GetStatus(model.StatusId);

            return PartialView("_UpdateInteropSystemPartial", model);
        }

        private void GetProcessingType(TransactionProcessingTypes selected)
        {
            var types = InteropHelper.GetTransactionProcessingTypes();
            types.Remove("");
            ViewData["TransactionProcessingType"] = types.ToSelectList(selected.ToString());
        }

        private void GetStatus(long? selectedId)
        {
            var status = _bll.GetAll<PersonStatusType>().Where(x => x.Deleted == null);
            ViewData["StatusId"] = status.ToSelectListUnencrypted(selectedId??0, false, MUI.SelectPrompt, x => x.Name, x => x.Id);
        }

        private void ValidateModel(UpdateInteropSystemModel model)
        {
//            var isUnique = _bll.IsUnique(model.Id, model.InstitutionTypeId, model.LegacyId);
//            if (!isUnique)
//            {
//                ModelState.AddModelError("", MUI.InstitutionCreate_UniqueLegacyIdError);
//            }
        }


        [HttpPost]
        public ActionResult CreateUpdateInteropSystem(UpdateInteropSystemModel model)
        {
            ValidateModel(model);

            if (model.PersonStatusConsignment && !model.StatusId.HasValue)
            {
                ModelState.AddModelError("StatusId", MUI.InteropController_SelectPersonStatus_Message);
            }

            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdateInteropSystem(model.Id, model.Name, model.Description, model.TransactionProcessingType, model.PersonStatusConsignment, model.StatusId, model.TemporaryAddressConsignment);
                return Content(Const.CloseWindowContent);
            }


            GetProcessingType(model.TransactionProcessingType);
            GetStatus(model.StatusId);

            return PartialView("_UpdateInteropSystemPartial", model);
        }


        public JqGridJsonResult ListInstitutionsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<InstitutionGridModel>();

            var data = _bll.Get<Institution>(pageRequest);

            return data.ToJqGridJsonResult<Institution, InstitutionGridModel>();
        }


        [HttpGet]
        public ActionResult CreateUpdateInstitution(long? institutionId)
        {
            if (institutionId.HasValue)
            {
                _bll.VerificationIsDeletedLookup<Institution>(institutionId.Value);
            }

            var model = new UpdateInstitutionModel();

            if (institutionId != null)
            {
                var institution = _bll.Get<Institution>(institutionId.Value);
                model = Mapper.Map<Institution, UpdateInstitutionModel>(institution);
            }
            return PartialView("_UpdateInstitutionPartial", model);
        }

    
        private void ValidateModel(UpdateInstitutionModel model)
        {
            var isUnique = _bll.IsUnique(model.Id, model.InstitutionTypeId, model.LegacyId);
            if (!isUnique)
            {
                ModelState.AddModelError("", MUI.InstitutionCreate_UniqueLegacyIdError);
            }


            if (model.AddressId <= 0)
            {
                ModelState.AddModelError("AddressId", MUI.InstitutionAddressError);
            }
        }


        [HttpPost]
        public ActionResult CreateUpdateInstitution(UpdateInstitutionModel model)
        {
            ValidateModel(model);

            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdateInstitution(model.Id, model.Name, model.Description, model.InstitutionTypeId, model.LegacyId, model.AddressId);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateInstitutionPartial", model);
        }


        [HttpPost]
        public void DeleteInteropSystem(long id)
        {
            _bll.Delete<InteropSystem>(id);
        }
        [HttpPost]
        public void UnDeleteInteropSystem(long id)
        {
            _bll.UnDelete<InteropSystem>(id);
        }

        [HttpPost]
        public void DeleteInstitution(long id)
        {
            _bll.Delete<Institution>(id);
        }

        [HttpPost]
        public void UnDeleteInstitution(long id)
        {
            _bll.UnDelete<Institution>(id);
        }


        public ActionResult Transactions()
        {
            return View();
        }



        public JqGridJsonResult ListTransactionsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<TransactionGridModel>();

            var data = _bll.Get<Transaction>(pageRequest);

            return data.ToJqGridJsonResult<Transaction, TransactionGridModel>();
        }


        [HttpGet]
        public ActionResult CreateUpdateTransaction(long? transactionId)
        {
            if (transactionId.HasValue)
            {
                //_bll.VerificationIsDeletedLookup<Transaction>(transactionId.Value);
            }

            var model = new UpdateTransactionModel();

            if (transactionId != null)
            {
                var bean = _bll.Get<Transaction>(transactionId.Value);
                model = Mapper.Map<Transaction, UpdateTransactionModel>(bean);
            }
            return PartialView("_UpdateTransactionPartial", model);
        }


        private void ValidateModel(UpdateTransactionModel model)
        {
           //todo:
//            var isUnique = _bll.IsUnique(model.Id, model.InstitutionTypeId, model.LegacyId);
//            if (!isUnique)
//            {
//                ModelState.AddModelError("", MUI.InstitutionCreate_UniqueLegacyIdError);
//            }
//
//
//            if (model.AddressId <= 0)
//            {
//                ModelState.AddModelError("AddressId", MUI.InstitutionAddressError);
//            }
        }


        [HttpPost]
        public ActionResult CreateUpdateTransaction(UpdateTransactionModel model)
        {
            ValidateModel(model);

            if (ModelState.IsValid)
            {
                _bll.SaveOrUpdateTransaction(model.Id, model.Idnp, model.LastName, model.FirstName, model.DateOfBirth, model.InstitutionTypeId, model.InstitutionId);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateTransactionPartial", model);
        }

        [HttpPost]
        public ActionResult AssignPollingStationForm(List<long> transactionIds)
        {
            var model = new AssignPollingStationModel
            {
                SameSystemAndInstitution = _bll.VerificationSameSystemAndInstitution(transactionIds)
            };

            if (model.SameSystemAndInstitution)
            {
                Transaction transaction = _bll.Get<Transaction>(transactionIds[0]);
                if (transaction.Institution?.InstitutionAddress?.PollingStation != null)
                {
                    model.NewPollingStation = new PersonPollingStationModel
                    {
                        PStationId = transaction.Institution.InstitutionAddress.PollingStation.Id
                    };
                }
            }
            return PartialView("_AssignPollingStation", model);
        }

        [HttpPost]
        public ActionResult AssignPollingStation(AssignPollingStationModel model)
        {
            if (ModelState.IsValid)
            {
                int success = 0, error = 0;

                _bll.ProcessTransactions(model.TransactionIds, model.NewPollingStation.PStationId, model.ElectionInfo.ElectionId, ref success, ref error);

                return Content(error > 0 
                    ? $"$$Au fost procesate {model.TransactionIds.Count} tranzactii din care {success} cu succes si {error} cu eroare" 
                    : $"$$Au fost procesate {success} tranzactii cu succes");
            }
            return PartialView("_AssignPollingStation", model);
        }

        [HttpPost]
        public ActionResult UndoTransactions(List<long> transactionIds)
        {
            int success = 0, error = 0;

            _bll.UndoTransactions(transactionIds, ref success, ref error);

            string message = error > 0 
                ? $"Au fost anulate {transactionIds.Count} tranzactii din care {success} cu succes si {error} cu eroare " 
                : $"Au fost anulate {success} tranzactii";

            return Json(new { Message = message });
        }

        public JsonResult GetInstitutionTypesAjax(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _bll.SearchInstitutionTypes(pageRequest);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id, x => x.Name).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstitutionsAjax(Select2Request request, long? institutionTypeId)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _bll.SearchInstitutions(pageRequest, institutionTypeId);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id, x => x.Name).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}