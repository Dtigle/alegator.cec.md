using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using System.Web.Mvc;
using CEC.SRV.Domain.Constants;
using CEC.Web.SRV.Infrastructure.Logger;
using CEC.Web.SRV.LoggingService;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Properties;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using RSP.CEC.WebClient.RspCecService;
using SoftTehnica.MvcReportViewer;
using CEC.SRV.Domain.ViewItem;
using CEC.Web.SRV.Models.Address;
using CEC.Web.SRV.Models.Conflict;
using CEC.SRV.Domain.Importer;
using UpdateAddressModel = CEC.Web.SRV.Models.Voters.UpdateAddressModel;

namespace CEC.Web.SRV.Controllers
{
    public class VotersController : BaseController
    {
        private readonly IConfigurationSettingBll _settingsBll;
        private readonly IVotersBll _bll;
        private readonly IPrintBll _printBll;
        private readonly IConflictBll _conflictBll;
        private string _rspUserName;
        private string _rspUserPwd;
        private const string RspUserParamName = "RspUser";
        private const string RspUserPwdParamName = "RspPass";

        public VotersController(IConfigurationSettingBll settingsBll, IVotersBll bll, IPrintBll printBll, IConflictBll conflictBll)
        {
            _settingsBll = settingsBll;
            _bll = bll;
            _printBll = printBll;
            _conflictBll = conflictBll;
        }

        // GET: /Voters/
        [Authorize(Roles = Transactions.Voters)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StayStatements()
        {
            return View();
        }

        public JqGridJsonResult ListVotersAjax(JqGridRequest request, long? regionId, long? pollingStationId)
        {
            var pageRequest = request.ToPageRequest<VotersGridModel>();

            var data = _bll.GetByFilters(pageRequest, regionId, pollingStationId);

            return data.ToJqGridJsonResult<VoterViewItem, VotersGridModel>();
        }

        public JqGridJsonResult ListVotersAjax2(JqGridRequest request, long? regionId, long? pollingStationId,
            long? localityId = null,
            long? streetId = null,
            long? addressId = null,
            int? houseNumber = null,
            int? apNumber = null,
            string apSuffix = null
            )
        {
            var pageRequest = request.ToPageRequest<VotersGridModel>();

            var data = _bll.GetByFilters(pageRequest, regionId, pollingStationId,
                localityId,
                streetId,
                addressId,
                houseNumber,
                apNumber,
                apSuffix
            );

            return data.ToJqGridJsonResult<VoterViewItem, VotersGridModel>();
        }

        [HttpPost]
        public ActionResult ExportVoters(JqGridRequest request, ExportType exportType, long? regionId, long? pollingStationId)
        {
            return ExportGridData(request, exportType, "Alegatori", typeof(VotersGridModel),
                x => ListVotersAjax(x, regionId, pollingStationId));
        }

        public ActionResult SelectVoterStatus()
        {
            var voterStatus = _bll.GetAll<PersonStatusType>();

            return PartialView("_Select", voterStatus.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Id));
        }

        public ActionResult SelectVoterGender()
        {
            var voterGender = _bll.GetAll<Gender>();

            return PartialView("_Select", voterGender.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Name));
        }

        public ActionResult SearchVotersByIdnpAjax(JqGridRequest request, string idnp)
        {
            var pageRequest = request.ToPageRequest<SearchVotersGridModel>();

            var data = _bll.GetIdnp(pageRequest, idnp);

            return data.ToJqGridJsonResult<Person, SearchVotersGridModel>();
        }

        public JqGridJsonResult ListStayStatementsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<StayStatementsGridModel>();

            var data = _bll.GetStayStatements(pageRequest);

