using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class Person : SRVBaseEntity, INotificationEntity
    {
        public const int AdultHoodAge = 18;
        private readonly string _idnp;

        private readonly IList<PersonAddress> _addresses;

        private readonly IList<PersonStatus> _personStatuses; 

		private int _age;

        private string _fullName;

        [Obsolete("NHibernate usage only")]
        protected Person()
        {}

        public Person(string idnp)
        {
            if (!ValidateIdnp(idnp))
            {
                throw new NotSupportedException("IDNP should have 13 digits");
            }

            _idnp = idnp;
            _addresses = new List<PersonAddress>();
            _personStatuses = new List<PersonStatus>();
        }

        public virtual string FirstName { get; set; }

        public virtual string MiddleName { get; set; }

        public virtual string Surname { get; set; }

        public virtual string FullName
        {
            get { return _fullName; }
        }

        public virtual string Idnp
        {
            get { return _idnp; }
        }

        public virtual Gender Gender { get; set; }

        public virtual DateTime DateOfBirth { get; set; }

        public virtual long? AlegatorId { get; set; }

        public virtual PersonDocument Document { get; set; }
        public virtual DataFromEday DataFromEday { get; set; }

        public virtual bool ExportedToSaise { get; set; }

        public virtual IReadOnlyCollection<PersonAddress> Addresses
        {
            get
            {
                return new ReadOnlyCollection<PersonAddress>(_addresses);
            }
        }

        public virtual IReadOnlyCollection<PersonStatus> PersonStatuses
        {
            get
            {
                return new ReadOnlyCollection<PersonStatus>(_personStatuses);
            }
        }

        public virtual PersonStatus CurrentStatus
        {
            get { return _personStatuses.FirstOrDefault(x => x.IsCurrent); }
        }

        public virtual void AddAddress(PersonAddress personAddress)
        {
            if (personAddress == null) throw new ArgumentNullException("personAddress");

            if (!_addresses.Contains(personAddress))
            {
                personAddress.Person = this;
                _addresses.Add(personAddress);
            }

        }

        public virtual void ModifyStatus(PersonStatusType statusType, string confirmation)
        {
            if (CurrentStatus == null || CurrentStatus.StatusType != statusType)
            {
                foreach (var ps in _personStatuses.Where(x => x.IsCurrent))
                {
                    ps.IsCurrent = false;
                }

                var personStatus = new PersonStatus
                {
                    Person = this,
                    Confirmation = confirmation,
                    StatusType = statusType,
                    IsCurrent = true,
                    
                };
                _personStatuses.Add(personStatus);
            }
            else
            {
                CurrentStatus.Confirmation = confirmation;
            }
        }

        

        
        public virtual PersonAddress EligibleAddress
        {
            get { return _addresses.FirstOrDefault(x => x.IsEligible); }
        }

        public virtual void SetEligibleAddress(PersonAddress eligibleAddress)
        {
            foreach (var personAddress in Addresses.Where(x => x.IsEligible))
            {
                personAddress.IsEligible = false;
            }

            eligibleAddress.IsEligible = true;
            AddAddress(eligibleAddress);
        }

        public virtual void SetEligibleAddress(StayStatement stayStatement)
        {
            SetEligibleAddress(stayStatement.DeclaredStayAddress);
        }

        private bool ValidateIdnp(string idnp)
        {
            if (idnp.Length != 13)
            {
                return false;
            }

            return idnp.All(char.IsDigit);
        }

	    public virtual int Age { get { return _age; } }

		public virtual string Comments { get; set; }

        public virtual bool HasValidAgeForElectionDate(DateTime electionDate)
        {
            return electionDate >= this.DateOfBirth.AddYears(AdultHoodAge);
        }
        public virtual  long? ElectionListNr { get; set; }
        string INotificationEntity.GetNotificationType()
        {
            return "Person_Notification";
        }

    }
}