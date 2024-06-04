
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exporters;
using CEC.SRV.BLL.ReportService;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Dto;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Resources;
using NHibernate.Linq;
using NHibernate.Loader.Custom;
using NHibernate.Transform;

namespace CEC.SRV.BLL.Impl
{
    public class PrintBll : Bll, IPrintBll
    {
        public PrintBll(ISRVRepository repository)
            : base(repository)
        {
        }

        public void CreatePrintSession(long electionId, IEnumerable<long> pollingStationIds)
        {
            var election = Get<Election>(electionId);
            var pollingStations = Repository.Query<PollingStation>()
                .Where(x => x.Deleted == null && pollingStationIds.ToList().Contains(x.Id)).ToList();

            var printSession = new PrintSession(election, pollingStations);

            Repository.SaveOrUpdate(printSession);
        }


        public void CreatePrintSession(long electionId, IList<ExportPollingStationDto> exportPollingStationsDto)
        {
            var election = Get<Election>(electionId);

            List<ExportPollingStation> exportPollingStations = new List<ExportPollingStation>();

            foreach(var item in exportPollingStationsDto)
            {
                var ps = Repository.Get<PollingStation>(item.PollingStationId);
                var er = Repository.Get<ElectionRound>(item.ElectionRoundId);
                var circ = Repository.Get<Circumscription>(item.CircumscriptionId);
                if (ps != null && er != null && circ != null)
                {
                    exportPollingStations.Add(new ExportPollingStation { ElectionRound = er, Circumscription = circ, PollingStation = ps, NumberPerElection = item.NumberPerElection });
                }
            }

            var printSession = new PrintSession(election, exportPollingStations);

            Repository.SaveOrUpdate(printSession);
        }


        public void CancelPrintSession(long printSessionId)
        {
            var printSession = Get<PrintSession>(printSessionId);
            printSession.Cancel();

            Repository.SaveOrUpdate(printSession);
        }

        public void ProcessPrintSession(SSRSPrintParameters printParams)
        {
            var pendingPrintSession = Repository.Query<PrintSession>()
                .Where(x => x.Status == PrintStatus.Pending)
                .OrderBy(x=>x.Created)
                .FirstOrDefault();
            if (pendingPrintSession != null)
                ProcessPrintSession(pendingPrintSession, printParams);

        }

		public IList<PrintSession> GetPrintSessionByStatus(PrintStatus status)
		{
			return Repository.Query<PrintSession>().Where(x => x.Status == status).ToList();
		}

		public PageResponse<ExportPollingStation> GetExportPollingStations(PageRequest pageRequest, long? printSessionId)
		{
			if (!printSessionId.HasValue)
			{
				printSessionId = Repository.Query<PrintSession>()
				.Where(x => x.Status == PrintStatus.InProgress)
				.Select(x => x.Id).FirstOrDefault();
			}


			ExportPollingStation exportPollingStation = null;
			PollingStation pollingStation = null;

			return Repository.QueryOver(() => exportPollingStation)
					.JoinAlias(x => x.PollingStation, () => pollingStation)
					.Where(x => x.PrintSession.Id == printSessionId)
					.OrderBy(x=>pollingStation.FullNumber).Asc
					.TransformUsing(Transformers.DistinctRootEntity).RootCriteria.CreatePage<ExportPollingStation>(pageRequest);
		}

		public PageResponse<ExportPollingStation> GetHistoryExportPollingStations(PageRequest pageRequest, long? printSessionId)
		{
			ExportPollingStation exportPollingStation = null;
			PollingStation pollingStation = null;

			return Repository.QueryOver(() => exportPollingStation)
					.JoinAlias(x => x.PollingStation, () => pollingStation)
					.Where(x => x.PrintSession.Id == printSessionId)
					.OrderBy(x => pollingStation.FullNumber).Asc
					.TransformUsing(Transformers.DistinctRootEntity).RootCriteria.CreatePage<ExportPollingStation>(pageRequest);
		}

		public PageResponse<PrintSession> GetExportPrintSessions(PageRequest pageRequest)
		{
			return Repository.QueryOver<PrintSession>()
				.RootCriteria.CreatePage<PrintSession>(pageRequest);
		}
		public PageResponse<SaiseExporter> GetSaiseExporter(PageRequest pageRequest)
		{
			return Repository.QueryOver<SaiseExporter>()
				.RootCriteria.CreatePage<SaiseExporter>(pageRequest);
		}

        public byte[] RequestStayStatementReport(SSRSPrintParameters ssrsParameters, long stayStatementId)
        {
            var reportParameters = new List<ParameterValue>
            {
                new ParameterValue {Name = "stayStatementNumber", Value = stayStatementId.ToString()}
            };

            var reportData = SSRSClient.RequestReportStream(ssrsParameters, reportParameters);
            return reportData.Report;
        }

        public IList<ExportPollingStation> GetExportPollingStationsByPrintSession(long printSessionId)
		{
			return Repository.Query<ExportPollingStation>()
				.Where(x => x.PrintSession.Id == printSessionId).ToList();
        }

