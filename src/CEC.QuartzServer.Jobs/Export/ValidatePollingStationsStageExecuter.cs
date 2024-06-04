using System.Text;
using Amdaris;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using NHibernate;

namespace CEC.QuartzServer.Jobs.Export
{
    public class ValidatePollingStationsStageExecuter : SaiseExportStageExecuter
    {
        private readonly long? _saiseElectionId;
        private readonly SaiseRepository _saiseRepository;

        public ValidatePollingStationsStageExecuter(
            ProgressInfo stageProgress,
            long? saiseElectionId, SaiseRepository saiseRepository,
            SaiseExporterStage saiseExporterStage, IStatelessSession session, ILogger logger)
                : base(stageProgress, saiseExporterStage, session, logger)
        {
            _saiseElectionId = saiseElectionId;
            _saiseRepository = saiseRepository;
            StageStatistic = new PollingStationStageStatistic();
        }

        public PollingStationStageStatistic StageStatistic { get; set; }

        protected override void ExecuteStateInternal()
        {
            if (!_saiseElectionId.HasValue)
            {
                SetError("SAISE Election Id doen not have a value");
            }

            var saisePollingStations = _saiseRepository.GetPollingStation(_saiseElectionId.Value);
            StageProgress.SetMaximum(saisePollingStations.Count);

            StageStatistic.Total = saisePollingStations.Count;

            if (saisePollingStations.Count == 0)
            {
                SetError(string.Format("Saise does not have any assigned Polling station for election with id {0}", _saiseElectionId));
            }

            var errorMessages = new StringBuilder();
            foreach (var saisePollingStation in saisePollingStations)
            {
                if (_interruptPending) break;
                var srvPollingStation = GetSrvPollingStationBySaiseId(saisePollingStation.Id);

                StageProgress.Increase();

                if (srvPollingStation == null)
                {
                    StageStatistic.Error++;
                    errorMessages.AppendLine(
                        string.Format("Polling station with id {0} and number {1} does not exist in SRV",
                            saisePollingStation.Id, saisePollingStation.Number));
                    continue;
                }

                int srvPollingStationNumber;

                if (!int.TryParse(srvPollingStation.Number, out srvPollingStationNumber))
                {
                    StageStatistic.Error++;
                    errorMessages.AppendLine(string.Format("Can not parse SRV polling station number {0}, polling station Id = {1} ", srvPollingStation.Number, srvPollingStation.Id));
                    continue;
                }

                if (srvPollingStationNumber != saisePollingStation.Number)
                {
                    StageStatistic.Error++;
                    errorMessages.AppendLine(string.Format("The SRV polling station number {0} differs from SAISE number {1}", srvPollingStationNumber, saisePollingStation.Number));
                    continue;
                }

                //Always cast subnumber 'NULL' value to empty string
                if ((srvPollingStation.SubNumber??string.Empty) != (saisePollingStation.SubNumber??string.Empty))
                {
                    StageStatistic.Error++;
                    errorMessages.AppendLine(string.Format("The SRV polling station with number {0} (SRV ID {3} / SAISEID {4}). Subnumber missmatch between SRV and SAISE(SAISE subnumer '{1}', SRV subnumer '{2}' )", srvPollingStationNumber, saisePollingStation.SubNumber, srvPollingStation.SubNumber, srvPollingStation.Id, saisePollingStation.Id));
                    continue;
                }
            }

            if (errorMessages.Length > 0)
            {
                WriteLog(LogType.Info, errorMessages.ToString());
                StageStatistic.ErrorMessage = "Există regiuni fără SaiseId. Lista regiunilor vezi în log file.";
                SetError(StageStatistic.ErrorMessage);
            }
        }

        private PollingStation GetSrvPollingStationBySaiseId(long pollingStationId)
        {
            return Session.QueryOver<PollingStation>().Where(x => x.SaiseId == pollingStationId && x.Deleted == null).SingleOrDefault<PollingStation>();
        }
    }

    public class PollingStationStageStatistic
    {
        public int Total { get; set; }
        public long Error { get; set; }
        public string ErrorMessage { get; set; }
    }
}