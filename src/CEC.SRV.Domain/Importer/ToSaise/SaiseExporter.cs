using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CEC.SRV.Domain.Importer.ToSaise
{

    public enum SaiseExporterStatus
    {
        [Description("In așteptare")]
        New = 1,
        [Description("In execuție")]
        InProgress = 2,
        [Description("Eronat")]
        Failed = 3,
        [Description("Procesat")]
        Success = 4,
        [Description("Întrerupt")]
        Cancelled = 5
    }

    public class SaiseExporter : SRVBaseEntity
    {
        private long _electionDayId;

        private long _electionId;

        protected SaiseExporter()
        {

        }

        public SaiseExporter(long electionDayId, long electionId = -1)
        {
            Status = SaiseExporterStatus.New;
            _electionDayId = electionDayId;
            _electionId = electionId;
            CreateStages();
        }

        public virtual long ElectionDayId
        {
            get { return _electionDayId; }
        }

        public virtual long ElectionId
        {
            get { return _electionId; }
        }

        public virtual bool ExportAllVoters { get; set; }

        public virtual SaiseExporterStatus Status { get; set; }

        public virtual IEnumerable<SaiseExporterStage> Stages { get; set; }
        public virtual string ErrorMessage { get; set; }

        private void CreateStages()
        {
            Stages = new List<SaiseExporterStage>
            {
                new SaiseExporterStage(this) { StageType = SaiseExporterStageType.ElectionValidation, Description = "Validarea alegerilor",
                    Status = SaiseExporterStageStatus.Pending },
                new SaiseExporterStage(this) { StageType = SaiseExporterStageType.PollingStationValidation, Description = "Validarea secțiilor de votare",
                    Status = SaiseExporterStageStatus.Pending },
                new SaiseExporterStage(this) { StageType = SaiseExporterStageType.RegionValidation, Description = "Validarea regiunilor din SRV",
                    Status = SaiseExporterStageStatus.Pending },
                new SaiseExporterStage(this) { StageType = SaiseExporterStageType.VoterUpdate, Description = "Sincronizare liste alegători și asignarea lor la secțiile de votare",
                    Status = SaiseExporterStageStatus.Pending },
                new SaiseExporterStage(this) { StageType = SaiseExporterStageType.VoterUpdateConflicted, Description = "Sincronizare liste alegători din conflicte",
                    Status = SaiseExporterStageStatus.Pending },

            };
        }

        public virtual DateTimeOffset? StartDate { get; set; }

        public virtual DateTimeOffset? EndDate { get; set; }
    }
}