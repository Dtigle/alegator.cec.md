using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.PollingStation;
using CEC.Web.SRV.Models.Statistics;
using DocumentFormat.OpenXml.InkML;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.Owin.Mapping;
using NHibernate.Mapping.ByCode;

namespace CEC.Web.SRV.Controllers
{
    public class StatisticsController : BaseController
    {

        private readonly IStatisticsBll _bll;

        public StatisticsController(IStatisticsBll bll)
        {
            _bll = bll;
        }

        [HttpGet]
        public ActionResult LoadInfoTab()
        {
            return PartialView("_InfoTab");
        }

        [HttpGet]
        public ActionResult LoadAgeDistributionTab()
        {
            return PartialView("_AgeDistributionTab");
        }

        [HttpGet]
        public ActionResult LoadProblematicDataTab()
        {
            return PartialView("_ProblematicDataTab");
        }

        [HttpGet]
        public ActionResult LoadStatisticsFromImportTab()
        {
            return PartialView("_StatisticsFromImportTab");
        }

        [HttpGet]
        public ActionResult LoadStatisticsForPollingStation()
        {
            return PartialView("_StatisticsForPollingStationTab");
        }

        public JqGridJsonResult ListStatisticsPollingStationsAjax(JqGridRequest request, long? regionId, long? pollingStationId)
        {
            var pageRequest = request.ToPageRequest<StatisticsPollingStationGridModel>();

            var data = _bll.GetStatisticsForPollingStation(pageRequest, regionId, pollingStationId);

            return data.ToJqGridJsonResult<PollingStationStatistics, StatisticsPollingStationGridModel>();
        }

        public ActionResult ExportStatisticsPollingStations(JqGridRequest request, ExportType exportType, long? regionId, long? pollingStationId)
        {
            return ExportGridData(request, exportType, regionId, pollingStationId, "VotersPerPS", typeof(StatisticsPollingStationGridModel), ListStatisticsPollingStationsAjax);
        }

        [HttpPost]
        public ActionResult GetTotalPeople(long? regionId, long? pollingStationId)
        {
            var peopleNumber = _bll.GetTotalNumberOfPeople(regionId, pollingStationId);
            return Json(peopleNumber);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfPeopleWithoutDeads(long? regionId, long? pollingStationId)
        {
            var peopleNumber = _bll.GetTotalNumberOfPeopleWithoutDeads(regionId, pollingStationId);
            return Json(peopleNumber);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfDeads(long? regionId, long? pollingStationId)
        {
            var peopleNumber = _bll.GetTotalNumberOfDeads(regionId, pollingStationId);
            return Json(peopleNumber);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfVoters(long? regionId, long? pollingStationId)
        {
            var votersNumber = _bll.GetTotalNumberOfVoters(regionId, pollingStationId);
            return Json(votersNumber);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfStayStatementDeclarations(long? regionId, long? pollingStationId)
        {
            var decllarationsNumber = _bll.GetTotalNumberOfStayStatementDeclarations(regionId, pollingStationId);
            return Json(decllarationsNumber);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfMilitary(long? regionId, long? pollingStationId)
        {
            var number = _bll.GetTotalNumberOfMilitary(regionId, pollingStationId);
            return Json(number);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfDetainee(long? regionId, long? pollingStationId)
        {
            var number = _bll.GetTotalNumberOfDetainee(regionId, pollingStationId);
            return Json(number);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfStatementAbroad(long? regionId, long? pollingStationId)
        {
            var number = _bll.GetTotalNumberOfStatementAbroad(regionId, pollingStationId);
            return Json(number);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfPeopleByGender(GenderTypes gender, long? regionId, long? pollingStationId)
        {
            var peopleByGender = _bll.GetTotalNumberOfPeopleByGender(gender, regionId, pollingStationId);
            return Json(peopleByGender);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfPeopleWithDoBMissing(long? regionId, long? pollingStationId)
        {
            var peopleWithDoBMissing = _bll.GetTotalNumberOfPeopleWithDoBMissing(regionId, pollingStationId);
            return Json(peopleWithDoBMissing);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfPeopleWithAddressMissing(long? regionId, long? pollingStationId)
        {
            var peopleWithAddressMissing = _bll.GetTotalNumberOfPeopleWithAddressMissing(regionId, pollingStationId);
            return Json(peopleWithAddressMissing);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfPeopleWithDocMissing(long? regionId, long? pollingStationId)
        {
            var peopleWithDoBMissing = _bll.GetTotalNumberOfPeopleWithDocMissing(regionId, pollingStationId);
            return Json(peopleWithDoBMissing);
        }

        [HttpPost]
        public ActionResult GetTotalNumberOfPeopleWithDocExpired(long? regionId, long? pollingStationId)
        {
            var peopleWithDoBMissing = _bll.GetTotalNumberOfPeopleWithDocExpired(regionId, pollingStationId);
            return Json(peopleWithDoBMissing);
        }

        [HttpPost]
        public ActionResult GetNumberOfPeopleForAgeIntervals(long? regionId, long? pollingStationId)
        {
            var result =
                Enum.GetValues(typeof(AgeIntervals))
                    .Cast<AgeIntervals>()
                    .Select(
                        x => new { value = _bll.GetNumberOfPeopleForAgeIntervals((int)x, regionId, pollingStationId), text = x.GetEnumDescription() }).ToList();

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetNewVoters(long? regionId, long? pollingStationId)
        {
            var newVoters = _bll.GetNewVoters();
            return Json(newVoters);
        }

        [HttpPost]
        public ActionResult GetNewDeadPeople(long? regionId, long? pollingStationId)
        {
            var newVoters = _bll.GetNewDeadPeople();
            return Json(newVoters);
        }

        [HttpPost]
        public ActionResult GetPersonalDataChanges()
        {
            var newVoters = _bll.GetPersonalDataChanges();
            return Json(newVoters);
        }

        [HttpPost]
        public ActionResult GetAddressesChanges()
        {
            var newVoters = _bll.GetAddressesChanges();
            return Json(newVoters);
        }

        [HttpPost]
        public ActionResult GetImportSuccessful()
        {
            var newVoters = _bll.GetImportSuccessful();
            return Json(newVoters);
        }

        [HttpPost]
        public ActionResult GetImportFailed()
        {
            var newVoters = _bll.GetImportFailed();
            return Json(newVoters);
        }

        public JqGridJsonResult ListProblematicDataPollingStationAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<ProblematicDataPollingStationGridModel>();

            var data = _bll.GetStatisticsForProblematicDataPollingStation(pageRequest);

            return data.ToJqGridJsonResult<ProblematicDataPollingStationStatistics, ProblematicDataPollingStationGridModel>();
        }

        public JqGridJsonResult ImporterListAjax(JqGridRequest request, long? regionId)
        {
            var pageRequest = request.ToPageRequest<ImportsGridModel>();

            var data = _bll.GetImportStatistics(pageRequest, regionId.GetValueOrDefault());

            return data.ToJqGridJsonResult<ImportStatisticsGridDto, ImportsGridModel>();
        }

        public ActionResult GetImportStatistics(string importDataTime, long regionId)
        {
            if (importDataTime != null)
            {
                var dateTime = DateTime.Parse(importDataTime);
                var data = _bll.GetImportStatistics(dateTime, regionId);
                var model = Mapper.Map<ImportStatisticsDto, ImportStatisticsModel>(data);
                return PartialView("_ViewImportStatistics", model);
            }
            return null;
        }

    }

}