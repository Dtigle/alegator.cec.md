using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto.Concurents;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.PoliticalParty;
using CEC.SRV.BLL.Dto;

namespace CEC.SAISE.EDayModule.Controllers
{
   [PermissionRequired(SaisePermissions.PoliticalPartyView)]
    public class PoliticalPartyController : BaseDataController
    {
        private readonly ILogoImageBll _logoImageBll;
        private readonly IUserBll _userBll;
        private readonly IConcurentsBll _concurentsBll;

        public PoliticalPartyController(IUserBll userBll, IConcurentsBll concurentsBll, ILogoImageBll logoImageBll)
        {
           _logoImageBll = logoImageBll;
            _userBll = userBll;
            _concurentsBll = concurentsBll;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetElection(long electionId)
        {
            var election = _concurentsBll.GetElection(electionId);

            return Json(
                    new
                    {
                        ElectionId = election.Id,
                        IsLocal = election.IsSubTypeOfLocalElection(),
                        IsMayorElection = election.IsSubTypeOfMayorElection()
                    });
        }

        [HttpPost]
        public ActionResult GetAllParties(DelimitationModel delimitation)
        {
            var parties = _concurentsBll.GetAllParties(Mapper.Map<DelimitationDto>(delimitation));
            var result = Mapper.Map<IList<PoliticalPartyDto>, IList<PoliticalPartyModel>>(parties);

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetAllocatedParties(DelimitationModel delimitation)
        {
            var parties = _concurentsBll.GetAllocatedParties(Mapper.Map<DelimitationDto>(delimitation));
            var result = Mapper.Map<IList<PoliticalPartyDto>, IList<PoliticalPartyModel>>(parties);

            return Json(result);
        }

        //[OutputCache(Duration = 600, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "partyId")]
        public ActionResult ImageLoad(long partyId)
        {
            _logoImageBll.NoLogoPath = Server.MapPath("~/Content/img/NoLogo.png");
            byte[] buffer = _logoImageBll.Get(partyId);
            return File(buffer, "image/jpeg");
            return (null);
        }

        [HttpPost]
		public ActionResult UpdatePartyStatus(long partyId, PoliticalPartyStatus status)
        {
            var result = _concurentsBll.UpdatePartyStatus(partyId, status);
            return Json(result);
        }

		[HttpPost]
		public ActionResult OverridePartyStatus(DelimitationModel delimitation, long partyId, PoliticalPartyStatus status)
		{
			var delimitationDto = Mapper.Map<DelimitationDto>(delimitation);

			var result = _concurentsBll.OverridePartyStatus(delimitationDto, partyId, status);

			return Json(result);
		}


        [HttpPost]
        public ActionResult UpdateCandidateStatus(long candidateId, CandidateStatus status)
        {
            var result = _concurentsBll.UpdateCandidateStatus(candidateId, status);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetCandidatesForParty(DelimitationModel delimitation, long partyId)
        {
            var result = Mapper.Map<IList<CandidateDto>, IList<CandidateModel>>(
                _concurentsBll.GetCandidatesForParty(Mapper.Map<DelimitationDto>(delimitation), partyId));
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteCandidates(List<DeleteCandidateModel> itemsToDelete)
        {
            var result = _concurentsBll.DeleteCandidates(Mapper.Map<List<DeleteCandidateDto>>(itemsToDelete));
            return Json(result);
        }

        [HttpPost]
        public ActionResult UpdateCandidatesOrder(List<UpdateCandidateOrderModel> itemsToUpdate)
        {
            var result = _concurentsBll.UpdateCandidatesOrder(Mapper.Map<List<UpdateCandidateOrderDto>>(itemsToUpdate));
            return Json(result);
        }

        public ActionResult CreateUpdateParty()
        {
            //todo: not implemented
            return Json("");
        }

        [HttpPost]
        public async Task<ActionResult> GetPersonalData(string idnp)
        {
            var result = await _concurentsBll.RequestPersonalData(idnp);

            return Json(result);
        }

        [HttpPost]
        public ActionResult SaveUpdateParty(DelimitationModel delimitation, PoliticalPartyModel partyData, HttpPostedFileBase fileData)
        {
            var logoData = GetUploadedFile(fileData);
            _concurentsBll.SaveUpdateParty(
                Mapper.Map<DelimitationDto>(delimitation),
                Mapper.Map<PoliticalPartyDto>(partyData), logoData);

            return Json("success");
        }

        [HttpPost]
        public ActionResult SaveUpdateCandidate(DelimitationModel delimitation, CandidateModel candidateData, bool IsResultConfirmed)
        {
			var delimitationDto = Mapper.Map<DelimitationDto>(delimitation);
	        var candidateToUpdate = Mapper.Map<CandidateDto>(candidateData);

			var candidatesConflicted = _concurentsBll.CheckPersonAllocation(delimitationDto, candidateToUpdate);
			if (!IsResultConfirmed && candidatesConflicted != null && candidatesConflicted.Count > 0)
				return Json(candidatesConflicted.Select(x => new 
				{
					Election = x.ElectionRound.Election.GetFullName(),
					Party = x.PoliticalParty.Code,
                    AssignedCircumscription = x.AssignedCircumscription.GetFullName(),
					Region=""
				}));
			
	        _concurentsBll.SaveUpdateCandidate(
                delimitationDto,
                candidateToUpdate);

            return Json("success");
        }

		[HttpPost]
		public ActionResult RemoveConcurents(DelimitationDto delimitation, IEnumerable<long> itemsToDelete)
	    {
			var delimitationDto = Mapper.Map<DelimitationDto>(delimitation);
			var result = _concurentsBll.DeleteConcurents(delimitationDto, itemsToDelete);
			return Json("success");
	    }

        [HttpPost]
        public ActionResult FireAllocation(DelimitationModel delimitation, List<AllocationItemModel> itemsToAllocate)
        {
            _concurentsBll.FireAllocation(
                Mapper.Map<DelimitationDto>(delimitation),
                Mapper.Map<IList<AllocationItemModel>, IList<AllocationItemDto>>(itemsToAllocate));

            return Json("success");
        }


	    public ActionResult GetExcelPreview(DelimitationModel delimitation)
	    {
		    Stream stream;
			string fileName;
            const string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

			_concurentsBll.ExportCandidatesToExcel(out stream, out fileName, Mapper.Map<DelimitationDto>(delimitation));
		    
		    return File(stream, mimeType, fileName);
	    }

        private byte[] GetUploadedFile(HttpPostedFileBase fu)
        {
            if (fu == null || fu.InputStream.Length <= 0)
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                fu.InputStream.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }
}