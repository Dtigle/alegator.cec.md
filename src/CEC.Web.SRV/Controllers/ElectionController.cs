using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Election;
using CEC.Web.SRV.Models.Lookup;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Controllers
{
    public class ElectionController : BaseController
    {
        private readonly IElectionBll _bll;
        private readonly ILookupBll _lookupBll;

        public ElectionController(IElectionBll bll, ILookupBll lookupBll)
        {
            _bll = bll;
            _lookupBll = lookupBll;
        }

		// GET: Election
		public ActionResult Index()
        {
            return View();
        }

        public JqGridJsonResult ListElectionsAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ElectionGridModel>();

            var data = _bll.Get<Election>(pageRequest);

            return data.ToJqGridJsonResult<Election, ElectionGridModel>();
        }

        [HttpPost]
        public ActionResult ExportElections(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "Elections", typeof (ElectionGridModel), ListElectionsAjax);
        }

        [HttpGet]
		public ActionResult CreateUpdate(long? electionId)
        {
	        if (electionId.HasValue)
	        {
				_bll.VerificationIsDeletedSrv<Election>(electionId.Value);
	        }
            var model = new UpdateElectionModel();

            if (electionId != null)
            {
                var election = _bll.Get<Election>((long)electionId);
                model = Mapper.Map<Election, UpdateElectionModel>(election);
            }

            ViewData["ElectionType"] = _lookupBll.GetAll<ElectionType>().Where(x => x.Deleted == null)
                .ToSelectListUnencrypted(model.ElectionType, false, null, x => x.Name, x => x.Id);

            return PartialView("_UpdateElectionPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUpdate(UpdateElectionModel model)
        {
            if (ModelState.IsValid)
            {
                //_bll.SaveOrUpdate(model.Id, model.ElectionType, 
                //    model.ElectionDate.Value, model.SaiseId, model.Comments, (bool)model.AcceptAbroadDeclaration);
                return Content(Const.CloseWindowContent);
            }

            ViewData["ElectionType"] = _lookupBll.GetAll<ElectionType>().Where(x => x.Deleted == null)
                .ToSelectListUnencrypted(model.ElectionType, false, null, x => x.Name, x => x.Id);

            return PartialView("_UpdateElectionPartial", model);
        }

        [HttpPost]
        public void Delete(long id)
        {
			_bll.VerificationIfElectionHasReference(id);
            _bll.Delete<Election>(id);
        }

        [HttpPost]
        public void UnDelete(long id)
        {
            _bll.UnDelete<Election>(id);
        }
    }
}