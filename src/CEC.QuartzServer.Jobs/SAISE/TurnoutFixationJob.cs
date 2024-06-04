using System;
using System.Linq;
using Amdaris;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.QuartzServer.Jobs.Export;
using NHibernate;
using Quartz;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.SAISE
{
    [DisallowConcurrentExecution]
    public class TurnoutFixationJob : SrvJob, IInterruptableJob
    {
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;
        private bool _interruptPending;
        private IStatelessSession _session;
        private SaiseRepository _saiseRepository;

        public TurnoutFixationJob(ISessionFactory sessionFactory, ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;
            _session = sessionFactory.OpenStatelessSession();
            _saiseRepository = new SaiseRepository();
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            try
            {
                var electionId = _configurationSettingManager.Get("saiseTargetElectionId").GetValue<long>();
                var turnoutControlTimesValues = _configurationSettingManager.Get("TurnoutFixationJob_TurnoutControlTimes").Value;

                var turnoutControlTimes = turnoutControlTimesValues
                    .Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => TimeSpan.Parse(x.Trim()));
                var currentTime = DateTime.Now.TimeOfDay;
                var controlTime = turnoutControlTimes.FirstOrDefault(x => (currentTime - x).TotalMinutes <= 10);

                if (controlTime == TimeSpan.Zero)
                {
                    _logger.Debug("No controlTime could be found");
                    return;
                }

                InsertTurnouts(electionId, controlTime);

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void InsertTurnouts(long electionId, TimeSpan controlTime)
        {
            var assignedPollingStations = _saiseRepository.GetOpenedPollingStations(electionId);
            foreach (var assignedPollingStation in assignedPollingStations)
            {
                var psListCount = _saiseRepository.GetBaseListVotedCount(electionId, assignedPollingStation.PollingStation.Id, controlTime);
                var psSupplimentaryCount = _saiseRepository.GetSupplimentaryListVotedCount(electionId, assignedPollingStation.PollingStation.Id, controlTime);
                var turnoutEntry = new ElectionTurnout
                {
                    AssignedPollingStationId = assignedPollingStation.Id,
                    TimeOfEntry = controlTime.ToString("hh\\:mm"),
                    ListCount = psListCount,
                    SupplementaryCount = psSupplimentaryCount
                };

                _saiseRepository.SaveOrUpdate(turnoutEntry);
            }
        }

        public void Interrupt()
        {
            _interruptPending = true;
        }
    }
}
