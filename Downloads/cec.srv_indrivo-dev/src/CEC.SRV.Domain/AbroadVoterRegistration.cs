using System;
using Amdaris.Domain;

namespace CEC.SRV.Domain
{
    public class AbroadVoterRegistration : Entity
    {
        private readonly string _abroadAddress;
        private readonly string _abroadAddressCountry;
        private readonly double _abroadAddressLat;
        private readonly double _abroadAddressLong;
        private readonly DateTimeOffset _created;
        private readonly string _email;
        private readonly string _ipAddress;
        private readonly Person _person;
        private readonly string _residenceAddress;
        private DateTime _creationDate;

        public AbroadVoterRegistration()
        {
        }

        public AbroadVoterRegistration(Person person, string abroadAddress, string residenceAddress,
            string abroadAddressCountry, double abroadAddressLat, double abroadAddressLong, string email,
            string ipAddress)
        {
            _person = person;
            _abroadAddress = abroadAddress;
            _abroadAddressLat = abroadAddressLat;
            _abroadAddressLong = abroadAddressLong;
            _residenceAddress = residenceAddress;
            _abroadAddressCountry = abroadAddressCountry;
            _email = email;
            _ipAddress = ipAddress;
            _created = DateTimeOffset.Now;
        }

        public virtual Person Person
        {
            get { return _person; }
        }

        public virtual string AbroadAddress
        {
            get { return _abroadAddress; }
        }

        public virtual string AbroadAddressCountry
        {
            get { return _abroadAddressCountry; }
        }

        public virtual double AbroadAddressLat
        {
            get { return _abroadAddressLat; }
        }

        public virtual double AbroadAddressLong
        {
            get { return _abroadAddressLong; }
        }

        public virtual string ResidenceAddress
        {
            get { return _residenceAddress; }
        }

        public virtual string Email
        {
            get { return _email; }
        }

        public virtual string IpAddress
        {
            get { return _ipAddress; }
        }

        public virtual DateTimeOffset Created
        {
            get { return _created; }
        }

        public virtual DateTime CreationDate
        {
            get { return _creationDate; }
        }
    }
}