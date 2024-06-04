using Amdaris;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace CEC.Web.Results.Api.Infrastructure
{

    public class SqlFactory
    {
        private const string CurrentConnectionStringName = "SAISEConnectionString";

        public IDbConnection OpenConnect()
        {
            ConnectionStringSettings foundCs = ConfigurationManager.ConnectionStrings[CurrentConnectionStringName];
            string currentProviderName = foundCs.ProviderName;
            string currentConnectionString = foundCs.ConnectionString;
        
            DbConnection connection = null;

            if (!string.IsNullOrEmpty(currentConnectionString))
            {
                try
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(currentProviderName);

                    connection = factory.CreateConnection();
                    if (connection != null)
                    {
                        connection.ConnectionString = currentConnectionString;
                        connection.Open();
                    }
                }
                catch (Exception ex)
                { 
                    LogError(ex);
                }
            }
            
            // Return the connection. 
            return connection;
        }

        /// <summary>
        /// Detailed log of different problems
        /// </summary> 
        private void LogError(Exception ex)
        {
            DependencyResolver.Current.Resolve<ILogger>().Error(ex);
        }

        IDbCommand CreateSPCommand(IDbConnection connection, string procNameWithParams)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;

            command.CommandText = procNameWithParams;
            
            return command;
        }

        private List<T> CreateExecuteCommand<T>(Action<IDbCommand> setupCommandAction) where T : IDbMap, new()
        {
            try
            {
                using (IDbConnection connection = OpenConnect())
                {
                    IDbCommand cmd = connection.CreateCommand();

                    setupCommandAction(cmd);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        var result = new List<T>();

                        while (reader.Read())
                        {
                            var item = new T();
                            item.Map(reader);

                            result.Add(item);
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        private object ExecuteScalar(Action<IDbCommand> setupCommandAction)
        {
            try
            {
                using (IDbConnection connection = OpenConnect())
                {
                    IDbCommand cmd = connection.CreateCommand();

                    setupCommandAction(cmd);

                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        public List<VotersActivity> GeneralOnTimeResults(int electionId, DateTime startTime,
            DateTime endTime, int? interval)
        {
            const string procedureName = "[dbo].[CEC_GeneralOnTimeResults]";
            return CreateExecuteCommand<VotersActivity>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.DateTime, "@StartHour", startTime);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.DateTime, "@EndHour", endTime);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@interval", interval);
            });
        }

        public List<VotersActivity> GeneralOnTimeAddResults(int electionId, DateTime openVote, DateTime currentTime, int interval)
        {
            const string procedureName = "[dbo].[CEC_GeneralOnTimeAddResults]";

            return CreateExecuteCommand<VotersActivity>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.DateTime, "@StartHour", openVote);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.DateTime, "@EndHour", currentTime);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@interval", interval);
            });
        }

        public int GeneralTotalVoters(int electionId)
        {
            const string procedureName = "[dbo].[CEC_GeneralTotal]";

            return (int) ExecuteScalar((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
            });
        }

        public List<GeneralPerPartidsResult> GeneralPartidResults(int electionId)
        {
            const string procedureName = "[dbo].[CEC_GeneralPartidResults]";

            return CreateExecuteCommand<GeneralPerPartidsResult>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
            });
        }

		public List<GeneralPerPartidsResult> GeneralPartidResultsForDistrict(int electionId, long districtId)
		{
			const string procedureName = "[dbo].[CEC_GeneralPartidResultsForDistrict]";

			return CreateExecuteCommand<GeneralPerPartidsResult>((cmd) =>
			{
				cmd.CommandText = procedureName;
				cmd.CommandType = CommandType.StoredProcedure;

				CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@ElectionId", electionId);
				CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@DistrictId", districtId);
			});
		}

        public DateTime VotingDayById(int electionId)
        {
            const string procedureName = "[dbo].[CEC_ElectionDayById]";

            return (DateTime)ExecuteScalar((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
            });
        }

        public byte[] LogoByCandidateId(int id)
        {
            const string procedureName = "[dbo].[CEC_LogoByCandidateId]";

            var result = ExecuteScalar((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@CandidateId", id);
            });

            // Candidate haven't logo
            if (result is DBNull)
            {
                return null;
            }
            else
            {
                return (byte[]) result;
            }
        }

        private void CreateAndSetParameter(IDbCommand cmd, ParameterDirection paramDirecttion, DbType paramType, string paramName, object paramValue)
        {
            var param = cmd.CreateParameter();
            param.DbType = paramType;
            param.Direction = paramDirecttion;
            param.ParameterName = paramName;
            param.Value = paramValue;

            cmd.Parameters.Add(param);
        }

        public List<TResult> ReadDbElementsList<TResult>(string procName) where TResult : IDbMap, new()
        {
            try
            {
                using (IDbConnection connection = OpenConnect())
                {
                    IDbCommand cmd = CreateSPCommand(connection, procName);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        var result = new List<TResult>();

                        while (reader.Read())
                        {
                            var item = new TResult();
                            item.Map(reader);

                            result.Add(item);
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        public TResult ReadDbElement<TResult>(string procName) where TResult : IDbMap, new()
        {
            using (IDbConnection connection = OpenConnect())
            {
                IDbCommand cmd = CreateSPCommand(connection, procName);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    TResult item = new TResult();
                    item.Map(reader);

                    return item;
                }
            }

        }

        public object ReadDbElement(string procName)
        {
            try
            {
                using (IDbConnection connection = OpenConnect())
                {
                    IDbCommand cmd = CreateSPCommand(connection, procName);

                    object votersCount = cmd.ExecuteScalar();

                    return votersCount;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        public List<AssignedPollingStation> GetAssignedPollingStations(int electionId)
        {
            const string procedureName = "[dbo].[CEC_AssignedPollingStation]";

            return CreateExecuteCommand<AssignedPollingStation>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
            });
        }
        public int GetTotalVotersReceivedBallotsPerPollingStation(int electionId, long pollingStation)
        {
            const string procedureName = "[dbo].[CEC_CountOfVotersReceivedBallots]";

            return (int)ExecuteScalar((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@PollingStationId", pollingStation);
            });
        }

        public int GetTotalVotersInSupplimentaryListPerPollingStation(int electionId, long pollingStation)
        {
            const string procedureName = "[dbo].[CEC_CountOfVotersInSupplimentaryList]";

            return (int)ExecuteScalar((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@PollingStationId", pollingStation);
            });
        }

        public List<BallotPapersCountData> GetBallotPapers(int electionId)
        {
            const string procedureName = "[dbo].[CEC_TotalBallots]";

            return CreateExecuteCommand<BallotPapersCountData>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
            });
        }

		public List<BallotPapersCountData> GetBallotPapersForDistrict(long electionId, long districtId)
        {
            const string procedureName = "[dbo].[CEC_TotalBallotsForDistrict]";

            return CreateExecuteCommand<BallotPapersCountData>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@ElectionId", electionId);
				CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@DistrictId", districtId);
            });
        }

		public List<TotalVotesData> GetTotalValidVotes(int electionId)
        {
            const string procedureName = "[dbo].[CEC_TotalVotes]";

            return CreateExecuteCommand<TotalVotesData>((cmd) =>
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int32, "@ElectionId", electionId);
            });
        }

		public List<TotalVotesData> GetTotalValidVotesForDistrict(long electionId, long districtId)
	    {
			const string procedureName = "[dbo].[CEC_TotalVotesForDistrict]";

			return CreateExecuteCommand<TotalVotesData>((cmd) =>
			{
				cmd.CommandText = procedureName;
				cmd.CommandType = CommandType.StoredProcedure;

				CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@ElectionId", electionId);
				CreateAndSetParameter(cmd, ParameterDirection.Input, DbType.Int64, "@DistrictId", districtId);
			});
	    }
    }

}