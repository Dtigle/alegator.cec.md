using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Dto;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.Export
{
    public class SaiseRepository
    {
        private readonly IStatelessSession _session;
        
        public SaiseRepository()
        {
            ISessionFactory sessionFactory = SaiseDatabaseFactoryProvider.GetFactory;
            _session = sessionFactory.OpenStatelessSession();
            _session.SetBatchSize(1000);
        }

        public SaiseElection GetElection(long saiseElectionId)
        {
            return _session.Get<SaiseElection>(saiseElectionId);
        }

        public IList<SaisePollingStation> GetPollingStation(long electionRoundId)
        {
            AssignedPollingStation assignedPollingStation = null;
            
            return _session.QueryOver<SaisePollingStation>()
                    .JoinAlias(x => x.AssignedPollingStations, () => assignedPollingStation)

                    .Where(x => assignedPollingStation.ElectionRound.Id == electionRoundId)
                    .And(x => x.Number > 0).List<SaisePollingStation>();

        }
        
        public Voter GetSaiseVoterByIdnp(long idnp)
        {
            return _session.QueryOver<Voter>().Where(x => x.Idnp == idnp).SingleOrDefault<Voter>();
        }

        public void SaveOrUpdate(SaiseEntity entity)
        {
            if (entity.Id == 0)
            {
                _session.Insert(entity);
            }
            else
            {
                _session.Update(entity);
            }
        }

        public IStatelessSession Session
        {
            get { return _session; }
        }

        public AssignedVoter GetAssignedVoter(Voter voter, SaiseElectionRound saiseElectionRound)
        {
            return _session.QueryOver<AssignedVoter>()
                .Where(x => x.Voter == voter)
                //.And(x => x.ElectionRound == saiseElectionRound)
                .SingleOrDefault<AssignedVoter>();
        }

        public IEnumerable<SaiseElection> GetElectionsByDate(DateTime electionDate)
        {
            return _session.QueryOver<SaiseElection>()
                .Where(x => x.DateOfElection == electionDate)
                .OrderBy(x => x.Id).Asc
                .List();
        }

        public IEnumerable<SaiseRegion> GetRegionsForElection(long electionRoundId)
        {
            return _session.Query<AssignedPollingStation>()
                .Where(x => x.ElectionRound.Id == electionRoundId)
                .Select(x => x.PollingStation.Region)
                .OrderBy(x => x.Name)
                .ToList().Distinct();
        }

        public IEnumerable<AssignedPollingStation> GetOpenedPollingStations(long electionRoundId)
        {
            return _session.Query<AssignedPollingStation>()
                .Where(x => x.ElectionRound.Id == electionRoundId && x.IsOpen)
                .ToList();
        }

        public long GetBaseListVotedCount(long electionRoundId, long pollingStationId, TimeSpan controlTime)
        {
            var result = _session.Query<AssignedVoter>()
                .Where(x => //x.ElectionRound.Id == electionRoundId && 
                            x.RequestingPollingStation.Id == pollingStationId && 
                            x.Status == 5000)
                .LongCount();

            return result;
        }

        public long GetSupplimentaryListVotedCount(long electionRoundId, long pollingStationId, TimeSpan controlTime)
        {
            var supplimentaryListStatuses = new long[] {5001, 5002, 5020};
            var result = _session.Query<AssignedVoter>()
                .Where(x => //x.ElectionRound.Id == electionRoundId && 
                            x.PollingStation.Id == pollingStationId && 
                            supplimentaryListStatuses.Contains(x.Status))
                .LongCount();

            return result;
        }

        public IEnumerable<SaiseRegion> GetAllRegions(long regionId, long electionRoundId )
        {
            if (regionId < 0 && regionId != -1)
            {
                return _session.Query<AssignedPollingStation>()
                    .Where(x => x.PollingStation.Region.Id >= 0 && x.ElectionRound.Id == electionRoundId)
                    .OrderBy(x=>x.PollingStation.Region.Name)
                    .Select(x =>x.PollingStation.Region)
                    .ToList().Distinct();
            }
            return _session.Query<AssignedPollingStation>()
                    .Where(x => x.PollingStation.Region.Id == regionId && x.ElectionRound.Id == electionRoundId)
                    .OrderBy(x => x.PollingStation.Region.Name)
                    .Select(x => x.PollingStation.Region)
                    .ToList().Distinct();
        }

        public IEnumerable<SaisePollingStation> GetAllPollingStationsByRegion(long electionRoundId, long regionId)
        {
           return _session.Query<AssignedPollingStation>()
                .Where(x=>x.ElectionRound.Id == electionRoundId && x.PollingStation.Region.Id == regionId)
                .Select(x=>x.PollingStation)
                .OrderBy(x =>x.Number)
                .ThenBy(x =>x.SubNumber)
                .ToList().Distinct();
        }


        public void InsertVotersInTemporaryTable(List<Voter> voters)
        {
            ClearVoterTemporaryTable();
            voters.ForEach(SaveOrUpdate);
        }

        public void CopyDataFromTemporaryTableToVoter()
        {
            const string updateCmd = @"
                            UPDATE v SET
                            v.NameRo = t.NameRo,
                            v.LastNameRo = t.LastNameRo,
                            v.PatronymicRo = t.PatronymicRo,
                            v.LastNameRu = t.LastNameRu,
                            v.NameRu = t.NameRu,
                            v.PatronymicRu = t.PatronymicRu,
                            v.DateOfBirth = t.DateOfBirth,
                            v.PlaceOfBirth = t.PlaceOfBirth,
                            v.PlaceOfResidence = t.PlaceOfResidence,
                            v.Gender = t.Gender,
                            v.DateOfRegistration = t.DateOfRegistration,
                            v.DocumentNumber = t.DocumentNumber,
                            v.DateOfIssue = t.DateOfIssue,
                            v.DateOfExpiry = t.DateOfExpiry,
                            v.Status = t.Status,
                            v.BatchId = t.BatchId,
                            v.StreetId = t.StreetId,
                            v.StreetName = t.StreetName,
                            v.StreetNumber = t.StreetNumber,
                            v.StreetSubNumber = t.StreetSubNumber,
                            v.BlockNumber = t.BlockNumber,
                            v.BlockSubNumber = t.BlockSubNumber,
                            v.EditUserId = 1,
                            v.EditDate = GETDATE(),
                            v.Version = v.Version + 1

                            FROM VoterSRV t
                            INNER JOIN Voter v on t.idnp = v.idnp ";
            ExecuteQuery(updateCmd);

            const string insertCmd = @"INSERT INTO Voter (NameRo, LastNameRo, PatronymicRo, LastNameRu, NameRu, PatronymicRu, DateOfBirth, PlaceOfBirth, PlaceOfResidence,
                                            Gender, DateOfRegistration, Idnp, DocumentNumber, DateOfIssue, DateOfExpiry, Status, BatchId, StreetId, StreetName, StreetNumber,
                                            StreetSubNumber, BlockNumber, BlockSubNumber, EditUserId, EditDate, Version)

                                      SELECT t.NameRo, t.LastNameRo, t.PatronymicRo, t.LastNameRu, t.NameRu, t.PatronymicRu, t.DateOfBirth, t.PlaceOfBirth, t.PlaceOfResidence,
                                            t.Gender, t.DateOfRegistration, t.Idnp, t.DocumentNumber, t.DateOfIssue, t.DateOfExpiry, t.Status, t.BatchId, t.StreetId, t.StreetName, t.StreetNumber,
                                            t.StreetSubNumber, t.BlockNumber, t.BlockSubNumber, 1, GETDATE(), 1
                                      FROM VoterSRV t 
                                      LEFT JOIN Voter v on t.idnp = v.idnp 
                                      WHERE v.voterId  IS NULL";

            ExecuteQuery(insertCmd);

        }

        public void CreateVoterTemporaryTable()
        {
            DropVoterTemporaryTable();
            const string createTableCmd = @"CREATE TABLE [dbo].[VoterSRV](
	                                        [VoterId] [bigint] IDENTITY(1,1) NOT NULL,
	                                        [NameRo] [nvarchar](100) NOT NULL,
	                                        [LastNameRo] [nvarchar](100) NOT NULL,
	                                        [PatronymicRo] [nvarchar](100) NULL,
	                                        [LastNameRu] [nvarchar](100) NULL,
	                                        [NameRu] [nvarchar](100) NULL,
	                                        [PatronymicRu] [nvarchar](100) NULL,
	                                        [DateOfBirth] [datetime] NOT NULL,
	                                        [PlaceOfBirth] [nvarchar](max) NULL,
                                        	[PlaceOfResidence] [nvarchar](max) NULL,
                                        	[Gender] [int] NOT NULL,
                                        	[DateOfRegistration] [datetime] NOT NULL,
                                        	[Idnp] [bigint] NOT NULL,
                                        	[DocumentNumber] [nvarchar](50) NOT NULL,
                                        	[DateOfIssue] [datetime] NULL,
                                        	[DateOfExpiry] [datetime] NULL,
                                        	[Status] [bigint] NOT NULL,
                                        	[BatchId] [bigint] NULL,
                                        	[StreetId] [bigint] NULL,
                                        	[StreetName] [nvarchar](max) NULL,
                                        	[StreetNumber] [bigint] NULL,
                                        	[StreetSubNumber] [nvarchar](50) NULL,
                                        	[BlockNumber] [bigint] NULL,
                                        	[BlockSubNumber] [nvarchar](50) NULL,
                                            [PollingStationId] bigint NULL,
                                            [RegionId] bigint NULL,
                                        CONSTRAINT [PK_PersonSRV] PRIMARY KEY CLUSTERED 
                                        ( [VoterId] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                    CREATE INDEX IX_VoterSrv_idnp ON VoterSRV(idnp)

                            ";

            ExecuteQuery(createTableCmd);
        }

        public void AssignVoterToElection(long saiseElectionId)
        {
            const string assignVoters = @"
                
                        UPDATE av SET
                        av.RequestingPollingStationId = t.pollingStationId,
                        av.PollingStationId = t.pollingStationId,
                        av.RequestingVillageId = ISNULL(t.regionId, -1),
                        av.VillageId = ISNULL(t.regionId, -1),
                        av.RopVillageId = ISNULL(t.regionId, -1),
                        av.Category = -1,
                        av.EditUserId = 1,
                        av.EditDate = GETDATE(),
                        av.Version = ISNULL(av.Version, 0) + 1
                        FROM VoterSRV t
                        INNER JOIN Voter v on t.idnp = v.idnp
                        INNER JOIN AssignedVoter av on av.VoterId = v.VoterId
                        WHERE 
                        t.pollingStationId is not null and v.Status = 1000
                        and av.ElectionId = :electionId

                        INSERT INTO AssignedVoter (ElectionId, RequestingVillageId, VillageId, RopVillageId, RequestingPollingStationId, PollingStationId,
                        VoterId, Category, Status, EditUserId, EditDate, Version)

                        SELECT :electionId, ISNULL(t.regionId, -1), ISNULL(t.regionId, -1), ISNULL(t.regionId, -1), t.pollingStationId, t.pollingStationId, v.VoterId, -1, v.Status, 1, GetDate(), 1

                        FROM VoterSRV t
                        INNER JOIN Voter v on t.idnp = v.idnp
                        LEFT JOIN AssignedVoter av on av.VoterId = v.VoterId and av.ElectionId = :electionId
                        WHERE av.AssignedVoterId IS NULL
                        and t.pollingStationId is not null and v.Status = 1000

                ";

            var query = Session.CreateSQLQuery(assignVoters).SetInt64("electionId", saiseElectionId);
            query.ExecuteUpdate();

        }

        public void ClearAssignedVoters(long electionId)
        {
            const string cmd = @"
                    DELETE FROM AssignedVoter WHERE ElectionId = :electionId
            ";

            var query = Session.CreateSQLQuery(cmd).SetInt64("electionId", electionId);
            query.ExecuteUpdate();
        }

        public void DisableIndexesOnVoter()
        {
            const string cmd = @"
                IF EXISTS ( SELECT 1 FROM sys.indexes WHERE name='_dta_index_Voter_5_1616112898__K13_K1_2_3_4_5_6_7_8_9_10_11_12_14_15_16_17_18_19_20_21_22_23_24_25_26_27' AND object_id = OBJECT_ID('Voter'))
                    ALTER INDEX [_dta_index_Voter_5_1616112898__K13_K1_2_3_4_5_6_7_8_9_10_11_12_14_15_16_17_18_19_20_21_22_23_24_25_26_27]
                        ON Voter DISABLE
                
                IF EXISTS ( SELECT 1 FROM sys.indexes WHERE name='_dta_index_Voter_5_1616112898__K14_1_2_3_4_5_6_7_8_9_10_11_12_13_15_16_17_18_19_20_21_22_23_24_25_26_27' AND object_id = OBJECT_ID('Voter'))
                    ALTER INDEX [_dta_index_Voter_5_1616112898__K14_1_2_3_4_5_6_7_8_9_10_11_12_13_15_16_17_18_19_20_21_22_23_24_25_26_27]
                        ON Voter DISABLE

                IF NOT EXISTS ( SELECT 1 FROM sys.indexes WHERE name='IX_Voter_idnp' AND object_id = OBJECT_ID('Voter'))
                    CREATE INDEX IX_Voter_idnp ON Voter(idnp)
            ";

            ExecuteQuery(cmd);
        }

        public void EnableIndexesOnVoter()
        {
            const string cmd = @"
                
                IF EXISTS ( SELECT 1 FROM sys.indexes WHERE name='IX_Voter_idnp' AND object_id = OBJECT_ID('Voter'))
                    DROP INDEX IX_Voter_idnp ON Voter

                IF EXISTS ( SELECT 1 FROM sys.indexes WHERE name='_dta_index_Voter_5_1616112898__K13_K1_2_3_4_5_6_7_8_9_10_11_12_14_15_16_17_18_19_20_21_22_23_24_25_26_27' AND object_id = OBJECT_ID('Voter'))
                    ALTER INDEX [_dta_index_Voter_5_1616112898__K13_K1_2_3_4_5_6_7_8_9_10_11_12_14_15_16_17_18_19_20_21_22_23_24_25_26_27]
                        ON Voter REBUILD
                
                IF EXISTS ( SELECT 1 FROM sys.indexes WHERE name='_dta_index_Voter_5_1616112898__K14_1_2_3_4_5_6_7_8_9_10_11_12_13_15_16_17_18_19_20_21_22_23_24_25_26_27' AND object_id = OBJECT_ID('Voter'))
                    ALTER INDEX [_dta_index_Voter_5_1616112898__K14_1_2_3_4_5_6_7_8_9_10_11_12_13_15_16_17_18_19_20_21_22_23_24_25_26_27]
                        ON Voter REBUILD
            ";

            ExecuteQuery(cmd);
        }

        public void UpdateAssignedVoterVillage(long electionId)
        {
            const string cmd = @"
                
                        UPDATE av SET
                        av.RequestingVillageId = ps.VillageId,
                        av.VillageId = ps.VillageId,
                        av.RopVillageId = ps.VillageId

                        FROM AssignedVoter av

                        INNER JOIN PollingStation ps on av.PollingStationId = ps.PollingStationId
                        WHERE av.VillageId = -1 and av.electionId = :electionId 
                                ";
            var query = Session.CreateSQLQuery(cmd).SetInt64("electionId", electionId);
            query.ExecuteUpdate();
        }

        public void DropVoterTemporaryTable()
        {
            const string dropCmd = @"IF (EXISTS (SELECT 1 FROM sys.tables WHERE name = 'VoterSRV')) 
                                DROP TABLE [dbo].[VoterSRV] ";
            ExecuteQuery(dropCmd);
        }

        private void ClearVoterTemporaryTable()
        {
            const string query = "truncate table VoterSRV";
            ExecuteQuery(query);
        }

        private void ExecuteQuery(string query)
        {
            var updateQuery = Session.CreateSQLQuery(query);
            updateQuery.ExecuteUpdate();
        }

        public int CopyElectionTurnout(long electionId, DateTime timeOfEntry)
        {
            const string fixElectionSPQuery = "exec [dbo].[sp_FixElectionTurnouts] @ElectionId = :ElectionId, @timeOfEntry = :timeOfEntry";
            return Session.CreateSQLQuery(fixElectionSPQuery)
                .SetInt64("ElectionId", electionId)
                .SetDateTime("timeOfEntry", timeOfEntry)
                .ExecuteUpdate();
        }

        public int FixElectionTurnout(long electionId)
        {
            const string fixElectionSPQuery = "exec [dbo].[pDK_Final_FixElectionTurnout] @ElectionId = :ElectionId";
            return Session.CreateSQLQuery(fixElectionSPQuery)
                .SetInt64("ElectionId", electionId)
                .ExecuteUpdate();
        }

        /// <summary>
        /// Fix for cases where VillageId of AssignedVoter does not match VillageId of assigned PollingStation.
        /// </summary>
        /// <param name="electionId"></param>
        public void FixAssignedVotersVillages(long electionId)
        {
            const string fixAssignedVotersVillageQry = @"
                update av
                    set av.VillageId = ps.VillageId
                from AssignedVoter av
                inner join PollingStation ps on av.PollingStationId = ps.PollingStationId
                where av.ElectionId = :electionId and av.VillageId <> ps.VillageId";

            Session.CreateSQLQuery(fixAssignedVotersVillageQry)
                .SetInt64("electionId", electionId)
                .ExecuteUpdate();
        }

        public int GetBallotsCount(long electionId)
        {
            return _session.Query<SaiseBallotPaper>().Count(x => x.Election.Id == electionId);
        }

        public bool BallotPaperTransmited(long pollingStationId, long electionId)
        {
            var ballotPaper = _session.Query<SaiseBallotPaper>()
                .FirstOrDefault(x => x.Election.Id == electionId && x.PollingStation.Id == pollingStationId);
            if (ballotPaper != null)
            {
                return ballotPaper.Status > 0;
            }

            return false;
        }

        public List<AssignedPollingStation> GetAssignedPollingStations(long electionRoundId)
        {
            return _session.Query<AssignedPollingStation>()
                .Fetch(x => x.PollingStation)
                .ThenFetch(x => x.Region)
                .Where(x => x.ElectionRound.Id == electionRoundId)
                .ToList();
        }
    }
}