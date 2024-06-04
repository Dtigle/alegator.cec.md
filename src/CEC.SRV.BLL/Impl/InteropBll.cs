using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Interop;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using Transaction = CEC.SRV.Domain.Interop.Transaction;


namespace CEC.SRV.BLL.Impl
{
    public class InteropBll : Bll, IInteropBll
    {
        private readonly IVotersBll _votersBll;

        public InteropBll(ISRVRepository repository, IVotersBll votersBll) : base(repository)
        {
            _votersBll = votersBll;
        }

        public void SaveOrUpdateInteropSystem(long? id, string name, string description, TransactionProcessingTypes transactionProcessType,
            bool personStatusConsignment, long? personStatusTypeId, bool temporaryAddressConsignment
            )
        {
            var entity = id == null ? new InteropSystem() : Get<InteropSystem>((long)id);

            entity.Name = name;
            entity.Description = description;
            entity.TransactionProcessingType = transactionProcessType;
            entity.PersonStatusConsignment = personStatusConsignment;
            if (personStatusConsignment && personStatusTypeId.HasValue)
            {
                entity.PersonStatusType = Get<PersonStatusType>(personStatusTypeId.Value);
            }
            else
            {
                entity.PersonStatusType = null;
            }
            
            entity.TemporaryAddressConsignment = temporaryAddressConsignment;

            Repository.SaveOrUpdate(entity);
        }


        public void SaveOrUpdateInstitution(long? id, string name, string description, long institutionTypeId, long legacyId, 
            long addressId)
        {

            var entity = id == null ? new Institution() : Get<Institution>((long)id);

            entity.Name = name;
            entity.Description = description;
            entity.InteropSystem = Get<InteropSystem>(institutionTypeId);
            entity.InstitutionAddress = Get<Address>(addressId);
            entity.LegacyId = legacyId;

            Repository.SaveOrUpdate(entity);
        }

        public void SaveOrUpdateTransaction(long? id, string idnp, string lastName, string firstName, DateTime dateOfBirth, long institutionTypeId,
            long? institutionId)
        {
            var entity = id == null ? new Transaction
            {
                TransactionStatus = TransactionStatus.New
            } : Get<Transaction>((long)id);

            entity.Idnp = idnp;
            entity.LastName = lastName;
            entity.FirstName = firstName;
            entity.DateOfBirth = dateOfBirth;
            entity.InteropSystem = Get<InteropSystem>(institutionTypeId);
            entity.Institution = institutionId.HasValue ? Get<Institution>(institutionId.Value) : null;
            
            Repository.SaveOrUpdate(entity);
        }

        public bool IsUnique(long? id, long institutionTypeId, long legacyId)
        {
            var institution =
                Repository.Query<Institution>()
                    .FirstOrDefault(x => x.LegacyId == legacyId && x.InteropSystem.Id == institutionTypeId);

            return institution == null || institution.Id == id;
        }

        public PageResponse<InteropSystem> SearchInstitutionTypes(PageRequest pageRequest)
        {
            return Repository.QueryOver<InteropSystem>()
                .RootCriteria
                .CreatePage<InteropSystem>(pageRequest);
        }

        public PageResponse<Institution> SearchInstitutions(PageRequest pageRequest, long? institutionTypeId)
        {
            var query = Repository.QueryOver<Institution>();

            if (institutionTypeId.HasValue)
            {
                query = query.Where(x => x.InteropSystem.Id == institutionTypeId.Value);
            }
                
            return query.RootCriteria.CreatePage<Institution>(pageRequest);
        }

