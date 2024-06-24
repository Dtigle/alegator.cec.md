using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris;
using Amdaris.Domain.Identity;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using NHibernate.Criterion;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Linq;

namespace CEC.SRV.BLL.Impl
{
    public class ImportBll : Bll, IImportBll
    {
        private readonly ISRVRepository _repository;
        private readonly ILookupCacheDataProvider _lookupCache;
        private readonly ILogger _logger;

        public ImportBll(ISRVRepository repository, ILookupCacheDataProvider lookupCacheDataProvider, ILogger logger) : base(repository)
        {
            _repository = repository;
            _lookupCache = lookupCacheDataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Use as main point to import from RspModificationData to SRV Person with modified personal information, status and address
        /// </summary>
        /// <param name="rawData">RspModificationData</param>
        /// <param name="status">Returned status of processing, like New, Update or Conflict</param>
        /// <returns></returns>
        public Person Import(RspModificationData rawData, ref StatisticChanges status)
        {
            var person = _repository.QueryOver<Person>().Where(x => x.Idnp == rawData.Idnp).SingleOrDefault();

            //if(rawData.Idnp== "2008004010423")
            //{
            //    int fd = 0;
            //}

            if (person == null)
            {
                var query = _repository.CreateSQLQuery(string.Format("SELECT {{p.*}} FROM SRV.People as p WHERE idnp = '{0}'", rawData.Idnp));

                query.AddEntity("p", typeof(Person));

                person = query.UniqueResult<Person>();
            }

            if (person == null)
            {

                person = GetPerson(rawData, ref status);
            }

            try
            {
                UpdateGeneralInformation(rawData, person, ref status);

                UpdateStatus(rawData, person, ref status);

                UpdateAddress(rawData, person, ref status);
            }
            catch (ConflictStatusException conflictStatusException)
            {
                _logger.Info(string.Format("Conflict de statut pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictStatusException.Message));
                rawData.SetStatusConflict(conflictStatusException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictStatus;
            }
            catch (ConflictAddressException conflictAddressException)
            {
                _logger.Info(string.Format("Conflict de adresa pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictAddressException.Message));
                rawData.SetAddressConflict(conflictAddressException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictRegionException conflictRegionException)
            {
                _logger.Info(string.Format("Conflict de regiune pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictRegionException.Message));
                rawData.SetRegionConflict(conflictRegionException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictStreetException conflictStreetException)
            {
                _logger.Info(string.Format("Conflict de stradă pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictStreetException.Message));
                rawData.SetStreetConflict(conflictStreetException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictPollingException conflictPollingException)
            {
                _logger.Info(string.Format("Conflict de sectie de votare pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictPollingException.Message));
                rawData.SetPollingStationConflict(conflictPollingException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictPolling;
            }
            catch (ConflictFatalAddressException conflictFatalAddressException)
            {
                _logger.Info(string.Format("Conflict de pollingstation pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictFatalAddressException.Message));
                rawData.SetFatalAddressConflict(conflictFatalAddressException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictStreetZeroException conflictStreetZeroException)
            {
                _logger.Info(string.Format("Conflict de pollingstation pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictStreetZeroException.Message));
                rawData.SetStreetZeroConflict(conflictStreetZeroException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (LocalityConflictException localityConflictException)
            {
                _logger.Info(string.Format("Conflict de sectie de votare pentru {0}, messaj : {1}", rawData.Idnp,
                    localityConflictException.Message));
                rawData.SetLocalityConflict(localityConflictException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictPolling;
            }

            return person;
        }

        /// <summary>
        /// Retrive Person with specified IDNP from SRV database. If person with such IDNP doesn't exist, it will create new person and add flag 'New' to status 
        /// </summary>
        private Person GetPerson(RspModificationData rawData, ref StatisticChanges status)
        {
            _logger.Info(string.Format("Create new person {0}", rawData.Idnp));
            status |= StatisticChanges.New;
            var person = new Person(rawData.Idnp)
            {
                Comments = "Importat din Rsp",
                FirstName = rawData.FirstName,
                MiddleName = rawData.SecondName,
                Surname = rawData.LastName,
                Gender = GetLookupByName<Gender>(rawData.GetSexCode()),
                DateOfBirth = rawData.Birthdate
            };


            var document = new PersonDocument
            {
                Seria = rawData.Seria,
                Number = rawData.Number,
                IssuedDate = rawData.IssuedDate,
                ValidBy = rawData.ValidBydate,
                Type = GetLookupById<DocumentType>(DocumentType.Parse((int)rawData.DocumentTypeCode))
            };
            person.Document = document;
            _repository.SaveOrUpdate(person);
            return person;
        }

        #region Resolve Conflict methods

        public void AcceptRspStatus(RspModificationData data)
        {
            var person = _repository.Query<Person>().SingleOrDefault(x => x.Idnp == data.Idnp);
            if (person == null) throw new SrvException(MUI.Conflict_StatusConflict_PeronsNotFound, data.Idnp);

            person.ModifyStatus(GetLookupByName<PersonStatusType>(data.GetPersonStatus()), "Importat din Rsp");

            data.AcceptConflict(ConflictStatusCode.StatusConflict);
            _repository.SaveOrUpdate(person);

            _repository.SaveOrUpdate(data);
        }

        public void RejectRspStatus(RspModificationData data)
        {
            var person = _repository.Query<Person>().SingleOrDefault(x => x.Idnp == data.Idnp);
            if (person == null) throw new SrvException(MUI.Conflict_StatusConflict_PeronsNotFound, data.Idnp);

            data.AcceptConflict(ConflictStatusCode.StatusConflict);
            //data.RejectConflict(ConflictStatusCode.StatusConflict);
            data.Comments = data.Comments.Truncate(255);

            _repository.SaveOrUpdate(data);
        }

        public void MapAddress(RspModificationData rawData, Address address, int appNr, string apSuffix, bool addressWasCreatedByUserForConflictSolving = false)
        {
            var person = _repository.Query<Person>().SingleOrDefault(x => x.Idnp == rawData.Idnp);
            var status = StatisticChanges.None;

            try
            {
                if (person == null)
                {
                    person = new Person(rawData.Idnp);
                }

                var basePersonAddress = person.EligibleAddress;

                var registrationData = rawData.Registrations.FirstOrDefault(x => x.IsInConflict);
                var personAddress = GetPersonAddress(address, person, registrationData);

                personAddress.ApNumber = appNr;
                personAddress.ApSuffix = apSuffix;

                ChangeAddress(person, personAddress);

                registrationData.SetConflicted(false);

                _repository.SaveOrUpdate(person);

                AddToMappingAddress(address, rawData);

                rawData.AcceptConflict(ConflictStatusCode.AddressConflict);
                rawData.SetEnd();
                _repository.SaveOrUpdate(rawData);

                // 
                // Send notification 
                //
                var notification =
                    addressWasCreatedByUserForConflictSolving
                        ? string.Format(
                            MUI.ConflictAddress_Resolve_Address_Added_MessageNotification,
                            rawData.Id, rawData.Idnp, address.GetFullAddress())
                        : string.Format(
                            MUI.ConflictAddress_Resolve_Address_Selected_MessageNotification,
                            rawData.Id, rawData.Idnp, address.GetFullAddress());

                var users1 = basePersonAddress != null ? Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(basePersonAddress.Address.Street.Region.Id))
                .HasPropertyIn(x => x.Id)
                .List() : new List<SRVIdentityUser>();

                var regionOfNewAddressId = personAddress.Address.Street.Region.Id;
                var users2 = Repository.QueryOver<SRVIdentityUser>()
                    .WithUdf(new UsersWithAccessToRegionCriterion(regionOfNewAddressId))
                    .HasPropertyIn(x => x.Id)
                    .List();

                var users = users1.Union(users2).Union(GetUsersToBeNotified()).Distinct();

                CreateNotification(EventTypes.Update, rawData, rawData.Id, notification, users);
            }
            catch (ConflictStatusException conflictStatusException)
            {
                _logger.Info(string.Format("Conflict de statut pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictStatusException.Message));
                rawData.SetStatusConflict(conflictStatusException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictStatus;
            }
            catch (ConflictAddressException conflictAddressException)
            {
                _logger.Info(string.Format("Conflict de adresa pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictAddressException.Message));
                rawData.SetAddressConflict(conflictAddressException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictRegionException conflictRegionException)
            {
                _logger.Info(string.Format("Conflict de regiune pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictRegionException.Message));
                rawData.SetRegionConflict(conflictRegionException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictStreetException conflictStreetException)
            {
                _logger.Info(string.Format("Conflict de stradă pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictStreetException.Message));
                rawData.SetStreetConflict(conflictStreetException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictPollingException conflictPollingException)
            {
                _logger.Info(string.Format("Conflict de pollingstation pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictPollingException.Message));
                rawData.SetPollingStationConflict(conflictPollingException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictPolling;
            }
            catch (ConflictFatalAddressException conflictFatalAddressException)
            {
                _logger.Info(string.Format("Conflict de pollingstation pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictFatalAddressException.Message));
                rawData.SetFatalAddressConflict(conflictFatalAddressException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (ConflictStreetZeroException conflictStreetZeroException)
            {
                _logger.Info(string.Format("Conflict de pollingstation pentru {0}, messaj : {1}", rawData.Idnp,
                    conflictStreetZeroException.Message));
                rawData.SetStreetZeroConflict(conflictStreetZeroException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictAddress;
            }
            catch (LocalityConflictException localityConflictException)
            {
                _logger.Info(string.Format("Conflict de sectie de votare pentru {0}, messaj : {1}", rawData.Idnp,
                    localityConflictException.Message));
                rawData.SetLocalityConflict(localityConflictException.Message);
                _repository.SaveOrUpdate(rawData);
                status |= StatisticChanges.ConflictPolling;
            }

        }

        public void AssignRspAddress(RspModificationData data)
        {
            var status = StatisticChanges.None;
            var person = _repository.Query<Person>().SingleOrDefault(x => x.Idnp == data.Idnp);
            if (person == null)
            {
                person = new Person(data.Idnp);
                UpdateGeneralInformation(data, person, ref status);
                UpdateStatus(data, person, ref status);
            }

            UpdateAddress(data, person, ref status);
            if (status.HasFlag(StatisticChanges.AddressUpdate))
                data.AcceptConflict(ConflictStatusCode.AddressConflict);


            _repository.SaveOrUpdate(person);
            _repository.SaveOrUpdate(data);
        }

        public void RejectRspAddress(RspModificationData data)
        {
            data.RejectConflict(ConflictStatusCode.AddressConflict);
            _repository.SaveOrUpdate(data);
        }

        public void AssignRspPollingStation(RspModificationData data)
        {
            data.AcceptConflict(ConflictStatusCode.PollingStationConflict);

            var person = _repository.Query<Person>().SingleOrDefault(x => x.Idnp == data.Idnp);
            if (person == null) throw new SrvException(MUI.Conflict_StatusConflict_PeronsNotFound, data.Idnp);

            _repository.SaveOrUpdate(data);
        }

        public void RejectRspPollingStation(RspModificationData data)
        {
            data.RejectConflict(ConflictStatusCode.PollingStationConflict);
            _repository.SaveOrUpdate(data);
        }

        public void AcceptRsvLocality(long conflictId)
        {
            var conflictRegistration = _repository.Get<RspRegistrationData>(conflictId);
            var conflict = conflictRegistration.RspModificationData;
            conflict.Comments = conflict.Comments == null
                                ? string.Format(MUI.Conflict_ChangeLocality_AcceptAddress_comment)
                                : string.Format("{0} {1}", conflict.Comments, MUI.Conflict_ChangeLocality_AcceptAddress_comment);
            conflict.AcceptConflict(ConflictStatusCode.LocalityConflict);
            _repository.SaveOrUpdate(conflict);
        }

        public void ResolveByMappingAddress(long conflictId, long addressId, long[] applyToConflicts)
        {
            var conflictEntriesToMap = new List<RspRegistrationData>();
            var conflictRegistrationData = _repository.Get<RspRegistrationData>(conflictId);
            conflictEntriesToMap.Add(conflictRegistrationData);
            if (applyToConflicts != null && applyToConflicts.Length > 0)
            {
                RspModificationData rspModificationData = null;
                var otherAlikeConflicts = _repository.QueryOver<RspRegistrationData>()
                    .JoinAlias(x => x.RspModificationData, () => rspModificationData)
                    .Where(x => x.Id != conflictId &&
                                x.Administrativecode == conflictRegistrationData.Administrativecode &&
                                x.StreetCode == conflictRegistrationData.StreetCode &&
                                x.HouseNumber == conflictRegistrationData.HouseNumber &&
                                x.HouseSuffix == conflictRegistrationData.HouseSuffix &&
                                x.IsInConflict == true
                    ).AndRestrictionOn(x => x.Id).IsIn(applyToConflicts)
                    .OrderBy(x => rspModificationData.Created).Asc
                    .List();

                conflictEntriesToMap.AddRange(otherAlikeConflicts);
            }

            foreach (var conflictEntry in conflictEntriesToMap)
            {
                ResolveInternal(addressId, conflictEntry);
            }
        }

        private void ResolveInternal(long addressId, RspRegistrationData conflictRegistrationData)
        {
            var conflictData = conflictRegistrationData.RspModificationData;
            conflictData.Comments = MUI.Conflict_MapAddress_comment;
            var address = _repository.Get<Address>(addressId);

            MapAddress(conflictData, address, conflictRegistrationData.ApartmentNumber.GetValueOrDefault(), conflictRegistrationData.ApartmentSuffix);
        }

        #endregion



        /// <summary>
        /// Update personal information of person transmitted as parameter. Check if need to update, if check will pass, update all personal information and add flag 'Update' to status
        /// </summary>
        public void UpdateGeneralInformation(RspModificationData rawData, Person person, ref StatisticChanges status)
        {
            if (IfAllFieldsAreTheSame(person, rawData)) return;
            _logger.Info(string.Format("Update general information for person {0}", rawData.Idnp));
            person.Comments = "Importat din Rsp";
            person.FirstName = rawData.FirstName;
            person.MiddleName = rawData.SecondName;
            person.Surname = rawData.LastName;
            person.Gender = GetLookupByName<Gender>(rawData.GetSexCode());
            person.DateOfBirth = rawData.Birthdate;

            var document = new PersonDocument
            {
                Seria = rawData.Seria,
                Number = rawData.Number,
                IssuedDate = rawData.IssuedDate,
                ValidBy = rawData.ValidBydate,
                Type = GetLookupById<DocumentType>(DocumentType.Parse((int)rawData.DocumentTypeCode))
            };
            person.Document = document;

            status |= StatisticChanges.Update;
        }

        /// <summary>
        /// Update status of person. If in SRV this person is dead, but come to update status as live - throw ConflictStatus exception. 
        /// On update status for person, will add flag  StatusUpdate' to status
        /// </summary>
        public void UpdateStatus(RspModificationData rawData, Person person, ref StatisticChanges status)
        {
            if (person.CurrentStatus != null
                && person.CurrentStatus.StatusType == GetLookupByName<PersonStatusType>(rawData.GetPersonStatus()))
                return;

            if (person.CurrentStatus != null && person.CurrentStatus.IsCurrent &&
                person.CurrentStatus.StatusType.Name == "Decedat" && !rawData.Dead)
            {
                throw new ConflictStatusException(MUI.Conflict_StatusConflictErrorMessage);
            }

            _logger.Info(string.Format("Update status for person {0}", rawData.Idnp));
            status |= StatisticChanges.StatusUpdate;
            person.ModifyStatus(GetLookupByName<PersonStatusType>(rawData.GetPersonStatus()), "Importat din Rsp");
        }

        /// <summary>
        /// Update addresses of person. Modify Address, add new type of addresses(like temprorar or declaration).
        /// If from rawData come person without address, it will move to special region(region without addresses).
        /// Add flag 'AddressUpdate' to status
        /// </summary>
        public void UpdateAddress(RspModificationData rawData, Person person, ref StatisticChanges status)
        {
            if (rawData.Registrations.Any(x => x.RegTypeCode == null) || !rawData.Registrations.Any())
                MoveToSpecialRegion(person, rawData, ref status);
            else
            {
                //
                // GreenSoft: Fix. Procesare intii adresa de resedinta apoi adresa temporara, daca exista
                //
                IList<RspRegistrationData> ordered = rawData.Registrations.OrderBy(x => x.RegTypeCode).ToList();
                var hasDeclaration = person.Addresses.Any(x => x.PersonAddressType.Id == PersonAddressType.Declaration && x.Deleted == null && x.IsNotExpired());

                foreach (var registrationData in ordered /*rawData.Registrations*/)
                {
                    PersonAddress address = UpdateAddress(rawData, registrationData, person, ref status);

                    //
                    // GreenSoft: Fix decide which is eligible address
                    //
                    if (
                        address.PersonAddressType.Id == PersonAddressType.Residence || // noua adresa de resedinta devine eligibila - atentie ne bazam ca procesam in ordine adresele
                        (!hasDeclaration && (address.PersonAddressType.Id == PersonAddressType.Temporary && address.IsNotExpired())))
                    {
                        foreach (var personAddress in person.Addresses.Where(x => x.IsEligible))
                        {
                            personAddress.IsEligible = false;
                        }

                        address.IsEligible = true;

                        _logger.Debug("GreenSoft Fix: AdressId is " + address.Id);

                        _repository.SaveOrUpdate(address);
                    }
                }
            }
        }

        /// <summary>
        /// Move person to region without address(region with Id -1), and add flag 'AddressUpdate' to status
        /// </summary>
        private void MoveToSpecialRegion(Person person, RspModificationData rawData, ref StatisticChanges status)
        {
            var address = _repository.Query<Address>()
                .FirstOrDefault(x => x.Street.StreetType.Id == StreetType.UnknownStreetType
                                && x.Street.Region.Id == Region.NoResidenceRegionId);
            if (address == null)
            {
                throw new ConflictFatalAddressException(MUI.Conflict_MissingNoResidenceRegionError);
            }
            var existingPersonAddresses = _repository.Query<PersonAddress>().Where(x => x.Person.Id == person.Id).ToList();
            foreach (var existingPersonAddress in existingPersonAddresses)
            {
                _repository.Delete(existingPersonAddress);
            }

            if (
                existingPersonAddresses.Any(
                    x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId))
            {
                var firstOrDefault = existingPersonAddresses.FirstOrDefault(x => x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                                                                                 x.Address.Street.Region.Id == Region.NoResidenceRegionId);
                if (firstOrDefault != null)
                    _repository.UnDelete<PersonAddress>(firstOrDefault.Id);
            }
            else
            {
                var personAddress = new PersonAddress()
                {
                    Address = address,
                    Person = person,
                    PersonAddressType = GetLookupById<PersonAddressType>(PersonAddressType.NoResidence),
                    IsEligible = true,
                };

                _repository.SaveOrUpdate(person);
                ChangeAddress(person, personAddress);
                status |= StatisticChanges.AddressUpdate;
            }

            _logger.Info(string.Format("Move person {0} in special region", rawData.Idnp));
        }

        private PersonAddress UpdateAddress(RspModificationData rawData, RspRegistrationData registrationData, Person person, ref StatisticChanges status)
        {
            var personAddress = GetPersonAddress(rawData, registrationData, person, ref status);
            _logger.Info(string.Format("Update Address for person {0} in region {1} , localitatea {2} {3} (suffix {4})",
                person.Idnp, registrationData.Region, registrationData.Locality, registrationData.HouseNumber, registrationData.HouseSuffix));
            return ChangeAddress(person, personAddress);
        }

        private PersonAddress GetPersonAddress(RspModificationData rawData, RspRegistrationData registrationData, Person person, ref StatisticChanges status)
        {
            var region = GetRegion(registrationData);
            var street = GetStreet(region, registrationData, rawData);
            var address = GetAddress(street, registrationData, rawData);

            var personAddress = GetPersonAddress(address, person, registrationData);

            if (personAddress.ApNumber != registrationData.ApartmentNumber
                || personAddress.ApSuffix != registrationData.ApartmentSuffix
                || personAddress.DateOfRegistration != registrationData.DateOfRegistration
                || personAddress.DateOfExpiration != (registrationData.DateOfExpiration ?? DateTime.MinValue))
                status |= StatisticChanges.AddressUpdate;

            personAddress.ApNumber = registrationData.ApartmentNumber;
            personAddress.ApSuffix = registrationData.ApartmentSuffix;
            personAddress.DateOfRegistration = registrationData.DateOfRegistration;
            personAddress.DateOfExpiration = registrationData.DateOfExpiration;

            return personAddress;
        }

        private Address GetAddress(Street street, RspRegistrationData registrationData, RspModificationData rawData)
        {
            Address address;

            if (street.RopId != 9999)
                address = _repository.QueryOver<Address>()
                    .Where(x => x.Street == street)
                    .And(x => x.HouseNumber == (registrationData.HouseNumber.HasValue ? registrationData.HouseNumber : 0))
                    .And(x => x.Suffix == registrationData.GetHouseSuffix())
                    .And(x => x.Deleted == null).SingleOrDefault<Address>();
            else
                // In case we have few address for same tuple region & street, apply Take(1). For case when have values NULL and '-' , to take priority on NULL - applu order by {field} asc
                address = _repository.QueryOver<Address>()
                    .Where(x => x.Street == street)
                    .And(x => x.Deleted == null).OrderBy(f => f.HouseNumber).Asc.OrderBy(f => f.Suffix).Asc.Take(1).SingleOrDefault();

            if (address == null)
            {
                var id = _repository.Query<MappingAddress>().FirstOrDefault(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                                                                 x.RspStreetCode == registrationData.StreetCode &&
                                                                                 x.RspHouseNr == registrationData.HouseNumber &&
                                                                                 x.RspHouseSuf == registrationData.GetHouseSuffix());
                if (id != null)
                {
                    return _repository.Get<Address>(id.SrvAddressId);
                }
            }

            if (address == null)
            {
                if (rawData.Status != RawDataStatus.ForceImport)
                {

                    // ----------------------------------------------------------------------------
                    // GreenSoft: 
                    //   - Adaugare automata de adresa si alocare la o sectie de vot, functie de HouseNumber
                    //   - Daca exista adrese pe acelasi HouseNumber alocate la mai multe sectii, se genereaza un conflict de sectie
                    // ----------------------------------------------------------------------------


                    //
                    // Determinare sectii de vot pe HouseNumber
                    //
                    var pollingStations = _repository.QueryOver<Address>()
                            .Where(x => x.Street == street)
                            .And(x => x.HouseNumber == registrationData.HouseNumber)
                            .And(x => x.Deleted == null)
                            .And(x => x.PollingStation != null)
                            .Select(p => p.PollingStation)
                            .List<PollingStation>().Distinct();

                    if (pollingStations.Count() == 1)
                    {
                        //
                        // Adaugare automata adresa si asignare la sectie de vot
                        // TODO: Marcare adresa "Adaugata automat prin metoda GreenSoft"
                        //
                        address = new Address
                        {
                            BuildingType = BuildingTypes.Undefined,
                            HouseNumber = registrationData.HouseNumber,
                            Suffix = registrationData.GetHouseSuffix(),
                            Street = street,
                            PollingStation = pollingStations.ElementAt(0)
                        };

                        _logger.Info(string.Format("Inregistrare adresa noua pe baza numarului de strada: {0}", address.GetFullAddress(true, true, true)));

                        _repository.SaveOrUpdate(address);

                        AddToMappingAddress(address, rawData);

                        return address;
                    }
                    else if (pollingStations.Count() > 1)
                    {
                        SetRegistrationInConflict(registrationData);
                        throw new ConflictPollingException(
                            string.Format(MUI.Conflict_PollingStationAmbigous_StreetHouseNumber,
                                registrationData.StreetCode, registrationData.Administrativecode,
                                registrationData.HouseNumber, street.Name, street.Region.Name));
                    }

                    // ----------------------------------------------------------------------------
                    // Nu s-a reusit identificare PollingStation dupa HouseNumber
                    // Se ramine in situatide de conflict
                    // ----------------------------------------------------------------------------


                    SetRegistrationInConflict(registrationData);
                    throw new ConflictAddressException(string.Format(MUI.Conflict_RegionConflict_StreetBlocNuExista,
                        registrationData.StreetCode, registrationData.Administrativecode,
                        registrationData.HouseNumber,
                        registrationData.GetHouseSuffix(), street.Name, street.Region.Name));
                }

                address = new Address
                {
                    BuildingType = BuildingTypes.Undefined,
                    HouseNumber = registrationData.HouseNumber,
                    Suffix = registrationData.GetHouseSuffix(),
                    Street = street,
                };

                _repository.SaveOrUpdate(address);

                AddToMappingAddress(address, rawData);
            }
            return address;
        }

        private Street GetStreet(Region region, RspRegistrationData registrationData, RspModificationData rawData)
        {
            if (registrationData.StreetCode == 0)
            {
                throw new ConflictStreetZeroException(string.Format(MUI.Conflict_RegionConflict_StreetCodeInRegion,
                        registrationData.StreetCode, region.Name, registrationData.Administrativecode, registrationData.StreetName));
            }

            var street = _repository.Query<Street>().FirstOrDefault(x => x.Region == region && x.RopId == registrationData.StreetCode);

            if (street == null)
            {
                //if (rawData.Status != RawDataStatus.ForceImport)
                //{
                //    SetRegistrationInConflict(registrationData);
                //    throw new ConflictStreetException(string.Format(MUI.Conflict_RegionConflict_StreetCodeInRegion,
                //        registrationData.StreetCode, region.Name, registrationData.Administrativecode, registrationData.StreetName));
                //}

                var streetTypeCode = _lookupCache.StreetTypeCodeCache.GetByStreetTypeCode(registrationData.StreetCode);
                if (streetTypeCode != null && streetTypeCode.RspStreetTypeCodeId != 9999)
                {
                    var streetTypeLookup = GetStreetType(streetTypeCode.Docprint);

                    street = _repository.Query<Street>().FirstOrDefault(x => x.Region == region && x.Name == streetTypeCode.Name && x.StreetType == streetTypeLookup);
                    if (street == null)
                    {
                        street = new Street(region, streetTypeLookup, streetTypeCode.Name, true) { RopId = registrationData.StreetCode };
                    }
                    else
                    {
                        street.RopId = registrationData.StreetCode;
                    }
                    _repository.SaveOrUpdate(street);
                }
                else
                {
                    SetRegistrationInConflict(registrationData);
                    throw new ConflictStreetException(string.Format(MUI.Conflict_RegionConflict_StreetCodeInRegion,
                        registrationData.StreetCode, region.Name, registrationData.Administrativecode, registrationData.StreetName));
                }
            }

            return street;
        }

        private StreetType GetStreetType(string streetName)
        {
            var allTypes = _lookupCache.StreetTypeCache.GetAll();
            foreach (var streetType in allTypes)
            {
                if (streetName.Contains(streetType.Name))
                    return streetType;
            }
            return allTypes.Single(x => x.Name == "str.");
        }

        /// <summary>
        ///  Use to retrieve Region lookup, which is not from Problematic region, and doesn't have conflict with polling station.
        ///  Retrieve Region lookup by AdministrativeCode or StatisticCode (CUATM). Can throw ConflictAddressException
        /// </summary>
        private Region GetRegion(RspRegistrationData registrationData)
        {
            Region region;
            var administrativeCode = registrationData.Administrativecode;
            if (administrativeCode < 1000000)
                region = _lookupCache.RegionCache.GetByStatisticCode(administrativeCode);
            else
            {
                var regions = _lookupCache.RegionCache.GetByRegistruId(administrativeCode);
                region = regions.Count() == 1
                    ? regions.Single()
                    : regions.SingleOrDefault(x => x.HasStreets && x.Level == regions.Max(r => r.Level));
            }

            if (region == null)
            {
                SetRegistrationInConflict(registrationData);
                throw new ConflictRegionException(string.Format(MUI.Conflict_RegoinConflict_RegionNuExista,
                    registrationData.Administrativecode));
            }

            if (_lookupCache.LinkedRegionCache.GetByRegion(region.Id).Any())
            {
                SetRegistrationInConflict(registrationData);
                throw new LocalityConflictException(string.Format(MUI.Conflict_RegoinConflict,
                    registrationData.Administrativecode));
            }

            return region;
        }

        private void SetRegistrationInConflict(RspRegistrationData registrationData)
        {
            registrationData.SetConflicted(true);
            _repository.SaveOrUpdate(registrationData);
        }

        /// <summary>
        /// Return PersonAddress existent or create new one for tuple Person & Address.
        /// When create new  person address, check for PollingSTation conflict.
        /// If person already has address in region without street and many PollStations, don't throw conflict.
        /// </summary>
        private PersonAddress GetPersonAddress(Address address, Person person, RspRegistrationData registrationData)
        {
            var personAddress = _repository.Query<PersonAddress>()
                .FirstOrDefault(x => x.Address == address && x.Person == person
                //
                // GreenSoft: Dupa modificare constraint prin adaugare coloana peronAddressTypeId 
                // [UX_PersonAddresses_PersonId_AddressId] UNIQUE(personId asc, addressId asc, peronAddressTypeId asc, deleted asc),
                //
                && x.PersonAddressType.Id == (registrationData.RegTypeCode ?? 0));

            if (personAddress == null)
            {
                var region = address.Street.Region;

                if (!person.Addresses.Any(x => x.Address.Street.Region == region))
                {
                    if (address.PollingStation == null && !region.HasStreets && _lookupCache.PollingStationCache.GetByRegion(region.Id).Count() > 1)
                    {
                        SetRegistrationInConflict(registrationData);
                        throw new ConflictPollingException(string.Format(MUI.Conflict_PollingStationAmbigous, registrationData.Locality, registrationData.Administrativecode, registrationData.Region));
                    }
                }

                personAddress = new PersonAddress()
                {
                    Address = address,
                    Person = person,
                    PersonAddressType =
                        GetLookupById<PersonAddressType>(registrationData.RegTypeCode ?? 0),
                };
            }
            else
            {
                personAddress.ApNumber = registrationData.ApartmentNumber;
                personAddress.ApSuffix = registrationData.ApartmentSuffix;
                personAddress.DateOfRegistration = registrationData.DateOfRegistration;
                personAddress.DateOfExpiration = registrationData.DateOfExpiration;
            }

            return personAddress;

        }

        private PersonAddress ChangeAddress(Person person, PersonAddress personAddress)
        {
            var hasDeclaration =
                person.Addresses.Any(x => x.PersonAddressType.Id == PersonAddressType.Declaration && x.Deleted == null);

            if (personAddress.PersonAddressType.Id == PersonAddressType.Residence)
            {
                var address =
                    person.Addresses.SingleOrDefault(x => x.PersonAddressType.Id == PersonAddressType.Residence && x.Deleted == null);
                if (address == null)
                {
                    var temporaryResidence = person.Addresses.FirstOrDefault(x => x.PersonAddressType.Id == PersonAddressType.Temporary);

                    if (!hasDeclaration && (temporaryResidence == null))//  temporaryResidence != null && !temporaryResidence.IsNotExpired()))
                    {
                        person.SetEligibleAddress(personAddress);
                    }
                    else
                    {
                        person.AddAddress(personAddress);
                    }

                    _repository.SaveOrUpdate(personAddress);
                }
                else
                {
                    address.Address = personAddress.Address;
                    address.ApNumber = personAddress.ApNumber;
                    address.ApSuffix = personAddress.ApSuffix;
                    address.DateOfExpiration = personAddress.DateOfExpiration;
                    address.DateOfRegistration = personAddress.DateOfRegistration;
                    person.SetEligibleAddress(address);
                    _repository.SaveOrUpdate(address);

                    return address;
                }
            }
            else if (personAddress.IsNotExpired())
            {
                if (hasDeclaration)
                {
                    person.AddAddress(personAddress);
                }
                else
                {
                    person.SetEligibleAddress(personAddress);
                }
                _repository.SaveOrUpdate(personAddress);
            }

            return personAddress;
        }

        private void AddToMappingAddress(Address address, RspModificationData rawData)
        {
            //
            // GreenSoft: Temporar am comentat generearea de mapari, din urmatorul motiv:
            //            La rezolvare conflict de adrese ce vine cu 2 adrese, una de resedinta si una temporara
            //            se adauga mapari gresite
            //


            //            foreach (var registration in rawData.Registrations.Where(x => x.IsInConflict))
            //            {
            //                _repository.SaveOrUpdate(new MappingAddress()
            //                {
            //                    SrvAddressId = address.Id,
            //                    RspAdministrativeCode = registration.Administrativecode,
            //                    RspStreetCode = registration.StreetCode,
            //                    RspHouseNr = registration.HouseNumber,
            //                    RspHouseSuf = registration.GetHouseSuffix()
            //                });
            //            }

        }

        private bool IfAllFieldsAreTheSame(Person person, RspModificationData rawData)
        {
            return person.FirstName == rawData.FirstName &&
                   person.MiddleName == rawData.SecondName &&
                   person.Surname == rawData.LastName &&
                   person.Gender == GetLookupByName<Gender>(rawData.GetSexCode()) &&
                   person.DateOfBirth == rawData.Birthdate &&
                   person.Document != null && person.Document.Number == rawData.Number &&
                   person.Document.Seria == rawData.Seria &&
                   person.Document.ValidBy == rawData.ValidBydate;

        }

        private T GetLookupByName<T>(string name) where T : Lookup
        {
            T result = _repository.Query<T>().FirstOrDefault(x => x.Name == name);
            if (result == null)
                throw new SrvException(MUI.ObjectNotFound,
                    string.Format("Can not find lookup typeof({0}) with name {1}", typeof(T), name));

            return result;
        }

        private T GetLookupById<T>(long id) where T : Lookup
        {
            T result = _repository.Query<T>().FirstOrDefault(x => x.Id == id);
            if (result == null)
                throw new SrvException(MUI.ObjectNotFound,
                    string.Format("Can not find lookup typeof({0}) with name {1}", typeof(T), id));

            return result;
        }

        private IList<SRVIdentityUser> GetUsersToBeNotified()
        {
            var role = Repository.Query<IdentityRole>().FirstOrDefault(x => x.Name == Transactions.Administrator);
            var users = Repository.Query<SRVIdentityUser>().Where(x => x.Roles.Contains(role)).ToList();
            return users;
        }
    }
}