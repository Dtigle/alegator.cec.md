using Amdaris;
using Amdaris.NHibernateProvider.Web;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.VoterCertificate;
using Lib.Web.Mvc.JQuery.JqGrid;
using NHibernate;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{

    [NHibernateSession]
    [PermissionRequired(SaisePermissions.VoterCertificateCreate)]
    public class VoterCertificatController : BaseController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IUserBll _userBll;
        private readonly IVotingBll _votingBll;
        private readonly ILogger _logger;
        private readonly ISaiseRepository _saiseRepository;
        private readonly IVoterCertificatBll _voterCertificatBll;

        public VoterCertificatController(IConfigurationBll configurationBll, IUserBll userBll, ILogger logger, ISessionFactory sessionFactory, ISaiseRepository saiseRepository, IVoterCertificatBll voterCertificatBll, IVotingBll votinBll)
        {
            _configurationBll = configurationBll;
            _userBll = userBll;
            _logger = logger;
            _saiseRepository = saiseRepository;
            _voterCertificatBll = voterCertificatBll;
            _votingBll = votinBll;
        }

        // GET: VoterCertificat
        public async Task<ActionResult> Index()
        {
           

            if (User.IsInRole("Administrator"))
            {
                return View("NotImplementedForAdmin");
            }
            var userData = await _userBll.GetCurrentUserData();

            if(userData.CircumscriptionAcces)
            {
                return View("_NotImplementForUC");
            }

            return View();
        }


        public JqGridJsonResult GetAllCertificats(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<VoterCertificateGridModel>();

            var userIsAdmin = User.IsInRole("Administrator");

            var certificats = _voterCertificatBll.GetVoterCertificat(pageRequest);

            return certificats.ToJqGridJsonResult<VoterCertificatDto, VoterCertificateGridModel>();

        }


        public JqGridJsonResult SearchVoterAsync(JqGridRequest request, string idnp)
        {
            var pageRequest = request.ToPageRequest<VoterGridViewModel>();

            if (!string.IsNullOrWhiteSpace(idnp))
            {
                var searchResult = _voterCertificatBll.GetVoter(pageRequest, idnp);
                return searchResult.ToJqGridJsonResult<AssignedVoter, VoterGridViewModel>();
            }

            return null;
        }

        [HttpGet]
        public  ActionResult CreateCertifcat(string idnp)
        {
            var Election = _voterCertificatBll.GetDateOfElection();

            var trustPersone = _votingBll.SearchVoterAsync(idnp,"");

            var person = _voterCertificatBll.GetForCreateCertificat(trustPersone.Result.VoterData.Assignement.AssignedVoterId);



            string adress = person.AssignedVoter.Voter.GetAddress();



            if (trustPersone.Result.Status == VoterSearchStatus.Success && trustPersone.Result.VoterData.Assignement.IsSamePollingStation == true)
            {


                var result = new CertificatModel
                {
                    VoterSearchStatus = 1,
                    FullName = trustPersone.Result.VoterData.FirstName + " " + trustPersone.Result.VoterData.LastName,
                    Adres = person.AssignedVoter.Voter.Region.RegionType.Name+ person.AssignedVoter.Voter.Region.Name+ " " + adress,
                    BirthDate = person.AssignedVoter.Voter.DateOfBirth,
                    ElectionDate = Election.Result.ElectionDayDate.DateTime,
                    IDNP = trustPersone.Result.VoterData.Idnp,
                    DocumentNumber = trustPersone.Result.VoterData.DocumentNumber,
                    TypeElection =Election.Result.Name,
                    DocumentData = person.AssignedVoter.Voter.DateOfIssue,
                    DocumentExpireData = person.AssignedVoter.Voter.DateOfExpiry,
                    Circumscription = trustPersone.Result.VoterData.Assignement.Circumscription.Name,
                    ElectionOffice = trustPersone.Result.VoterData.Assignement.PollingStation.Name,
                    AssignedVoterId = trustPersone.Result.VoterData.Assignement.AssignedVoterId,
                    PolingStationId = trustPersone.Result.VoterData.Assignement.PollingStation.Id,
                    VoterId = trustPersone.Result.VoterData.VoterId
                    

                };
                return PartialView(result);
            }
            else if (trustPersone.Result.Status == VoterSearchStatus.Success && trustPersone.Result.VoterData.Assignement.IsSamePollingStation == false)
            {
                var result = new CertificatModel
                {
                    VoterSearchStatus = 2,

                    FullName = trustPersone.Result.VoterData.FirstName + " " + trustPersone.Result.VoterData.LastName,
                    TypeElection = "",
                    Circumscription = trustPersone.Result.VoterData.Assignement.Region.Name,
                    ElectionOffice = trustPersone.Result.VoterData.Assignement.PollingStation.Name
                };
                return PartialView(result);
            }
            else
            {
                var result = new CertificatModel
                {
                    VoterSearchStatus = 3
                };
                return PartialView(result);
            }

        }


        public ActionResult SelectVoterStatus()
        {
            var voterStatus = _saiseRepository.Query<PollingStation>();

            return PartialView("_Select", voterStatus.ToSelectListUnencrypted(0, false, null, x => x.FullName, x => x.Id));
        }


        [HttpPost]
        public ActionResult CreateCertifcat(CertificatModel model)
        {
            if (ModelState.IsValid)
            {

                var result = _voterCertificatBll.SaveCertificatAsync(Convert.ToInt64(model.Id),Convert.ToInt64(model.AssignedVoterId), model.CertificatNr, Convert.ToInt64(model.PolingStationId), model.ReleaseDate);

                if (result.Result)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            return PartialView(model);
        }



        [HttpGet]
        public ActionResult ViewCertificat(long id)
        {
            if (id != null)
            {
                var Certificat = _voterCertificatBll.GetCertificat(id);

                var CertifactModel = Mapper.Map<CertificatModel>(Certificat);
                return PartialView(CertifactModel);
            }

            return Content("Sa produs o eroare");
        }

        [HttpPost]
        public ActionResult ViewCertificat(CertificatModel model)
        {

            if (ModelState.IsValid)
            {
                var result = _voterCertificatBll.SaveCertificatAsync(Convert.ToInt64(model.Id), Convert.ToInt64(model.AssignedVoterId), model.CertificatNr, Convert.ToInt64(model.PolingStationId), model.ReleaseDate);

                if (result.Result)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            return PartialView(model);

        }

        public JsonResult DeleteCertificat(long id)
        {

            if (id != null)
            {
                var deleteCertificat = _voterCertificatBll.DeleteCerificat(id);
                return Json(deleteCertificat.Result, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);



        }

    }
}