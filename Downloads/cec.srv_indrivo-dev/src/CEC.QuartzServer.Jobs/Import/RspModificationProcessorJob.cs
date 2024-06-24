using System;
using System.Configuration;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Importer;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Importer;
using Quartz;

namespace CEC.QuartzServer.Jobs.Import
{
    [DisallowConcurrentExecution]
    public class RspModificationProcessorJob : SrvJob
    {
        private readonly IImportStatisticsBll _importStatisticsBll;
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;
        private readonly RspModificationProcesser _rspModificationProcesser;
        private const int BatchSize = 200;

        public RspModificationProcessorJob(ISRVRepository srvRepository, 
                                            IImportBll importBll, 
                                            IImportStatisticsBll importStatisticsBll, 
                                            ILogger logger, 
                                            IConfigurationSettingManager configurationSettingManager)
        {
            _importStatisticsBll = importStatisticsBll;
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;
            _rspModificationProcesser = new RspModificationProcesser(srvRepository, importBll, _importStatisticsBll, logger, BatchSize);
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            int remain = 0;

            var ignoreNewEntries =
                _configurationSettingManager.Get("RspModificationProcessorJob_IgnoreNewEntries").GetValue<bool>();

            var statuses = (ignoreNewEntries) ? new[] { RawDataStatus.Retry, RawDataStatus.ForceImport } : new[] { RawDataStatus.New, RawDataStatus.Retry, RawDataStatus.ForceImport, RawDataStatus.ToRemove };
            _logger.Info(string.Format("Run RspModificationProcessorJob at {0} with flag IgnoreNewEntries = {1}", DateTime.Now, ignoreNewEntries));

            var progressInfo = InitProgress(statuses);

            do
            {
                using (var unit = new NhUnitOfWork())
                {
                    remain = _rspModificationProcesser.CountRawByStatus(statuses);

                    _logger.Info(string.Format("Not processed (RspModificationProcessorJob) {1} , time {0}", DateTime.Now, remain));

                    progressInfo.SetProgress(progressInfo.Maximum - remain);

                    _rspModificationProcesser.ProcessNew(statuses);
                    unit.Complete();

                }
            } while (remain > 0);

            using (var unit = new NhUnitOfWork())
            {
                var statistics = _rspModificationProcesser.GetOverallStatistic();
                _importStatisticsBll.Save(statistics);
                unit.Complete();
            }

            _logger.Info(string.Format("Finish RspModificationProcessorJob at {0}", DateTime.Now));
        }

        private ProgressInfo InitProgress(RawDataStatus[] statuses)
        {
            using (var unit = new NhUnitOfWork())
            {
                var progressInfo = Progress.CreatStageProgressInfo("Modifications processing", 0, 0);
                var totalRows = _rspModificationProcesser.CountRawByStatus(statuses);
                progressInfo.SetMaximum(totalRows);
                return progressInfo;
            }
        }
    }
}