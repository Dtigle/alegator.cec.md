using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.QuartzAdmin;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Controllers
{
	[Authorize(Roles = Transactions.QuartzAdmin)]
	public class QuartzAdminController : BaseController
    {
        private readonly IJobsScheduler _jobsScheduler;

        public QuartzAdminController(IJobsScheduler jobsScheduler)
        {
            _jobsScheduler = jobsScheduler;
        }

        public ActionResult Index()
        {
            var model = new QuartzAdminModel();
            try
            {
                _jobsScheduler.Connect();
                model.IsConnected = _jobsScheduler.IsRunning;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult ListQuartzJobs(JqGridRequest request)
        {
            var data = _jobsScheduler.GetJobs().OrderBy(x => x.GroupName).ThenBy(x => x.JobName).ToList();
            var pageResponse = PageRequestExtensions.GetDataPerPage(data, request);

            return pageResponse.ToJqGridJsonResult<QuartzJobData, QuartzScheduledJobsGridModel>();
        }

        [HttpPost]
        public ActionResult ListRunningJobs(JqGridRequest request)
        {
            var data = _jobsScheduler.GetRunningJobs().OrderBy(x => x.GroupName).ThenBy(x => x.JobName).ToList();
            var pageResponse = PageRequestExtensions.GetDataPerPage(data, request);
            
            return pageResponse.ToJqGridJsonResult<QuartzJobData, QuartzRunningJobsGridModel>();
        }

        [HttpPost]
        public ActionResult GetQuartzJobs()
        {
            var data = _jobsScheduler.GetJobs().OrderBy(x => x.GroupName).ThenBy(x => x.JobName).ToList();
            var modelData = data.Select(Mapper.Map<QuartzJobData, QuartzScheduledJobsGridModel>).ToArray();

            return Json(modelData);
        }

        [HttpPost]
        public ActionResult GetRunningJobs()
        {
            var data = _jobsScheduler.GetRunningJobs().OrderBy(x => x.GroupName).ThenBy(x => x.JobName).ToList();
            var modelData = data.Select(Mapper.Map<QuartzJobData, QuartzRunningJobsGridModel>).ToList();

            //for testing
            var progressInfo = new ProgressInfoModel
            {
                Minimum = 0,
                Maximum = 86400,
                Value = Convert.ToInt32((DateTime.Now - DateTime.Today).TotalSeconds),
            };
            progressInfo.Ratio = (double)progressInfo.Value/progressInfo.Maximum*100.0;

            var progress = new JobProgressModel
            {
                OverallProgress = progressInfo,
            };
            progress.StageInfos.Add(new ProgressInfoModel{ Id = Guid.Empty.ToString(), Minimum = 0, Maximum = 86400, Value = progressInfo.Value, Ratio = progressInfo.Ratio, Comments = "Test"});

            var testRunningJob = new QuartzRunningJobsGridModel
            {
                Id = "GX_JX",
                GroupName = "GX",
                JobDescription = "Test",
                JobName = "JX",
                RunTime = (DateTime.Now - DateTime.Today).ToString(),
                TriggerGroup = "TgGX",
                TriggerName = "TgN",
                Progress = progress
            };
            modelData.Add(testRunningJob);

            return Json(modelData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformAction(QuartzJobActionModel model)
        {
            SetViewData(model.SelectedAction);
            if (model.SelectedAction != 0)
            {
                if ((JobAction)model.SelectedAction == JobAction.ReSchedule && string.IsNullOrWhiteSpace(model.JobDetails.CronExpression))
                {
                    ModelState.AddModelError("JobDetails.CronExpression", MUI.QuartzAdmin_ErrorRequired_CronExpresion);

                    return PartialView("_JobActions", model);
                }
                //if (string.IsNullOrWhiteSpace(model.ReasonComment))
                //{
                //    ModelState.AddModelError("ReasonComment", "Please specify reason.");

                //    return PartialView("_JobActions", model);
                //}

                try
                {
                    _jobsScheduler.ExecuteAction((JobAction) model.SelectedAction,
                        model.JobDetails.GroupName,
                        model.JobDetails.JobName,
                        model.JobDetails.TriggerName,
                        model.JobDetails.TriggerGroup,
                        model.JobDetails.CronExpression);
                }
                catch (SrvException exp)
                {
                    ModelState.AddModelError("JobDetails.CronExpression", exp.GetLocalizedMessage());

                    return PartialView("_JobActions", model);
                }
            }
            else
            {
                ModelState.AddModelError("SelectedAction", "Please select an action.");
                return PartialView("_JobActions", model);
            }

            return Content(Const.CloseWindowContent);
        }

        [HttpPost]
        public ActionResult JobActions(QuartzScheduledJobsGridModel jobModel)
        {
            SetViewData(0);
            var actionModel = PopulateModel(jobModel);

            return PartialView("_JobActions", actionModel);
        }

        [HttpPost]
        public ActionResult InterruptRunningJob(QuartzRunningJobsGridModel jobModel)
        {
            _jobsScheduler.ExecuteAction(JobAction.Interrupt,
                        jobModel.GroupName,
                        jobModel.JobName,
                        jobModel.TriggerName,
                        jobModel.TriggerGroup,
                        null);

            return null;
        }

        private QuartzJobActionModel PopulateModel(QuartzScheduledJobsGridModel jobModel)
        {
            var actionModel = new QuartzJobActionModel
            {
                JobDetails = jobModel,
            };

            return actionModel;
        }

        private void SetViewData(int selectedAction)
        {
            var actionList = Enum.GetValues(typeof (JobAction))
                .Cast<JobAction>()
                .ToSelectListUnencrypted(selectedAction, false, null, x => x.ToString(), x => ((int) x).ToString())
                .ToList();
            ViewData["SelectedAction"] = actionList;
        }
    }
}