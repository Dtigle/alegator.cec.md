using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace CEC.SRV.BLL.Quartz
{
    public class QuartzJobData
    {
        public string Id 
        {
            get { return string.Format("{0}_{1}", JobName, GroupName).Replace(' ', '_'); }
        }

        public string GroupName { get; set; }

        public string JobName { get; set; }

        public string JobDescription { get; set; }

        public string JobClassName { get; set; }

        public string TriggerName { get; set; }

        public string TriggerGroupName { get; set; }

        public string TriggerType { get; set; }

        public bool HasCronTrigger { get; set; }

        public string CronExpression { get; set; }

        public TriggerState TriggerState { get; set; }

        public DateTime? NextFireTime { get; set; }

        public DateTime? PreviousFireTime { get; set; }

        public DateTime? FireTime { get; set; }

        public bool IsInterruptable { get; set; }

        public bool IsManuallyTriggered
        {
            get { return TriggerGroupName == "MANUAL_TRIGGER"; }
        }

        public TimeSpan GetRunTime()
        {
            if (FireTime == null)
            {
                return new TimeSpan();
            }
            return DateTime.Now - FireTime.Value;
        }

        public JobProgress Progress { get; set; }
    }
}
