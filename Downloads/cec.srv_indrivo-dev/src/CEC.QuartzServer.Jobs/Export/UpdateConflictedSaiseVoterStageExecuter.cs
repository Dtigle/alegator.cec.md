using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using Amdaris;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Importer.ToSaise;
using NHibernate;
using NHibernate.Criterion;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.Export
{
    public class UpdateConflictedSaiseVoterStageExecuter : SaiseExportStageExecuter
    {
        private readonly SaiseRepository _saiseRepository;
        private readonly bool _ignorePeopleWithoutDoc;
        private readonly bool _ignoreMissingSaiseIdinRegion;

        public UpdateConflictedSaiseVoterStageExecuter(ProgressInfo stageProgress, SaiseRepository saiseRepository,
            SaiseExporterStage saiseExporterStage, IStatelessSession session, ILogger logger, bool ignoreMissingSaiseIdinRegion, bool ignorePeopleWithoutDoc)
            : base(stageProgress, saiseExporterStage, session, logger)
        {
            _saiseRepository = saiseRepository;
            _ignorePeopleWithoutDoc = ignorePeopleWithoutDoc;
            _ignoreMissingSaiseIdinRegion = ignoreMissingSaiseIdinRegion;

            StageStatistic = new UpdateSaiseVoterStageStatistic();
        }

        public UpdateSaiseVoterStageStatistic StageStatistic { get; set; }

        protected override void ExecuteStateInternal()
        {
            StageProgress.SetMaximum(1);
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            WriteLog(LogType.Info, string.Format("Started at: {0}", DateTime.Now));

            Session.SetBatchSize(300);

            var people = GetBatchOfPeopleFromConflict();
            StageStatistic.Total = people.Count;

            WriteLog(LogType.Error, string.Format("Imported from Conflict  {0} people", people.Count));

            if (people.Count > 0)
            {
                try
                {
                    _saiseRepository.CreateVoterTemporaryTable();

                    var voters = CreateVoters(people);

                    using (var transaction = _saiseRepository.Session.BeginTransaction())
                    {
                        _saiseRepository.InsertVotersInTemporaryTable(voters);
                        _saiseRepository.CopyDataFromTemporaryTableToVoter();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(LogType.Error, string.Format("Unhandled exception occured: {0}", ex.Message), ex);
                }
                finally
                {
                    _saiseRepository.DropVoterTemporaryTable();
                }
            }

            WriteLog(LogType.Info, string.Format("\n{0} : Processed {1} from {2} \n", DateTime.Now, StageStatistic.Processed, StageStatistic.Total));

            stopWatch.Stop();
            WriteLog(LogType.Info, string.Format("Synch voter from conflict in {0}", stopWatch.Elapsed));

            StageProgress.Increase();
        }

        private List<Voter> CreateVoters(IEnumerable<RspConflictDataAdmin> people)
        {
            var voters = new List<Voter>();
            
            foreach (var person in people)
            {
                long idnp;

                if (!Int64.TryParse(person.Idnp, out idnp))
                {
                    SetError(string.Format("Can not convert SRV IDNP {0} to long", person.Idnp));
                    continue;
                }

                var voter = new Voter { Idnp = idnp };
                MapRspConflictToVoter(voter, person);
                voters.Add(voter);
            }
            
            return voters;
        }

        private void MapRspConflictToVoter(Voter voter, RspConflictDataAdmin person)
        {
            voter.NameRo = person.FirstName;
            voter.LastNameRo = person.LastName;
            voter.PatronymicRo = person.SecondName;
            if (IsValidSqlDateTime(person.Birthdate))
            {
                voter.DateOfBirth = person.Birthdate;
            }
            else
            {
                WriteLog(LogType.Error, string.Format("Person {0}, error on converting DateOfBirth {1}", person.Idnp, person.Birthdate));
            }

            voter.Gender = GetGender(person);

            if (_ignorePeopleWithoutDoc && person.Number == null)
            {
                WriteLog(LogType.Info, string.Format("Person with IDNP {0}, DocumentNumber is null. Inserted with '<'", person.Idnp));
                voter.DocumentNumber = PersonDocument.MissingDocument;
            }
            else
            {
                voter.DocumentNumber = person.Series + person.Number;
            }


            if (person.Issuedate.HasValue &&
                IsValidSqlDateTime(person.Issuedate.Value))
            {
                voter.DateOfIssue = person.Issuedate;
            }

            if (person.Expirationdate.HasValue && IsValidSqlDateTime(person.Expirationdate.Value))
            {
                voter.DateOfExpiry = person.Expirationdate;
            }
            else
            {
                WriteLog(LogType.Error, string.Format("Person {0}, error on converting Document.ValidBy (Expirationdate) {1}", person.Idnp, person.Expirationdate));
            }

            if (IsValidSqlDateTime(person.DateOfRegistration))
            {
                voter.DateOfRegistration = person.DateOfRegistration;
            }
            else
            {
                WriteLog(LogType.Trace, string.Format("Person {0}, error on converting DateOfRegistration {1}", person.Idnp, person.DateOfRegistration));
                voter.DateOfRegistration = new DateTime(1900, 1, 1);    
            }
            
            voter.Status = person.Dead ? 9010 : 1000; //SAISE VoterStatus.Imported = 1000
            StageStatistic.Processed++;
        }

        private static int GetGender(RspConflictDataAdmin person)
        {
            int sexCode;
            int.TryParse(person.SexCode, out sexCode);
            return sexCode;
        }

        private IList<RspConflictDataAdmin> GetBatchOfPeopleFromConflict()
        {
            var idsImported = QueryOver.Of<Person>()
                .Select(Projections.Property<Person>(x => x.Idnp));

            return Session.QueryOver<RspConflictDataAdmin>()
                .WithSubquery.WhereProperty(x => x.Idnp)
                .NotIn(idsImported)
                .List<RspConflictDataAdmin>();
        }

        private bool IsValidSqlDateTime(DateTime dateTime)
        {
            try
            {
                var sqlDt = new SqlDateTime(dateTime);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}