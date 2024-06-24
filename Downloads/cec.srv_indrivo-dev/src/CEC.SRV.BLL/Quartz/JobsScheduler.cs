using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.BLL.Exceptions;
using CEC.Web.SRV.Resources;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;

namespace CEC.SRV.BLL.Quartz
{
    public class JobsScheduler : IJobsScheduler
    {
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _instance;

        public void Connect()
        {
            _schedulerFactory = new StdSchedulerFactory();
            try
            {
                _instance = _schedulerFactory.GetScheduler();
            }
            catch (SchedulerException ex)
            {
                throw new Exception(string.Format("Failed: {0}", ex.Message));
            }
        }

        public bool IsRunning
        {
            get { return _instance != null && _instance.IsStarted; }
        }

        public List<QuartzJobData> GetJobs()
        {
            var result = new List<QuartzJobData>();
            foreach (var group in _instance.GetJobGroupNames())
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = _instance.GetJobKeys(groupMatcher);

                foreach (var jobKey in jobKeys)
                {
                    var jobDetails = _instance.GetJobDetail(jobKey);
                    var jobTriggers = _instance.GetTriggersOfJob(jobKey);
                    if (jobTriggers.Any())
                    {
                        foreach (var jobTrigger in jobTriggers)
                        {
                            var jobData = GetJobData(jobDetails, jobTrigger);
                            result.Add(jobData);
                        }
                    }
                    else
                    {
                        result.Add(GetJobData(jobDetails, null));
                    }
                }
            }

            return result;
        }

        public IList<QuartzJobData> GetRunningJobs()
        {
            var result = new List<QuartzJobData>();
            var runningJobs = _instance.GetCurrentlyExecutingJobs();
            foreach (var jobContext in runningJobs)
            {
                var jobData = GetJobData(jobContext.JobDetail, jobContext.Trigger);
                jobData.FireTime = AdjustTimeToLocal(jobContext.FireTimeUtc);
                jobData.Progress = jobContext.Get(JobProgress.JobContextKey) as JobProgress;
                result.Add(jobData);
            }

            return result;
        }

        public void ExecuteAction(JobAction action, string groupName, string jobName, string triggerName, string triggerGroupName, string cronExpression)
        {
            switch (action)
            {
                case JobAction.Interrupt:
                    InterruptJob(groupName, jobName);
                    break;
                case JobAction.RunImmediate:
                    RunImmediate(groupName, jobName);
                    break;
                case JobAction.Pause:
                    PauseJob(groupName, jobName);
                    break;
                case JobAction.Resume:
                    ResumeJob(groupName, jobName);
                    break;
                case JobAction.ReSchedule:
                    RescheduleJob(groupName, jobName, triggerName, triggerGroupName, cronExpression);
                    break;
            }
        }

        private void ResumeJob(string groupName, string jobName)
        {
            _instance.ResumeJob(new JobKey(jobName, groupName));
        }

        private void PauseJob(string groupName, string jobName)
        {
            _instance.PauseJob(new JobKey(jobName, groupName));
        }

        private void RunImmediate(string groupName, string jobName)
        {
            var runningJobs = _instance
                .GetCurrentlyExecutingJobs();
            var runningJob = runningJobs.Count > 0
                ? runningJobs
                    .FirstOrDefault(x => x.JobDetail.Key.Name == jobName && x.JobDetail.Key.Group == groupName)
                : null;

            if (runningJob == null)
            {
                _instance.TriggerJob(new JobKey(jobName, groupName));
            }
        }

        private void InterruptJob(string groupName, string jobName)
        {
            _instance.Interrupt(new JobKey(jobName, groupName));
        }

        private QuartzJobData GetJobData(IJobDetail jobDetail, ITrigger trigger)
        {
            var result = new QuartzJobData
            {
                GroupName = jobDetail.Key.Group,
                JobName = jobDetail.Key.Name,
                JobDescription = jobDetail.Description,
                JobClassName = jobDetail.JobType.FullName,
                IsInterruptable = typeof(IInterruptableJob).IsAssignableFrom(jobDetail.JobType),
            };

            if (trigger != null)
            {
                var triggerState = _instance.GetTriggerState(trigger.Key);

                result.TriggerName = trigger.Key.Name;
                result.TriggerGroupName = trigger.Key.Group;
                result.TriggerState = triggerState;
                result.NextFireTime = AdjustTimeToLocal(trigger.GetNextFireTimeUtc());
                result.PreviousFireTime = AdjustTimeToLocal(trigger.GetPreviousFireTimeUtc());

                var cronTrigger = trigger as CronTriggerImpl;
                if (cronTrigger != null)
                {
                    result.HasCronTrigger = true;
                    result.CronExpression = cronTrigger.CronExpressionString;
                }
            }

            return result;
        }

        private static DateTime? AdjustTimeToLocal(DateTimeOffset? dt)
        {
            if (dt == null)
            {
                return null;
            }

            return dt.Value.LocalDateTime;
        }

        private void RescheduleJob(string group, string job, string triggerName, string triggerGroupName, string cronExpression)
        {
            var triggerKey = new TriggerKey(triggerName, triggerGroupName);
            var trigger = _instance.GetTrigger(triggerKey);
            var cronTrigger = trigger as CronTriggerImpl;
            if (cronTrigger != null && cronTrigger.CronExpressionString != cronExpression)
            {
                if (!IsValidCronExpression(cronExpression))
                {
                    throw new SrvException("CronExpression_NotValid", MUI.CronExpression_NotValid);
                }
                var newTrigger = new CronTriggerImpl(triggerName, triggerGroupName, job, group, cronExpression);

                _instance.RescheduleJob(triggerKey, newTrigger);
            }
        }

        public bool IsValidCronExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }
    }
}
