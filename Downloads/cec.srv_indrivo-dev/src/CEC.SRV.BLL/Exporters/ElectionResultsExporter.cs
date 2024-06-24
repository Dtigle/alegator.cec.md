using Amdaris;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.BLL.ReportService;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.Print;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Exporters
{
    public class ElectionResultsExporter
    {
        public JobProgress Progress { get; set; }
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        object lockObj = new object();

        public ElectionResultsExporter(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void Process(ElectionResultsPrintingParameters printParams)
        {
            while (true)
            {
                PrintSession pendingPrintSession;
                List<ElectionResultPrint> ElectionResultPrintData = new List<ElectionResultPrint>();
                using (var uow = new NhUnitOfWork())
                {
                    pendingPrintSession = _repository.Query<PrintSession>()
                        .Fetch(x => x.Election)
                        .ThenFetch(x => x.ElectionType)
                        .Where(x => x.Status == PrintStatus.Pending)
                        .OrderBy(x => x.Created)
                        .FirstOrDefault();

                    if (pendingPrintSession == null)
                    {
                        return;
                    }

                    var ps = _repository.Query<ExportPollingStation>()
                    .Fetch(x => x.ElectionRound)
                    .Fetch(x => x.Circumscription)
                    .Fetch(x => x.PollingStation).ThenFetch(s => s.Region)
                    .Where(x => x.Status == PrintStatus.Pending && x.PrintSession.Id == pendingPrintSession.Id)
                    .OrderBy(x => x.Created);

                    foreach (var x in ps)
                    {
                        var ElectionResultPrint = new ElectionResultPrint
                        {
                            Id = x.Id,
                            ElectionId = x.PrintSession.Election.Id,
                            ElectionRoundId = x.ElectionRound.Id,
                            CircumscriptionId = x.Circumscription.Id,
                            ExportPollingStation = x,
                            PollingStationId = x.PollingStation.Id,
                            RegionId = x.PollingStation.Region.Id,
                            NumberPerElection = x.PollingStation.Number,
                            CircumscriptionFolder = x.Circumscription.Id.ToString(),
                            ElectionFolder = x.PrintSession.Election.Id.ToString(),
                            ElectionRoundFolder = x.ElectionRound.Id.ToString(),
                            PrintSession = x.PrintSession,
                        };

                        ElectionResultPrintData.Add(ElectionResultPrint);
                    }

                    uow.Complete();
                }

                ProcessPrintSession(pendingPrintSession, ElectionResultPrintData, printParams);
            }
        }

        private void ProcessPrintSession(PrintSession printSession, IEnumerable<ElectionResultPrint> electionResultPrintData, ElectionResultsPrintingParameters printParams)
        {
            var election = printSession.Election;

            printSession.Status = PrintStatus.InProgress;
            printSession.StartDate = DateTimeOffset.Now;
            UpdateDbStatus(printSession);
            var sessionFolder = printSession.StartDate.Value.LocalDateTime.ToString("yyyy-MM-dd_HH-mm-ss");
            var sessionPath = Path.Combine(printParams.ExportPath, sessionFolder);

            Directory.CreateDirectory(sessionPath);
            string connectionStr = string.Empty;
            using (var uow = new NhUnitOfWork())
            {
                ISession session = _repository.GetSession();
                connectionStr = session.Connection.ConnectionString;
                uow.Complete();
            }

            var processedCircumscriptions = new List<ElectionResultPrint>();
            var processedRegions = new List<ElectionResultPrint>();

            var progressInfo = Progress.CreatStageProgressInfo(sessionPath, 0, electionResultPrintData.Count());
            Parallel.ForEach(electionResultPrintData, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (eps) =>
              {
                  if (eps.ExportPollingStation.Status == PrintStatus.Pending)
                  {
                      eps.ExportPollingStation.Status = PrintStatus.InProgress;
                      eps.ExportPollingStation.StartDate = DateTimeOffset.Now;
                      UpdateDbStatus(eps.ExportPollingStation);
                      try
                      {

                          if (!processedCircumscriptions.Any(x => x.ElectionRoundId == eps.ElectionRoundId && x.CircumscriptionId == eps.CircumscriptionId))
                          {
                              foreach (var extension in printParams.ExportFormats)
                              {
                                  var circEr = new ElectionResultPrint
                                  {
                                      ElectionId = eps.ElectionId,
                                      ElectionRoundId = eps.ElectionRoundId,
                                      CircumscriptionId = eps.CircumscriptionId,
                                      PollingStationId = 0
                                  };
                                  processedCircumscriptions.Add(circEr);
                                  var data = RequestReportStream(printParams, election, circEr, extension.ToUpper());
                                  string fileName;
                                  string destinationPath;
                                  fileName = String.Format("{0}.{1}", eps.CircumscriptionId, data.FileExtension);
                                  destinationPath = Path.Combine(sessionPath, eps.ElectionFolder, eps.ElectionRoundFolder, eps.CircumscriptionFolder);
                                  if (!Directory.Exists(destinationPath))
                                  {
                                      Directory.CreateDirectory(destinationPath);
                                  }
                                  SaveReportStreamToFile(data.Report, destinationPath, fileName);
                              }
                          }


                          if (!processedRegions.Any(x => x.ElectionRoundId == eps.ElectionRoundId && x.CircumscriptionId == eps.CircumscriptionId && x.RegionId == eps.RegionId))
                          {
                              foreach (var extension in printParams.ExportFormats)
                              {
                                  var regEr = new ElectionResultPrint
                                  {
                                      ElectionId = eps.ElectionId,
                                      ElectionRoundId = eps.ElectionRoundId,
                                      CircumscriptionId = eps.CircumscriptionId,
                                      RegionId = eps.RegionId,
                                      PollingStationId = 0
                                  };
                                  processedRegions.Add(regEr);
                                  var data = RequestReportStream(printParams, election, regEr, extension.ToUpper());
                                  string fileName;
                                  string destinationPath;
                                  fileName = String.Format("{0}.{1}", eps.RegionId, data.FileExtension);
                                  destinationPath = Path.Combine(sessionPath, eps.ElectionFolder, eps.ElectionRoundFolder, eps.CircumscriptionFolder, "regions", eps.RegionId.ToString());
                                  if (!Directory.Exists(destinationPath))
                                  {
                                      Directory.CreateDirectory(destinationPath);
                                  }
                                  SaveReportStreamToFile(data.Report, destinationPath, fileName);
                              }
                          }

                          foreach (var extension in printParams.ExportFormats)
                          {
                              var data = RequestReportStream(printParams, election, eps, extension.ToUpper());
                              string fileName;
                              string destinationPath;
                              fileName = String.Format("{0}.{1}", eps.PollingStationId, data.FileExtension);
                              destinationPath = Path.Combine(sessionPath, eps.ElectionFolder, eps.ElectionRoundFolder, eps.CircumscriptionFolder, "regions", eps.RegionId.ToString(), "pollingStation");
                              if (!Directory.Exists(destinationPath))
                              {
                                  Directory.CreateDirectory(destinationPath);
                              }
                              SaveReportStreamToFile(data.Report, destinationPath, fileName);
                          }
                      }
                      catch (Exception ex)
                      {
                          _logger.Error(ex);
                          eps.ExportPollingStation.Status = PrintStatus.Failed;
                          eps.ExportPollingStation.StatusMessage = ex.ToString().Truncate(255);
                          eps.ExportPollingStation.EndDate = DateTimeOffset.Now;
                          UpdateDbStatus(eps.ExportPollingStation);
                          printSession.Status = PrintStatus.Failed;
                          printSession.EndDate = DateTimeOffset.Now;
                          UpdateDbStatus(printSession);
                          return;
                      }


                      eps.ExportPollingStation.Status = PrintStatus.Finished;
                      eps.ExportPollingStation.EndDate = DateTimeOffset.Now;
                      UpdateDbStatus(eps.ExportPollingStation);
                  }
                  else if (eps.ExportPollingStation.Status == PrintStatus.Canceled)
                  {
                      printSession.Status = PrintStatus.Canceled;
                      printSession.EndDate = DateTimeOffset.Now;
                      UpdateDbStatus(printSession);
                      return;
                  }

                  progressInfo.Increase();
              });
            printSession.Status = PrintStatus.Finished;
            printSession.EndDate = DateTimeOffset.Now;
            UpdateDbStatus(printSession);
        }

        private string GetCircumscriptionDirectoryName(int? circumscription)
        {
            var region = _repository.Query<Region>()
                        .FirstOrDefault(x => x.Deleted == null && x.Circumscription == circumscription);
            return string.Format("{0}_{1}", circumscription,
                region != null ? region.Name : string.Empty);
        }

        private void UpdateDbStatus(PrintEntity printEntity)
        {
            lock (lockObj)
            {
                using (var uow = new NhUnitOfWork())
                {
                    _repository.SaveOrUpdate(printEntity);

                    uow.Complete();
                }
            }
        }

        private void SaveReportStreamToFile(byte[] data, string exportPath, string fileName)
        {
            using (var fileStream = new FileStream(Path.Combine(exportPath, fileName), FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(data, 0, data.Length);
            }
        }

        private ReportData RequestReportStream(ElectionResultsPrintingParameters printParams, Election election, ElectionResultPrint ElectionResultPrintData, string exportFormat)
        {
            var reportParams = new List<ParameterValue>
            {
                new ParameterValue {Name = "ElectionId", Value = ElectionResultPrintData.ElectionId.ToString()},
                new ParameterValue {Name = "ElectionRoundId", Value = ElectionResultPrintData.ElectionRoundId.ToString()},
                new ParameterValue {Name = "CircumscriptionId", Value = ElectionResultPrintData.CircumscriptionId.ToString()},
                new ParameterValue {Name = "RegionId", Value = ElectionResultPrintData.RegionId.ToString()},
                new ParameterValue {Name = "PollingStationId", Value = ElectionResultPrintData.PollingStationId.ToString()},
                //new ParameterValue {Name = "ElectionDate", Value = ""}
            };

            string reportUrl = printParams.ReportName;


            return SSRSClient.RequestReportStream(printParams.ServerUrl, reportUrl, printParams.GetCredentials(), reportParams, exportFormat);
        }
    }

    internal class ElectionResultPrint
    {
        public long Id { get; set; }

        public PrintSession PrintSession { get; set; }

        public long ElectionId { get; set; }

        public long ElectionRoundId { get; set; }

        public long CircumscriptionId { get; set; }

        public long RegionId { get; set; }

        public long PollingStationId { get; set; }

        public string NumberPerElection { get; set; }

        public string CircumscriptionFolder { get; set; }

        public string ElectionFolder { get; set; }

        public string ElectionRoundFolder { get; set; }

        public ExportPollingStation ExportPollingStation { get; set; }
    }
}
