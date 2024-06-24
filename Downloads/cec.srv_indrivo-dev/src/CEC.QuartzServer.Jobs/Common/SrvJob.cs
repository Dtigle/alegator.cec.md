using System.Security.Claims;
using CEC.SRV.BLL.Quartz;
using Quartz;

namespace CEC.QuartzServer.Jobs.Common
{
    public abstract class SrvJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ClaimsPrincipal.Current.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"), 
                new Claim(ClaimTypes.Name, "System"),
                new Claim(ClaimTypes.Role, "System")
            }, "QuatzAuthentication"));

            Progress = new JobProgress();
            context.Put(JobProgress.JobContextKey, Progress);

            ExecuteInternal(context);
        }

        protected internal abstract void ExecuteInternal(IJobExecutionContext context);

        protected JobProgress Progress { get; private set; }
    }

}