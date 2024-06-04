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
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Exporters
{
    public class PrePrintListsExporter
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        object xxx = new object();

        public PrePrintListsExporter(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public JobProgress Progress { get; set; }

        public void Process(VotersListPrintingParameters printParams)
        {
            while (true)
            {
                PrintSession pendingPrintSession;
                List<PSPrint> psPrintData = new List<PSPrint>();
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
                    .Where(x => x.Status == PrintStatus.Pending && x.PrintSession.Id == pendingPrintSession.Id)
                    .OrderBy(x => x.Created);

                    foreach (var x in ps)
                    {
                        var psprint = new PSPrint
                        {
                            Id = x.Id,
                            ExportPollingStation = x,
                            PollingStationId = x.PollingStation.Id,
                            Number = x.PollingStation.Number,
                            NumberPerElection = x.NumberPerElection,
                            FullNumber = string.Format("{0}/{1}", x.Circumscription.Number, x.NumberPerElection),
                            RegionFullName = string.Format("{0}", x.Circumscription.NameRo, x.Circumscription.Number),
                            CircumscriptionFolder = x.Circumscription.NameRo.Replace(@"\", "-").Replace(@"/", "-").Replace(@" ", "_").Replace(@".", "").Replace(@",", "").Normalize(),
                            //GetCircumscriptionDirectoryName(x.Circumscription.NameRo),
                            PrintSession = x.PrintSession,
                            RegionId = x.Circumscription.Region.Id.ToString()
                        };

                        psPrintData.Add(psprint);
                    }

                    uow.Complete();
                }

                ProcessPrintSession(pendingPrintSession, psPrintData, printParams);
            }
        }

        private void ProcessPrintSession(PrintSession printSession, IEnumerable<PSPrint> psPrintData, VotersListPrintingParameters printParams)
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

            var progressInfo = Progress.CreatStageProgressInfo(sessionPath, 0, psPrintData.Count());
            Parallel.ForEach(psPrintData, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (eps) =>
              {
                  if (eps.ExportPollingStation.Status == PrintStatus.Pending)
                  {
                      eps.ExportPollingStation.Status = PrintStatus.InProgress;
                      eps.ExportPollingStation.StartDate = DateTimeOffset.Now;
                      UpdateDbStatus(eps.ExportPollingStation);
                      try
                      {
                          using (SqlConnection cn = new SqlConnection(connectionStr))
                          {
                              using (SqlCommand cmd = new SqlCommand("[SRV].[sp_SetVoterList]", cn))
                              {
                                  cmd.CommandTimeout = 10000;
                                  cmd.CommandType = CommandType.StoredProcedure;
                                  cmd.Parameters.Add(new SqlParameter("@pollingStationId", SqlDbType.BigInt)
                                  {
                                      Value = eps.ExportPollingStation.PollingStation.Id,
                                      Direction = ParameterDirection.Input
                                  });

                                  cmd.Parameters.Add(new SqlParameter("@electionDate", SqlDbType.DateTime2)
                                  {
                                      Value = eps.ExportPollingStation.ElectionRound.ElectionDate,
                                      Direction = ParameterDirection.Input
                                  });

                                  cn.Open();
                                  cmd.ExecuteNonQuery();
                                  cn.Close();
                              }
                          }

                          var data = RequestReportStream(printParams, election, eps);


                          string fileName;
                          string destinationPath;
                          if (printParams.WebPageVotersListEnable != "1")
                          {
                              fileName = String.Format("Sect_{0}_data_{1}.{2}", eps.NumberPerElection.Replace(@"\", "-").Replace(@"/", "-"),
                                  DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                                  data.FileExtension);

                              destinationPath = Path.Combine(sessionPath, eps.CircumscriptionFolder);
                          }
                          else
                          {
                              fileName = $"{eps.PollingStationId.ToString()}.{data.FileExtension}";
                              destinationPath = Path.Combine(sessionPath, eps.RegionId);
                          }

                          if (!Directory.Exists(destinationPath))
                              Directory.CreateDirectory(destinationPath);
                          SaveReportStreamToFile(data.Report, destinationPath, fileName);
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
            lock (xxx)
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

        private ReportData RequestReportStream(VotersListPrintingParameters printParams, Election election, PSPrint psPrintData)
        {
            var reportParams = new List<ParameterValue>
            {
                new ParameterValue {Name = "pollingStationId", Value = psPrintData.PollingStationId.ToString()},
                new ParameterValue {Name = "electionTitleRO", Value = election.ElectionType.Name},
                new ParameterValue {Name = "pollingStationNr", Value = psPrintData.FullNumber},
                new ParameterValue {Name = "electionDate", Value = psPrintData.ExportPollingStation.ElectionRound.ElectionDate.Date.ToString("yyyy-MM-dd")},
                new ParameterValue
                {
                    Name = "electionDateAsText",
                    Value = psPrintData.ExportPollingStation.ElectionRound.ElectionDate.Date.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("ro-RO"))
                },
                new ParameterValue {Name = "regionName", Value = psPrintData.RegionFullName},
            };

            string reportUrl;
            if (!election.ElectionType.IsLocal())
            {
                reportUrl = psPrintData.ExportPollingStation.PollingStation.PollingStationType ==
                            PollingStationTypes.Normal
                    ? printParams.ReportName
                    : printParams.AbroadListReportName;
            }
            else
            {
                reportUrl = printParams.LocalElectionsListReportName;
            }

            return SSRSClient.RequestReportStream(printParams.ServerUrl, reportUrl, printParams.GetCredentials(), reportParams, printParams.ExportFormat);
        }
    }

    internal class PSPrint
    {
        public PrintSession PrintSession { get; set; }

        public long PollingStationId { get; set; }

        public long Id { get; set; }

        public string Number { get; set; }

        public string FullNumber { get; set; }

        public string RegionFullName { get; set; }

        public string CircumscriptionFolder { get; set; }

        public ExportPollingStation ExportPollingStation { get; set; }

        public string NumberPerElection { get; set; }
        public string RegionId { get; set; }
    }
}
