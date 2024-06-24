using Amdaris;
using Quartz;
using Quartz.Spi;

namespace CEC.QuartzServer.Core
{
    public class UnityJobFactory : IJobFactory
    {
        private readonly IDependencyResolver _container;

        public UnityJobFactory(IDependencyResolver container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_container.Resolve(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            // no op for now
        }
    }
}