            return data.ToJqGridJsonResult<StayStatement, StayStatementsGridModel>();
        }

        [HttpPost]
        public ActionResult ExportStayStatements(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "DeclaratiiSedere",
                typeof(StayStatementsGridModel), ListStayStatementsAjax);
        }

        public ActionResult ListVoterAdressesHistoryAjax(JqGridRequest request, long personId)
        {
            var pageRequest = request.ToPageRequest<PersonFullAdressGridModel>();

            var data = _bll.GetAddressHistory(pageRequest, personId);

            return data.ToJqGridJsonResult<PersonAddress, PersonFullAdressGridModel>();
        }

        public ActionResult ListVoterConflictsHistoryAjax(JqGridRequest request, string personId)
        {
            var pageRequest = request.ToPageRequest<VoterConflictGridModel>();

            var data = _conflictBll.GetConflictHistory(pageRequest, personId);

            return data.ToJqGridJsonResult<RspModificationData, VoterConflictGridModel>();
        }

        public ActionResult ListVoterDocumentHistoryAjax(JqGridRequest request, long personId)
        {
            var pageRequest = request.ToPageRequest<VoterDocumentGridModel>();

            var data = _bll.GetIdentityDocumentsHistory(personId);

            return data.ToJqGridJsonResult<PersonDocument, VoterDocumentGridModel>();
        }

        public ActionResult ListVoterVotingStationHistoryAjax(JqGridRequest request, long personId)
        {
            var pageRequest = request.ToPageRequest<VoterPollingStationHistoryGridModel>();

            var data = _bll.GetPollingStationHistory(pageRequest, personId);

            return data.ToJqGridJsonResult<PollingStation, VoterPollingStationHistoryGridModel>();
        }

        public ActionResult CreateVoterProfile(long? personId, string idnp)
        {
            var person = personId.HasValue ? _bll.Get<Person>(personId.Value) : !string.IsNullOrEmpty(idnp) ? _bll.GetByIdnp(idnp) : null;
            if (person == null)
            {
                return PartialView("_VoterNotFound");
            }

            var personInfo = new VoterProfileModel
            {
                PersonId = person.Id,
                IDNP = person.Idnp,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                SurName = person.Surname,
                Age = "" + person.Age,
                Sex = person.Gender.Name,
                Status = person.CurrentStatus.StatusType.Name,
                DateOfBirth = person.DateOfBirth,

                Modified = person.Modified.HasValue ? person.Modified.Value.DateTime.ToString("dd.MM.yyyy") : string.Empty,
                ModifiedBy = person.ModifiedBy != null ? person.ModifiedBy.UserName : string.Empty
            };

            if (person.Document != null)
            {
                personInfo.DocType = person.Document.Type.Description;
                personInfo.DocNumber = person.Document.DocumentNumber;
                personInfo.DocIssuedDate = person.Document.IssuedDate;
                personInfo.DocIssuedBy = person.Document.IssuedBy;
                personInfo.DocValidBy = person.Document.ValidBy;
            }

            var model = new ViewVoterProfileModel
            {
                PersonInfo = personInfo,
                BaseAddressInfo = new VoterAdressModel
                {
                    PersonAddressId = person.EligibleAddress.Id,
                    AddressId = person.EligibleAddress.Address.Id,
                    ApNumber = person.EligibleAddress.ApNumber,
                    ApSuffix = person.EligibleAddress.ApSuffix,
                    BlNumber = person.EligibleAddress.Address.GetBuildingNumber(),
                    FullAddress = person.EligibleAddress.GetFullPersonAddress(true),
                    Street = person.EligibleAddress.Address.Street?.Name,
                    StreetId = person.EligibleAddress.Address.Street?.Id,
                    Region = person.EligibleAddress.Address.Street?.Region.Parent?.Name,
                    RegionId = person.EligibleAddress.Address.Street?.Region.Parent?.Id,
                    Locality = person.EligibleAddress.Address.Street?.Region.GetFullName(),
                    LocalityId = person.EligibleAddress.Address.Street?.Region.Id
                },

                RegionStreetsType = _bll.GetRegionStreetsType(),
                DeclaredStayAddressInfo = new PersonAddressModel(),
                ElectionInfo = new ElectionModel()
            };


            //SetViewData(model);
            return PartialView(model);
        }

        public ActionResult VotersBySurname(long? personId, string idnp)
        {
            var person = personId.HasValue ? _bll.Get<Person>(personId.Value) : !string.IsNullOrEmpty(idnp) ? _bll.GetByIdnp(idnp) : null;
            if (person == null)
            {
                return PartialView("_VoterNotFound");
            }

            var model = new VotersBySurnameModel()
            {
                PersonId = person.Id,
                IDNP = person.Idnp,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                SurName = person.Surname,
                BaseAdress = new VoterAdressModel
                {
                    PersonAddressId = person.EligibleAddress.Id,
                    AddressId = person.EligibleAddress.Address.Id,
                    ApNumber = person.EligibleAddress.ApNumber,
                    ApSuffix = person.EligibleAddress.ApSuffix,
                    BlNumber = person.EligibleAddress.Address.GetBuildingNumber(),
                    FullAddress = person.EligibleAddress.GetFullPersonAddress(true),
                    Street = person.EligibleAddress.Address.Street?.Name,
                    StreetId = person.EligibleAddress.Address.Street?.Id,
                    Region = person.EligibleAddress.Address.Street?.Region.Parent?.Name,
                    RegionId = person.EligibleAddress.Address.Street?.Region.Parent?.Id,
                    Locality = person.EligibleAddress.Address.Street?.Region.GetFullName(),
                    LocalityId = person.EligibleAddress.Address.Street?.Region.Id
                },
            };

            return PartialView(model);
        }

        public ActionResult ListVotersBySurname(JqGridRequest request, string idnp, string surname)
        {
            var pageRequest = request.ToPageRequest<VotersGridModel>();

            var data = _bll.GetByFilters(pageRequest, null, null, null, null, null, null, null, null, surname, idnp);

            return data.ToJqGridJsonResult<VoterViewItem, VotersGridModel>();
        }

        public JqGridJsonResult ListAllAdresses(JqGridRequest request, string personId)
        {
            var pageRequest = request.ToPageRequest<StayStatementsGridModel>();

            var data = _bll.GetStayStatements(pageRequest);

            return data.ToJqGridJsonResult<StayStatement, StayStatementsGridModel>();
        }

        public JqGridJsonResult ListVoterStayStatementHistoryAjax(JqGridRequest request, long personId)
        {
            var pageRequest = request.ToPageRequest<StayStatementsGridModel>();

            var data = _bll.GetStayStatementForPerson(pageRequest, personId);

            return data.ToJqGridJsonResult<StayStatement, StayStatementsGridModel>();
        }

        public ActionResult ModalDataGrid(long? personId, ViewVoterProfileModel.Section data,
            long? regionId,
            long? localityId,
            long? streetId,
            long? addressId,
            int? houseNumber,
            int? apNumber,
            string apSuffix)
        {
            ModalDataModel model;
            switch (data)
            {
                case ViewVoterProfileModel.Section.General:
                    model = new ModalDataModel
                    {
                        ModelType = typeof(VotersGridModel),
                        RegionId = regionId,
                        LocalityId = localityId,
                        StreetId = streetId,
                        AddressId = addressId,
                        HouseNumber = houseNumber,
                        ApNumber = apNumber,
                        ApSuffix = apSuffix
                    };
                    return PartialView(model);
                case ViewVoterProfileModel.Section.Addresses:
                    model = new ModalDataModel
                    {
                        ModelType = typeof(PersonFullAdressGridModel),
                        PersonId = "" + personId
                    };
                    return PartialView(model);
                case ViewVoterProfileModel.Section.Conflicts:
                    var person = _bll.Get<Person>(personId.Value);
                    model = new ModalDataModel
                    {
                        ModelType = typeof(VoterConflictGridModel),
                        PersonId = person.Idnp
                    };
                    return PartialView(model);
                case ViewVoterProfileModel.Section.IdentityDocuments:
                    model = new ModalDataModel
                    {
                        ModelType = typeof(VoterDocumentGridModel),
                        PersonId = "" + personId
                    };
                    return PartialView(model);
                case ViewVoterProfileModel.Section.VotingStation:
                    model = new ModalDataModel
                    {
                        ModelType = typeof(VoterPollingStationHistoryGridModel),
                        PersonId = "" + personId
                    };
                    return PartialView(model);
                case ViewVoterProfileModel.Section.StayDeclaration:
                    model = new ModalDataModel
                    {
                        ModelType = typeof(StayStatementsGridModel),
                        PersonId = "" + personId
                    };
                    return PartialView(model);
                default:
                    return PartialView("_ImplementationMissing");
            }

        }

        public ActionResult CreateStayStatement(long personId)
        {
            var person = _bll.Get<Person>(personId);
            var personInfo = new PersonModel
            {
                PersonId = personId,
                IDNP = person.Idnp,
                FullName = person.FullName,
                DateOfBirth = person.DateOfBirth,
            };

            if (person.Document != null)
            {
                personInfo.DocType = person.Document.Type.Description;
                personInfo.DocNumber = person.Document.DocumentNumber;
                personInfo.DocIssuedDate = person.Document.IssuedDate;
                personInfo.DocIssuedBy = person.Document.IssuedBy;
                personInfo.DocValidBy = person.Document.ValidBy;
            }

            var model = new StayStatementModel
            {
                PersonInfo = personInfo,
                BaseAddressInfo = new PersonAddressModel
                {
                    PersonAddressId = person.EligibleAddress?.Id ?? -1,
                    AddressId = person.EligibleAddress?.Address?.Id ?? -1,
                    FullAddress = person.EligibleAddress?.GetFullPersonAddress(true) ?? string.Empty
                },
                RegionStreetsType = _bll.GetRegionStreetsType(),
                DeclaredStayAddressInfo = new PersonAddressModel(),
                ElectionInfo = new ElectionModelStaytment()
            };

            //SetViewData(model);
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStayStatement(StayStatementModel model)
        {
            //SetViewData(model);
            long staystatementId;

            if (model.ElectionInfo.ElectionId == 0)
            {
                ModelState.AddModelError("ElectionInfo.ElectionId", MUI.StayStatementErrorRequired_Election);
                return PartialView(model);
            }

            var stayStatementExists = _bll.ElectionUniqueStayStatement(model.PersonInfo.PersonId, model.ElectionInfo.ElectionId);
            if (stayStatementExists)
            {
                ModelState.AddModelError("ElectionInfo.ElectionId", MUI.StayStatementErrorUnique_Election);
                return PartialView(model);
            }

            if (model.StayStatementRegionId == 0)
            {
                ModelState.AddModelError("StayStatementRegionId", MUI.StayStatementErrorRequired_StayStatementRegionId);
                return PartialView(model);
            }

            if (model.HasStreets)
            {
                model.RegionStreetsType = RegionStreetsType.WithStreets;
                if (model.DeclaredStayAddressInfo.AddressId == 0)
                {
                    ModelState.AddModelError("DeclaredStayAddressInfo.AddressId", MUI.StayStatementErrorRequired_DeclaredStayAddress);
                    return PartialView(model);
                }

                if (_bll.VerificationSameAddress(model.BaseAddressInfo.PersonAddressId, model.DeclaredStayAddressInfo.AddressId))
                {
                    ModelState.AddModelError("DeclaredStayAddressInfo.AddressId", MUI.StayStatementError_SameAddress);
                    return PartialView(model);
                }

                var person = _bll.Get<Person>(model.PersonInfo.PersonId);
                if (person.EligibleAddress.Address.Id == model.DeclaredStayAddressInfo.AddressId)
                {
                    ModelState.AddModelError("DeclaredStayAddressInfo.AddressId", MUI.StayStatementErrorDuplicate_DeclaredStayAddress);
                    return PartialView(model);
                }

                staystatementId = _bll.CreateStayStatement(model.Id, model.PersonInfo.PersonId,
                    model.DeclaredStayAddressInfo.AddressId, model.DeclaredStayAddressInfo.ApNumber,
                    model.DeclaredStayAddressInfo.ApSuffix,
                    model.ElectionInfo.ElectionId);
            }
            else
            {
                model.RegionStreetsType = RegionStreetsType.WithoutStreets;
                if (_bll.VerificationSameRegion(model.BaseAddressInfo.PersonAddressId, model.StayStatementRegionId))
                {
                    ModelState.AddModelError("StayStatementRegionId", MUI.StayStatementError_SameRegion);
                    return PartialView(model);
                }

                if (model.StayStatementPollingStationId == 0)
                {
                    ModelState.AddModelError("StayStatementPollingStationId",
                        MUI.StayStatementErrorRequired_StayStatementPollingStationId);
                    return PartialView(model);
                }

                staystatementId = _bll.CreateStayStatement(model.PersonInfo.PersonId, model.StayStatementPollingStationId,
                    model.ElectionInfo.ElectionId);
            }

            return Json(new { ssId = staystatementId });
        }

        //private void SetViewData(StayStatementModel model)
        //{
        //	var elections = _bll.GetElection()
        //		.ToSelectListUnencrypted(model.ElectionInfo.ElectionId, false, null,
        //			e => string.Format("{0}: {1}", e.ElectionDate.ToShortDateString(), e.ElectionType.Name),
        //			e => e.Id);
        //	ViewData["ElectionId"] = elections;
        //}

        [HttpPost]
        public bool GetRegionStreetsType(long regionId)
        {
            var region = _bll.Get<Region>(regionId);
            return region.HasStreets;
        }

        public ActionResult SetStatus(long personId)
        {
            var person = _bll.Get<Person>(personId);
            var model = Mapper.Map<Person, UpdateVotersStatusModel>(person);
            GetStatus();
            return PartialView("_UpdateVotersStatus", model);
        }

        [HttpPost]
        public ActionResult SetStatus(UpdateVotersStatusModel model)
        {
            if (ModelState.IsValid)
            {
                _bll.UpdateStatus(model.PersonInfo.Id, model.StatusId, model.ConfirmationNew);

                try
                {
                    LoggerUtils logEvent = new LoggerUtils();
                    logEvent.LogEvent(LogLevel.Information, Events.VoterStatus.Value, Events.VoterStatus.Description, new Dictionary<string, string>
                    {
                        { Events.VoterStatus.Attributes.Action, "Update Status"},
                        { Events.VoterStatus.Attributes.Voter,  model.PersonInfo.Id.ToString()},
                        { Events.VoterStatus.Attributes.Status,  model.StatusId.ToString()}
                    });
                }
                catch
                { // 
                }

                return Content(Const.CloseWindowContent);

            }
            return PartialView("_UpdateVotersStatus", model);
        }

        private void GetStatus()
        {
            var status = _bll.GetAll<PersonStatusType>().Where(x => x.Deleted == null);
            ViewData["StatusId"] = status.ToSelectListUnencrypted(0, false, MUI.SelectPrompt, x => x.Name, x => x.Id);
        }

        public JsonResult GetRegions(Select2Request request)
        {
            var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
            var data = _bll.SearchRegion(pageRequest);
            var response = new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(),
                data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveElections(Select2Request request, DateTime? electionDate)
        {
            var pageRequest = request.ToPageRequest("ElectionType.Name", ComparisonOperator.Contains);
            var data = _bll.SearchActiveElections(pageRequest, electionDate);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id,
                        x => string.Format("{0} - {1}", x.ElectionRounds != null && x.ElectionRounds.Count > 0 ? x.ElectionRounds.FirstOrDefault().ElectionDate.ToShortDateString() : string.Empty, x.NameRo)).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveElectionsForStaytments(Select2Request request, DateTime? electionDate)
        {
            var pageRequest = request.ToPageRequest("ElectionType.Name", ComparisonOperator.Contains);
            var data = _bll.SearchActiveElections(pageRequest, electionDate);
            var response =
                new Select2PagedResponse(
                    data.Items.Where(x => x.ElectionType.AcceptResidenceDoc == true).ToSelectSelect2List(x => x.Id,
                        x => string.Format("{0} - {1}", x.ElectionRounds != null && x.ElectionRounds.Count > 0 ? x.ElectionRounds.FirstOrDefault().ElectionDate.ToShortDateString() : string.Empty, x.NameRo)).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetElectionRounds(Select2Request request, long electionId)
        {
            var pageRequest = request.ToPageRequest("ElectionType.Name", ComparisonOperator.Contains);
            var data = _bll.SearchElectionRounds(pageRequest, electionId);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id,
                        x => string.Format("{0} - {1}", x.ElectionDate.ToShortDateString(), x.NameRo)).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPollingStations(Select2Request request, long? regionId)
        {
            var pageRequest = request.ToPageRequest("FullNumber", ComparisonOperator.Contains);
            var data = _bll.SearchPollingStations(pageRequest, regionId);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id,
                        x =>
                            string.Format("{0} - {1}", x.FullNumber,
                                x.PollingStationAddress != null ? x.GetFullAddress() : !string.IsNullOrEmpty(x.Location) ? x.Location : MUI.FilterForVoters_PollingStation_MissingAddress))
                        .ToList(), data.Total, data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectElectionInstance()
        {
            var electionInstance = _bll.GetAll<Election>();
            return PartialView("_Select",
                electionInstance.ToSelectListUnencrypted(0, false, null, x => x.NameRo, x => x.Id));
        }

        public JsonResult GetAddresses(Select2Request request, long? regionId, long? streetId)
        {
            var pageRequest = request.ToPageRequest("FullAddress", ComparisonOperator.Contains);
            var data = _bll.SearchAddress(pageRequest, regionId, streetId);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id, x => x.FullAddress).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetStreets(Select2Request request, long? regionId)
        {
            var pageRequest = request.ToPageRequest("FullName", ComparisonOperator.Contains);
            var data = _bll.SearchStreets(pageRequest, regionId);
            var response =
                new Select2PagedResponse(
                    data.Items.ToSelectSelect2List(x => x.Id, x => x.FullName).ToList(), data.Total,
                    data.PageSize);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StayStatementView(long id)
        {
            var stayStatement = _bll.Get<StayStatement>(id);
            var person = stayStatement.Person;
            var declaredStayAddress = stayStatement.DeclaredStayAddress;
            var baseAddressInfo = stayStatement.BaseAddress;
            var electionInfo = stayStatement.ElectionInstance;
            var personInfo = new PersonModel
            {
                PersonId = person.Id,
                IDNP = person.Idnp,
                FullName = person.FullName,
                DateOfBirth = person.DateOfBirth,
            };

            if (person.Document != null)
            {
                personInfo.DocType = person.Document.Type.Description;
                personInfo.DocNumber = person.Document.DocumentNumber;
                personInfo.DocIssuedDate = person.Document.IssuedDate;
                personInfo.DocIssuedBy = person.Document.IssuedBy;
                personInfo.DocValidBy = person.Document.ValidBy;

            }
            var model = new StayStatementViewModel
            {
                Id = id,
                PersonInfo = personInfo,
                BaseAddressInfo = new PersonAddressModel
                {
                    PersonAddressId = baseAddressInfo.Id,
                    AddressId = baseAddressInfo.Address.Id,
                    FullAddress = baseAddressInfo.GetFullPersonAddress(true),
                },
                DeclaredStayAddressInfo = new PersonAddressModel
                {
                    AddressId = declaredStayAddress.Address.Id,
                    ApNumber = declaredStayAddress.ApNumber,
                    ApSuffix = declaredStayAddress.ApSuffix,
                    FullAddress = declaredStayAddress.GetFullPersonAddress(true)
                },
                ElectionInfo = new ElectionModelStaytment
                {
                    ElectionId = electionInfo.Id,
                    ElectionDate = electionInfo.ElectionRounds.LastOrDefault().ElectionDate,
                    ElectionTypeName = electionInfo.NameRo
                },
                CreationDate = stayStatement.Created.Value.LocalDateTime.ToShortDateString(),
                IsDeleted = stayStatement.Deleted.HasValue
            };
            //SetViewData(model);
            return PartialView(model);
        }

        public ActionResult SelectAddressType()
        {
            var voterStatus = _bll.GetAll<PersonAddressType>();

            return PartialView("_Select", voterStatus.ToSelectListUnencrypted(0, false, null, x => x.Name, x => x.Id));
        }

        public ActionResult UpdateAddress(long personId)
        {
            var person = _bll.GetPersonWithEligibleResidence(personId);

            var model = new UpdateAddressModel
            {
                Id = personId,
                PersonInfo = new PersonModel
                {
                    PersonId = personId,
                    FullName = person.FullName
                },

                BaseAddressInfo = new PersonAddressModel
                {
                    PersonAddressId = person.EligibleAddress.Id,
                    AddressId = person.EligibleAddress.Address.Id,
                    FullAddress = person.EligibleAddress.GetFullPersonAddress(true),
                },

                DeclaredStayAddressInfo = new PersonAddressModel()
            };

            return PartialView("_UpdateAddress", model);
        }

        [HttpPost]
        public ActionResult UpdateAddress(UpdateAddressModel model)
        {
            // this property not support Required attribute because it is used in many contexts
            if (model.DeclaredStayAddressInfo.AddressId == 0)
            {
                ModelState.AddModelError("DeclaredStayAddressInfo.AddressId", MUI.StayStatementErrorRequired_DeclaredStayAddress);
            }

            if (ModelState.IsValid)
            {
                _bll.UpdateAddress(model.Id, model.DeclaredStayAddressInfo.AddressId, model.DeclaredStayAddressInfo.ApNumber,
                    model.DeclaredStayAddressInfo.ApSuffix);
                try
                {
                    LoggerUtils logEvent = new LoggerUtils();
                    logEvent.LogEvent(LogLevel.Information, Events.VoterAddress.Value, Events.VoterAddress.Description, new Dictionary<string, string>
                    {
                        { Events.VoterAddress.Attributes.Action, "Update Address"},
                        { Events.VoterAddress.Attributes.Voter,  model.Id.ToString()},
                        { Events.VoterAddress.Attributes.Address,  model.DeclaredStayAddressInfo.AddressId.ToString()}
                    });
                }
                catch
                { // 
                }
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_UpdateAddress", model);
        }

        [HttpPost]
        public ActionResult ChangePollingStationForm(long[] peopleIds)
        {
            _bll.VerificationSameRegion(peopleIds);
            var model = new ChangePollingStationModel();
            return PartialView("_ChangePollingStation", model);
        }

        [HttpPost]
        public ActionResult ChangePollingStation(ChangePollingStationModel model)
        {
            if (ModelState.IsValid)
            {
                _bll.ChangePollingStation(model.NewPollingStation.PStationId, model.PeopleIds);
                return Content(Const.CloseWindowContent);
            }
            return PartialView("_ChangePollingStation", model);
        }

        [HttpPost]
        public void CancelStayStatement(long stayStatementId)
        {
            _bll.CancelStayStatement(stayStatementId);
        }

        public ActionResult StayStatementPrinting(long stayStatementId)
        {
            var model = new StayStatementPrintingModel
            {
                StayStatementId = stayStatementId,
                ReportPath = Settings.Default.RS_SRV_StayStatementForm
            };
            return PartialView(model);
        }

        public ActionResult StayStatementReport(long stayStatementId)
        {
            var ssrsParameters = new SSRSPrintParameters
            {
                ServerUrl = ConfigurationManager.AppSettings["SSRS_ReportExecutionService"],
                UserName = ConfigurationManager.AppSettings["MvcReportViewer.Username"],
                Password = ConfigurationManager.AppSettings["MvcReportViewer.Password"],
                ReportName = Settings.Default.RS_SRV_StayStatementForm
            };
            var reportData = _printBll.RequestStayStatementReport(ssrsParameters, stayStatementId);
            return File(reportData, "application/pdf");
        }

        public JsonResult GetPollingStationName(long id)
        {
            var pollingStation = _bll.Get<PollingStation>(id);
            return Json(pollingStation != null
                ? string.Format("{0} - {1}", pollingStation.FullNumber, pollingStation.PollingStationAddress != null
                    ? pollingStation.GetFullAddress()
                    : MUI.FilterForVoters_PollingStation_MissingAddress)
                : string.Empty);
        }

        public JsonResult GetElectionsName(long id)
        {
            var election = _bll.Get<Election>(id);
            return Json(election != null
                ? string.Format("{0} - {1}", election.ElectionRounds.LastOrDefault().ElectionDate.ToShortDateString(), election.ElectionType.Name)
                : string.Empty);
        }

        public JsonResult GetRegionName(long id)
        {
            var region = _bll.Get<Region>(id);
            return Json(region != null ? region.GetFullName() : string.Empty);
        }

        public JsonResult GetPollingStationsName(long id)
        {
            var pollingStation = _bll.Get<PollingStation>(id);
            return
                Json(pollingStation != null
                    ? string.Format("{0} - {1}", pollingStation.FullNumber,
                        pollingStation.PollingStationAddress != null
                            ? pollingStation.GetFullAddress()
                            : MUI.FilterForVoters_PollingStation_MissingAddress)
                    : string.Empty);
        }

        [Authorize(Roles = Transactions.CheckRSP)]
        [HttpGet]
        public ActionResult RspChecking()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Transactions.CheckRSP)]
        public ActionResult RspChecking(string idnp)
        {
            string Succes = "False";
            if (String.IsNullOrEmpty(idnp))
            {
                return Json(new { Error = string.Format(MUI.Voters_PersonIdnpError, idnp) });
            }

            try
            {
                _rspUserName = _settingsBll.GetValue(RspUserParamName);
                _rspUserPwd = _settingsBll.GetValue(RspUserPwdParamName);
                using (var client = new CecClient())
                {
                    client.ClientCredentials.UserName.UserName = _rspUserName;
                    client.ClientCredentials.UserName.Password = _rspUserPwd;

                    var longIdnp = Int64.Parse(idnp);
                    var physicalPersonData = client.getPhysicalPersonData(longIdnp);

                    if (physicalPersonData != null)
                    {
                        Succes = "true";
                        if (physicalPersonData.result.resultCode.GetValueOrDefault() != 0)
                        {
                            return Json(new { Error = string.Format("RSP_errorCode: {0}, message: {1}", physicalPersonData.result.resultCode, physicalPersonData.result.errorText) });
                        }

                        var rspInfoModel = Mapper.Map<PhysicalPersonRequestData, RspInfoModel>(physicalPersonData);
                        if (physicalPersonData.person.identDocument.docTypeCode != null)
                        {
                            var docType = _bll.Get<DocTypeCode>((long)physicalPersonData.person.identDocument.docTypeCode);
                            rspInfoModel.DocType = docType != null ? docType.Name : string.Empty;
                        }

                        if (physicalPersonData.person.sexCode != null)
                        {
                            var sex = _bll.Get<SexCode>((long)physicalPersonData.person.sexCode);
                            rspInfoModel.Sex = sex != null ? sex.Name : string.Empty;
                        }

                        foreach (var registration in rspInfoModel.Registration)
                        {
                            if (registration.RegTypeCode != null)
                            {
                                var regType = _bll.Get<RegTypeCode>((long)registration.RegTypeCode);
                                registration.RegType = regType != null ? regType.Name : string.Empty;
                            }

                            if (registration.StreetCode != null)
                            {
                                var streetAddress = _bll.Get<StreetTypeCode>((long)registration.StreetCode);
                                registration.StreetAddress = streetAddress != null ? streetAddress.Docprint : string.Empty;
                            }
                        }

                        try
                        {
                            LoggerUtils logEvent = new LoggerUtils();
                            // Log To admin
                            logEvent.LogEvent(LogLevel.Information, Events.RspCheck.Value, Events.RspCheck.Description, new Dictionary<string, string>
                            {
                                { Events.RspCheck.Attributes.Person, idnp},
                                { Events.RspCheck.Attributes.Status, Succes},
                            });
                            //Log to MLog
                            logEvent.MLogEvent(LogLevel.Information, Events.RspCheck.Value, Events.RspCheck.Description, new Dictionary<string, string>
                            {
                                { Events.RspCheck.Attributes.Person, idnp},
                                { Events.RspCheck.Attributes.Status, Succes},
                            });
                        }
                        catch
                        { // 
                        }

                        return Json(new { Data = rspInfoModel });
                    }

                    return Json(new { Error = MUI.Voters_IdnpNotFound });
                }
            }
            catch (Exception e)
            {
                return Json(new { Error = e.Message });
            }
        }

        [HttpPost]

        public async Task<ActionResult> DeleteElectionNumberList()
        {
            try
            {
                LoggerUtils logEvent = new LoggerUtils();
                // Log To admin
                logEvent.LogEvent(LogLevel.Information, Events.DeleteNumberList.Value, Events.DeleteNumberList.Description, new Dictionary<string, string>
                            {
                                { Events.DeleteNumberList.Attributes.Action, "Delete"},
                            });
            }
            catch
            {
            }
            await _bll.DeleteElectionNumberList();

            return Content("Sters cu succes");
        }
    }
}