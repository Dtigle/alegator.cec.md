using Amdaris;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Web;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.EDayExport;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Synchronizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using SAISE.Admin.WebApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace CEC.Web.SRV.Controllers
{
    [NHibernateSession]
    public class ClassifierSyncController : ApiController
    {

        [Authorize]
        // POST: api/ClassifierSync
        public object Post([FromBody]SyncItem entity)
        {
            var sessionFactory = IoC.GetSessionFactory();

            switch (entity.Type)
            {
                case "RegionTypes":
                    SyncHelper.CreateUpdateDelete<RegionType>(entity);
                    break;
                case "Regions":
                    SyncHelper.CreateUpdateDelete<Region>(entity);
                    break;
                case "Streets":
                    SyncHelper.CreateUpdateDelete<Street>(entity);
                    break;
                case "StreetTypes":
                    SyncHelper.CreateUpdateDelete<StreetType>(entity);
                    break;
                case "Genders":
                    SyncHelper.CreateUpdateDelete<Gender>(entity);
                    break;
                case "DocumentTypes":
                    SyncHelper.CreateUpdateDelete<DocumentType>(entity);
                    break;
                case "PersonStatusTypes":
                    SyncHelper.CreateUpdateDelete<PersonStatusType>(entity);
                    break;
                case "PersonAddressTypes":
                    SyncHelper.CreateUpdateDelete<PersonAddressType>(entity);
                    break;
                case "PollingStations":
                    SyncHelper.CreateUpdateDelete<PollingStation>(entity);
                    break;
                case "ElectionTypes":
                    SyncHelper.CreateUpdateDelete<ElectionType>(entity);
                    break;
                case "Circumscriptions":
                    SyncHelper.CreateUpdateDelete<Circumscription>(entity);
                    break;
                case "CircumscriptionList":
                    SyncHelper.CreateUpdateDelete<CircumscriptionList>(entity);
                    break;
                case "Elections":
                    SyncHelper.CreateUpdateDelete<Election>(entity);
                    if (entity.Model != null && entity.Model.ContainsKey("ElectionRounds"))
                    {
                        var electionRounds = new List<ElectionRound>();
                        var electionRoundsJ = ((JArray)entity.Model["ElectionRounds"]).ToList();
                        if (electionRoundsJ != null && electionRoundsJ.Count > 0)
                        {
                            foreach (var electionRoundJ in electionRoundsJ)
                            {
                                var electionRound = electionRoundJ.ToObject<Dictionary<string, object>>();
                                SyncItem electionRoundSync = new SyncItem
                                {
                                    Id = (long)electionRound["Id"],
                                    Model = electionRound,
                                    Operation = entity.Operation,
                                    Type = "ElectionRounds"
                                };
                                SyncHelper.CreateUpdateDelete<ElectionRound>(electionRoundSync);
                            }
                        }
                    }
                    break;
                case "ElectionRounds":
                    SyncHelper.CreateUpdateDelete<ElectionRound>(entity);
                    break;
                case "MigrateSRVData":

                    var s = ConfigurationManager.ConnectionStrings["EDayConnectionString"];
                    var electionDate = ((DateTime)entity.Model["ElectionDate"]);

                    bool exportAllVoters = true;

                    if (entity.Model != null && entity.Model.ContainsKey("ExportAllVoters"))
                    {
                        exportAllVoters = (bool)entity.Model["ExportAllVoters"];
                    }

                    var _saiseRepository = new SaiseRepository(string.Format("{0};Initial Catalog=SAISE.ElectionDay{1}{2}{3};",
                        s,
                        electionDate.Year,
                        electionDate.Month.ToString("00"),
                        electionDate.Day.ToString("00")));

                    ProgressInfo stageProgress = new ProgressInfo("Migrating Voters data SRV -> SAISE", 0, 0);
                    var _logger = IoC.Resolve<ILogger>();


                    if (sessionFactory.GetCurrentSession() == null)
                    {
                        Lazy<ISession> session = new Lazy<ISession>(() => sessionFactory.OpenSession());
                        LazySessionContext.Bind(session, sessionFactory);
                    }

                    var _session = sessionFactory.OpenStatelessSession();

                    var repository = new SrvRepository(sessionFactory);


                    ExporterBll _exporterBll = new ExporterBll(repository);

                    var existingExporter = _exporterBll.GetActiveSaiseExporter();

                    using (var transaction = sessionFactory.GetCurrentSession().BeginTransaction(IsolationLevel.RepeatableRead))
                    {

                        if (existingExporter != null)
                        {
                            transaction.Begin();
                            existingExporter.Status = SaiseExporterStatus.Cancelled;
                            _exporterBll.SaveOrUpdate(existingExporter);
                            sessionFactory.GetCurrentSession().Flush();
                            transaction.Commit();
                        }
                    }

                    _exporterBll.CreateSaiseExporter(entity.Id, exportAllVoters);

                    var saiseExporter = _session.QueryOver<SaiseExporter>()
                    .Fetch(x => x.Stages).Lazy
                    .Where(x => x.Status == SaiseExporterStatus.New && x.ElectionDayId == entity.Id)
                    .SingleOrDefault<SaiseExporter>();

                    if (saiseExporter != null)
                    {

                        var exporter = new VoterExporter(
                            stageProgress,
                            exportAllVoters,
                            entity.Id,
                            _saiseRepository,
                            saiseExporter.Stages.First(x => x.StageType == SaiseExporterStageType.VoterUpdate),
                            _session, _logger, new ConfigurationSettingManager(sessionFactory));
                        exporter.ExecuteState();
                    }

                    break;
                case "GetSRVExportStages":
                    {
                        SrvRepository rep = new SrvRepository(sessionFactory);
                        ExporterBll bll = new ExporterBll(rep);
                        var result = bll.GetSaiseExporterByEdayId(entity.Id);

                        SyncExporterViewModel _exporter = new SyncExporterViewModel();
                        _exporter.Id = result.Id;
                        _exporter.Status = result.Status.ToString();

                        _exporter.Stages = result.Stages.Select(k => new SyncExporterStageViewModel
                        {
                            Description = k.Description,
                            Status = k.Status.ToString(),
                            Statistics = k.Statistics,
                            Type = k.StageType.ToString()

                        }).ToList();

                        var _exporterSer = JsonConvert.SerializeObject(_exporter);
                        return _exporterSer;
                    }
            }

            return null;
        }
    }
}