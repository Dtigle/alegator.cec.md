using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Quartz
{
    public interface IJobsScheduler
    {
        void Connect();
        bool IsRunning { get; }
        List<QuartzJobData> GetJobs();
        IList<QuartzJobData> GetRunningJobs();
        void ExecuteAction(JobAction action, string groupName, string jobName, string triggerName, string triggerGroupName, string cronExpression);
    }
}
