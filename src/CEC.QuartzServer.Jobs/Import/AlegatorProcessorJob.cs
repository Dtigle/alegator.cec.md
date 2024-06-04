using Amdaris;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL.Importer;
using CEC.SRV.BLL.Repositories;
using Quartz;

namespace CEC.QuartzServer.Jobs.Import
{
    [DisallowConcurrentExecution]
    public class AlegatorProcessorJob : SrvJob
    {
        private readonly AlegatorProcesser _alegatorProcesser;
        private ISRVRepository _srvRepository;
        private const int BatchSize = 200;

        public AlegatorProcessorJob(ISRVRepository srvRepository, ILogger logger)
        {
            _srvRepository = srvRepository;
            _alegatorProcesser = new AlegatorProcesser(_srvRepository, logger, BatchSize);
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            _alegatorProcesser.ProcessAllNew();
        }
    }
}