using System;
using Amdaris;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain.Importer.ToSaise;
using NHibernate;

namespace CEC.QuartzServer.Jobs.Export
{
    public abstract class SaiseExportStageExecuter
    {
        private readonly ProgressInfo _stageProgress;
        private readonly SaiseExporterStage _saiseExporterStage;
        private readonly IStatelessSession _session;
        private readonly ILogger _logger;
        protected bool _interruptPending;
        private object o = new object();
        private const int maxMessageSize = 1000;

        protected SaiseExportStageExecuter(
            ProgressInfo stageProgress,
            SaiseExporterStage saiseExporterStage,
            IStatelessSession session, ILogger logger)
        {
            _stageProgress = stageProgress;
            _saiseExporterStage = saiseExporterStage;
            _session = session;
            _logger = logger;
        }

        public void ExecuteState()
        {
            if (_saiseExporterStage.Status == SaiseExporterStageStatus.Cancelled)
            {
                return;
            }

            SetStatus(SaiseExporterStageStatus.InProgress);
            
            ExecuteStateInternal();

            SetStatus(SaiseExporterStageStatus.Done);
        }

        public SaiseExporterStage SaiseExporterStage
        {
            get { return _saiseExporterStage; }
        }
        
        public void Interrupt()
        {
            lock (o)
            {
                _interruptPending = true;
                if (_saiseExporterStage.Status == SaiseExporterStageStatus.Pending ||
                    _saiseExporterStage.Status == SaiseExporterStageStatus.InProgress)
                {
                    SetStatus(SaiseExporterStageStatus.Cancelled);
                }
            }
        }

        private void SetStatus(SaiseExporterStageStatus status, string errorMessage = null)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _saiseExporterStage.Status = status;

				if (status == SaiseExporterStageStatus.InProgress)
				{
					_saiseExporterStage.StartDate = DateTimeOffset.Now;
				}

                if (status == SaiseExporterStageStatus.Done || status == SaiseExporterStageStatus.Failed || status == SaiseExporterStageStatus.Cancelled)
				{
					_saiseExporterStage.EndDate = DateTimeOffset.Now;
				}

                if (status == SaiseExporterStageStatus.Failed)
                {
                    _saiseExporterStage.ErrorMessage = errorMessage.Truncate(maxMessageSize);
                }

                _session.Update(_saiseExporterStage);
                transaction.Commit();
            }

            if (status == SaiseExporterStageStatus.Failed)
            {
                throw new SaiseExporterBusinessException(errorMessage);
            }
        }

        protected void SetError(string errorMessage)
        {
            SetStatus(SaiseExporterStageStatus.Failed, errorMessage);
        }

        protected IStatelessSession Session
        {
            get { return _session; }
        }

        protected abstract void ExecuteStateInternal();

        protected void WriteLog(LogType logType,string message, Exception exception = null)
        {
            if (_logger != null)
            {
                _logger.Log(logType, message, exception);
            }
        }

        protected ProgressInfo StageProgress
        {
            get { return _stageProgress; }
        }
    }
}