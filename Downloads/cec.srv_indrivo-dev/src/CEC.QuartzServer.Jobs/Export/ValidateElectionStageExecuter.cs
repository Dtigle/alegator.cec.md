using System;
using Amdaris;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using NHibernate;

namespace CEC.QuartzServer.Jobs.Export
{
    public class ValidateElectionStageExecuter : SaiseExportStageExecuter
    {
        private readonly Election _srvElection;
        private readonly SaiseRepository _saiseRepository;

        public ValidateElectionStageExecuter(
            ProgressInfo stageProgress,
            Election srvElection, SaiseRepository saiseRepository, 
            SaiseExporterStage saiseExporterStage, IStatelessSession session, ILogger logger) 
                : base(stageProgress, saiseExporterStage, session, logger)
        {
            if (srvElection == null) throw new ArgumentNullException("srvElection");
            
            _srvElection = srvElection;
            _saiseRepository = saiseRepository;
           StageStatistic = new ElectionStageStatistic();
        }

        public ElectionStageStatistic StageStatistic { get; set; }

        protected override void ExecuteStateInternal()
        {
            //StageProgress.SetMaximum(1);
            //if (!_srvElection.SaiseId.HasValue)
            //{
            //    SetError("There is not Election or SRV election does not have a SAISE election Id defined");
            //}

            //var saiseElection = _saiseRepository.GetElection(_srvElection.SaiseId.Value);

            //StageStatistic.Election = (saiseElection != null);

            //if (saiseElection == null)
            //{
            //    SetError(string.Format("There is not SAISE election with id {0}", _srvElection.SaiseId));
            //}

            //if (_srvElection.ElectionDate != saiseElection.DateOfElection)
            //{
            //    SetError(string.Format("Election dates differs. SRV election date is [{0}] but SAISE is [{1}]",
            //        _srvElection.ElectionDate, saiseElection.DateOfElection));
            //}

            //StageProgress.Increase();
        }
    }

    public class ElectionStageStatistic
    {
        public bool Election { get; set; }
    }
}