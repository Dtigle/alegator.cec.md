using Amdaris;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using NHibernate;
using NHibernate.Criterion;
using SAISE.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CEC.Web.SRV.EDayExport
{
    public class VoterExporter : ExportStageExecuter
    {
        private readonly bool _exportAllVoters;
        private readonly SaiseRepository _saiseRepository;
        private IList<Gender> _genders;
        private IList<Street> _streets;
        private IList<PollingStation> _srvPollingStations;
        private IList<SaisePollingStation> _saisePollingStations;
        private readonly ElectionDay _electionDay;
        private readonly bool _ignoreMissingSaiseIdinRegion;
        private readonly bool _ignorePeopleWithoutDoc;

        public VoterExporter(
            ProgressInfo stageProgress,
            bool exportAllVoters, long? electionDayId, SaiseRepository saiseRepository,
            SaiseExporterStage saiseExporterStage, IStatelessSession session, ILogger logger, IConfigurationSettingManager configurationSettingManager)
            : base(stageProgress, saiseExporterStage, session, logger)
        {
            _exportAllVoters = exportAllVoters;
            _saiseRepository = saiseRepository;

            if (!electionDayId.HasValue)
            {
                SetError("Saise Election Day Id is not defined");
            }

            _ignoreMissingSaiseIdinRegion =
                configurationSettingManager.Get("SaiseExporterJob_IgnoreMissingSAISEIdinSRVRegion").GetValue<bool>();
            _ignorePeopleWithoutDoc =
                configurationSettingManager.Get("SaiseExporterJob_IgnorePeopleWithoutDoc").GetValue<bool>();

            _electionDay = _saiseRepository.GetElectionDay(electionDayId.Value);
            StageStatistic = new UpdateSaiseVoterStageStatistic();
        }

        public UpdateSaiseVoterStageStatistic StageStatistic { get; set; }

        protected void ExecuteOperationInternal(IList<Person> people)
        {
            List<Voter> voters = CreateVoters(people);

            if (!_exportAllVoters)
            {
                var voterToUpdateIdnps = voters.Select(s => s.Idnp);
                _saiseRepository.ClearAssignedVoters(voterToUpdateIdnps);
            }

            using (var transaction = _saiseRepository.Session.BeginTransaction())
            {
                _saiseRepository.InsertVotersInTemporaryTable(voters);
                _saiseRepository.CopyDataFromTemporaryTableToVoter();
                _saiseRepository.AssignVoterToElection();
                transaction.Commit();
            }

            UpdatePersonExportStatus(people.Select(x => x.Id));
            StageStatistic.Processed += people.Count;

            SetProgress(Math.Round(((double)StageStatistic.Processed / StageStatistic.Total) * 100, 2).ToString());

            WriteLog(LogType.Info,
                string.Format("\n{0} : Processed {1} from {2} \n", DateTime.Now, StageStatistic.Processed,
                    StageStatistic.Total));
        }

        protected override void ExecuteStateInternal()
        {
            LoadGenders();
            LoadStreets();
            LoadSrvPollingStations();
            LoadSaisePollingStations();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if (_exportAllVoters)
            {
                _saiseRepository.ClearAssignedVoters();
                UpdateSrvAllVoterToBeExported();
            }
            else
            {
                var query = Session.QueryOver<Person>()
                    .Where(x => x.Deleted == null)
                    .And(x => (x.Created != null && x.Created.Value >= _electionDay.DeployDbDate) || (x.Modified != null && x.Modified.Value >= _electionDay.DeployDbDate));
                var personsToUpdateIds = query.Select(s => s.Id).List<long>();
                UpdatePersonExportStatus(personsToUpdateIds, 0);
            }

            IList<Person> people;

            Session.SetBatchSize(100);

            StageStatistic.Total = GetTotalPeopleToProcess();
            StageProgress.SetMaximum(StageStatistic.Total);

            if (StageStatistic.Total > 0)
            {

                WriteLog(LogType.Info, "Create VoterSrv temporary table");
                _saiseRepository.CreateVoterTemporaryTable();
                WriteLog(LogType.Info, "Disable Voter Indexes and Create one on idnp");
                _saiseRepository.DisableIndexesOnVoter();
                WriteLog(LogType.Info, string.Format("Started process voters at: {0}", DateTime.Now));

                do
                {
                    if (InterruptPending)
                    {
                        return;
                    }
                    people = GetBatchOfPeopleFromSRV(1000);

                    List<Voter> voters = null;

                    if (!people.Any()) break;

                    try
                    {
                        ExecuteOperationInternal(people);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Thread.Sleep(30000);
                            ExecuteOperationInternal(people);
                        }
                        catch (Exception ex2)
                        {
                            try
                            {
                                Thread.Sleep(30000);
                                ExecuteOperationInternal(people);
                            }
                            catch (Exception ex3)
                            {
                                WriteLog(LogType.Error, string.Format("Unhandled exception occured: {0}", ex3.Message), ex3);
                                SetError(ex3.Message);
                                return;
                            }
                        }
                        //return;
                    }

                    StageProgress.SetProgress(StageStatistic.Processed);
                } while (people.Count > 0);

            }
            else
            {
                SetProgress("100");
            }

            WriteLog(LogType.Info, "Update AssignedVoter villages where -1");
            // _saiseRepository.UpdateAssignedVoterVillage(_saiseElection.Id);

            WriteLog(LogType.Info, "Update AssignedVoter villages where AssignedVoter.VillageId <> AssignedVoter.PollingStation.VillageId");
            //_saiseRepository.FixAssignedVotersVillages(_saiseElection.Id);

            WriteLog(LogType.Info, "Drop VoterSrv temporary table");
            _saiseRepository.DropVoterTemporaryTable();
            WriteLog(LogType.Info, "Enable Voter Indexes and drop created one");
            _saiseRepository.EnableIndexesOnVoter();

            stopWatch.Stop();
            WriteLog(LogType.Info, string.Format("Synch voter in {0}", stopWatch.Elapsed));
        }

        private List<Voter> CreateVoters(IEnumerable<Person> people)
        {
            var voters = new List<Voter>();
            using (Session.BeginTransaction())
            {
                foreach (var person in people)
                {
                    long idnp;

                    if (!Int64.TryParse(person.Idnp, out idnp))
                    {
                        SetError(string.Format("Can not convert SRV IDNP {0} to long", person.Idnp));
                    }

                    var voter = new Voter { Idnp = idnp };
                    try
                    {
                        MapPersonToVoter(voter, person);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(LogType.Error, string.Format("An error occured in MapPersonToVoter for SRV.Person.Id: {0}", person.Id), ex);
                        throw;
                    }

                    voters.Add(voter);
                }
            }
            return voters;
        }


        private void LoadSaisePollingStations()
        {
            _saisePollingStations = _saiseRepository.GetPollingStation();
        }

        private void LoadSrvPollingStations()
        {
            _srvPollingStations =
                Session.QueryOver<PollingStation>().Where(x => x.Deleted == null).List<PollingStation>();
        }


        private void MapPersonToVoter(Voter voter, Person person)
        {
            voter.NameRo = person.FirstName;
            voter.LastNameRo = person.Surname;
            voter.PatronymicRo = person.MiddleName;
            voter.ElectionListNr = person.ElectionListNr;

            if (IsValidSqlDateTime(person.DateOfBirth))
            {
                voter.DateOfBirth = person.DateOfBirth;
            }
            else
            {
                WriteLog(LogType.Trace, string.Format("Person {0}, error on converting DateOfBirth {1}", person.Idnp, person.DateOfBirth));
            }
            voter.Gender = GetVoterGender(person.Gender.Id);

            if (_ignorePeopleWithoutDoc && person.Document.DocumentNumber == null)
            {
                WriteLog(LogType.Info, string.Format("Person with IDNP {0}, DocumentNumber is null. Inserted with '<'", person.Idnp));
                voter.DocumentNumber = PersonDocument.MissingDocument;
            }
            else
            {
                voter.DocumentNumber = person.Document.DocumentNumber;
            }


            if (person.Document.IssuedDate.HasValue &&
                IsValidSqlDateTime(person.Document.IssuedDate.Value))
            {
                voter.DateOfIssue = person.Document.IssuedDate;
            }

            if (person.Document.ValidBy.HasValue && IsValidSqlDateTime(person.Document.ValidBy.Value))
            {
                voter.DateOfExpiry = person.Document.ValidBy;
            }
            else
            {
                WriteLog(LogType.Trace, string.Format("Person {0}, error on converting Document.ValidBy {1}", person.Idnp, person.Document.ValidBy));
            }
            voter.Status = GetVoterStatus(person);

            var personEligibleAddress = GetPersonEligibleAddress(person);

            if (personEligibleAddress == null)
            {
                StageStatistic.WithoutAddress++;
                WriteLog(LogType.Error,
                    string.Format("SRV person Id= {0}, IDNP = {1} does not have an eligible address", person.Id, person.Idnp));

                voter.DateOfRegistration = new DateTime(1900, 1, 1);

                return;
            }

            if (IsValidSqlDateTime(personEligibleAddress.DateOfRegistration))
            {
                voter.DateOfRegistration = personEligibleAddress.DateOfRegistration;
            }
            else
            {
                WriteLog(LogType.Trace, string.Format("Person {0}, error on converting DateOfRegistration {1}", person.Idnp, personEligibleAddress.DateOfRegistration));
                voter.DateOfRegistration = new DateTime(1900, 1, 1);
            }



            if (personEligibleAddress == null || personEligibleAddress.Address == null || personEligibleAddress.Address.Street == null)
            {
                voter.RegionId = -1;
            }
            else
            {
                var address = personEligibleAddress.Address;

                var street = GetStreet(address.Street.Id);

                if (street == null || street.Region == null)
                {
                    voter.RegionId = -1;
                }
                else
                {
                    voter.RegionId = street.Region.Id;
                    voter.StreetId = street.Id;
                    voter.StreetName = street.Name;
                    voter.StreetNumber = address.HouseNumber;
                    voter.StreetSubNumber = address.Suffix;
                    voter.BlockNumber = personEligibleAddress.ApNumber;
                    voter.BlockSubNumber = personEligibleAddress.ApSuffix;
                    if (person.HasValidAgeForElectionDate(_electionDay.ElectionDayDate.Date) &&
                        address.PollingStation != null && address.PollingStation.Id > 0)
                    {
                        AssignPollingStation(voter, address.PollingStation.Id, street);
                    }
                }

            }

            if (!voter.RegionId.HasValue)
            {
                voter.RegionId = -1;
            }
        }

        private long GetVoterStatus(Person person)
        {
            var currentStatus = GetCurrentPersonStatus(person);
            switch (currentStatus.StatusType.Id)
            {
                case PersonStatusType.Death:
                    return 9010;
                case PersonStatusType.Judged:
                    return 9004;
                case PersonStatusType.ForeignCitizen:
                    return 9002;
                default:
                    return 1000;

            }
        }

        private void AssignPollingStation(Voter voter, long srvPollingStaionId, Street street)
        {
            var saisePollingStation = GetSaisePollingStationBySRVPollingStation(srvPollingStaionId);

            //If saisePOllingStation is null this means that this pollingStation does not participate in current election, so we do not assign voter.
            if (saisePollingStation == null)
            {
                return;
            }

            voter.PollingStationId = srvPollingStaionId;

            var region = street.Region;

            voter.RegionId = region.Id;

            //if (!region.SaiseId.HasValue)
            //{
            //    if (!_ignoreMissingSaiseIdinRegion)
            //    {
            //        throw new ExporterBusinessException(string.Format("Region {0}, missing SAISE ID ", region.Id));
            //    }
            //}
            //else
            //{
            //    voter.RegionId = region.SaiseId.Value;
            //}
        }

        private SaisePollingStation GetSaisePollingStationBySRVPollingStation(long srvPollingStaionId)
        {
            //var srvPollingStation = _srvPollingStations.FirstOrDefault(x => x.Id == srvPollingStaionId);
            //if (srvPollingStation == null)
            //{
            //    throw new ExporterBusinessException(string.Format("Cannot find SRV.PollingStation with Id: {0}. Probably assignement to deleted PollingStation.", srvPollingStaionId));
            //}

            //var saisePollingStationId = srvPollingStation.SaiseId;

            return _saisePollingStations.FirstOrDefault(x => x.Id == srvPollingStaionId);
        }

        private IList<Person> GetBatchOfPeopleFromSRV(int batchSize = 1000)
        {
            return Session.QueryOver<Person>()
                .Where(x => x.Deleted == null)
                .And(x => x.ExportedToSaise == false)
                .Take(batchSize).List<Person>();
        }

        private PersonAddress GetPersonEligibleAddress(Person person)
        {
            return Session.QueryOver<PersonAddress>()
                .Fetch(x => x.Address).Eager
                .Where(x => x.Person == person)
                .And(x => x.IsEligible == true).List().FirstOrDefault();
        }

        private PersonStatus GetCurrentPersonStatus(Person person)
        {
            return Session.QueryOver<PersonStatus>()
                .Fetch(x => x.StatusType).Eager
                .Where(x => x.Person == person)
                .And(x => x.IsCurrent == true).List().FirstOrDefault();
        }

        private long GetTotalPeopleToProcess()
        {
            if (_exportAllVoters)
            {
                return Session.QueryOver<Person>().Where(x => x.Deleted == null)
                    .And(x => x.ExportedToSaise == false)
                    .Select(Projections.RowCountInt64()).SingleOrDefault<long>();
            }
            else
            {
                return Session.QueryOver<Person>().Where(x => x.Deleted == null)
                    .And(x => !x.ExportedToSaise || (x.Created != null && x.Created.Value >= _electionDay.DeployDbDate) || (x.Modified != null && x.Modified.Value >= _electionDay.DeployDbDate))
                    .Select(Projections.RowCountInt64()).SingleOrDefault<long>();
            }
        }


        private void UpdateSrvAllVoterToBeExported()
        {
            // Set exportedToSaise to 0 in order to be reexported
            using (var transaction = Session.BeginTransaction())
            {
                const string sqlQuery = "update SRV.People set exportedToSaise = 0";
                var updateQuery = Session.CreateSQLQuery(sqlQuery);
                updateQuery.SetTimeout(1200);
                updateQuery.ExecuteUpdate();
                transaction.Commit();
            }
        }

        private void UpdatePersonExportStatus(IEnumerable<long> personIds, int exportedToSaise = 1)
        {
            if (personIds != null && personIds.Count() > 0)
            {
                using (var transaction = Session.BeginTransaction())
                {
                    string sqlQuery = string.Format("update SRV.People set exportedToSaise = {1} where personId IN ({0})", string.Join(",", personIds), exportedToSaise);
                    var updateQuery = Session.CreateSQLQuery(sqlQuery);
                    updateQuery.ExecuteUpdate();
                    transaction.Commit();
                }
            }
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

        private void LoadGenders()
        {
            _genders = Session.QueryOver<Gender>().List<Gender>();
        }

        private void LoadStreets()
        {
            _streets = Session.QueryOver<Street>()
                .Fetch(x => x.Region).Eager
                .List<Street>();
        }


        private Street GetStreet(long streetId)
        {
            return _streets.FirstOrDefault(x => x.Id == streetId);
        }

        private int GetVoterGender(long genderId)
        {
            var gender = _genders.First(x => x.Id == genderId);

            switch (gender.Name)
            {
                case "M":
                    return 1;
                case "F":
                    return 2;
                default:
                    return 0;
            }
        }
    }

    public class UpdateSaiseVoterStageStatistic
    {
        public long Processed { get; set; }
        public long Total { get; set; }
        public long WithoutAddress { get; set; }
        public long Ignored { get; set; }
    }
}