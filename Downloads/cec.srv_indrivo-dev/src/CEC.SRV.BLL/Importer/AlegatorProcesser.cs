using System;
using System.Linq;
using Amdaris;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;

namespace CEC.SRV.BLL.Importer
{
    public class AlegatorProcesser : ImportProcesser<AlegatorData>
    {
        private readonly ISRVRepository _srvRepository;

        public AlegatorProcesser(ISRVRepository srvRepository, ILogger loger, int batchSize)
            : base(srvRepository, loger, batchSize)
        {
            _srvRepository = srvRepository;
        }

        protected override void ProcessInternal(AlegatorData rawData)
        {
            if (GetByIdnp(rawData.Idnp) != null)
                throw new Exception(string.Format("Persoana cu idnp {0} deja exista in sistem", rawData.Idnp));

            var person = new Person(rawData.Idnp);

            person.Comments = "imported from Alegator";
            person.FirstName = rawData.FirstName;
            person.MiddleName = rawData.MiddleName;
            person.Surname = rawData.Surname;
            person.DateOfBirth = rawData.DateOfBirth.GetValueOrDefault();
            person.Gender = GetLookupByName<Gender>(rawData.GetGender());
            person.AlegatorId = rawData.AlegatorId;
            person.ModifyStatus(GetLookupByName<PersonStatusType>(rawData.GetPersonStatus()), "Importat din Alegator");

            person.Document = CreatePrsonDocument(rawData);
            var personAddress = CreatePersonAddress(rawData);

            _srvRepository.SaveOrUpdate(person);

            person.AddAddress(personAddress);
            
            _srvRepository.SaveOrUpdate(personAddress.Address);
            _srvRepository.SaveOrUpdate(personAddress);
        }

        private PersonAddress CreatePersonAddress(AlegatorData rawData)
        {
            var pollingStation = GetPollingStation(rawData.AlegatorPollingStationId.Value);
            var street = GetStreet(rawData, pollingStation);

            var address = GetAddress(street, pollingStation, rawData) ?? new Address()
            {
                //BuildingType = BuildingTypes.ApartmentBuilding,
				BuildingType = BuildingTypes.Undefined,
                HouseNumber = rawData.GetHouseNumber(),
                Suffix = rawData.GetHouseSuffix(),
                PollingStation = pollingStation,
                Street = street,
            };

            var personAddress = new PersonAddress
            {
                Address = address,
                PersonAddressType = GetLookupByName<PersonAddressType>("Reședință"),
                ApNumber = rawData.GetApartmentNumber(),
                ApSuffix = rawData.GetApartmentSuffix(),
                IsEligible = true
            };

            return personAddress;
        }

        private Address GetAddress(Street street, PollingStation pollingStation, AlegatorData rawData)
        {
            return _srvRepository.QueryOver<Address>()
                //.And(x => x.PollingStation == pollingStation)
                .Where(x => x.Street.Id == street.Id)
                //.And(x => x.BuildingType == BuildingTypes.ApartmentBuilding)
                .And(x => x.HouseNumber == rawData.GetHouseNumber())
                .And(x => x.Suffix == rawData.GetHouseSuffix()).SingleOrDefault<Address>();
        }

        private Street GetStreet(AlegatorData rawData, PollingStation pollingStation)
        {
            var rawStreetName = rawData.GetStreetName();
            var streetType = GetLookupByName<StreetType>(rawData.GetStreetType());
            if (rawStreetName == ">") rawStreetName = string.Format("${0} ({1})", pollingStation.Id, pollingStation.FullNumber);

            var street = _srvRepository.QueryOver<Street>()
                .Where(x => x.Region == pollingStation.Region)
                .And(x => x.Name == rawStreetName)
                .And(x => x.StreetType == streetType).SingleOrDefault<Street>();

            if (street == null)
            {
                street = new Street(pollingStation.Region, streetType, rawStreetName);
                _srvRepository.SaveOrUpdate(street);
            }

            return street;
        }

        private PersonDocument CreatePrsonDocument(AlegatorData rawData)
        {
            var documentType = GetLookupByName<DocumentType>(rawData.GetDocumentType());

            var document = new PersonDocument()
            {
                Seria = rawData.GetDocumentSeria(),
                Number = rawData.GetDocumentNumber(),
                Type = documentType

            };

            return document;
        }



        protected override void NotifySuccess(AlegatorData rawData)
        {
            Delete(rawData);
        }

        protected override void NotifyFailure(AlegatorData rawData)
        {
        }


        private Person GetByIdnp(string idnp)
        {
            return _srvRepository.Query<Person>().FirstOrDefault(x => x.Idnp == idnp && x.Deleted == null);
        }

        private T GetLookupByName<T>(string name) where T : Lookup
        {
            var result = _srvRepository.Query<T>().FirstOrDefault(x => x.Name == name);
            if (result == null)
                throw new SrvException(MUI.ObjectNotFound, string.Format("Can not find lookup typeof({0}) with name {1}", typeof(T), name));

            return result;
        }

        private PollingStation GetPollingStation(long saiseId)
        {
            var result = _srvRepository.Query<PollingStation>().FirstOrDefault(x => x.SaiseId == saiseId);

            if (result == null)
                throw new SrvException(MUI.ObjectNotFound, string.Format("Polling station with saiseId {0} not found", saiseId));

            return result;
        }

    }
}