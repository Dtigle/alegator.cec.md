using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using NHibernate.Criterion;
using Quartz;

namespace CEC.QuartzServer.Jobs.ListsPrinting
{
    [DisallowConcurrentExecution]
    public class ExpiredAddressSwapJob : SrvJob
    {
        private readonly ISRVRepository _repository;
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;

        public ExpiredAddressSwapJob(ISRVRepository repository, ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _repository = repository;
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;
            _logger.Info("ExpiredAddressSwapJob instantiated");
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            _logger.Info("ExpiredAddressSwapJob started");
            try
            {
                using (var uow = new NhUnitOfWork())
                {
                    DateTime electionDate = _configurationSettingManager.Get("SRV_ElectionDate").GetValue<DateTime>();
                    string idnp = _configurationSettingManager.Get("ExpiredAddressSwapJob_IDNP").GetValue<string>();
                    var persons = GetPeopleWithExpiredCurrentAddress(electionDate, idnp);

                    _logger.Info($"{persons.Count} people with expired current address found.");
                    var progressInfo = Progress.CreatStageProgressInfo("Updating expired addresses", 0, persons.Count);

                    foreach (var person in persons)
                    {
                        var currentEligibleAddress = person.EligibleAddress;
                        if (currentEligibleAddress == null)
                        {
                            _logger.Info($"No eligible address for {person.Idnp}");
                        }
                        else
                        {
                            var newAddress = MoveToAddress(person, GetNextAddressTypeId(currentEligibleAddress.PersonAddressType.Id));

                            _logger.Info($"Eligible address for {person.Idnp} changed from PersonAdressId: {currentEligibleAddress.Id} (type:{currentEligibleAddress.PersonAddressType.Name}, AddressId:{currentEligibleAddress.Address.Id}) to PersonAddressId: {newAddress.Id} (type:{newAddress.PersonAddressType.Name}, AddressId:{newAddress.Address.Id})");
                        }
                        progressInfo.Increase();
                    }

                    uow.Complete();

                    _logger.Info($"{persons.Count} people with expired current address found.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            _logger.Info("ExpiredAddressSwapJob done");
        }
        
        /// <summary>
        /// From -> To
        /// </summary>
        private IDictionary<long, long> AddressTypesOrder = new Dictionary<long, long>()
        {
            { PersonAddressType.Declaration, PersonAddressType.Temporary },
            { PersonAddressType.Temporary, PersonAddressType.Residence },
            { PersonAddressType.Residence, PersonAddressType.NoResidence },
            { PersonAddressType.NoResidence, PersonAddressType.NoResidence }
        };

        private long GetNextAddressTypeId(long currentAddressTypeId)
        {
            if (!AddressTypesOrder.ContainsKey(currentAddressTypeId))
            {
                throw new ApplicationException("Tip adresa expirata necunoscut, (" + currentAddressTypeId +" ).");
            }

            long newAddressTypeId;

            AddressTypesOrder.TryGetValue(currentAddressTypeId, out newAddressTypeId);

            return newAddressTypeId;
        }

        private PersonAddress MoveToAddress(Person person, long newAddressTypeId)
        {
            if (PersonAddressType.NoResidence == newAddressTypeId)
            {
                return MoveToSpecialRegion(person);
            }
            
            var newAddress = person.Addresses.FirstOrDefault(x => x.PersonAddressType.Id == newAddressTypeId);

            //
            // Is Valid address
            //
            if (newAddress != null &&
                (newAddress.DateOfExpiration == null ||
                 newAddress.DateOfExpiration.Value.CompareTo(DateTime.Now) > 0))
            {


                person.SetEligibleAddress(newAddress);
                _repository.SaveOrUpdate(person);

                return newAddress;
            }

            //
            // Search deeper
            //
            return MoveToAddress(person, GetNextAddressTypeId(newAddressTypeId));
        }

        /// <summary>
        /// Move person to region without address
        /// Inspired from ImportBll.MoveToSpecialRegion
        /// </summary>
        /// <param name="person"></param>
        private PersonAddress MoveToSpecialRegion(Person person)
        {
            var address = _repository.Query<Address>()
                .FirstOrDefault(x => x.Street.StreetType.Id == StreetType.UnknownStreetType
                                && x.Street.Region.Id == Region.NoResidenceRegionId);
            if (address == null)
            {
                throw new ConflictFatalAddressException("Eroare fatală, lipsește regiunea specială cu ID = -1");
            }

            PersonAddress personAddress = _repository.QueryOver<PersonAddress>()
                .Where(x => x.Address.Id == address.Id && x.Person.Id == person.Id)
                .SingleOrDefault();

            if (personAddress == null)
            {
                personAddress = new PersonAddress()
                {
                    Address = address,
                    Person = person,
                    PersonAddressType = _repository.Get<PersonAddressType>(PersonAddressType.NoResidence),
                    IsEligible = true,
                };
            }
            
            person.SetEligibleAddress(personAddress);

            var session = _repository.GetSession();// SaveOrUpdate(person);

            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(person);
                session.SaveOrUpdate(personAddress);
                transaction.Commit();
            }

            return personAddress;
        }

        private IList<Person> GetPeopleWithExpiredCurrentAddress(DateTime electionDate, string idnp)
        {
            PersonAddress pa = null;
            PersonStatus ps = null;
            PersonStatusType pst = null;
            Address a = null;
            Street s = null;
            Person p = null;

            object[] idnpArray = null;

            var query = _repository.QueryOver(() => p)
                .JoinAlias(x => x.Addresses, () => pa)
                .JoinAlias(x => x.PersonStatuses, () => ps)
                .JoinAlias(() => ps.StatusType, () => pst)
                .JoinAlias(() => pa.Address, () => a)
                .JoinAlias(() => a.Street, () => s)
                .Where(x =>
                    ps.IsCurrent && !pst.IsExcludable && 
                    pa.IsEligible && pa.DateOfExpiration != null && pa.DateOfExpiration < electionDate &&
                    pa.PersonAddressType.Id.IsIn(new [] {
                        PersonAddressType.Residence,
                        PersonAddressType.Temporary,
                        PersonAddressType.Declaration
                    }) &&
                    s.Region.Id != Region.NoResidenceRegionId
                    /*&& p.Idnp.IsIn(new object[] { "0950512541813", "0950512899329"}) */
                );

            if (!string.IsNullOrEmpty(idnp) && (idnpArray = idnp.Split(',').Select(x => x.Trim()).ToArray<object>()).Length > 0)
            {
                _logger.Info($"Processing for people(s) with idnp: {string.Join(",", idnpArray)}");
                query = query.Where( () => p.Idnp.IsIn(idnpArray));
            }

            return query.List<Person>();
        }
    }
}
