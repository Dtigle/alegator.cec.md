using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.QuartzServer.Jobs.Import;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Importer.ToSaise;
using NHibernate;
using Quartz;

namespace CEC.QuartzServer.Jobs.Export
{
    [DisallowConcurrentExecution]
    public class SaiseExporterJob : SrvJob, IInterruptableJob
    {
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;
        private readonly SaiseRepository _saiseRepository;
        private readonly IStatelessSession _session;
        
        private IList<SaiseExportStageExecuter> _stageExecuters;
        private SaiseExporter _saiseExporter;
        private ISRVRepository _srvRepository;
        private bool _interrupt;

        public SaiseExporterJob(ISessionFactory sessionFactory, ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;

            try
            {
                _session = sessionFactory.OpenStatelessSession();
                _saiseRepository = new SaiseRepository();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            if (_session == null)
            {
                _logger.Debug("srv session is null");
                return;
            }

            if (_saiseRepository == null)
            {
                _logger.Debug("saise repository is null");
                return;
            }

            try
            {
                _saiseExporter = GetNewSaiseExporter();
                if (_saiseExporter != null)
                {
                    SetStateExecuters(_saiseExporter);

                    SetSaiseExporterStatus(_saiseExporter, SaiseExporterStatus.InProgress);

                    ExecuteExportToSaise();
                }
            }
            catch (Exception ex)
            {
                if (_saiseExporter != null)
                {
                    var stageExporter =
                        _saiseExporter.Stages.FirstOrDefault(x => x.Status == SaiseExporterStageStatus.Failed);

                    if (stageExporter != null)
                    {
                        SetSaiseExporterStatus(_saiseExporter, SaiseExporterStatus.Failed,
                            string.Format("Failed on stage '{0}'", stageExporter.Description));
                    }
                    else
                    {
                        SetSaiseExporterStatus(_saiseExporter, SaiseExporterStatus.Failed, ex.Message);
                    }

                    _logger.Error(ex);
                }
            }
        }

        private void SetStateExecuters(SaiseExporter saiseExporter)
        {
            var ignoreMissingSaiseIdInSrvRegion =
            _configurationSettingManager.Get("SaiseExporterJob_IgnoreMissingSAISEIdinSRVRegion").GetValue<bool>();
            var ignorePeopleWithoutDoc =
                _configurationSettingManager.Get("SaiseExporterJob_IgnorePeopleWithoutDoc").GetValue<bool>();

            //_stageExecuters = new List<SaiseExportStageExecuter>
            //{
            //    new ValidateElectionStageExecuter(Progress.CreatStageProgressInfo("Election validation", 0, 0), 
            //        saiseExporter.Election, _saiseRepository,
            //        GetByType(saiseExporter, SaiseExporterStageType.ElectionValidation), _session, _logger),
            //    new ValidatePollingStationsStageExecuter(Progress.CreatStageProgressInfo("PollingStations validation", 0, 0),
            //        saiseExporter.Election.Id, _saiseRepository,
            //        GetByType(saiseExporter, SaiseExporterStageType.PollingStationValidation), _session, _logger),
            //    new ValidateRegionsStageExecuter(Progress.CreatStageProgressInfo("Regions validation", 0, 0),
            //        saiseExporter.Election.Id, 
            //        GetByType(saiseExporter, SaiseExporterStageType.RegionValidation), _session, _logger, ignoreMissingSaiseIdInSrvRegion),
            //    new UpdateSaiseVoterStageExecuter(Progress.CreatStageProgressInfo("Migrating Voters data SRV -> SAISE", 0, 0),
            //        saiseExporter.ExportAllVoters, saiseExporter.Election.Id, _saiseRepository,
            //        GetByType(saiseExporter, SaiseExporterStageType.VoterUpdate), _session, _logger, _configurationSettingManager),
            //    new UpdateConflictedSaiseVoterStageExecuter(Progress.CreatStageProgressInfo("Migrating conflicting Voters data SRV -> SAISE", 0, 0),
            //        _saiseRepository,
            //        GetByType(saiseExporter, SaiseExporterStageType.VoterUpdateConflicted), _session, _logger, 
            //        ignoreMissingSaiseIdInSrvRegion, ignorePeopleWithoutDoc),
            //};
        }

        private void ExecuteExportToSaise()
        {
            foreach (var saiseExportStageExecuter in _stageExecuters)
            {
                if (_interrupt)
                {
                    SetSaiseExporterStatus(saiseExportStageExecuter.SaiseExporterStage.SaiseExporter,
                        SaiseExporterStatus.Cancelled, "Cancelled by user.");
                    continue;
                }

                saiseExportStageExecuter.ExecuteState();

                if (saiseExportStageExecuter.SaiseExporterStage.Status == SaiseExporterStageStatus.Failed)
                {
                    SetSaiseExporterStatus(saiseExportStageExecuter.SaiseExporterStage.SaiseExporter,
                        SaiseExporterStatus.Failed, saiseExportStageExecuter.SaiseExporterStage.ErrorMessage);
                }
            }

            if (!_interrupt)
            {
                SetSaiseExporterStatus(_saiseExporter, SaiseExporterStatus.Success);
            }

        }

        private void SetSaiseExporterStatus(SaiseExporter saiseExporter, SaiseExporterStatus status, string errorMessage = null)
        {
            if (saiseExporter == null) throw new ArgumentNullException("saiseExporter");

            using (var transaction = _session.BeginTransaction())
            {
                saiseExporter.Status = status;

	            if (status == SaiseExporterStatus.InProgress)
	            {
		            saiseExporter.StartDate = DateTimeOffset.Now;
	            }

                if (status == SaiseExporterStatus.Success || status == SaiseExporterStatus.Failed || status == SaiseExporterStatus.Cancelled)
				{
					saiseExporter.EndDate = DateTimeOffset.Now;
				}

                if (status == SaiseExporterStatus.Failed)
                {
                    saiseExporter.ErrorMessage = errorMessage.Truncate(1000);
                }

                _session.Update(saiseExporter);
                transaction.Commit();
            }
        }

        private SaiseExporterStage GetByType(SaiseExporter saiseExporter, SaiseExporterStageType type)
        {
            return saiseExporter.Stages.First(x => x.StageType == type);
        }

        private SaiseExporter GetNewSaiseExporter()
        {
            return _session.QueryOver<SaiseExporter>()
                    .Fetch(x => x.Stages).Lazy
                    //.Fetch(x => x.Election).Lazy
                    .Where(x => x.Status == SaiseExporterStatus.New)
                    .SingleOrDefault<SaiseExporter>();
        }

        public void Interrupt()
        {
            foreach (var executer in _stageExecuters)
            {
                executer.Interrupt();
            }

            _interrupt = true;
        }
    }
}