        private void ProcessPrintSession(PrintSession printSession, SSRSPrintParameters printParams)
        {
            var election = printSession.Election;

            printSession.Status = PrintStatus.InProgress;
            printSession.StartDate = DateTimeOffset.Now;
            Repository.SaveOrUpdate(printSession);
            var sessionFolder = printSession.StartDate.Value.LocalDateTime.ToString("yyyy-MM-dd_HH-mm-ss");
            var sessionPath = Path.Combine(printParams.ExportPath, sessionFolder);

            Directory.CreateDirectory(sessionPath);
            foreach (var pollingStation in printSession.ExportPollingStations)
            {
                if (pollingStation.Status == PrintStatus.Pending)
                {
                    var circumscription = pollingStation.PollingStation.Region.GetCircumscription();
                    var region =
                        Repository.Query<Region>()
                            .FirstOrDefault(x => x.Deleted == null && x.Circumscription == circumscription);
                    var circumscriptionFolder = string.Format("{0}_{1}", circumscription, region != null ? region.GetFullName() : string.Empty);

                    pollingStation.Status = PrintStatus.InProgress;
                    pollingStation.StartDate = DateTimeOffset.Now;
                    Repository.SaveOrUpdate(pollingStation);
                    try
                    {
                        var data = RequestReportStream(printParams, election, pollingStation.PollingStation);
                        var fileName = String.Format("Sect_{0}_data_{1}.{2}",
                            pollingStation.PollingStation.Number, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), data.FileExtension);
                        
                        var destinationPath = Path.Combine(sessionPath, circumscriptionFolder);
                        if (!Directory.Exists(destinationPath))
                            Directory.CreateDirectory(destinationPath);
                        SaveReportStreamToFile(data.Report, destinationPath, fileName);
                    }
                    catch (Exception ex)
                    {
                        pollingStation.Status = PrintStatus.Failed;
                        pollingStation.StatusMessage = ex.ToString();
                        pollingStation.EndDate = DateTimeOffset.Now;
                        Repository.SaveOrUpdate(pollingStation);
                        printSession.Status = PrintStatus.Failed;
                        printSession.EndDate = DateTimeOffset.Now;
                        Repository.SaveOrUpdate(printSession);
                        return;
                    }
                    pollingStation.Status = PrintStatus.Finished;
                    pollingStation.EndDate = DateTimeOffset.Now;
                    Repository.SaveOrUpdate(pollingStation);
                }
                else if (pollingStation.Status == PrintStatus.Canceled)
                {
                    printSession.Status = PrintStatus.Canceled;
                    printSession.EndDate = DateTimeOffset.Now;
                    Repository.SaveOrUpdate(printSession);
                    return;
                }
            }
            printSession.Status = PrintStatus.Finished;
            printSession.EndDate = DateTimeOffset.Now;
            Repository.SaveOrUpdate(printSession);
        }

        private void SaveReportStreamToFile(byte[] data, string exportPath, string fileName)
        {
            using (var fileStream = new FileStream(Path.Combine(exportPath, fileName), FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(data, 0, data.Length);
            }
        }

        private ReportData RequestReportStream(SSRSPrintParameters printParams, Election election,
            PollingStation pollingStation)
        {
            const string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";
            string encoding;
            string mimeType;
            string extension;
            Warning[] warnings = null;
            string[] streamIDs = null;

            var ssrs = new ReportExecutionService();
            ssrs.Credentials = printParams.GetCredentials();
            ssrs.Url = printParams.ServerUrl;

            var reportParams = new List<ParameterValue>
            {
                new ParameterValue {Name = "pollingStationId", Value = pollingStation.Id.ToString()},
                new ParameterValue {Name = "electionTitleRO", Value = election.ElectionType.Name},
                new ParameterValue {Name = "electionTitleRU", Value = election.ElectionType.Name},
                new ParameterValue {Name = "pollingStationNr", Value = pollingStation.FullNumber},
                new ParameterValue {Name = "electionDate", Value = election.ElectionRounds.LastOrDefault().ElectionDate.Date.ToString("MM/dd/yyyy")},
                new ParameterValue
                {
                    Name = "electionDateAsText",
                    Value = election.ElectionRounds.LastOrDefault().ElectionDate.Date.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("ro-RO"))
                },
                new ParameterValue {Name = "regionName", Value = pollingStation.Region.GetFullName()},
                new ParameterValue {Name = "managerTypeName", Value = MUI.MissingData},
                new ParameterValue {Name = "managerName", Value = MUI.MissingData},
            };

            var execHeader = new ExecutionHeader();
            ssrs.ExecutionHeaderValue = execHeader;

            ssrs.LoadReport(printParams.ReportName, null);
            ssrs.SetExecutionParameters(reportParams.ToArray(), "en-us");

            var result = ssrs.Render("PDF", devInfo, out extension, out encoding, 
                out mimeType, out warnings, out streamIDs);

            return new ReportData {Report = result, FileExtension = "pdf"};
        }
    }
}