        public bool VerificationSameSystemAndInstitution(List<long> transactionIds)
        {
            InteropSystem interopSystem = null;
            Institution institution = null;

            foreach (var transactionId in transactionIds)
            {
                var transaction = Get<Transaction>(transactionId);

                //
                // Save first interop sistem and institution
                //
                if (interopSystem == null)
                {
                    interopSystem = transaction.InteropSystem;
                    institution = transaction.Institution;
                }
                else
                {
                    //
                    // Check if same interop system
                    //
                    if (transaction.InteropSystem.Id != interopSystem.Id)
                    {
                        return false;
                    }

                    //
                    // Check if same interop institution
                    //
                    if (institution != null && transaction.Institution == null ||
                        institution == null && transaction.Institution != null ||
                        institution != null && transaction.Institution != null && institution.Id != transaction.Institution.Id)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void ProcessTransactions(List<long> transactionIds, long pollingStationId, long electionId, ref int success, ref int error)
        {
            VerificationSameSystemAndInstitution(transactionIds);

            var pollingStation = Get<PollingStation>(pollingStationId);
            var election = Get<Election>(electionId);
            
            foreach (var transactionId in transactionIds)
            {
                var transaction = Get<Transaction>(transactionId);

                try
                {
                    ProcessTransaction(transaction, pollingStation, election);
                    SetTransactionSuccess(transaction);
                    success++;
                }
                catch (Exception ex)
                {
                    SetTransactionError(transaction, ex.Message);
                    error++;
                }

                Repository.SaveOrUpdate(transaction);
            }
        }

        private void ProcessTransaction(Transaction transaction, PollingStation pollingStation, Election election)
        {
            if (transaction.TransactionStatus == TransactionStatus.Success)
            {
                throw new ApplicationException(
                    $"Tranzactia cu id ${transaction.Id} se afla in stare invalida pentru procesare, {transaction.TransactionStatus}"
                );
            }
            
            var person = Repository.QueryOver<Person>().Where(x => x.Idnp == transaction.Idnp).SingleOrDefault();

            if (person == null)
            {
                throw new ApplicationException($"Nu exista persoana cu INDP: {transaction.Idnp}");
            }

            InteropSystem interopSystem = transaction.InteropSystem;

            //
            // Change person status
            //
            if (interopSystem.PersonStatusConsignment)
            {
                PersonStatusType newPersonStatus = interopSystem.PersonStatusType;

                if (newPersonStatus == null)
                {
                    throw new ArgumentNullException("PersonStatusType is not defined for interop system " + interopSystem.Name);
                }

                if (person.CurrentStatus.StatusType.Id == newPersonStatus.Id)
                {
                    throw new ApplicationException($"Persoana cu idnp:{person.Idnp} este deja in starea {person.CurrentStatus.StatusType.Name}");
                }

                _votersBll.UpdateStatus(person.Id, newPersonStatus.Id, "Interop " + interopSystem.Name);
            }


            //
            // Change temporary address
            //
            if (interopSystem.TemporaryAddressConsignment)
            {
                var oldAddress = person.EligibleAddress?.Address;
                var oldPollingStation = person.EligibleAddress?.Address.PollingStation;
                var oldPollingStationNumber = oldPollingStation != null ? oldPollingStation.FullNumber : "[N/A]";

                // 
                // Assign polling stations's address 
                //
                var pollingStationAddress = pollingStation.PollingStationAddress;
                if (pollingStationAddress == null)
                {

                    var unknownStreetType = Get<StreetType>(StreetType.UnknownStreetType);
                    var virtualStreetName = $"${pollingStation.Id}";

                    var baseAddress = pollingStation.Addresses
                        .FirstOrDefault(x => x.Street.StreetType.Id == StreetType.UnknownStreetType &&
                                             x.Street.Name == virtualStreetName &&
                                             x.HouseNumber.GetValueOrDefault() == 0);
                    if (baseAddress == null)
                    {
                        var street = new Street(pollingStation.Region, unknownStreetType, virtualStreetName, true)
                        {
                            Description = $"Autogenerated for PS: {pollingStation.FullNumber}"
                        };
                        baseAddress = new Address { Street = street, PollingStation = pollingStation };
                        pollingStation.AddAddress(baseAddress);
                        Repository.SaveOrUpdate(street);
                        Repository.SaveOrUpdate(baseAddress);
                        Repository.SaveOrUpdate(pollingStation);
                    }

                    pollingStationAddress = baseAddress;
                }


                var currentAddress = person.EligibleAddress;

                //
                // Persoana nu are adresa eligibila sau este de resedinta sau temporara
                //
                if (currentAddress == null ||
                    currentAddress.PersonAddressType.Id == PersonAddressType.Residence ||
                    currentAddress.PersonAddressType.Id == PersonAddressType.Temporary
                )
                {
                    var personAddress = new PersonAddress
                    {
                        Address = pollingStationAddress,
                        Person = person,
                        PersonAddressType = Get<PersonAddressType>(PersonAddressType.Declaration),
                        DateOfRegistration = DateTime.Now,
                        DateOfExpiration = election.ElectionRounds.FirstOrDefault().ElectionDate.AddDays(16)
                    };

                    Repository.SaveOrUpdate(personAddress);

                    person.SetEligibleAddress(personAddress);

                    transaction.OldPersonAddress = currentAddress;
                    transaction.NewPersonAddress = personAddress;
                }
                else
                {
                   throw new ApplicationException("Tip adresa curenta ilegala pentru procesare tranzactie, " + currentAddress.PersonAddressType.Name);
                }

                Repository.SaveOrUpdate(person);

                var users = Repository.QueryOver<SRVIdentityUser>()
                    .WithUdf(new UsersWithAccessToRegionCriterion(pollingStation.Region.Id))
                    .HasPropertyIn(x => x.Id)
                    .List();

                var notificationMessage = string.Format(MUI.Notification_PersonPollingStation_Change,
                    person.Idnp, oldPollingStationNumber, pollingStation.FullNumber);
                CreateNotification(EventTypes.Update, person, person.Id, notificationMessage, users);

            }
        }


        private void SetTransactionNew(Transaction transaction)
        {
            transaction.TransactionStatus = TransactionStatus.New;
            transaction.StatusMessage = null;
        }

        private void SetTransactionSuccess(Transaction transaction)
        {
            transaction.TransactionStatus = TransactionStatus.Success;
            transaction.StatusMessage = null;
        }

        private void SetTransactionError(Transaction transaction, string message)
        {
            transaction.TransactionStatus = TransactionStatus.Error;
            transaction.StatusMessage = message;
        }

        public void UndoTransactions(List<long> transactionIds, ref int success, ref int error)
        {
            if (!transactionIds.Any()) return;

            foreach (var transactionId in transactionIds)
            {
                var transaction = Get<Transaction>(transactionId);

                try
                {
                    UndoTransaction(transaction);
                    SetTransactionNew(transaction);
                    success++;
                }
                catch (Exception ex)
                {
                    SetTransactionError(transaction, ex.Message);
                    error++;
                }

                Repository.SaveOrUpdate(transaction);
            }
        }

        private void UndoTransaction(Transaction transaction)
        {
            if (transaction.TransactionStatus != TransactionStatus.Success)
            {
                throw new ApplicationException($"Tranzactia se afla in starea {transaction.TransactionStatus}, nu poate fi anulata");
            }

            var oldAddress = transaction.OldPersonAddress;
            var newAddress = transaction.NewPersonAddress;

            if (oldAddress == null)
            {
                throw new ApplicationException("Tranzactia nu are salvata adresa veche , nu poate fi anulata");
            }

            if (newAddress == null)
            {
                throw new ApplicationException("Tranzactia nu are salvata adresa noua, nu poate fi anulata");
            }

            var person = Repository.QueryOver<Person>().Where(x => x.Idnp == transaction.Idnp).SingleOrDefault();

            if (person == null)
            {
                throw new ApplicationException($"Nu exista persoana cu INDP: {transaction.Idnp}");
            }

            if (person.EligibleAddress.Id != newAddress.Id)
            {
                throw new ApplicationException($"Persoana cu idnp {person.Idnp} are adresa eligibila schimbata de la operarea tranzactiei");
            }
            
            person.SetEligibleAddress(oldAddress);
            Repository.SaveOrUpdate(person);
            Repository.Delete(newAddress);
        }
    }
}