using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using Quartz;

namespace CEC.QuartzServer.Jobs.Import
{
    [DisallowConcurrentExecution]
    public class RemoveStayStatementsJob : SrvJob, IInterruptableJob
    {
        private readonly ISRVRepository _repository;
        private readonly IVotersBll _votersBll;
        private readonly ILogger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public RemoveStayStatementsJob(ISRVRepository repository, IVotersBll votersBll, ILogger logger)
        {
            _repository = repository;
            _votersBll = votersBll;
            _logger = logger;
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4,
                CancellationToken = _cancellationTokenSource.Token
            };
            
            _logger.Warning("Start process at " + DateTime.Now);
            var globalStopWatch = new Stopwatch();
            globalStopWatch.Start();

            var stayStatementIds = GetStayStatementsToProcess();
            var stageProgress = Progress.CreatStageProgressInfo("Cancelling Stay Statements", 0, stayStatementIds.Count());

            try
            {
                Parallel.ForEach(stayStatementIds, parallelOptions, (stayStatementId) =>
                {
                    using (var unit = new NhUnitOfWork())
                    {
                        _votersBll.CancelStayStatement(stayStatementId);
                        unit.Complete();
                    }

                    stageProgress.Increase();

                    parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException e)
            {
                _logger.Warning("Job cancelled by user.");
            }

            globalStopWatch.Stop();
            var globalStr = string.Format("Job has worked for {0}", globalStopWatch.Elapsed);
            _logger.Warning(globalStr);
        }

        private IEnumerable<long> GetStayStatementsToProcess()
        {
            using (new NhUnitOfWork())
            {
                return _repository.Query<StayStatement>()
                    .Where(x => x.Deleted == null)
                    .Select(x => x.Id)
                    .ToList();
            }
        }

        public void Interrupt()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}