using Amdaris;
using Amdaris.Domain;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exporters;
using Quartz;
using System;

namespace CEC.QuartzServer.Jobs.Reporting
{
    [DisallowConcurrentExecution]
    public class ResultsReportsPrintOutJob : SrvJob
    {
        private readonly ElectionResultsExporter _electionResultsExporter;
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;

        public ResultsReportsPrintOutJob(IRepository srvRepository, ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;
            _electionResultsExporter = new ElectionResultsExporter(srvRepository, _logger);
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            _electionResultsExporter.Progress = Progress;
            try
            {
                var printParams = new ElectionResultsPrintingParameters
                {
                    ServerUrl = _configurationSettingManager.Get("SSRS_ReportExecutionService").Value,
                    UserName = _configurationSettingManager.Get("SSRS_UserName").Value,
                    Password = _configurationSettingManager.Get("SSRS_Password").Value,
                    ReportName = _configurationSettingManager.Get("SSRS_ResultsReport").Value,
                    ExportPath = _configurationSettingManager.Get("ResultsReportsPrintOutJob_ExportPath").Value,
                    ExportFormats = _configurationSettingManager.Get("ResultsReports_ExportFormats").Value.Split(','),
                };

                _electionResultsExporter.Process(printParams);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
