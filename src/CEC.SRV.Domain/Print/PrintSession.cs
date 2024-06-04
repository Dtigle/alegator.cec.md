using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CEC.SRV.Domain.Print
{
    public class PrintSession : PrintEntity
    {
        private readonly Election _election;
        private IList<ExportPollingStation> _exportPollingStations;

        public PrintSession()
        {
        }

        public PrintSession(Election election, IEnumerable<PollingStation> pollingStations)
        {
            _election = election;
            CreateExportPollingStations(pollingStations);
        }

        public PrintSession(Election election, IList<ExportPollingStation> exportPollingStations)
        {
            _election = election;
            _exportPollingStations = exportPollingStations
                    .Select(x => new ExportPollingStation(this, x.PollingStation, x.ElectionRound, x.Circumscription, x.NumberPerElection))
                    .ToList();
        }

        public virtual Election Election
        {
            get { return _election; }

        }

        public virtual IReadOnlyList<ExportPollingStation> ExportPollingStations
        {
            get { return new ReadOnlyCollection<ExportPollingStation>(_exportPollingStations); }
        }

        private void CreateExportPollingStations(IEnumerable<PollingStation> pollingStations)
        {
            if (pollingStations != null && pollingStations.Any())
            {
                _exportPollingStations = pollingStations
                    .Select(x => new ExportPollingStation(this, x))
                    .ToList();
            }
        }

        public virtual void Cancel()
        {
            foreach (var exportItem in _exportPollingStations)
            {
                if (exportItem.Status != PrintStatus.Finished)
                {
                    exportItem.Status = PrintStatus.Canceled;
                    exportItem.EndDate = DateTimeOffset.Now;
                }
            }
            Status = PrintStatus.Canceled;
            EndDate = DateTimeOffset.Now;
        }
        public virtual string GetObjectType()
        {
            return GetType().Name;
        }
    }
}