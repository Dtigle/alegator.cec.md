using AutoMapper;
using CEC.Web.SRV.Profiles;

namespace CEC.Web.SRV.App_Start
{
    /// <summary>
    /// Provides the bootstrapping for <see cref="AutoMapper"/>
    /// </summary>
    public static class AutoMapperConfig
    {
        /// <summary>
        /// Initialize AutoMapper
        /// </summary>
        public static void Initialize()
        {
            Mapper.Initialize(arg =>
            {
                arg.AddProfile<IdentityUserProfile>();
                arg.AddProfile<LookupProfile>();
                arg.AddProfile<SrvGridModelsProfile>();
                arg.AddProfile<HistoryProfile>();
                arg.AddProfile<ElectionProfile>();
				arg.AddProfile<VotersProfile>();
				arg.AddProfile<AddressProfile>();
				arg.AddProfile<ConflictProfile>();
				arg.AddProfile<ExportPollingStationsProfile>();
				arg.AddProfile<PrintSessionProfile>();
				arg.AddProfile<SaiseExporterStageProfile>();
				arg.AddProfile<SaiseExporterProfile>();
                arg.AddProfile<QuartzDataProfile>();
                arg.AddProfile<StatisticsProfile>();
				arg.AddProfile<RspCheckingProfile>();
				arg.AddProfile<ProblematicDataPollingStationStatisticsProfile>();
                arg.AddProfile<AppConfigurationProfile>();
				arg.AddProfile<InteropProfile>();
            });
        }
    }
}