using System;
using Amdaris;
using Amdaris.Domain;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exporters;
using Quartz;

namespace CEC.QuartzServer.Jobs.ListsPrinting
{
    [DisallowConcurrentExecution]
    public class ListsPrintingJob : SrvJob
    {
        private readonly PrePrintListsExporter _listsExporter;
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;

        public ListsPrintingJob(IRepository srvRepository, ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;
            _listsExporter = new PrePrintListsExporter(srvRepository, _logger);
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            _listsExporter.Progress = Progress;
            try
            {
                var printParams = new VotersListPrintingParameters
                                  {
                                      ServerUrl = _configurationSettingManager.Get("SSRS_ReportExecutionService").Value,
                                      UserName = _configurationSettingManager.Get("SSRS_UserName").Value,
                                      Password = _configurationSettingManager.Get("SSRS_Password").Value,
                                      ReportName = _configurationSettingManager.Get("SSRS_VoterReport").Value,
                                      AbroadListReportName = _configurationSettingManager.Get("SSRS_AbroadVoterReport").Value,
                                      LocalElectionsListReportName = _configurationSettingManager.Get("SSRS_LocalElectionsReport").Value,
                                      ExportPath = _configurationSettingManager.Get("ListPrintingJob_ExportPath").Value,
                                      ExportFormat = _configurationSettingManager.Get("SSRS_ExportFormat").Value,
                                      WebPageVotersListEnable = _configurationSettingManager.Get("WebPageVotersListEnable").Value
                                  };

                _listsExporter.Process(printParams);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}