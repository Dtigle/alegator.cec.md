
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.ReportService;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Resources;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace CEC.SRV.BLL.Impl
{
    public class ExporterBll : Bll, IExporterBll
    {
        private static object _lockObj = new object();
        public ExporterBll(ISRVRepository repository)
            : base(repository)
        {
        }

        public void CreateSaiseExporter(long electionDayId, bool exportAllVoters)
        {
            lock (_lockObj)
            {
                VerifyIfAlreadyExistDataToExport();
                var newSaiseExporter = new SaiseExporter(electionDayId);
                newSaiseExporter.ExportAllVoters = exportAllVoters;
                Repository.SaveOrUpdate(newSaiseExporter);
            }
        }

        public PageResponse<SaiseExporterStage> GetSaiseExporter(PageRequest pageRequest, long? saiseExporterId)
        {
            SaiseExporterStage saiseExporterStage = null;
            SaiseExporter saiseExporter = null;

            var query = Repository.QueryOver(() => saiseExporterStage)
                .JoinAlias(x => x.SaiseExporter, () => saiseExporter);

            if (saiseExporterId.HasValue)
            {
                query.Where(x => x.SaiseExporter.Id == saiseExporterId);
            }
            else
            {
                query.Where(() => saiseExporter.Status == SaiseExporterStatus.New || saiseExporter.Status == SaiseExporterStatus.InProgress);
            }

            return query.TransformUsing(Transformers.DistinctRootEntity)
                        .RootCriteria.CreatePage<SaiseExporterStage>(pageRequest);
        }

        public PageResponse<SaiseExporterStage> GetHistorySaiseExporter(PageRequest pageRequest, long? saiseExporterId)
        {
            SaiseExporterStage saiseExporterStage = null;
            SaiseExporter saiseExporter = null;

            return Repository.QueryOver(() => saiseExporterStage)
                .JoinAlias(x => x.SaiseExporter, () => saiseExporter)
                .Where(x => x.SaiseExporter.Id == saiseExporterId)
                .TransformUsing(Transformers.DistinctRootEntity).RootCriteria.CreatePage<SaiseExporterStage>(pageRequest);
        }

        public IEnumerable<SaiseExporter> GetUnProcessedSaiseExporter()
        {
            return Repository.Query<SaiseExporter>()
                    .Where(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress)
                    .ToList();
        }


        public SaiseExporter GetSaiseExporterByEdayId(long eDayId)
        {
            return Repository.Query<SaiseExporter>()
                          .Where(s => s.ElectionDayId == eDayId)
                          .OrderByDescending(x => x.Id).FirstOrDefault();

        }

        private void VerifyIfAlreadyExistDataToExport()
        {
            var saiseExporterData = GetUnProcessedSaiseExporter();
            if (saiseExporterData.Any())
            {
                throw new SrvException("SaiseExporter_ExistForProcessing", MUI.SaiseExporter_ExistForProcessing);
            }
        }

        public SaiseExporter GetActiveSaiseExporter()
        {
            return Repository.Query<SaiseExporter>()
                .FirstOrDefault(x => x.Status == SaiseExporterStatus.New || x.Status == SaiseExporterStatus.InProgress);
        }

        public int GetProgressOfSaiseExporter(long saiseExporterId)
        {
            var totalStage = Repository.Query<SaiseExporterStage>().Count(x => x.SaiseExporter.Id == saiseExporterId);
            var finishedStage = Repository.Query<SaiseExporterStage>().Count(x => x.SaiseExporter.Id == saiseExporterId && x.Status == SaiseExporterStageStatus.Done);
            return finishedStage * 100 / totalStage;
        }

        public bool GetFailedMessageOfSaiseExporter(long saiseExporterId)
        {
            return Repository.Query<SaiseExporterStage>()
                .FirstOrDefault(x => x.Status == SaiseExporterStageStatus.Failed && x.SaiseExporter.Id == saiseExporterId) != null;
        }

        public bool SetElectionListNr(List<long> pollingStationsId, long electionId)
        {
            var dateelection = Repository.Get<ElectionRound>(electionId);
            DateTime dateTime = dateelection.ElectionDate;

            var sb = new StringBuilder();

            // do some other stuff with sb

            sb = pollingStationsId.Aggregate(sb, (b, d) => b.Append(d).Append(','));
            if (pollingStationsId.Count > 0) sb.Length--;

            //pollingStationsId = pollingStationsId.Take(100).ToList();
            //do some more stuff with sb
            var str = sb.ToString();
            int psCount = 100;
            int m = 0;
            List<IFutureValue<object>> t = new List<IFutureValue<object>>();
            List<Task> tasks = new List<Task>();

            ISession session = ((SrvRepository)Repository).GetSession();
            string connectionStr = session.Connection.ConnectionString;

            for (m = 0; m < pollingStationsId.Count / psCount; m++)
            {
                var result = string.Join(",", pollingStationsId.Skip(m * psCount).Take(psCount));
                tasks.Add(new Task(() =>
                {
                    using (SqlConnection cn = new SqlConnection(connectionStr))
                    {
                        using (SqlCommand cmd = new SqlCommand("[SRV].[sv_StartProcedureAddNumberInList]", cn))
                        {
                            cmd.CommandTimeout = 10000;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@pollingstation", SqlDbType.VarChar)
                            {
                                Value = result,
                                Direction = ParameterDirection.Input
                            });

                            cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)
                            {
                                Value = dateTime,
                                Direction = ParameterDirection.Input
                            });

                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                        }
                    }
                }));
            }

            if ((pollingStationsId.Count % psCount) != 0)
            {
                var result = string.Join(",", pollingStationsId.Skip(m * psCount).Take(pollingStationsId.Count % psCount));

                tasks.Add(new Task(() =>
                {
                    using (SqlConnection cn = new SqlConnection(connectionStr))
                    {
                        using (SqlCommand cmd = new SqlCommand("[SRV].[sv_StartProcedureAddNumberInList]", cn))
                        {
                            cmd.CommandTimeout = 10000;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@pollingstation", SqlDbType.VarChar)
                            {
                                Value = result,
                                Direction = ParameterDirection.Input
                            });

                            cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)
                            {
                                Value = dateTime,
                                Direction = ParameterDirection.Input
                            });

                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                        }
                    }
                }));
            }

            foreach (Task task in tasks.AsParallel())
            {
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());

            return true;
        }
    }
}