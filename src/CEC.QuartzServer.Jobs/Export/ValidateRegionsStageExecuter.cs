using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amdaris;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using NHibernate;
using NHibernate.Criterion;

namespace CEC.QuartzServer.Jobs.Export
{
    public class ValidateRegionsStageExecuter : SaiseExportStageExecuter
    {
        private readonly SaiseExporterStage _saiseExporterStage;
        private readonly bool _ignoreMissingSaiseIdinRegion;

        public ValidateRegionsStageExecuter(
            ProgressInfo stageProgress,
            long? saiseElectionId,
            SaiseExporterStage saiseExporterStage, IStatelessSession session, ILogger logger, bool ignoreMissingSaiseIdinRegion)
            : base(stageProgress, saiseExporterStage, session, logger)
        {
            _saiseExporterStage = saiseExporterStage;

            if (!saiseElectionId.HasValue)
            {
                SetError("Saise Election Id is not defined");
            }

            StageStatistic = new RegionStageStatistic();

            _ignoreMissingSaiseIdinRegion = ignoreMissingSaiseIdinRegion;
        }

        public RegionStageStatistic StageStatistic { get; set; }

        protected override void ExecuteStateInternal()
        {
            StageProgress.SetMaximum(1);

            var srvRegions = GetSRVRegionWithoutSaiseId();

            StageStatistic.Total = srvRegions.Count();

            var message = new StringBuilder();
            foreach (var region in srvRegions)
            {
                message.AppendLine(region);
            }

            StageProgress.Increase();

            if (message.Length > 0 && !_ignoreMissingSaiseIdinRegion)
            {
                SetError(message.ToString());
            }
            else
            {
                SetMessage(message.ToString());
            }
        }

        private void SetMessage(string message)
        {
            using (var transaction = Session.BeginTransaction())
            {
                _saiseExporterStage.ErrorMessage = message.Truncate(1000);
                _saiseExporterStage.Status = SaiseExporterStageStatus.Done;

                Session.Update(_saiseExporterStage);
                transaction.Commit();
            }
        }

        private IList<string> GetSRVRegionWithoutSaiseId()
        {
            return Session.QueryOver<RegionWithoutSaiseId>()
                .Select(Projections.Property<RegionWithoutSaiseId>(x=>x.FullyQualifiedName))
                .List<string>();
        }
    }

    public class RegionStageStatistic
    {
        public long Total { get; set; }
    }
}