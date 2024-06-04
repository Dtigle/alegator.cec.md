using System.Linq;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Amdaris;
using NHibernate;
using NHibernate.Linq;
using CEC.SRV.Domain.Lookup;
using System.Collections.Generic;
using System.Text;
using Amdaris.Domain;
using System.Linq.Expressions;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class ImportBllTests : BaseTests<ImportBll, ISRVRepository>
    {
        private readonly Dictionary<Type, long> _ids;
        private readonly Dictionary<Type, Dictionary<long, IEntity>> _db;
        private readonly Mock<ISRVRepository> _mockRepository;

        #region Initialization

        public ImportBllTests()
        {
            _ids = new Dictionary<Type, long>();
            _db = new Dictionary<Type, Dictionary<long, IEntity>>();

            var logger = new Mock<ILogger>();
            logger.Setup(x => x.Info(It.IsAny<string>()));

            IsMockedRepository = true;

            _mockRepository = new Mock<ISRVRepository>();

            MockDb<Gender>();
            MockDb<DocumentType>();
            MockDb<PersonStatusType>();
            MockDb<RspModificationData>();
            MockDb<RspRegistrationData>();
            MockDb<Person>();
            MockDb<Region>();
            MockDb<Address>();
            MockDb<Street>();
            MockDb<PersonAddress>();
            MockDb<RegionType>();
            MockDb<StreetType>();
            MockDb<PersonAddressType>();
            MockDb<LinkedRegion>();
            MockDb<PollingStation>();
            MockDb<MappingAddress>();

            var genderM = new Gender() { Name = "M" };
            var genderF = new Gender() { Name = "F" };
            _mockRepository.Setup(x => x.Query<Gender>())
                .Returns(() => new EnumerableQuery<Gender>(new[] { genderM, genderF }));

            var buletinDocument = new DocumentType() { Name = "Buletin" };
            buletinDocument.SetId(1);
            var passaportDocument = new DocumentType() { Name = "Pașaport" };
            passaportDocument.SetId(2);
            var temporarDocument = new DocumentType() { Name = "F-9" };
            temporarDocument.SetId(3);
            var sovieticDocument = new DocumentType() { Name = "Sovietic" };
            sovieticDocument.SetId(4);
            _mockRepository.Setup(x => x.Query<DocumentType>())
                .Returns(() => new EnumerableQuery<DocumentType>(new[] { buletinDocument, temporarDocument, sovieticDocument, passaportDocument }));

            var alegator = new PersonStatusType() { Name = "Alegător" };
            alegator.SetId(1);
            var decedat = new PersonStatusType() { Name = "Decedat" };
            decedat.SetId(2);
            var judecat = new PersonStatusType() { Name = "Hotarârea judecății" };
            judecat.SetId(3);
            var strain = new PersonStatusType() { Name = "Altă cetățenie" };
            strain.SetId(4);
            var militar = new PersonStatusType() { Name = "Militar" };
            militar.SetId(5);
            _mockRepository.Setup(x => x.Query<PersonStatusType>())
                .Returns(() => new EnumerableQuery<PersonStatusType>(new[] { alegator, decedat, judecat, strain, militar }));

            Repository = _mockRepository.Object;
            var lookupBll = new LookupBll(Repository);
            var lookupCacheDataProvider = new WebLookupCacheDataProvider(lookupBll);
            Bll = new ImportBll(Repository, lookupCacheDataProvider, logger.Object);
        }

        private void MockDb<T>() where T : Entity
        {
            var type = typeof(T);
            var list = new Dictionary<long, IEntity>();

            if (!_db.ContainsKey(type))
            {
                _db.Add(type, list);
            }
            else
            {
                list = _db[type];
            }

            if (!_ids.ContainsKey(type))
            {
                _ids.Add(type, 0);
            }

            _mockRepository.Setup(x => x.Query<T>()).Returns(() => new EnumerableQuery<T>(list.Values.Cast<T>()));
            _mockRepository.Setup(x => x.Get<T>(It.IsAny<long>())).Returns((long id) => list.Values.Cast<T>().FirstOrDefault(x => x.Id == id));
            _mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<T>())).Callback((T entity) => OurSaveOrUpdate(entity));
            _mockRepository.Setup(x => x.Delete(It.IsAny<T>())).Callback((T entity) => OurDelete<T>(entity.Id));
        }

        private void OurSaveOrUpdate<T>(T entity) where T : Entity
        {
            var type = typeof(T);

            if (entity.Id == 0)
            {
                _ids[type]++;
                entity.SetId(_ids[type]);
                _db[type].Add(_ids[type], entity);
            }
            else
            {
                if (_db[type].ContainsKey(entity.Id))
                {
                    _db[type][entity.Id] = entity;
                }
                else
                {
                    entity.SetId(entity.Id);
                    _db[type].Add(entity.Id, entity);
                }
            }
        }

        private void OurDelete<T>(long id) where T : Entity
        {
            var type = typeof(T);

            if (_db[type].ContainsKey(id))
            {
                _db[type].Remove(id);
            }
        }

        private T LastCreated<T>() where T : Entity
        {
            return (T)_db[typeof(T)].Values.Last();
        }
        
        [TestInitialize]
        public new void Startup()
        {
            _db.ForEach(x => x.Value.Clear());
        }

        #endregion Initialization

        #region Import

        [TestMethod]
        public void Import_DataWithConflictStatus_sets_correct_status()
        {
            // Arrange
            var data = new RspModificationData()
            {
                Dead = false, 
                Idnp = "1234567890123",
                FirstName = "firstName",
                SecondName = "secondName",
                LastName = "lastName",
                SexCode = "1",
                Birthdate = DateTime.Now,
                Number = "1",
                Seria = "A",
                ValidBydate = DateTime.Now
            };

            var gender = GetFirstObjectFromDbTable(x => x.Name == data.GetSexCode(), GetGender);

            var person = new Person(data.Idnp)
            {
                FirstName = data.FirstName,
                MiddleName = data.SecondName,
                Surname = data.LastName,
                Gender = gender,
                DateOfBirth = data.Birthdate,
                Document = new PersonDocument()
                {
                    Number = data.Number,
                    Seria = data.Seria,
                    ValidBy = data.ValidBydate
                }
            };
            var personStatusType = GetLookupByName<PersonStatusType>("Decedat");
            person.ModifyStatus(personStatusType, string.Empty);
            var status = StatisticChanges.None;

           MockPersonQueryOver(person);

            // Act

            Bll.Import(data, ref status);

            //Assert

            Assert.IsTrue(status.HasFlag(StatisticChanges.ConflictStatus));
        }

        [TestMethod]
        [Ignore]
        public void Import_DataWithConflictAddress_sets_correct_status()
        {
            // Arrange

            //var data = GetFirstObjectFromDbTable(x => !x.Any(), () =>
            //{
            //    var rmd = GetRspModificationData();
            //    rmd.Registrations.Clear();
            //    return rmd;
            //});

            //var status = StatisticChanges.None;

            //var person = GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            //MockPersonQueryOver(person);
            
            //GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            //// Act

            //Bll.Import(data, ref status);
            
            //// Assert
            
            //Assert.IsTrue(status.HasFlag(StatisticChanges.ConflictAddress));
        }

        [TestMethod]
        public void Import_DataWithConflictPolling_sets_correct_status()
        {
            // Arrange

            var registrationData = GetRspRegistrationData();
            var administrativeCode = registrationData.Administrativecode;

            GetFirstObjectFromDbTable(x => x.Idnp == registrationData.RspModificationData.Idnp, () => new Person(registrationData.RspModificationData.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            var srvAddress = GetFirstObjectFromDbTable(x => !x.Street.Region.HasStreets, GetAddressWithoutStreets);

            var mappingAddress = GetFirstObjectFromDbTable(
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = registrationData.Administrativecode,
                        RspHouseNr = registrationData.HouseNumber,
                        RspHouseSuf = registrationData.GetHouseSuffix(),
                        RspStreetCode = registrationData.StreetCode,
                        SrvAddressId = srvAddress.Id
                    }, true);

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var person = GetFirstObjectFromDbTable(x => !x.Addresses.Any(), GetPerson);
            MockPersonQueryOver(person);

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Address == address) && (x.Person == person)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "123";
                return ps;
            }, true);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "255";
                return ps;
            }, true);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act

            Bll.Import(registrationData.RspModificationData, ref status);

            // Assert

            Assert.IsTrue(status.HasFlag(StatisticChanges.ConflictPolling));
        }

        [TestMethod]
        public void Import_DataWithoutConflict_returns_correct_result()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            
            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                }, PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            var personAddress = GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);
            var person = personAddress.Person;
            MockPersonQueryOver(person);

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            var status = StatisticChanges.None;

            // Act

            var newPerson = Bll.Import(data, ref status);

            // Assert
            
            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);

            Assert.IsFalse(status.HasFlag(StatisticChanges.Conflict));
        }

        private void MockPersonQueryOver(Person person)
        {
            var queryOver = new Mock<IQueryOver<Person, Person>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Person, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault()).Returns(person);
            _mockRepository.Setup(x => x.QueryOver<Person>()).Returns(queryOver.Object);
        }

        #endregion Import

        #region UpdateGeneralInformation

        [TestMethod]
        public void UpdateGeneralInformation_ForThePersonFromRawData_has_correct_logic()
        {
            // Arrange
            var data = new RspModificationData()
            {
                FirstName = "firstName",
                SecondName = "secondName",
                LastName = "lastName",
                SexCode = "1",
                Birthdate = DateTime.Now,
                Number = "1",
                Seria = "A",
                ValidBydate = DateTime.Now
            };

            var gender = GetFirstObjectFromDbTable(x => x.Name == data.GetSexCode(), GetGender);

            var person = new Person("1234567890123")
            {
                FirstName = data.FirstName,
                MiddleName = data.SecondName,
                Surname = data.LastName,
                Gender = gender,
                DateOfBirth = data.Birthdate,
                Document = new PersonDocument()
                {
                    Number = data.Number,
                    Seria = data.Seria,
                    ValidBy = data.ValidBydate
                }
            };

            var status = StatisticChanges.None;

            // Act

            Bll.UpdateGeneralInformation(data, person, ref status);

            // Assert

            Assert.AreEqual(StatisticChanges.None, status);
        }

        [TestMethod]
        public void UpdateGeneralInformation_ForAnotherPerson_has_correct_logic()
        {
            // Arrange

            var data = new RspModificationData()
            {
                FirstName = "firstName",
                SecondName = "secondName",
                LastName = "lastName",
                SexCode = "1",
                Birthdate = DateTime.Now,
                Number = "1",
                Seria = "A",
                ValidBydate = DateTime.Now,
                DocumentTypeCode = DocumentType.Parse(5),
            };

            var person = new Person("1234567890123")
            {
                FirstName = "firstName2",
                MiddleName = data.SecondName,
                Surname = data.LastName,
                Gender = GetLookupByName<Gender>(data.GetSexCode()),
                DateOfBirth = data.Birthdate,
                Document = new PersonDocument()
                {
                    Number = "1",
                    Type = GetLookupById<DocumentType>(DocumentType.Parse((int)data.DocumentTypeCode)),
                    Seria = "A"
                }
            };
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateGeneralInformation(data, person, ref status);

            // Assert

            Assert.AreEqual(StatisticChanges.Update, status);
            Assert.AreEqual("Importat din Rsp", person.Comments);
            Assert.AreEqual(data.FirstName, person.FirstName);
            Assert.AreEqual(data.SecondName, person.MiddleName);
            Assert.AreEqual(data.LastName, person.Surname);
            Assert.AreSame(GetLookupByName<Gender>(data.GetSexCode()), person.Gender);
            Assert.AreEqual(data.Birthdate, person.DateOfBirth);

            Assert.IsNotNull(person.Document);
            Assert.AreEqual(data.Seria, person.Document.Seria);
            Assert.AreEqual(data.Number, person.Document.Number);
            Assert.AreEqual(data.IssuedDate, person.Document.IssuedDate);
            Assert.AreEqual(data.ValidBydate, person.Document.ValidBy);
            Assert.AreEqual(data.DocumentTypeCode, person.Document.Type.Id);
        }

        #endregion UpdateGeneralInformation

        #region UpdateStatus

        [TestMethod]
        public void UpdateStatus_ForThePersonWithStatusFromRawData_has_correct_logic()
        {
            // Arrange

            var data = new RspModificationData() { Dead = true };

            var personStatusType = GetLookupByName<PersonStatusType>("Decedat");

            var person = new Person("1234567890123");
            person.ModifyStatus(personStatusType, string.Empty);

            var status = StatisticChanges.None;

            // Act

            Bll.UpdateStatus(data, person, ref status);

            // Assert

            Assert.AreEqual(StatisticChanges.None, status);
            Assert.IsNotNull(person.CurrentStatus);
            Assert.AreSame(personStatusType, person.CurrentStatus.StatusType);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictStatusException), "Status conflict, deja decedat")]
        public void UpdateStatus_ForTheDeadPersonThatIsNotDeadInRawData_throws_an_exception()
        {
            // Arrange
            var data = new RspModificationData() { Dead = false };

            var person = new Person("1234567890123");
            var personStatusType = GetLookupByName<PersonStatusType>("Decedat");
            person.ModifyStatus(personStatusType, string.Empty);
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateStatus(data, person, ref status);

            //Assert

            Assert.AreEqual(StatisticChanges.None, status);
        }

        [TestMethod]
        public void UpdateStatus_ForTheNotDeadPersonThatIsDeadInRawData_has_correct_logic()
        {
            // Arrange

            var data = new RspModificationData() { Dead = true };

            var personStatusType = GetLookupByName<PersonStatusType>("Alegător");

            var person = new Person("1234567890123");
            person.ModifyStatus(personStatusType, string.Empty);
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateStatus(data, person, ref status);

            // Assert
            Assert.AreEqual(StatisticChanges.StatusUpdate, status);
            Assert.IsNotNull(person.CurrentStatus);
            Assert.AreNotSame(personStatusType, person.CurrentStatus.StatusType);
        }

        #endregion UpdateStatus

        #region UpdateAddress

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void UpdateAddress_ForDataWithoutRegsAndAddrWithUnknStrTypes_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, null, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void UpdateAddress_ForDataWithoutRegsAndAddrWithNoResidRegs_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetAllObjectsFromDbTable<Address>(x => x.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(data, null, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void UpdateAddress_ForDataWithoutRegsAndAddrWithUnknStrTypeAndNoResidReg_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetAllObjectsFromDbTable<Address>(x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId)).ForEach(Repository.Delete);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, null, ref status);

            //Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void UpdateAddress_ForDataWithRegsWithoutRegTypeCodeAndAddrWithUnknStrTypes_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, null, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void UpdateAddress_ForDataWithRegsWithoutRegTypeCodeAndAddrWithNoResidRegs_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetAllObjectsFromDbTable<Address>(x => x.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, null, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void UpdateAddress_ForDataWithRegsWithoutRegTypeCodeAndAddrWithUnknStrTypeAndNoResidReg_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetAllObjectsFromDbTable<Address>(x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId)).ForEach(Repository.Delete);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, null, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        public void UpdateAddress_ForDataWithoutRegsWithAddrAndWithoutPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                }, PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            var personAddress = GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);
            var person = personAddress.Person;

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            const StatisticChanges expStatus = StatisticChanges.AddressUpdate;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, person, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);
        }

        [TestMethod]
        public void UpdateAddress_ForDataWithoutRegsWithAddrAndPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            var person = GetFirstObjectFromDbTable(GetPerson);

            GetFirstObjectFromDbTable(
                x => (x.Address.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Address.Street.Region.Id == Region.NoResidenceRegionId),
                () => new PersonAddress() { Address = address, Person = person});
            
            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, person, ref status);

            // Assert

            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        public void UpdateAddress_ForDataWithRegsWithoutRegTypeCodeWithAddrAndWithoutPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                },
                PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            var personAddress = GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);
            var person = personAddress.Person;

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            const StatisticChanges expStatus = StatisticChanges.AddressUpdate;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, person, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);
        }

        [TestMethod]
        public void UpdateAddress_ForDataWithRegsWithoutRegTypeCodeWithAddrAndPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });


            var personAddress = GetFirstObjectFromDbTable(
                x => (x.Address.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Address.Street.Region.Id == Region.NoResidenceRegionId),
                GetPersonAddressWithUnknownStreetTypeAndNoResidenceRegion);

            var person = personAddress.Person;

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data, person, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictRegionException), "Conflict de adresă: Regiunea cu ID 1 nu a fost gasită")]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndWithoutRegWithDataAdminCode_throws_an_exception()
        {
            // Arrange
            var data = GetRspRegistrationData();

            GetAllObjectsFromDbTable<Region>(x => x.RegistruId == data.Administrativecode).ForEach(x =>
            {
                x.RegistruId = null;
                Repository.SaveOrUpdate(x);
            });

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            Bll.UpdateAddress(data.RspModificationData, null, ref status);

            // Assert
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(LocalityConflictException), "Address conflict, Regiunea cu ID", true)]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndWithLinkRegWithDataAdminCode_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData();
            var administrativeCode = data.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegion();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetFirstObjectFromDbTable(x => x.Regions.Any(y => y.Id == region.Id), () => new LinkedRegion(new List<Region>() { region }));

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            
            Bll.UpdateAddress(data.RspModificationData, null, ref status);

            // Assert

            Assert.AreEqual(expStatus, status);
        }
        
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictStreetException), "Conflict de adresă: Nu a fost găsită strada  (StreetCode = 1) în regiunea Generated (CUATM 1)", true)]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndValidRegAndNonValidStr_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData();
            var administrativeCode = data.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<Street>(x => x.Region == region && x.RopId == data.StreetCode).ForEach(Repository.Delete);


            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act
            
            Bll.UpdateAddress(data.RspModificationData, null, ref status);
            
            // Assert
            
            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictAddressException), "Conflict de adresă: Nu a fost găsită adresa cu Strada = \"Generated\" (StreetCode = 1),  în regiunea \"Generated\" (CUATM 1) cu Num bloc 12 / ()", true)]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndValidRegAndValidStrAndNonValidAddr_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData();
            var administrativeCode = data.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });


            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            var street = GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == data.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = data.StreetCode;
                return str;
            });

            GetAllObjectsFromDbTable<MappingAddress>(x => x.RspAdministrativeCode == data.Administrativecode &&
                                                                             x.RspStreetCode == data.StreetCode &&
                                                                             x.RspHouseNr == data.HouseNumber &&
                                                                             x.RspHouseSuf == data.GetHouseSuffix()).ForEach(Repository.Delete);

            var queryOver = new Mock<IQueryOver<Address, Address>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault<Address>()).Returns((Address)null);
            _mockRepository.Setup(x => x.QueryOver<Address>()).Returns(queryOver.Object);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(data.RspModificationData, null, ref status);
            
            // Assert

            Assert.AreEqual(expStatus, status);
        }

        [TestMethod]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndValidInfoAndNotNullMapAddr_has_correct_logic()
        {
            // Arrange

            var data = GetRspRegistrationData();
            var administrativeCode = data.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });


            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == data.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = data.StreetCode;
                return str;
            });

            var mappingAddress = GetFirstObjectFromDbTable(x => x.RspAdministrativeCode == data.Administrativecode &&
                                               x.RspStreetCode == data.StreetCode &&
                                               x.RspHouseNr == data.HouseNumber &&
                                               x.RspHouseSuf == data.GetHouseSuffix(),
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = data.Administrativecode,
                        RspHouseNr = data.HouseNumber,
                        RspHouseSuf = data.GetHouseSuffix(),
                        RspStreetCode = data.StreetCode,
                        SrvAddressId = GetFirstObjectFromDbTable(GetAddress).Id
                    });

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var person = GetFirstObjectFromDbTable(GetPerson);

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address == address) && (x.Person == person), () =>
            {
                var paddr = GetPersonAddress();
                paddr.Address = address;
                paddr.Person = person;
                paddr.ApNumber = data.ApartmentNumber + 1;
                return paddr;
            });

            var expStatus = (personAddress.ApNumber != data.ApartmentNumber
                                        || personAddress.ApSuffix != data.ApartmentSuffix
                                        || personAddress.DateOfRegistration != data.DateOfRegistration
                                        || personAddress.DateOfExpiration != (data.DateOfExpiration ?? DateTime.MinValue)) ?
                             StatisticChanges.AddressUpdate : StatisticChanges.None;

            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(data.RspModificationData, person, ref status);
            
            // Assert

            Assert.AreEqual(expStatus, status);

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            var newAddress = newPerson.Addresses.FirstOrDefault(
                x => (x.Address == address) && (x.ApNumber == data.ApartmentNumber) &&
                     (x.ApSuffix == data.ApartmentSuffix) &&
                     (x.DateOfRegistration == data.DateOfRegistration) &&
                     (x.DateOfExpiration == data.DateOfExpiration));

            Assert.IsNotNull(newPerson);
            Assert.IsNotNull(newAddress);
        }

        [TestMethod]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndValidInfoAndNullMapAddr_has_correct_logic()
        {
            // Arrange

            var data = GetRspRegistrationData();
            var administrativeCode = data.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });


            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            var street = GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == data.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = data.StreetCode;
                return str;
            });

            GetAllObjectsFromDbTable<MappingAddress>(x => x.RspAdministrativeCode == data.Administrativecode &&
                                                           x.RspStreetCode == data.StreetCode &&
                                                           x.RspHouseNr == data.HouseNumber &&
                                                           x.RspHouseSuf == data.GetHouseSuffix()).ForEach(Repository.Delete);

            var address = GetFirstObjectFromDbTable(x => (x.Street == street) && (x.HouseNumber == data.HouseNumber) &&
                (x.Suffix == data.GetHouseSuffix()) && (x.Deleted == null), () =>
                {
                    var addr = GetAddress();
                    addr.Street = street;
                    addr.HouseNumber = data.HouseNumber;
                    addr.Suffix = data.GetHouseSuffix();
                    addr.Deleted = null;
                    return addr;
                });

            var person = GetFirstObjectFromDbTable(GetPerson);

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address == address) && (x.Person == person), () =>
            {
                var paddr = GetPersonAddress();
                paddr.Address = address;
                paddr.Person = person;
                paddr.ApNumber = data.ApartmentNumber + 1;
                return paddr;
            });

            var queryOver = new Mock<IQueryOver<Address, Address>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault<Address>()).Returns(address);
            _mockRepository.Setup(x => x.QueryOver<Address>()).Returns(queryOver.Object);

            var expStatus = (personAddress.ApNumber != data.ApartmentNumber
                                        || personAddress.ApSuffix != data.ApartmentSuffix
                                        || personAddress.DateOfRegistration != data.DateOfRegistration
                                        || personAddress.DateOfExpiration != (data.DateOfExpiration ?? DateTime.MinValue)) ?
                             StatisticChanges.AddressUpdate : StatisticChanges.None;

            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(data.RspModificationData, person, ref status);
            
            // Assert

            Assert.AreEqual(expStatus, status);

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            var newAddress = newPerson.Addresses.FirstOrDefault(
                x => (x.Address == address) && (x.ApNumber == data.ApartmentNumber) &&
                     (x.ApSuffix == data.ApartmentSuffix) &&
                     (x.DateOfRegistration == data.DateOfRegistration) &&
                     (x.DateOfExpiration == data.DateOfExpiration));

            Assert.IsNotNull(newPerson);
            Assert.IsNotNull(newAddress);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictPollingException), "Conflict de selectare a secției de votare pentru localitatea Ungheni (CUATM 1) din reguinea Ungheni", true)]
        public void UpdateAddress_ForDataWithRegsWithRegTypeCodeAndValidInfoAndConflictPollingExceptionCase_throws_an_exception()
        {
            // Arrange

            var registrationData = GetRspRegistrationData();
            var administrativeCode = registrationData.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });
            
            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            
            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            var srvAddress = GetFirstObjectFromDbTable(x => !x.Street.Region.HasStreets, GetAddressWithoutStreets);

            var mappingAddress = GetFirstObjectFromDbTable(
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = registrationData.Administrativecode,
                        RspHouseNr = registrationData.HouseNumber,
                        RspHouseSuf = registrationData.GetHouseSuffix(),
                        RspStreetCode = registrationData.StreetCode,
                        SrvAddressId = srvAddress.Id
                    }, true);

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var person = GetFirstObjectFromDbTable(x => !x.Addresses.Any(), GetPerson);

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Address == address) && (x.Person == person)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "123";
                return ps;
            }, true);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "255";
                return ps;
            }, true);
            
            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(registrationData.RspModificationData, person, ref status);
            
            // Assert

            Assert.AreEqual(expStatus, status);
        }
        
        #endregion UpdateAddress

        #region Assign and Reject Rsp Address

        #region AssignRspAddress For Raw Data with Existing Person

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForExPersAndDataWithoutRegsAndAddrWithUnknStrTypes_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForExPersAndDataWithoutRegsAndAddrWithNoResidRegs_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            GetAllObjectsFromDbTable<Address>(x => x.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForExPersAndDataWithoutRegsAndAddrWithUnknStrTypeAndNoResidReg_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            GetAllObjectsFromDbTable<Address>(x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId)).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictRegionException), "Conflict de adresă: Regiunea cu ID 1 nu a fost gasită")]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithoutRegTypeCodeAndAddrWithUnknStrTypes_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictRegionException), "Conflict de adresă: Regiunea cu ID 1 nu a fost gasită")]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithoutRegTypeCodeAndAddrWithNoResidRegs_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            GetAllObjectsFromDbTable<Address>(x => x.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictRegionException), "Conflict de adresă: Regiunea cu ID 1 nu a fost gasită")]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithoutRegTypeCodeAndAddrWithUnknStrTypeAndNoResidReg_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));
            GetAllObjectsFromDbTable<Address>(x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId)).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert
            
            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        public void AssignRspAddress_ForExPersAndDataWithoutRegsWithAddrAndWithoutPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                }, PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            var personAddress = GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);
            var person = personAddress.Person;

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status | ConflictStatusCode.AddressConflict, data.AcceptConflictCode);

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);
        }

        [TestMethod]
        public void AssignRspAddress_ForExPersAndDataWithoutRegsWithAddrAndPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var status = data.AcceptConflictCode;

            var person = GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            GetFirstObjectFromDbTable(
                x => (x.Address.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Address.Street.Region.Id == Region.NoResidenceRegionId),
                () => new PersonAddress() { Address = address, Person = person});

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        public void AssignRspAddress_ess_ForExPersAndDataWithRegs_WithoutRegTypeCode_WithAddrAnd_WithoutPersAddr_WithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            var data = GetRspRegistrationData().RspModificationData;
            data.Registrations.Clear();

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                },
                PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            var personAddress = GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);
            var person = personAddress.Person;

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status | ConflictStatusCode.AddressConflict, data.AcceptConflictCode);

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);
        }

        [TestMethod]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithoutRegTypeCodeWithAddrAndPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            GetFirstObjectFromDbTable(
                x => (x.Address.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Address.Street.Region.Id == Region.NoResidenceRegionId),
                GetPersonAddressWithUnknownStreetTypeAndNoResidenceRegion);

            // Act

            try
            {
                Bll.AssignRspAddress(data);
            }
            catch { }

            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictRegionException), "Conflict de adresă: Regiunea cu ID 1 nu a fost gasită", true)]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndWithoutRegWithDataAdminCode_throws_an_exception()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => x.RegistruId == registrationData.Administrativecode).ForEach(x =>
            {
                x.RegistruId = null;
                Repository.SaveOrUpdate(x);
            });

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(LocalityConflictException), "Address conflict, Regiunea cu ID ", true)]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndWithLinkRegWithDataAdminCode_throws_an_exception()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;
            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegion();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetFirstObjectFromDbTable(x => x.Regions.Any(y => y.Id == region.Id), () => new LinkedRegion(new List<Region>() { region }));

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }
        
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictStreetException), "Conflict de adresă: Nu a fost găsită strada  (StreetCode = ", true)]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndValidRegAndNonValidStr_throws_an_exception()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;
            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<Street>(x => x.Region == region && x.RopId == registrationData.StreetCode).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictAddressException), "Conflict de adresă: Nu a fost găsită adresa cu Strada = \"Generated\" (StreetCode = 1),  în regiunea \"Generated", true)]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndValidRegAndValidStrAndNonValidAddr_throws_an_exception()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;

            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });


            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            var street = GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            GetAllObjectsFromDbTable<MappingAddress>(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                                                             x.RspStreetCode == registrationData.StreetCode &&
                                                                             x.RspHouseNr == registrationData.HouseNumber &&
                                                                             x.RspHouseSuf == registrationData.GetHouseSuffix()).ForEach(Repository.Delete);

            var queryOver = new Mock<IQueryOver<Address, Address>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault<Address>()).Returns((Address)null);
            _mockRepository.Setup(x => x.QueryOver<Address>()).Returns(queryOver.Object);


            GetAllObjectsFromDbTable<Address>(x => (x.Street == street) &&
                (x.HouseNumber == registrationData.HouseNumber) &&
                (x.Suffix == registrationData.GetHouseSuffix()) &&
                (x.Deleted == null)).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.AreEqual(status, data.AcceptConflictCode);
        }

        [TestMethod]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndValidInfoAndNotNullMapAddr_has_correct_logic()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;
            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            var person = GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            var mappingAddress = GetFirstObjectFromDbTable(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                               x.RspStreetCode == registrationData.StreetCode &&
                                               x.RspHouseNr == registrationData.HouseNumber &&
                                               x.RspHouseSuf == registrationData.GetHouseSuffix(),
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = registrationData.Administrativecode,
                        RspHouseNr = registrationData.HouseNumber,
                        RspHouseSuf = registrationData.GetHouseSuffix(),
                        RspStreetCode = registrationData.StreetCode,
                        SrvAddressId = GetFirstObjectFromDbTable(GetAddress).Id
                    });

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address == address) && (x.Person == person), () =>
            {
                var paddr = GetPersonAddress();
                paddr.Address = address;
                paddr.Person = person;
                paddr.ApNumber = registrationData.ApartmentNumber + 1;
                return paddr;
            });

            var isUpdated = (personAddress.ApNumber != registrationData.ApartmentNumber
                                        || personAddress.ApSuffix != registrationData.ApartmentSuffix
                                        || personAddress.DateOfRegistration != registrationData.DateOfRegistration
                                        || personAddress.DateOfExpiration != (registrationData.DateOfExpiration ?? DateTime.MinValue));

            // Act
            Bll.AssignRspAddress(data);

            // Assert
            if (isUpdated)
            {
                Assert.AreEqual(status | ConflictStatusCode.AddressConflict, data.AcceptConflictCode);
            }
            else
            {
                Assert.AreEqual(status, data.AcceptConflictCode);
            }

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            var newAddress = newPerson.Addresses.FirstOrDefault(
                x => (x.Address == address) && (x.ApNumber == registrationData.ApartmentNumber) &&
                     (x.ApSuffix == registrationData.ApartmentSuffix) &&
                     (x.DateOfRegistration == registrationData.DateOfRegistration) &&
                     (x.DateOfExpiration == registrationData.DateOfExpiration));

            Assert.IsNotNull(newPerson);
            Assert.IsNotNull(newAddress);
        }

        [TestMethod]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndValidInfoAndNullMapAddr_has_correct_logic()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;
            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            var person = GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });


            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            var street = GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            GetAllObjectsFromDbTable<MappingAddress>(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                                           x.RspStreetCode == registrationData.StreetCode &&
                                                           x.RspHouseNr == registrationData.HouseNumber &&
                                                           x.RspHouseSuf == registrationData.GetHouseSuffix()).ForEach(Repository.Delete);

            var address = GetFirstObjectFromDbTable(x => (x.Street == street) && (x.HouseNumber == registrationData.HouseNumber) &&
                (x.Suffix == registrationData.GetHouseSuffix()) && (x.Deleted == null), () =>
                {
                    var addr = GetAddress();
                    addr.Street = street;
                    addr.HouseNumber = registrationData.HouseNumber;
                    addr.Suffix = registrationData.GetHouseSuffix();
                    addr.Deleted = null;
                    return addr;
                });

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address == address) && (x.Person == person), () =>
            {
                var paddr = GetPersonAddress();
                paddr.Address = address;
                paddr.Person = person;
                paddr.ApNumber = registrationData.ApartmentNumber + 1;
                return paddr;
            });

            var queryOver = new Mock<IQueryOver<Address, Address>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault<Address>()).Returns(address);
            _mockRepository.Setup(x => x.QueryOver<Address>()).Returns(queryOver.Object);

            var isUpdated = (personAddress.ApNumber != registrationData.ApartmentNumber
                                        || personAddress.ApSuffix != registrationData.ApartmentSuffix
                                        || personAddress.DateOfRegistration != registrationData.DateOfRegistration
                                        || personAddress.DateOfExpiration != (registrationData.DateOfExpiration ?? DateTime.MinValue));

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            if (isUpdated)
            {
                Assert.AreEqual(status | ConflictStatusCode.AddressConflict, data.AcceptConflictCode);
            }
            else
            {
                Assert.AreEqual(status, data.AcceptConflictCode);
            }

            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Id == person.Id);
            var newAddress = newPerson.Addresses.FirstOrDefault(
                x => (x.Address == address) && (x.ApNumber == registrationData.ApartmentNumber) &&
                     (x.ApSuffix == registrationData.ApartmentSuffix) &&
                     (x.DateOfRegistration == registrationData.DateOfRegistration) &&
                     (x.DateOfExpiration == registrationData.DateOfExpiration));

            Assert.IsNotNull(newPerson);
            Assert.IsNotNull(newAddress);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictPollingException), "Conflict de selectare a secției de votare pentru localitatea Ungheni (CUATM ", true)]
        public void AssignRspAddress_ForExPersAndDataWithRegsWithRegTypeCodeAndValidInfoAndConflictPollingExceptionCase_throws_an_exception()
        {
            // Arrange
            var registrationData = GetRspRegistrationData();
            var data = registrationData.RspModificationData;
            var administrativeCode = registrationData.Administrativecode;

            GetFirstObjectFromDbTable(x => x.Idnp == data.Idnp, () => new Person(data.Idnp));

            GetAllObjectsFromDbTable<Region>(x => (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            var srvAddress = GetFirstObjectFromDbTable(x => !x.Street.Region.HasStreets, GetAddressWithoutStreets);

            var mappingAddress = GetFirstObjectFromDbTable(
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = registrationData.Administrativecode,
                        RspHouseNr = registrationData.HouseNumber,
                        RspHouseSuf = registrationData.GetHouseSuffix(),
                        RspStreetCode = registrationData.StreetCode,
                        SrvAddressId = srvAddress.Id
                    }, true);

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var person = GetFirstObjectFromDbTable(x => !x.Addresses.Any(), GetPerson);

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Address == address) && (x.Person == person)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "123";
                return ps;
            }, true);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "255";
                return ps;
            }, true);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(data, person, ref status);

            // Assert

            Assert.AreEqual(expStatus, status);
        }

        #endregion AssignRspAddress For Raw Data with Existing Person

        #region AssignRspAddress For Raw Data with Not Existing Person

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForNonExPersAndDataWithoutRegsAndAddrWithUnknStrTypes_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.Clear();
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForNonExPersAndDataWithoutRegsAndAddrWithNoResidRegs_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => (!x.Registrations.Any()) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.Clear();
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Address>(x => x.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForNonExPersAndDataWithoutRegsAndAddrWithUnknStrTypeAndNoResidReg_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => (!x.Registrations.Any()) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.Clear();
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Address>(x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId)).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithoutRegTypeCodeAndAddrWithUnknStrTypes_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any(y => y.RegTypeCode == null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.First().RegTypeCode = null;
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Address>(x => x.Street.StreetType.Id == StreetType.UnknownStreetType).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithoutRegTypeCodeAndAddrWithNoResidRegs_throws_an_exception()
        {
            // Arrange
            
            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any(y => y.RegTypeCode == null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.First().RegTypeCode = null;
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Address>(x => x.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictFatalAddressException), "Eroare fatală, lipsește regiunea specială cu ID = -1")]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithoutRegTypeCodeAndAddrWithUnknStrTypeAndNoResidReg_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any(y => y.RegTypeCode == null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.First().RegTypeCode = null;
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Address>(x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId)).ForEach(Repository.Delete);

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        public void AssignRspAddress_ForNonExPersAndDataWithoutRegsWithAddrAndWithoutPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => (!x.Registrations.Any()) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.Clear();
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                }, PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status | ConflictStatusCode.AddressConflict));

            var newPerson = LastCreated<Person>();
            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);
        }

        [TestMethod]
        public void AssignRspAddress_ForNonExPersAndDataWithoutRegsWithAddrAndPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            
            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => (!x.Registrations.Any()) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.Clear();
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });
            
            GetFirstObjectFromDbTable(
                x => (x.Address.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Address.Street.Region.Id == Region.NoResidenceRegionId),
                () => new PersonAddress() { Address = address, Person = GetPerson()});

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));

        }

        [TestMethod]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithoutRegTypeCodeWithAddrAndWithoutPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange
            
            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any(y => y.RegTypeCode == null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.First().RegTypeCode = null;
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetFirstObjectFromDbTable(
                x => x.Id == PersonAddressType.NoResidence,
                () =>
                {
                    var personAddressType = GetPersonAddressType();
                    personAddressType.Name = personAddressType.Description = "NoResidence";
                    return personAddressType;
                },
                PersonAddressType.NoResidence);

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            GetFirstObjectFromDbTable(x => x.Person != null, GetPersonAddress);

            GetAllObjectsFromDbTable<PersonAddress>(x =>
                        x.Address.Street.StreetType.Id == StreetType.UnknownStreetType &&
                        x.Address.Street.Region.Id == Region.NoResidenceRegionId).ForEach(Repository.Delete);

            var pat = GetPersonAddressType();
            pat.SetId(PersonAddressType.NoResidence);
            if (!_db[typeof(PersonAddressType)].ContainsKey(pat.Id))
            {
                _db[typeof(PersonAddressType)].Add(pat.Id, pat);
            }

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status | ConflictStatusCode.AddressConflict));

            var newPerson = LastCreated<Person>();

            Assert.AreSame(address, newPerson.EligibleAddress.Address);
            Assert.AreEqual(PersonAddressType.NoResidence, newPerson.EligibleAddress.PersonAddressType.Id);
        }

        [TestMethod]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithoutRegTypeCodeWithAddrAndPersAddrWithUnknStrTypeAndNoResidReg_has_correct_logic()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any(y => y.RegTypeCode == null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                rmd.Registrations.First().RegTypeCode = null;
                return rmd;
            });

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            var streetType = GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType, StreetType.UnknownStreetType);
            var region = GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegion, Region.NoResidenceRegionId);

            var address = GetFirstObjectFromDbTable(
                x => (x.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Street.Region.Id == Region.NoResidenceRegionId),
                () => new Address() { Street = new Street(region, streetType, "generated") });

            GetFirstObjectFromDbTable(
                x => (x.Address.Street.StreetType.Id == StreetType.UnknownStreetType) && (x.Address.Street.Region.Id == Region.NoResidenceRegionId),
                () => new PersonAddress() { Address = address, Person = GetPerson() });

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictRegionException), "Conflict de adresă: Regiunea cu ID 1 nu a fost gasită", true)]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndWithoutRegWithDataAdminCode_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Region>(x => x.RegistruId == registrationData.Administrativecode).ForEach(x =>
            {
                x.RegistruId = null;
                Repository.SaveOrUpdate(x);
            });

            // Act

            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(LocalityConflictException), "Address conflict, Regiunea cu ID ", true)]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndWithLinkRegWithDataAdminCode_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();
            var administrativeCode = registrationData.Administrativecode;

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegion();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetFirstObjectFromDbTable(x => x.Regions.Any(y => y.Id == region.Id), () => new LinkedRegion(new List<Region>() { region }));

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }
        
        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictStreetException), "Conflict de adresă: Nu a fost găsită strada  (StreetCode = 1) în regiunea Generated (CUATM 1)", true)]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndValidRegAndNonValidStr_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();
            var administrativeCode = registrationData.Administrativecode;

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<Street>(x => x.Region == region && x.RopId == registrationData.StreetCode).ForEach(Repository.Delete);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictAddressException), "Conflict de adresă: Nu a fost găsită adresa cu Strada = \"Generated\" (StreetCode = 1),  în regiunea \"Generated\" (CUATM 1) cu Num bloc 12 / ()", true)]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndValidRegAndValidStrAndNonValidAddr_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();
            var administrativeCode = registrationData.Administrativecode;

            var docTypeId = data.DocumentTypeCode;
            GetFirstObjectFromDbTable(x => x.Id == docTypeId, GetNonPrimaryDocumentType, docTypeId);

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });
            
            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            var street = GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            GetAllObjectsFromDbTable<MappingAddress>(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                                                             x.RspStreetCode == registrationData.StreetCode &&
                                                                             x.RspHouseNr == registrationData.HouseNumber &&
                                                                             x.RspHouseSuf == registrationData.GetHouseSuffix()).ForEach(Repository.Delete);

            GetAllObjectsFromDbTable<Address>(x => (x.Street == street) &&
                (x.HouseNumber == registrationData.HouseNumber) &&
                (x.Suffix == registrationData.GetHouseSuffix()) &&
                (x.Deleted == null)).ForEach(Repository.Delete);

            var queryOver = new Mock<IQueryOver<Address, Address>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault<Address>()).Returns((Address)null);
            _mockRepository.Setup(x => x.QueryOver<Address>()).Returns(queryOver.Object);

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            Assert.IsTrue(data.AcceptConflictCode.HasFlag(status));
        }

        [TestMethod]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndValidInfoAndNotNullMapAddr_has_correct_logic()
        {
            // Arrange
            
            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();
            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            var mappingAddress = GetFirstObjectFromDbTable(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                               x.RspStreetCode == registrationData.StreetCode &&
                                               x.RspHouseNr == registrationData.HouseNumber &&
                                               x.RspHouseSuf == registrationData.GetHouseSuffix(),
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = registrationData.Administrativecode,
                        RspHouseNr = registrationData.HouseNumber,
                        RspHouseSuf = registrationData.GetHouseSuffix(),
                        RspStreetCode = registrationData.StreetCode,
                        SrvAddressId = GetFirstObjectFromDbTable(GetAddress).Id
                    });

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var person = new Person(idnp);

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address == address) && (x.Person == person), () =>
            {
                var paddr = GetPersonAddress();
                paddr.Address = address;
                paddr.Person = person;
                paddr.ApNumber = registrationData.ApartmentNumber + 1;
                return paddr;
            });

            var isUpdated = (personAddress.ApNumber != registrationData.ApartmentNumber
                                        || personAddress.ApSuffix != registrationData.ApartmentSuffix
                                        || personAddress.DateOfRegistration != registrationData.DateOfRegistration
                                        || personAddress.DateOfExpiration != (registrationData.DateOfExpiration ?? DateTime.MinValue));

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            if (isUpdated)
            {
                Assert.AreEqual(status | ConflictStatusCode.AddressConflict, data.AcceptConflictCode);
            }
            else
            {
                Assert.AreEqual(status, data.AcceptConflictCode);
            }

            var newPerson = LastCreated<Person>();

            Assert.IsNotNull(newPerson);

            var newAddress = newPerson.Addresses.FirstOrDefault(
                x => (x.Address == address) && (x.ApNumber == registrationData.ApartmentNumber) &&
                     (x.ApSuffix == registrationData.ApartmentSuffix) &&
                     (x.DateOfRegistration == registrationData.DateOfRegistration) &&
                     (x.DateOfExpiration == registrationData.DateOfExpiration));

            Assert.IsNotNull(newAddress);

        }

        [TestMethod]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndValidInfoAndNullMapAddr_has_correct_logic()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();
            var administrativeCode = registrationData.Administrativecode;

            var status = data.AcceptConflictCode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });


            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);
            GetAllObjectsFromDbTable<PollingStation>(x => x.Region.Id == region.Id).ForEach(Repository.Delete);

            var street = GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            GetAllObjectsFromDbTable<MappingAddress>(x => x.RspAdministrativeCode == registrationData.Administrativecode &&
                                                           x.RspStreetCode == registrationData.StreetCode &&
                                                           x.RspHouseNr == registrationData.HouseNumber &&
                                                           x.RspHouseSuf == registrationData.GetHouseSuffix()).ForEach(Repository.Delete);

            var address = GetFirstObjectFromDbTable(x => (x.Street == street) && (x.HouseNumber == registrationData.HouseNumber) &&
                (x.Suffix == registrationData.GetHouseSuffix()) && (x.Deleted == null), () =>
                {
                    var addr = GetAddress();
                    addr.Street = street;
                    addr.HouseNumber = registrationData.HouseNumber;
                    addr.Suffix = registrationData.GetHouseSuffix();
                    addr.Deleted = null;
                    return addr;
                });

            var person = new Person(idnp);

            var personAddress = GetFirstObjectFromDbTable(x => (x.Address == address) && (x.Person == person), () =>
            {
                var paddr = GetPersonAddress();
                paddr.Address = address;
                paddr.Person = person;
                paddr.ApNumber = registrationData.ApartmentNumber + 1;
                return paddr;
            });

            var queryOver = new Mock<IQueryOver<Address, Address>>();
            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<Address, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.SingleOrDefault<Address>()).Returns(address);
            _mockRepository.Setup(x => x.QueryOver<Address>()).Returns(queryOver.Object);

            var isUpdated = (personAddress.ApNumber != registrationData.ApartmentNumber
                                        || personAddress.ApSuffix != registrationData.ApartmentSuffix
                                        || personAddress.DateOfRegistration != registrationData.DateOfRegistration
                                        || personAddress.DateOfExpiration != (registrationData.DateOfExpiration ?? DateTime.MinValue));

            // Act
            
            Bll.AssignRspAddress(data);
            
            // Assert

            if (isUpdated)
            {
                Assert.AreEqual(status | ConflictStatusCode.AddressConflict, data.AcceptConflictCode);
            }
            else
            {
                Assert.AreEqual(status, data.AcceptConflictCode);
            }

            var newPerson = LastCreated<Person>();

            Assert.IsNotNull(newPerson);

            var newAddress = newPerson.Addresses.FirstOrDefault(
                x => (x.Address == address) && (x.ApNumber == registrationData.ApartmentNumber) &&
                     (x.ApSuffix == registrationData.ApartmentSuffix) &&
                     (x.DateOfRegistration == registrationData.DateOfRegistration) &&
                     (x.DateOfExpiration == registrationData.DateOfExpiration));

            Assert.IsNotNull(newAddress);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ConflictPollingException), "Conflict de selectare a secției de votare pentru localitatea Ungheni (CUATM 1) din reguinea Ungheni", true)]
        public void AssignRspAddress_ForNonExPersAndDataWithRegsWithRegTypeCodeAndValidInfoAndConflictPollingExceptionCase_throws_an_exception()
        {
            // Arrange

            var idnp = GetNonExistingIdnp();
            var data = GetFirstObjectFromDbTable(x => x.Registrations.Any() && x.Registrations.All(y => y.RegTypeCode != null) && (x.Idnp == idnp), () =>
            {
                var rmd = GetRspRegistrationData().RspModificationData;
                rmd.Idnp = idnp;
                return rmd;
            });

            var registrationData = data.Registrations.Single();
            var administrativeCode = registrationData.Administrativecode;

            GetAllObjectsFromDbTable<Region>(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode)).ForEach(x =>
            {
                x.RegistruId = null;
                x.StatisticIdentifier = null;
                Repository.SaveOrUpdate(x);
            });

            var region = GetFirstObjectFromDbTable(x => (x.RegistruId == administrativeCode) || (x.StatisticIdentifier == administrativeCode), () =>
            {
                var reg = GetRegionWithoutStreets();
                reg.Name = "Generated";
                reg.RegistruId = administrativeCode;
                reg.StatisticIdentifier = administrativeCode;
                return reg;
            });

            GetAllObjectsFromDbTable<LinkedRegion>(x => x.Regions.Any(y => y.Id == region.Id)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(x => x.Region == region && x.RopId == registrationData.StreetCode, () =>
            {
                var str = GetStreet(region);
                str.Name = "Generated";
                str.RopId = registrationData.StreetCode;
                return str;
            });

            var srvAddress = GetFirstObjectFromDbTable(x => !x.Street.Region.HasStreets, GetAddressWithoutStreets);

            var mappingAddress = GetFirstObjectFromDbTable(
                    () => new MappingAddress()
                    {
                        RspAdministrativeCode = registrationData.Administrativecode,
                        RspHouseNr = registrationData.HouseNumber,
                        RspHouseSuf = registrationData.GetHouseSuffix(),
                        RspStreetCode = registrationData.StreetCode,
                        SrvAddressId = srvAddress.Id
                    }, true);

            var address = GetFirstObjectFromDbTable<Address>(x => x.Id == mappingAddress.SrvAddressId);

            var person = GetFirstObjectFromDbTable(x => !x.Addresses.Any(), GetPerson);

            GetAllObjectsFromDbTable<PersonAddress>(x => (x.Address == address) && (x.Person == person)).ForEach(Repository.Delete);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "123";
                return ps;
            }, true);

            GetFirstObjectFromDbTable(() =>
            {
                var ps = GetPollingStation(srvAddress.Street.Region);
                ps.Number = "255";
                return ps;
            }, true);

            const StatisticChanges expStatus = StatisticChanges.None;
            var status = StatisticChanges.None;

            // Act

            Bll.UpdateAddress(data, person, ref status);

            // Assert

            Assert.AreEqual(expStatus, status);
        }

        #endregion AssignRspAddress For Raw Data with Not Existing Person

        [TestMethod]
        public void RejectRspAddress_has_correct_result()
        {
            // Arrange
            var data = GetFirstObjectFromDbTable(GetRspRegistrationData).RspModificationData;
            var code = data.RejectConflictCode | ConflictStatusCode.AddressConflict;

            // Act

            SafeExec(Bll.RejectRspAddress, data);

            // Assert
            var newData = GetFirstObjectFromDbTable<RspRegistrationData>(x => x.Id == data.Id).RspModificationData;

            Assert.AreEqual(code, newData.RejectConflictCode);
            AssertStatusDate(newData);
        }

        #endregion Assign and Reject Rsp Address

        #region Accept and Reject Rsp Status

        [TestMethod]
        public void AcceptRspStatus_ByNotExistingIdnp_throws_an_exception()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;
            data.Idnp = "non valid idnp";

            // Act

            SafeExec(Bll.AcceptRspStatus, data, true, false, MUI.Conflict_StatusConflict_PeronsNotFound, data.Idnp);
        }

        [TestMethod]
        public void AcceptRspStatus_ByExistingIdnp_has_correct_result()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var dataId = data.Id;
            var code = data.AcceptConflictCode | ConflictStatusCode.StatusConflict;
            var personStatus = data.GetPersonStatus();

            // Act

            SafeExec(Bll.AcceptRspStatus, data);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspModificationData>(x => x.Id == data.Id);
            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Idnp == data.Idnp);

            Assert.AreEqual("Importat din Rsp", newPerson.CurrentStatus.Confirmation);
            Assert.AreEqual(personStatus, newPerson.CurrentStatus.StatusType.Name);
            Assert.AreEqual(code, newData.AcceptConflictCode);
            AssertStatusDate(newData);
        }

        [TestMethod]
        public void RejectRspStatus_has_correct_result()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var dataId = data.Id;
            var code = data.AcceptConflictCode | ConflictStatusCode.StatusConflict;

            // Act

            SafeExec(Bll.RejectRspStatus, data);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspModificationData>(x => x.Id == data.Id);

            Assert.AreEqual(code, newData.AcceptConflictCode);
            AssertStatusDate(newData);
        }

        #endregion Accept and Reject Rsp Status

        #region Accept and Reject Rsp Polling Station

        [TestMethod]
        public void AssignRspPollingStation_has_correct_result()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var dataId = data.Id;
            var code = data.AcceptConflictCode | ConflictStatusCode.PollingStationConflict;

            // Act

            SafeExec(Bll.AssignRspPollingStation, data);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspModificationData>(x => x.Id == data.Id);

            Assert.AreEqual(code, newData.AcceptConflictCode);
            AssertStatusDate(newData);
        }

        [TestMethod]
        public void RejectRspPollingStation_has_correct_result()
        {
            // Arrange

            var data = GetRspRegistrationData().RspModificationData;

            var dataId = data.Id;
            var code = data.RejectConflictCode | ConflictStatusCode.PollingStationConflict;

            // Act

            SafeExec(Bll.RejectRspPollingStation, data);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspModificationData>(x => x.Id == data.Id);

            Assert.AreEqual(code, newData.RejectConflictCode);
            AssertStatusDate(newData);
        }

        #endregion Accept and Reject Rsp Polling Station

        #region Accept Rsv Locality

        [TestMethod]
        public void AcceptRsvLocality__ByNullComments_has_correct_result()
        {
            // Arrange

            var data = GetFirstObjectFromDbTable(x => x.RspModificationData.Comments == null, () =>
            {
                var rmd = GetRspRegistrationData();
                rmd.RspModificationData.Comments = null;
                return rmd;
            });

            var dataId = data.Id;

            var code = data.RspModificationData.AcceptConflictCode | ConflictStatusCode.LocalityConflict;
            var expComments = string.Format(MUI.Conflict_ChangeLocality_AcceptAddress_comment);

            // Act

            SafeExec(Bll.AcceptRsvLocality, dataId);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspRegistrationData>(x => x.Id == data.Id);

            Assert.AreEqual(expComments, newData.RspModificationData.Comments);
            Assert.AreEqual(code, newData.RspModificationData.AcceptConflictCode);
            AssertStatusDate(newData.RspModificationData);
        }

        [TestMethod]
        public void AcceptRsvLocality__ByNotNullComments_has_correct_result()
        {
            // Arrange

            var data = GetFirstObjectFromDbTable(x => x.RspModificationData.Comments != null, GetRspRegistrationData);

            OurSaveOrUpdate(data);

            var code = data.RspModificationData.AcceptConflictCode | ConflictStatusCode.LocalityConflict;
            var expComments = string.Format("{0} {1}", data.RspModificationData.Comments, MUI.Conflict_ChangeLocality_AcceptAddress_comment);

            // Act
            SafeExec(Bll.AcceptRsvLocality, data.Id);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspModificationData>(x => x.Id == data.Id);

            Assert.AreEqual(expComments, newData.Comments);
            Assert.AreEqual(code, newData.AcceptConflictCode);
            AssertStatusDate(newData);
        }

        #endregion Accept Rsv Locality

        [TestMethod]
        public void MapAddress_ByExistingIdnp_has_correct_result()
        {
            // Arrange

            var data = GetFirstObjectFromDbTable(GetRspRegistrationData);
            var address = GetFirstObjectFromDbTable(GetAddress);

            var dataId = data.Id;
            var code = data.RspModificationData.AcceptConflictCode | ConflictStatusCode.AddressConflict;

            var expRegistrationDatasInConflictCount = data.RspModificationData.Registrations.Count(x => x.IsInConflict) - 1;

            // Act

            SafeExec((rawData, address1) => Bll.MapAddress(rawData.RspModificationData, address1, 1, null), data, address);

            // Assert

            var newData = GetFirstObjectFromDbTable<RspModificationData>(x => x.Id == data.Id);
            var newPerson = GetFirstObjectFromDbTable<Person>(x => x.Idnp == data.RspModificationData.Idnp);
            var newPersonAddress = LastCreated<PersonAddress>();

            Assert.IsNotNull(newPersonAddress);
            Assert.AreSame(address, newPersonAddress.Address);

            Assert.AreEqual(code, newData.AcceptConflictCode);
            AssertStatusDate(newData);
        
            Assert.AreSame(newPerson, newPersonAddress.Person);

            newData.Registrations.Where(x => x.IsInConflict).ForEach(x =>
                Assert.IsNotNull(GetFirstObjectFromDbTable<MappingAddress>(y =>
                    (y.SrvAddressId == address.Id) &&
                    (y.RspAdministrativeCode == x.Administrativecode) &&
                    (y.RspStreetCode == x.StreetCode) &&
                    (y.RspHouseNr == x.HouseNumber) &&
                    (y.RspHouseSuf == x.GetHouseSuffix()))));

            var registrationDatasInConflictCount = newData.Registrations.Count(x => x.IsInConflict);
            Assert.AreEqual(expRegistrationDatasInConflictCount, registrationDatasInConflictCount);
        }

        private void AssertStatusDate(RspModificationData data)
        {
            Assert.IsTrue(data.StatusDate.HasValue);
            Assert.AreEqual(DateTime.Now.Date, data.StatusDate.Value.Date.Date);
        }

        private string GenerateRandomIdnp()
        {
            var r = new Random();
            var sb = new StringBuilder();

            for (var i = 0; i < 13; i++)
            {
                sb.Append(r.Next(10).ToString());
            }

            return sb.ToString();
        }

        private string GetNonExistingIdnp()
        {
            string idnp;

            do
            {
                idnp = GenerateRandomIdnp();
            }
            while (GetDbTableCount<Person>(x => x.Idnp == idnp) > 0);

            return idnp;
        }

        private T GetLookupByName<T>(string name) where T : Lookup
        {
            T result = Repository.Query<T>().FirstOrDefault(x => x.Name == name);
            if (result == null)
                throw new SrvException(MUI.ObjectNotFound,
                    string.Format("Can not find lookup typeof({0}) with name {1}", typeof(T), name));

            return result;
        }

        private T GetLookupById<T>(long id) where T : Lookup
        {
            T result = Repository.Query<T>().FirstOrDefault(x => x.Id == id);
            if (result == null)
                throw new SrvException(MUI.ObjectNotFound,
                    string.Format("Can not find lookup typeof({0}) with name {1}", typeof(T), id));

            return result;
        }
    }
}
