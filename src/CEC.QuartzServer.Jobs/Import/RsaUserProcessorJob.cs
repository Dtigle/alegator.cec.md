using Amdaris;
using Amdaris.Domain;
using Amdaris.NHibernateProvider.Identity;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL.Importer;
using CEC.SRV.Domain;
using Microsoft.AspNet.Identity;
using NHibernate;
using Quartz;

namespace CEC.QuartzServer.Jobs.Import
{
    [DisallowConcurrentExecution]
    public class RsaUserProcessorJob : SrvJob
    {
        private readonly RsaUserProcessor _rsaUserProcessor;
        private const int BatchSize = 200;

        public RsaUserProcessorJob(ISessionFactory sessionFactory, IRepository repository, ILogger logger)
        {
            var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
            var userManager = new UserManager<SRVIdentityUser>(userStore);

            _rsaUserProcessor = new RsaUserProcessor(repository, userManager, logger, BatchSize);
        
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            _rsaUserProcessor.ProcessAllNew();
        }
    }
}