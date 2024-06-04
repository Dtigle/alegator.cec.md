using Quartz;
using Quartz.Impl;

namespace CEC.QuartzServer.Core
{
    public class CecQuartzServer : IQuartzServer
    {
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public void Initialize()
        {
            _schedulerFactory = new StdSchedulerFactory();
            _scheduler = _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = new UnityJobFactory(Amdaris.DependencyResolver.Current);
        }

        public void Start()
        {
            _scheduler.Start();
        }

        public void Stop()
        {
            _scheduler.Shutdown();
        }

        public void Pause()
        {
            _scheduler.PauseAll();
        }

        public void Resume()
        {
            _scheduler.ResumeAll();
        }

        public void Dispose()
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
            {
                _scheduler.Shutdown();
            }
        }
    }
}
