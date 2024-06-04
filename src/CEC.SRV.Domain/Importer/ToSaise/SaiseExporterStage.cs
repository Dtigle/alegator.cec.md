using System;
using System.ComponentModel;

namespace CEC.SRV.Domain.Importer.ToSaise
{
    public enum SaiseExporterStageStatus
    {
		[Description("In așteptare")]
        Pending = 1,
		[Description("In execuție")]
        InProgress = 2,
		[Description("Procesat")]
        Done = 3,
		[Description("Eronat")]
        Failed = 4,
        [Description("Întrerupt")]
        Cancelled = 5
    }

    public enum SaiseExporterStageType
    {
        ElectionValidation = 1,
        PollingStationValidation = 2,
        VoterUpdate = 3,
        VoterUpdateConflicted = 4,
        RegionValidation = 5,
        AssignVotersToPollingStations = 6,
    }

    public class SaiseExporterStage : SRVBaseEntity
    {
        private readonly SaiseExporter _saiseExporter;

        public SaiseExporterStage()
        {
        }

        public SaiseExporterStage(SaiseExporter saiseExporter)
        {
            _saiseExporter = saiseExporter;
        }

        public virtual SaiseExporterStageStatus Status { get; set; }

        public virtual SaiseExporterStageType StageType { get; set; }

        public virtual string Statistics { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual string Description { get; set; }

        public virtual SaiseExporter SaiseExporter
        {
            get { return _saiseExporter; }
        }

		public virtual DateTimeOffset? StartDate { get; set; }

		public virtual DateTimeOffset? EndDate { get; set; }
    }
}