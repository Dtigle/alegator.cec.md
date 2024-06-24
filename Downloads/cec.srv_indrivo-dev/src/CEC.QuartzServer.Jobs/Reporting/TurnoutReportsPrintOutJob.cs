using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Export;
using CEC.QuartzServer.Jobs.Import;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.BLL.ReportService;
using Quartz;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.Reporting
{
    [DisallowConcurrentExecution]
    public class TurnoutReportsPrintOutJob : PrintOutBaseJob, IInterruptableJob
    {
        private readonly IConfigurationSettingManager _configurationSettingManager;
        private readonly string _turnoutReportsPath;
        private readonly string[] _turnoutFixationTimes;
        private readonly long _turnoutElectionId;
        private bool _interruptPending;

        private readonly Dictionary<TurnoutReportsKey, string> _turnoutReports =
            new Dictionary<TurnoutReportsKey, string>
            {
                {TurnoutReportsKey.Districts, "TurnoutStats_ExtWeb_Districts"},
                {TurnoutReportsKey.Villages, "TurnoutStats_ExtWeb_Villages"},
                {TurnoutReportsKey.PStations, "TurnoutStats_ExtWeb_PStations"},
            };

        private ProgressInfo _stageProgressInfo;

        enum TurnoutReportsKey
        {
            Districts,
            Villages,
            PStations
        }

        public TurnoutReportsPrintOutJob(ILogger logger, IConfigurationSettingManager configurationSettingManager)
            : base(logger, configurationSettingManager)
        {
            _configurationSettingManager = configurationSettingManager;
            _turnoutElectionId = configurationSettingManager.Get("TurnoutFixationJob_ElectionId").GetValue<long>(); 
            _turnoutFixationTimes = configurationSettingManager.Get("TurnoutFixationJob_TurnoutControlTimes").Value
                    .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            ReportPageHeight = configurationSettingManager.Get("TurnoutReportsPrintOutJob_ReportPageHeight").Value;
            _turnoutReportsPath = configurationSettingManager.Get("SSRS_TurnoutReportsPath").Value;
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            var startTime = DateTime.Now;
            Logger.Trace("TurnoutReportsPrintOutJob started.");
            try
            {
                var election = new SaiseRepository().GetElection(_turnoutElectionId);

                if (election != null)
                {
                    DateTime timeOfEntry;

                    var configurationSetting = _configurationSettingManager.Get("TurnoutFixationJob_LastControlTimes");

                    TimeSpan lastControlTime;
                    if (string.IsNullOrWhiteSpace(configurationSetting.Value))
                    {
                        lastControlTime = TimeSpan.Parse(_turnoutFixationTimes.First());
                    }
                    else
                    {
                        var xx =_turnoutFixationTimes.SkipWhile(x => x != configurationSetting.Value).ElementAtOrDefault(1);
                        if (xx == null)
                        {
                            Logger.Trace("Nothing to export. No control time found.");
                            return;
                        }
                        lastControlTime = TimeSpan.Parse(xx);
                    }

                    if (lastControlTime == TimeSpan.MinValue)
                    {
                        timeOfEntry = election.DateOfElection.Add(TimeSpan.FromHours(7));
                    }
                    else
                    {
                        timeOfEntry = election.DateOfElection.Add(lastControlTime);
                    }

                    var copyAffectedRecords = new SaiseRepository().CopyElectionTurnout(election.Id, timeOfEntry);
                    Logger.Trace(string.Format("{0} affected by CopyElectionTurnout", copyAffectedRecords));
                    var fixationAffectedRecords = new SaiseRepository().FixElectionTurnout(election.Id);
                    Logger.Trace(string.Format("{0} affected by FixElectionTurnout", fixationAffectedRecords));

                    _stageProgressInfo = this.Progress.CreatStageProgressInfo(string.Format("Exporting Turnouts for PollingStations (reference time {0})", timeOfEntry), 0, copyAffectedRecords);
                    _stageProgressInfo.OnProgress = () => { Logger.Trace(string.Format("Processed {0} items out of {1}.", _stageProgressInfo.Value, copyAffectedRecords));};

                    var electionDateDirectoryName = string.Format("prezenta_{0}",
                        election.DateOfElection.ToString("yyyy-MM-dd"));
                    var subDir = GetOrCreateDirectory(RootExportDirectory, electionDateDirectoryName);

                    ProcessElection(subDir, election, timeOfEntry);

                    configurationSetting.Value = timeOfEntry.TimeOfDay.ToString(@"hh\:mm");
                    _configurationSettingManager.Update(configurationSetting);
                }
                else
                {
                    Logger.Trace(string.Format("Nothing to export. No election exists for ElectionId {0}", _turnoutElectionId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "TurnoutReportsPrintOutJob failed.");
            }

            var endTime = DateTime.Now;
            var ellapsedTime = endTime - startTime;
            Logger.Trace(string.Format("Export ellapsed time: {0}", ellapsedTime));
            Logger.Trace("TurnoutReportsPrintOutJob terminated.");
        }

        private string GetTimeOfEntryToExport(DirectoryInfo subDir)
        {
            foreach (var controlTime in _turnoutFixationTimes)
            {
                if (!subDir.GetDirectories(FixTimeOfEntry(controlTime), SearchOption.TopDirectoryOnly).Any())
                {
                    return controlTime;
                }
            }

            throw new Exception("No control time found or all exported.");
        }

        private DirectoryInfo GetOrCreateDirectory(DirectoryInfo rootDir, string subDirName)
        {
            var subDir = rootDir
                .GetDirectories(subDirName, SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            if (subDir == null)
            {
                subDir = rootDir.CreateSubdirectory(subDirName);
            }

            return subDir;
        }

        private void ProcessElection(DirectoryInfo subDir, SaiseElection election, DateTime timeOfEntry)
        {
            var electionDir = GetOrCreateDirectory(subDir, election.Id.ToString());

            var timeOfEntry1 = timeOfEntry.TimeOfDay.ToString(@"hh\:mm");
            var timeOfEntryDir = GetOrCreateDirectory(electionDir, FixTimeOfEntry(timeOfEntry1));

            ExportTurnoutReports(election, timeOfEntryDir, timeOfEntry1);
        }

        private void ExportTurnoutReports(SaiseElection election, DirectoryInfo timeOfEntryDir, string timeOfEntry)
        {
            WriteOutElectionAndDistrictList(timeOfEntryDir, election);

            // exportam raportul de prezenta pe intreaga tara
            ParameterValue electionParam = new ParameterValue { Name = "ElectionId", Value = election.Id.ToString() };
            ParameterValue timeOfEntryParam = new ParameterValue
            {
                Name = "TimeOfEntry",
                Value = timeOfEntry == "07:00" ? "Open" : timeOfEntry
            };
            RequestAndSaveReport(timeOfEntryDir, 
                string.Format("{0}_moldova", FixTimeOfEntry(timeOfEntry)),
                TurnoutReportsKey.Districts,
                electionParam, timeOfEntryParam);

            var districts = new SaiseRepository().GetRegionsForElection(election.Id);
            Parallel.ForEach(districts, new ParallelOptions{MaxDegreeOfParallelism = 2}, (district) =>
            //foreach (var district in districts)
            {
                if (_interruptPending)
                {
                    return;
                }

                //if (!IgnorableDistrictNumbers.Contains(district.Name))
                {
                    DirectoryInfo districtSubDir = timeOfEntryDir.CreateSubdirectory(district.Id.ToString());

                    var villages = new SaiseRepository().GetAllRegions(district.Id, election.Id).ToList();
                    XDocument xvillages = CreateXDocForSiteExport(villages,
                        (SaiseRegion v, out string o1, out string o2) =>
                        {
                            o1 = v.Id.ToString();
                            o2 = v.Name;
                        });
                    WriteToFile(districtSubDir.FullName, "localitati.xml", xvillages);


                    ParameterValue districtParam = new ParameterValue { Name = "DistrictId", Value = district.Id.ToString() };
                    RequestAndSaveReport(districtSubDir, district.Id.ToString(),
                        TurnoutReportsKey.Villages, 
                        timeOfEntryParam, districtParam, electionParam,
                        new ParameterValue { Name = "PollingStationItemLinkTemplate", Value = "''" });

                    Parallel.ForEach(villages, new ParallelOptions{MaxDegreeOfParallelism = 2}, (village) =>
                    //foreach (var village in villages)
                    {
                        if (_interruptPending)
                        {
                            return;
                        }
                        // exportam rapoartele de prezenta pentru fiecare localitate din circumscriptie
                        RequestAndSaveReport(districtSubDir, village.Id.ToString(),
                            TurnoutReportsKey.PStations, timeOfEntryParam, electionParam, districtParam,
                            new ParameterValue { Name = "VillageId", Value = village.Id.ToString() },
                            new ParameterValue { Name = "RootLink", Value = "''" },
                            new ParameterValue { Name = "ParentDistrictLinkTemplate", Value = "''" });
                    });
                }
            });
        }

        private void WriteOutElectionAndDistrictList(DirectoryInfo di, SaiseElection election)
        {
            XDocument xalegeri = CreateXDocForSiteExport(new[]{election},
                (SaiseElection e, out string a, out string b) =>
                {
                    a = e.Id.ToString();
                    b = "";
                });

            var districts = new SaiseRepository().GetRegionsForElection(election.Id).ToList();
            XDocument xdistricts = CreateXDocForSiteExport(districts,
                (SaiseRegion d, out string a, out string b) =>
                {
                    a = d.Id.ToString();
                    b = d.Name;
                });

            WriteToFile(di.FullName, "alegeri.xml", xalegeri);
            WriteToFile(di.FullName, "circumscriptii.xml", xdistricts);
        }

        private void RequestAndSaveReport(DirectoryInfo di, string fileName, TurnoutReportsKey reportKey, params ParameterValue[] reportParams)
        {
            var reportUrl = new UriBuilder(ReportServerUri);
            var requestUrlTemplate = "{0}&rs:Command=Render&rs:format=IMAGE&rc:OutputFormat=PNG" +
                                     "&rc:PageHeight={1}" +
                                     "&LanguageId=1";

            List<string> paramsList = new List<string>();
            foreach (ParameterValue pv in reportParams)
            {
                paramsList.Add(string.Format("{0}={1}", pv.Name, string.IsNullOrEmpty(pv.Value) ? "''" : pv.Value));
            }

            var stringOfParams = string.Join("&", paramsList);

            requestUrlTemplate = string.Format(requestUrlTemplate, GetReportPath(reportKey), ReportPageHeight);

            if (!string.IsNullOrEmpty(stringOfParams))
            {
                reportUrl.Query = requestUrlTemplate + "&" + stringOfParams;
            }

            HttpWebRequest request = WebRequest.Create(reportUrl.Uri) as HttpWebRequest;
            request.Timeout = 180000;
            request.Credentials = GetCredentials();
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                using (var fs = new FileStream(string.Format("{0}\\ro{1}.png", di.FullName, fileName),
                        FileMode.Create, FileAccess.Write))
                {
                    response.GetResponseStream().CopyTo(fs);
                }
            }
            catch (WebException wex)
            {
                LogRequestException(reportUrl.Uri, wex);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("Request URL was {0}", reportUrl.Uri));
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            _stageProgressInfo.Increase();
        }

        private string GetReportPath(TurnoutReportsKey reportKey)
        {
            var reportName = _turnoutReports[reportKey];
            return _turnoutReportsPath + reportName;
        }

        private string FixTimeOfEntry(string timeOfEntry)
        {
            return timeOfEntry == "Open" ? "07h00" : timeOfEntry.Replace(':', 'h');
        }

        public void Interrupt()
        {
            _interruptPending = true;
        }
    }
}
