using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Core;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Quartz;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using NHibernate;
using NHibernate.Criterion;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.Export
{
    public class UpdateSaiseVoterStageExecuter : SaiseExportStageExecuter
    {
        private readonly bool _exportAllVoters;
        private readonly SaiseRepository _saiseRepository;
        private IList<Gender> _genders;
        private IList<Street> _streets;
        private IList<PollingStation> _srvPollingStations;
        private IList<SaisePollingStation> _saisePollingStations;
        private readonly SaiseElection _saiseElection;
        private readonly bool _ignoreMissingSaiseIdinRegion;
        private readonly bool _ignorePeopleWithoutDoc;

        public UpdateSaiseVoterStageExecuter(
            ProgressInfo stageProgress,
            bool exportAllVoters, long? saiseElectionId, SaiseRepository saiseRepository,
            SaiseExporterStage saiseExporterStage, IStatelessSession session, ILogger logger, IConfigurationSettingManager configurationSettingManager)
            : base(stageProgress, saiseExporterStage, session, logger)
        {
            _exportAllVoters = exportAllVoters;
            _saiseRepository = saiseRepository;

            if (!saiseElectionId.HasValue)
            {
                SetError("Saise Election Id is not defined");
            }

            _ignoreMissingSaiseIdinRegion =
                configurationSettingManager.Get("SaiseExporterJob_IgnoreMissingSAISEIdinSRVRegion").GetValue<bool>();
            _ignorePeopleWithoutDoc =
                configurationSettingManager.Get("SaiseExporterJob_IgnorePeopleWithoutDoc").GetValue<bool>();

            _saiseElection = _saiseRepository.GetElection(saiseElectionId.Value);
            StageStatistic = new UpdateSaiseVoterStageStatistic();
        }

        public UpdateSaiseVoterStageStatistic StageStatistic { get; set; }

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
                _saiseRepository.ClearAssignedVoters(_saiseElection.Id);
                UpdateSrvAllVoterToBeExported();
            }

            IList<Person> people;

            Session.SetBatchSize(1000);

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
                    if (_interruptPending)
                    {
                        return;
                    }

                    people = GetBatchOfPeopleFromSRV();

                    if (!people.Any()) break;

                    try
                    {
                        List<Voter> voters = CreateVoters(people);

                        using (var transaction = _saiseRepository.Session.BeginTransaction())
                        {
                            _saiseRepository.InsertVotersInTemporaryTable(voters);
                            _saiseRepository.CopyDataFromTemporaryTableToVoter();
                            _saiseRepository.AssignVoterToElection(_saiseElection.Id);
                            transaction.Commit();
                        }

                        UpdatePersonExportStatus(people.Select(x => x.Id));
                        StageStatistic.Processed += people.Count;

                        WriteLog(LogType.Info,
                            string.Format("\n{0} : Processed {1} from {2} \n", DateTime.Now, StageStatistic.Processed,
                                StageStatistic.Total));
                    }
                    catch (Exception ex)
                    {
                        WriteLog(LogType.Error, string.Format("Unhandled exception occured: {0}", ex.Message), ex);
                    }

                    StageProgress.SetProgress(StageStatistic.Processed);
                } while (people.Count > 0);

            }

            WriteLog(LogType.Info, "Update AssignedVoter villages where -1");
            _saiseRepository.UpdateAssignedVoterVillage(_saiseElection.Id);

            WriteLog(LogType.Info, "Update AssignedVoter villages where AssignedVoter.VillageId <> AssignedVoter.PollingStation.VillageId");
            _saiseRepository.FixAssignedVotersVillages(_saiseElection.Id);

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
            _saisePollingStations = _saiseRepository.GetPollingStation(_saiseElection.Id);
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

            var address = personEligibleAddress.Address;
            var street = GetStreet(address.Street.Id);

            voter.StreetId = street.RopId;
            voter.StreetName = street.Name;
            voter.StreetNumber = address.HouseNumber;
            voter.StreetSubNumber = address.Suffix;
            voter.BlockNumber = personEligibleAddress.ApNumber;
            voter.BlockSubNumber = personEligibleAddress.ApSuffix;


            if (person.HasValidAgeForElectionDate(_saiseElection.DateOfElection) && 
                address.PollingStation != null && address.PollingStation.Id > 0)
            {
                AssignPollingStation(voter, address.PollingStation.Id, street);
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

            voter.PollingStationId = saisePollingStation.Id;

            var region = street.Region;

            if (!region.SaiseId.HasValue)
            {
                if (!_ignoreMissingSaiseIdinRegion)
                {
                    throw new SaiseExporterBusinessException(string.Format("Region {0}, missing SAISE ID ", region.Id));
                }
            }
            else
            {
                voter.RegionId = region.SaiseId.Value;
            }
        }

        private SaisePollingStation GetSaisePollingStationBySRVPollingStation(long srvPollingStaionId)
        {
            var srvPollingStation = _srvPollingStations.FirstOrDefault(x => x.Id == srvPollingStaionId);
            if (srvPollingStation == null)
            {
                throw new SaiseExporterBusinessException(string.Format("Cannot find SRV.PollingStation with Id: {0}. Probably assignement to deleted PollingStation.", srvPollingStaionId));
            }

            var saisePollingStationId = srvPollingStation.SaiseId;

            return _saisePollingStations.FirstOrDefault(x => x.Id == saisePollingStationId);
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
                .And(x => x.IsEligible == true).SingleOrDefault();
        }

        private PersonStatus GetCurrentPersonStatus(Person person)
        {
            return Session.QueryOver<PersonStatus>()
                .Fetch(x => x.StatusType).Eager
                .Where(x => x.Person == person)
                .And(x => x.IsCurrent == true).SingleOrDefault();
        }

        private long GetTotalPeopleToProcess()
        {
            return Session.QueryOver<Person>().Where(x => x.Deleted == null)
                .And(x => x.ExportedToSaise == false)
                .Select(Projections.RowCountInt64()).SingleOrDefault<long>();
        }


        private void UpdateSrvAllVoterToBeExported()
        {
            // Set exportedToSaise to 0 in order to be reexported
            const string query = "update People set exportedToSaise = 0";
            var updateQuery = Session.CreateQuery(query);

            updateQuery.ExecuteUpdate();
        }

        private void UpdatePersonExportStatus(IEnumerable<long> personIds)
        {
            string query = string.Format("update People set exportedToSaise = 1 where personId IN ({0})", string.Join(",", personIds));
            var updateQuery = Session.CreateQuery(query);
            updateQuery.ExecuteUpdate();
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
            return _streets.First(x => x.Id == streetId);
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