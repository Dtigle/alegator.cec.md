using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using AutoMapper;
using Moq;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.SRV.BLL.Quartz;
using CEC.Web.SRV.Models.QuartzAdmin;
using CEC.SRV.BLL.Exceptions;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class QuartzAdminControllerTests : BaseControllerTests
    {
        private readonly Mock<IJobsScheduler> _js;
        private readonly QuartzAdminController _controller;

        public QuartzAdminControllerTests()
        {
            _js = new Mock<IJobsScheduler>();
            _controller = new QuartzAdminController(_js.Object);
            BaseController = _controller;

            Mapper.Initialize(arg => arg.AddProfile<QuartzDataProfile>());
        }

        [TestMethod]
        public void Index_returns_correct_view()
        {
            // Arrange

            _js.Setup(x => x.Connect());
            _js.SetupGet(x => x.IsRunning).Returns(true);

            // Act

            var result = _controller.Index();

            // Assert

            Assert.IsNotNull(result);
            _js.Verify(x => x.Connect(), Times.Once);
            _js.VerifyGet(x => x.IsRunning, Times.Once);
        }

        [TestMethod]
        public void Index_returns_correct_model_when_missing_errors()
        {
            // Arrange

            const bool isRunning = true;

            _js.Setup(x => x.Connect());
            _js.SetupGet(x => x.IsRunning).Returns(isRunning);

            // Act

            var result = _controller.Index() as ViewResult;

            // Assert

            Assert.IsNotNull(result);

            _js.Verify(x => x.Connect(), Times.Once);
            _js.VerifyGet(x => x.IsRunning, Times.Once);

            var model = (QuartzAdminModel)result.Model;
            Assert.AreEqual(isRunning, model.IsConnected);
            Assert.IsNull(model.ErrorMessage);
        }

        [TestMethod]
        public void Index_returns_correct_model_when_there_are_errors()
        {
            // Arrange

            const string message = "error";
            const bool isRunning = true;

            _js.Setup(x => x.Connect()).Throws(new Exception(message));
            _js.SetupGet(x => x.IsRunning).Returns(isRunning);

            // Act

            var result = _controller.Index() as ViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = (QuartzAdminModel)result.Model;
            Assert.IsFalse(model.IsConnected);
            Assert.AreEqual(message, model.ErrorMessage);
        }

        [TestMethod]
        public void ListQuartzJobs_returns_correct_result()
        {
            // Arrange

            var request = new JqGridRequest
            {
                PageIndex = 1,
                PagesCount = 1,
                RecordsCount = 1
            };

            var expData = GetQuartzJobDatas();
            var expRecords = expData.Skip(request.PageIndex*request.RecordsCount).Take(request.RecordsCount).ToList();

            _js.Setup(x => x.GetJobs()).Returns(expData);

            // Act

            var result = _controller.ListQuartzJobs(request) as JqGridJsonResult;

            // Assert

            Assert.IsNotNull(result);

            var data = result.Data as JqGridResponse;
            Assert.IsNotNull(data);

            var records = data.Records; 
            Assert.IsNotNull(records);
            Assert.AreEqual(expRecords.Count(), records.Count);
            expRecords.ForEach(x =>
            {
                var record = records.FirstOrDefault(y => y.Id == x.JobName + "_" + x.GroupName);
                Assert.IsNotNull(record);

                var model = record.Value as QuartzScheduledJobsGridModel;

                Assert.IsNotNull(model);
                Assert.AreEqual(x.CronExpression, model.CronExpression);
                Assert.AreEqual(x.HasCronTrigger, model.HasCronTrigger);
                Assert.AreEqual(x.NextFireTime, model.NextFireTime);
                Assert.AreEqual(x.PreviousFireTime, model.PreviousFireTime);
                Assert.AreEqual(x.TriggerState.ToString(), model.TriggerState);
                Assert.AreEqual(x.JobName, model.JobName);
                Assert.AreEqual(x.GroupName, model.GroupName);
                Assert.AreEqual(x.JobDescription, model.JobDescription);

                Assert.IsTrue(record.Values.Contains(x.JobName));
                Assert.IsTrue(record.Values.Contains(x.JobDescription));
                Assert.IsTrue(record.Values.Contains(x.GroupName));
            });




        }

        [TestMethod]
        public void ListRunningJobs_returns_correct_result()
        {
            // Arrange

            var request = new JqGridRequest
            {
                PageIndex = 1,
                PagesCount = 1,
                RecordsCount = 1
            };

            var expData = GetQuartzJobDatas();
            var expRecords = expData.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount).ToList();

            _js.Setup(x => x.GetRunningJobs()).Returns(expData);

            // Act

            var result = _controller.ListRunningJobs(request) as JqGridJsonResult;

            // Assert

            Assert.IsNotNull(result);

            var data = result.Data as JqGridResponse;
            Assert.IsNotNull(data);

            var records = data.Records;
            Assert.IsNotNull(records);
            Assert.AreEqual(expRecords.Count(), records.Count);
            expRecords.ForEach(x =>
            {
                var record = records.FirstOrDefault(y => y.Id == x.JobName + "_" + x.GroupName);
                Assert.IsNotNull(record);

                var model = record.Value as QuartzRunningJobsGridModel;

                Assert.IsNotNull(model);
                Assert.AreEqual(x.GetRunTime().ToString(), model.RunTime);
                Assert.AreEqual(x.IsInterruptable, model.IsInterruptable);
                Assert.AreEqual(x.JobName, model.JobName);
                Assert.AreEqual(x.GroupName, model.GroupName);
                Assert.AreEqual(x.JobDescription, model.JobDescription);

                Assert.IsTrue(record.Values.Contains(x.JobName));
                Assert.IsTrue(record.Values.Contains(x.JobDescription));
                Assert.IsTrue(record.Values.Contains(x.GroupName));
            });




        }

        [TestMethod]
        public void GetRunningJobs_returns_correct_result()
        {
            var expData = GetQuartzJobDatas();

            _js.Setup(x => x.GetRunningJobs()).Returns(expData);

            // Act

            var result = _controller.GetRunningJobs() as JsonResult;

            // Assert

            Assert.IsNotNull(result);

            var data = result.Data as IEnumerable<QuartzRunningJobsGridModel>;
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void InterruptRunningJob_has_correct_logic()
        {
            // Arrange

            var jobModel = new QuartzRunningJobsGridModel
            {
                GroupName = "group_name",
                JobName = "job_name",
                TriggerGroup = "trigger_group",
                TriggerName = "trigger_name"
            };

            _js.Setup(x => x.ExecuteAction(JobAction.Interrupt,
                jobModel.GroupName,
                jobModel.JobName,
                jobModel.TriggerName,
                jobModel.TriggerGroup,
                null));

            // Act

            var result = _controller.InterruptRunningJob(jobModel);

            // Assert

            _js.Verify(x => x.ExecuteAction(JobAction.Interrupt,
                jobModel.GroupName,
                jobModel.JobName,
                jobModel.TriggerName,
                jobModel.TriggerGroup,
                null), Times.Once);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void JobActions_has_correct_model()
        {
            // Arrange

            var jobModel = new QuartzScheduledJobsGridModel
            {
                GroupName = "group_name",
                JobName = "job_name",
                TriggerGroup = "trigger_group",
                TriggerName = "trigger_name"
            };

            // Act

            var result = _controller.JobActions(jobModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as QuartzJobActionModel;

            Assert.IsNotNull(model);
            Assert.AreSame(jobModel, model.JobDetails);
        }

        [TestMethod]
        public void JobActions_has_correct_view_data()
        {
            // Arrange

            var jobModel = new QuartzScheduledJobsGridModel
            {
                GroupName = "group_name",
                JobName = "job_name",
                TriggerGroup = "trigger_group",
                TriggerName = "trigger_name"
            };

            // Act

            var result = _controller.JobActions(jobModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var viewData = result.ViewData["SelectedAction"] as List<SelectListItem>;

            Assert.IsNotNull(viewData);

            AssertListsAreEqual(viewData, Enum.GetValues(typeof(JobAction)).Cast<JobAction>().ToList(), x => x.ToString(), x => ((int)x).ToString(), 0);
        }

        [TestMethod]
        public void PerformAction_has_correct_view_data()
        {
            // Arrange

            var jobModel = new QuartzJobActionModel
            {
                JobDetails = new QuartzScheduledJobsGridModel
                {
                    GroupName = "group_name",
                    JobName = "job_name",
                    TriggerGroup = "trigger_group",
                    TriggerName = "trigger_name"
                },
                SelectedAction = 0
            };

            // Act

            var result = _controller.PerformAction(jobModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var viewData = result.ViewData["SelectedAction"] as List<SelectListItem>;

            Assert.IsNotNull(viewData);

            AssertListsAreEqual(viewData, Enum.GetValues(typeof(JobAction)).Cast<JobAction>().ToList(), x => x.ToString(), x => ((int)x).ToString(), jobModel.SelectedAction);
        }

        [TestMethod]
        public void PerformAction_ByZeroSelectedAction_has_correct_model()
        {
            // Arrange

            var jobModel = new QuartzJobActionModel
            {
                JobDetails = new QuartzScheduledJobsGridModel
                {
                    GroupName = "group_name",
                    JobName = "job_name",
                    TriggerGroup = "trigger_group",
                    TriggerName = "trigger_name"
                },
                SelectedAction = 0
            };

            // Act

            var result = _controller.PerformAction(jobModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as QuartzJobActionModel;

            Assert.IsNotNull(model);
            Assert.AreSame(jobModel, model);

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["SelectedAction"].Errors.Count > 0);

        }

        [TestMethod]
        public void PerformAction_ByReScheduleSelectedActionAndNullCronExpression_has_correct_model()
        {
            // Arrange

            var jobModel = new QuartzJobActionModel
            {
                JobDetails = new QuartzScheduledJobsGridModel
                {
                    GroupName = "group_name",
                    JobName = "job_name",
                    TriggerGroup = "trigger_group",
                    TriggerName = "trigger_name"
                },
                SelectedAction = (int)JobAction.ReSchedule
            };

            // Act

            var result = _controller.PerformAction(jobModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as QuartzJobActionModel;

            Assert.IsNotNull(model);
            Assert.AreSame(jobModel, model);

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["JobDetails.CronExpression"].Errors.Count > 0);

        }

        [TestMethod]
        public void PerformAction_ByReScheduleSelectedActionNotNullCronExpressionAndExceptionOcurred_has_correct_model()
        {
            // Arrange

            var jobModel = new QuartzJobActionModel
            {
                JobDetails = new QuartzScheduledJobsGridModel
                {
                    GroupName = "group_name",
                    JobName = "job_name",
                    TriggerGroup = "trigger_group",
                    TriggerName = "trigger_name",
                    CronExpression = "cronExpression"
                },
                SelectedAction = (int)JobAction.ReSchedule
            };

            _js.Setup(x => x.ExecuteAction((JobAction) jobModel.SelectedAction,
                jobModel.JobDetails.GroupName,
                jobModel.JobDetails.JobName,
                jobModel.JobDetails.TriggerName,
                jobModel.JobDetails.TriggerGroup,
                jobModel.JobDetails.CronExpression)).Throws(new SrvException("key", "exception"));

            // Act

            var result = _controller.PerformAction(jobModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as QuartzJobActionModel;

            Assert.IsNotNull(model);
            Assert.AreSame(jobModel, model);

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["JobDetails.CronExpression"].Errors.Count > 0);

        }

        [TestMethod]
        public void PerformAction_ByReScheduleSelectedActionNotNullCronExpressionAndExceptionMissing_returns_correct_content()
        {
            // Arrange

            var jobModel = new QuartzJobActionModel
            {
                JobDetails = new QuartzScheduledJobsGridModel
                {
                    GroupName = "group_name",
                    JobName = "job_name",
                    TriggerGroup = "trigger_group",
                    TriggerName = "trigger_name",
                    CronExpression = "cronExpression"
                },
                SelectedAction = (int)JobAction.ReSchedule
            };

            _js.Setup(x => x.ExecuteAction((JobAction)jobModel.SelectedAction,
                jobModel.JobDetails.GroupName,
                jobModel.JobDetails.JobName,
                jobModel.JobDetails.TriggerName,
                jobModel.JobDetails.TriggerGroup,
                jobModel.JobDetails.CronExpression));

            // Act

            var result = _controller.PerformAction(jobModel) as ContentResult;

            // Assert

            Assert.IsNotNull(result);

           _js.Verify(x => x.ExecuteAction((JobAction)jobModel.SelectedAction,
               jobModel.JobDetails.GroupName,
               jobModel.JobDetails.JobName,
               jobModel.JobDetails.TriggerName,
               jobModel.JobDetails.TriggerGroup,
               jobModel.JobDetails.CronExpression), Times.Once);
            
           Assert.AreEqual(result.Content, Const.CloseWindowContent);
        }

        private static List<QuartzJobData> GetQuartzJobDatas()
        {
            return new List<QuartzJobData>
            {
                GetQuartzJobData0(),
                GetQuartzJobData1()
            }.OrderBy(x => x.GroupName).ThenBy(x => x.JobName).ToList();
        }

        private static QuartzJobData GetQuartzJobData0()
        {
            var jobProgress = new JobProgress();
            jobProgress.CreatStageProgressInfo("Test Stage", 0, 1000, 100);
            return new QuartzJobData
            {
                CronExpression = "cronExpression0",
                JobName = "job0",
                JobDescription = "job0",
                GroupName = "group0",
                Progress = jobProgress
            };
        }

        private static QuartzJobData GetQuartzJobData1()
        {
            return new QuartzJobData
            {
                CronExpression = "cronExpression1",
                JobName = "job1",
                JobDescription = "job1",
                GroupName = "group1"
            };
        }

        private static void AssertListsAreEqual<T>(IEnumerable<SelectListItem> list1, List<T> list2, Func<T, string> textFunc, Func<T, string> valueFunc, long selectedId)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.Text) && string.Equals(valueFunc(x), item.Value)) &&
                                            item.Selected == (!string.IsNullOrEmpty(item.Value) && selectedId.ToString() == item.Value)));
        }

    }
}

