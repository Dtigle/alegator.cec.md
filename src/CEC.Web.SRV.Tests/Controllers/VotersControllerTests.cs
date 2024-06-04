using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using AutoMapper;
using Moq;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Infrastructure.Export;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class VotersControllerTests : BaseControllerTests
    {
        #region Common Fields

        private static Mock<IConfigurationSettingBll> _configBll;
        private static Mock<IVotersBll> _bll;
        private static Mock<IPrintBll> _printBll;
        private static VotersController _controller;

        #endregion Common Fields

        #region Tests initialization

        public VotersControllerTests()
        {
            Mapper.Initialize(arg =>
            {
                arg.AddProfile<IdentityUserProfile>();
                arg.AddProfile<LookupProfile>();
                arg.AddProfile<SrvGridModelsProfile>();
                arg.AddProfile<HistoryProfile>();
                arg.AddProfile<ElectionProfile>();
                arg.AddProfile<VotersProfile>();
            });
        }

        [TestInitialize]
        public void Startup()
        {
            _configBll = new Mock<IConfigurationSettingBll>();
            _bll = new Mock<IVotersBll>();
            _printBll = new Mock<IPrintBll>();
            _controller = new VotersController(_configBll.Object, _bll.Object, _printBll.Object);
            BaseController = _controller;
        }

        #endregion Tests initialization

        #region Simple view test methods

        [TestMethod]
        public void Index_returns_correct_view()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void StayStatements_returns_correct_view()
        {
            // Act
            var result = _controller.StayStatements() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void StayStatementPrinting_returns_correct_view()
        {
            // Arrange

            const long stayStatementId = 1;

            // Act

            var result = _controller.StayStatementPrinting(stayStatementId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(StayStatementPrintingModel));

            var model = result.Model as StayStatementPrintingModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(stayStatementId, model.StayStatementId);
        }

        #endregion Simple view test methods

        #region Get test methods

        [TestMethod]
        public void GetRegions_search_filter_is_correct_transformed()
        {
            // arrange
            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };
            _bll.Setup(x => x.SearchRegion(It.IsAny<PageRequest>())).Returns(new PageResponse<Region> { Items = new List<Region>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // act

            _controller.GetRegions(request);

            // assert

            _bll.Verify(x => x.SearchRegion(It.IsAny<PageRequest>()), Times.Once());
        }

        [TestMethod]
        public void GetAddresses_search_filter_is_correct_transformed()
        {
            // arrange

            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };

            _bll.Setup(x => x.SearchAddress(It.IsAny<PageRequest>(), null, null)).Returns(new PageResponse<AddressWithPollingStation> { Items = new List<AddressWithPollingStation>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // act

            _controller.GetAddresses(request, null, null);

            // assert

            _bll.Verify(x => x.SearchAddress(It.IsAny<PageRequest>(), null, null), Times.Once());
        }

        [TestMethod]
        public void GetRegions_returns_correct_json()
        {
            // arrange

            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };

            _bll.Setup(x => x.SearchRegion(It.IsAny<PageRequest>())).Returns(new PageResponse<Region> { Items = new List<Region>(), PageSize = 20, StartIndex = 1, Total = 2 });
            // act

            var result = _controller.GetRegions(request);

            // assert

            _bll.Verify(x => x.SearchRegion(It.IsAny<PageRequest>()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public void GetPollingStations_returns_correct_json()
        {
            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };
            GetPollingStationsTest(1, request);
        }

        [TestMethod]
        public void GetPollingStationsWithNullRegionId_returns_correct_json()
        {
            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };
            GetPollingStationsTest(null, request);
        }

        [TestMethod]
        public void GetAddresses_returns_correct_json()
        {
            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };
            GetAddressesTest(request, 1);
        }

        [TestMethod]
        public void GetAddressesWithNullRegionId_returns_correct_json()
        {
            var request = new Select2Request { page = 10, pageLimit = 10, q = "AAA" };
            GetAddressesTest(request, null);
        }

        private static void GetPollingStationsTest(long? regionId, Select2Request request)
        {
            // arrange

            _bll.Setup(x => x.SearchPollingStations(It.IsAny<PageRequest>(), regionId)).Returns(new PageResponse<PollingStation> { Items = new List<PollingStation>(), PageSize = 20, StartIndex = 1, Total = 2 });


            // act

            var result = _controller.GetPollingStations(request, regionId);

            // assert

            _bll.Verify(x => x.SearchPollingStations(It.IsAny<PageRequest>(), regionId), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        private static void GetAddressesTest(Select2Request request, long? regionId)
        {
            // arrange

            _bll.Setup(x => x.SearchAddress(It.IsAny<PageRequest>(), regionId, null)).Returns(new PageResponse<AddressWithPollingStation> { Items = new List<AddressWithPollingStation>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // act

            var result = _controller.GetAddresses(request, regionId, null);

            // assert

            _bll.Verify(x => x.SearchAddress(It.IsAny<PageRequest>(), regionId, null), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        private static void AssertListsAreEqual<T>(IEnumerable<Select2Item> list1, List<T> list2, Func<T, string> textFunc, Func<T, long> idFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.text) && (idFunc(x) == item.id))));
        }

        #endregion Get test methods

        #region Set test methods

        [TestMethod]
        public void SetStatusById_has_correct_view_data()
        {
            // Arrange

            const long personId = 1;

            var person = ArrangePerson();
            var viewData = ArrangeViewModel(GetPersonStatusTypes);

            // Act

            var result = _controller.SetStatus(personId);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertViewData((PartialViewResult)result, "StatusId", x => x.Name, x => x.Id.ToString(), viewData);
        }

        [TestMethod]
        public void SetStatusById_has_correct_view_model()
        {
            // Arrange

            const long personId = 1;

            var person = ArrangePerson();
            var viewData = ArrangeViewModel(GetPersonStatusTypes);

            // Act

            var result = _controller.SetStatus(personId);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertSetStatusViewModel((PartialViewResult)result, person);
        }

        [TestMethod]
        public void SetStatusByNotValidModel_returns_correct_view_model()
        {
            // Arrange

            var model = ArrangeNotValidModel(GetVotersStatusModel);

            // Act

            var result = _controller.SetStatus(model);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertSetStatusViewModel((PartialViewResult)result, model);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
        }

        [TestMethod]
        public void SetStatusByValidModel_returns_correct_content()
        {
            // Arrange

            var model = ArrangeValidSetStatusModel();

            // Act

            var result = _controller.SetStatus(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ContentResult));
            Assert.IsTrue(_controller.ViewData.ModelState.IsValid);

            _bll.Verify(x => x.UpdateStatus(model.PersonInfo.Id, model.StatusId, model.ConfirmationNew), Times.Once);

            Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
        }

        [TestMethod]
        public void CreateStayStatementById_has_correct_view_model()
        {
            // Arrange

            const long personId = 1;
            var person = ArrangePerson();

            var viewData = GetElections();
            _bll.Setup(x => x.GetElection()).Returns(viewData);

            // Act

            var result = _controller.CreateStayStatement(personId);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateStayStatementViewModel((PartialViewResult)result, person);
        }

        [TestMethod]
        public void CreateStayStatementByModel_returns_correct_json()
        {
            // Arrange

            var person = GetPerson();
            var elections = GetElections();
            const bool stayStatementExists = false;
            const long stayStatementId = 1;

            var model = ArrangeValidStayStatementModel(person, elections, stayStatementExists, stayStatementId);

            // Act

            var result = _controller.CreateStayStatement(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult));

            var json = result as JsonResult;

            Assert.IsTrue(_controller.ModelState.IsValid);
            Assert.AreNotEqual(model.DeclaredStayAddressInfo.AddressId, 0);
            Assert.AreNotEqual(model.DeclaredStayAddressInfo.AddressId, person.EligibleAddress.Address.Id);
            Assert.IsFalse(stayStatementExists);

            Assert.IsNotNull(json.Data);

            var ssIdProperty = json.Data.GetType().GetProperty("ssId");
            Assert.IsNotNull(ssIdProperty);

            var value = ssIdProperty.GetValue(json.Data);

            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(long));

            Assert.AreEqual((long)value, stayStatementId);
        }

        [TestMethod]
        public void CreateStayStatementByZeroDeclaredStayAddressIdModel_returns_correct_view_model()
        {
            // Arrange

            var person = GetPerson();
            var elections = GetElections();
            const bool stayStatementExists = false;
            const long stayStatementId = 1;

            var model = ArrangeValidStayStatementModel(person, elections, stayStatementExists, stayStatementId);
            model.DeclaredStayAddressInfo.AddressId = 0;

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.CreateStayStatement(model);

            // Assert

            Assert.AreEqual(model.DeclaredStayAddressInfo.AddressId, 0);
            Assert.IsTrue(_controller.ViewData.ModelState["DeclaredStayAddressInfo.AddressId"].Errors.Count > 0);

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateStayStatementViewModel((PartialViewResult)result, model);
        }

        [TestMethod]
        public void CreateStayStatementByCurrentDeclaredStayAddressIdModel_returns_correct_view_model()
        {
            // Arrange

            var person = GetPerson();
            var elections = GetElections();
            const bool stayStatementExists = false;
            const long stayStatementId = 1;

            person.EligibleAddress.Address.SetId(2);
            var model = ArrangeValidStayStatementModel(person, elections, stayStatementExists, stayStatementId);
            model.DeclaredStayAddressInfo.AddressId = person.EligibleAddress.Address.Id;

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.CreateStayStatement(model);

            // Assert

            Assert.AreNotEqual(model.DeclaredStayAddressInfo.AddressId, 0);
            Assert.AreEqual(model.DeclaredStayAddressInfo.AddressId, person.EligibleAddress.Address.Id);
            Assert.IsTrue(_controller.ViewData.ModelState["DeclaredStayAddressInfo.AddressId"].Errors.Count > 0);

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateStayStatementViewModel((PartialViewResult)result, model);
        }

        [TestMethod]
        public void CreateStayStatementWhenStayStatementExistsByModel_returns_correct_view_model()
        {
            // Arrange

            var person = GetPerson();
            var elections = GetElections();
            const bool stayStatementExists = true;
            const long stayStatementId = 1;

            var model = ArrangeValidStayStatementModel(person, elections, stayStatementExists, stayStatementId);

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.CreateStayStatement(model);

            // Assert

            Assert.AreNotEqual(model.DeclaredStayAddressInfo.AddressId, 0);
            Assert.AreNotEqual(model.DeclaredStayAddressInfo.AddressId, person.EligibleAddress.Address.Id);
            Assert.IsTrue(stayStatementExists);
            Assert.IsTrue(_controller.ViewData.ModelState["ElectionInfo.ElectionId"].Errors.Count > 0);

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateStayStatementViewModel((PartialViewResult)result, model);
        }

        [TestMethod]
        public void StayStatementView_has_correct_view_model()
        {
            // Arrange

            const long id = 1;
            var stayStatement = GetStayStatement();
            _bll.Setup(x => x.Get<StayStatement>(id)).Returns(stayStatement);

            // Act

            var result = _controller.StayStatementView(id) as PartialViewResult;

            // Assert

            AssertStayStatementViewModel(result, stayStatement);
        }

        [TestMethod]
        public void UpdateAddress_has_correct_view_model()
        {
            // Arrange

            const long personId = 1;

            var person = ArrangePersonWithEligibleResidence();

            // Act

            var result = _controller.UpdateAddress(personId) as PartialViewResult;

            // Assert

            AssertUpdateAddressViewModel(result, person, personId);
        }

        [TestMethod]
        public void UpdateAddressByNotValidModel_returns_correct_view_model()
        {
            // Arrange

            var model = ArrangeNotValidModel(GetUpdateAddressModel);

            // Act

            var result = _controller.UpdateAddress(model);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertUpdateAddressViewModel((PartialViewResult)result, model);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
        }

        [TestMethod]
        public void UpdateAddressByValidModelAndNonZeroDeclaredAddressId_returns_correct_content()
        {
            // Arrange

            var model = ArrangeValidUpdateAddressModel();

            // Act

            var result = _controller.UpdateAddress(model);

            // Assert

            Assert.AreNotEqual(model.DeclaredStayAddressInfo.AddressId, 0);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ContentResult));
            Assert.IsTrue(_controller.ViewData.ModelState.IsValid);

            _bll.Verify(x => x.UpdateAddress(model.Id, model.DeclaredStayAddressInfo.AddressId, model.DeclaredStayAddressInfo.ApNumber, model.DeclaredStayAddressInfo.ApSuffix), Times.Once);

            Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
        }

        [TestMethod]
        public void UpdateAddressByValidModelAndZeroDeclaredAddressId_returns_correct_view_model()
        {
            // Arrange

            var model = ArrangeValidUpdateAddressModel();
            model.DeclaredStayAddressInfo.AddressId = 0;

            // Act

            var result = _controller.UpdateAddress(model);

            // Assert

            Assert.AreEqual(model.DeclaredStayAddressInfo.AddressId, 0);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertUpdateAddressViewModel((PartialViewResult)result, model);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            _bll.Verify(x => x.UpdateAddress(model.Id, model.DeclaredStayAddressInfo.AddressId, model.DeclaredStayAddressInfo.ApNumber, model.DeclaredStayAddressInfo.ApSuffix), Times.Never);
            Assert.IsTrue(_controller.ViewData.ModelState["DeclaredStayAddressInfo.AddressId"].Errors.Count > 0);
        }

        [TestMethod]
        public void ChangePollingStationForm_has_correct_view_model()
        {
            // Arrange

            long[] peopleIds = { 1, 2, 3 };
            var pollingStations = GetPollingStations();

            _bll.Setup(x => x.VerificationSameRegion(peopleIds));
            _bll.Setup(x => x.GetRegionPollingStationsByPerson()).Returns(pollingStations);

            // Act

            var result = _controller.ChangePollingStationForm(peopleIds) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            _bll.Verify(x => x.VerificationSameRegion(peopleIds), Times.Once);
        }

        [TestMethod]
        public void ChangePollingStationByNotValidModel_returns_correct_view_model()
        {
            // Arrange

            var pollingStations = GetPollingStations();
            _bll.Setup(x => x.GetRegionPollingStationsByPerson()).Returns(pollingStations);

            var model = ArrangeNotValidModel(GetChangePollingStationModel);


            // Act

            var result = _controller.ChangePollingStation(model);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertChangePollingStationViewModel((PartialViewResult)result, model);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
        }

        [TestMethod]
        public void ChangePollingStationByValidModel_returns_correct_content()
        {
            // Arrange

            var model = ArrangeValidChangePollingStationModel();

            // Act

            var result = _controller.ChangePollingStation(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ContentResult));
            Assert.IsTrue(_controller.ViewData.ModelState.IsValid);

            _bll.Verify(x => x.ChangePollingStation(model.NewPollingStation.PStationId, model.PeopleIds), Times.Once);

            Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
        }

        #region Arrange Functions

        private static Person ArrangePerson()
        {
            var person = GetPerson();
            _bll.Setup(x => x.Get<Person>(It.IsAny<long>())).Returns(person);
            return person;
        }

        private static Person ArrangePersonWithEligibleResidence()
        {
            var person = GetPerson();
            _bll.Setup(x => x.GetPersonWithEligibleResidence(It.IsAny<long>())).Returns(person);
            return person;
        }

        private static T ArrangeNotValidModel<T>(Func<T> modelFunc)
        {
            var model = modelFunc();
            _controller.ModelState.AddModelError("", "Error");
            return model;
        }

        private static UpdateVotersStatusModel ArrangeValidSetStatusModel()
        {
            var model = GetVotersStatusModel();
            _bll.Setup(x => x.UpdateStatus(model.PersonInfo.Id, model.StatusId, model.ConfirmationNew));
            return model;
        }

        private static StayStatementModel ArrangeValidStayStatementModel(Person person, IList<Election> elections, bool stayStatementExists, long stayStatementId)
        {
            var model = GetStayStatementModel();

            _bll.Setup(x => x.Get<Person>(model.PersonInfo.PersonId)).Returns(person);
            _bll.Setup(x => x.ElectionUniqueStayStatement(model.PersonInfo.PersonId, model.ElectionInfo.ElectionId))
                .Returns(stayStatementExists);
            _bll.Setup(x => x.CreateStayStatement(model.Id, model.PersonInfo.PersonId, model.DeclaredStayAddressInfo.AddressId,
                        model.DeclaredStayAddressInfo.ApNumber, model.DeclaredStayAddressInfo.ApSuffix,
                        model.ElectionInfo.ElectionId)).Returns(stayStatementId);
            _bll.Setup(x => x.GetElection()).Returns(elections);

            return model;
        }

        private static UpdateAddressModel ArrangeValidUpdateAddressModel()
        {
            var model = GetUpdateAddressModel();
            _bll.Setup(x => x.UpdateAddress(model.Id, model.DeclaredStayAddressInfo.AddressId, model.DeclaredStayAddressInfo.ApNumber, model.DeclaredStayAddressInfo.ApSuffix));
            return model;
        }

        private static ChangePollingStationModel ArrangeValidChangePollingStationModel()
        {
            var model = GetChangePollingStationModel();
            _bll.Setup(x => x.ChangePollingStation(model.NewPollingStation.PStationId, model.PeopleIds));
            return model;
        }

        #endregion Arrange Functions

        #region Assert Functions

        private static void AssertSetStatusViewModel(ViewResultBase view, Person person)
        {
            Assert.IsNotNull(view);
            Assert.AreEqual("_UpdateVotersStatus", view.ViewName);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(UpdateVotersStatusModel));

            var model = view.Model as UpdateVotersStatusModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.PersonInfo.Idnp, person.Idnp);
            Assert.AreEqual(model.PersonInfo.FullName, person.FullName);
        }

        private static void AssertSetStatusViewModel(ViewResultBase view, UpdateVotersStatusModel expectedModel)
        {
            Assert.IsNotNull(view);
            Assert.AreEqual("_UpdateVotersStatus", view.ViewName);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(UpdateVotersStatusModel));

            var model = view.Model as UpdateVotersStatusModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.PersonInfo.Idnp, expectedModel.PersonInfo.Idnp);
            Assert.AreEqual(model.PersonInfo.FullName, expectedModel.PersonInfo.FullName);
        }

        private static void AssertCreateStayStatementViewModel(ViewResultBase view, Person person)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(StayStatementModel));

            var model = view.Model as StayStatementModel;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PersonInfo);
            Assert.AreEqual(model.PersonInfo.IDNP, person.Idnp);
            Assert.AreEqual(model.PersonInfo.DateOfBirth, person.DateOfBirth);
            Assert.AreEqual(model.PersonInfo.FullName, person.FullName);

            if (person.Document != null)
            {
                Assert.AreEqual(model.PersonInfo.DocType, person.Document.Type.Description);
                Assert.AreEqual(model.PersonInfo.DocNumber, person.Document.DocumentNumber);
                Assert.AreEqual(model.PersonInfo.DocIssuedDate, person.Document.IssuedDate);
                Assert.AreEqual(model.PersonInfo.DocIssuedBy, person.Document.IssuedBy);
                Assert.AreEqual(model.PersonInfo.DocValidBy, person.Document.ValidBy);
            }

            if (person.EligibleAddress != null)
            {
                Assert.IsNotNull(model.BaseAddressInfo);
                Assert.AreEqual(model.BaseAddressInfo.PersonAddressId, person.EligibleAddress.Id);
                Assert.IsNotNull(person.EligibleAddress.Address);
                Assert.AreEqual(model.BaseAddressInfo.AddressId, person.EligibleAddress.Address.Id);
                Assert.AreEqual(model.BaseAddressInfo.FullAddress, person.EligibleAddress.GetFullPersonAddress(true));
            }
        }

        private static void AssertCreateStayStatementViewModel(ViewResultBase view, StayStatementModel expectedModel)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(StayStatementModel));

            var model = view.Model as StayStatementModel;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PersonInfo);
            Assert.IsNotNull(expectedModel.PersonInfo);
            Assert.AreEqual(model.PersonInfo.IDNP, expectedModel.PersonInfo.IDNP);
            Assert.AreEqual(model.PersonInfo.DateOfBirth, expectedModel.PersonInfo.DateOfBirth);
            Assert.AreEqual(model.PersonInfo.FullName, expectedModel.PersonInfo.FullName);

            Assert.AreEqual(model.PersonInfo.DocType, expectedModel.PersonInfo.DocType);
            Assert.AreEqual(model.PersonInfo.DocNumber, expectedModel.PersonInfo.DocNumber);
            Assert.AreEqual(model.PersonInfo.DocIssuedDate, expectedModel.PersonInfo.DocIssuedDate);
            Assert.AreEqual(model.PersonInfo.DocIssuedBy, expectedModel.PersonInfo.DocIssuedBy);
            Assert.AreEqual(model.PersonInfo.DocValidBy, expectedModel.PersonInfo.DocValidBy);

            Assert.IsNotNull(model.BaseAddressInfo);
            Assert.IsNotNull(expectedModel.BaseAddressInfo);
            Assert.AreEqual(model.BaseAddressInfo.PersonAddressId, expectedModel.BaseAddressInfo.PersonAddressId);
            Assert.AreEqual(model.BaseAddressInfo.AddressId, expectedModel.BaseAddressInfo.AddressId);
            Assert.AreEqual(model.BaseAddressInfo.FullAddress, expectedModel.BaseAddressInfo.FullAddress);
        }

        private static void AssertStayStatementViewModel(ViewResultBase view, StayStatement stayStatement)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(StayStatementViewModel));

            var model = view.Model as StayStatementViewModel;

            var person = stayStatement.Person;

            Assert.IsNotNull(person);
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PersonInfo);
            Assert.AreEqual(model.PersonInfo.IDNP, person.Idnp);
            Assert.AreEqual(model.PersonInfo.DateOfBirth, person.DateOfBirth);
            Assert.AreEqual(model.PersonInfo.FullName, person.FullName);

            if (person.Document != null)
            {
                Assert.AreEqual(model.PersonInfo.DocType, person.Document.Type.Description);
                Assert.AreEqual(model.PersonInfo.DocNumber, person.Document.DocumentNumber);
                Assert.AreEqual(model.PersonInfo.DocIssuedDate, person.Document.IssuedDate);
                Assert.AreEqual(model.PersonInfo.DocIssuedBy, person.Document.IssuedBy);
                Assert.AreEqual(model.PersonInfo.DocValidBy, person.Document.ValidBy);
            }

            var baseAddress = stayStatement.BaseAddress;

            Assert.IsNotNull(baseAddress);
            Assert.IsNotNull(model.BaseAddressInfo);
            Assert.AreEqual(model.BaseAddressInfo.PersonAddressId, baseAddress.Id);
            Assert.IsNotNull(baseAddress.Address);
            Assert.AreEqual(model.BaseAddressInfo.AddressId, baseAddress.Address.Id);
            Assert.AreEqual(model.BaseAddressInfo.FullAddress, baseAddress.GetFullPersonAddress(true));

            var declaredAddress = stayStatement.DeclaredStayAddress;

            Assert.IsNotNull(declaredAddress);
            Assert.IsNotNull(model.DeclaredStayAddressInfo);
            Assert.IsNotNull(declaredAddress.Address);
            Assert.AreEqual(model.DeclaredStayAddressInfo.AddressId, declaredAddress.Address.Id);
            Assert.AreEqual(model.DeclaredStayAddressInfo.ApNumber, declaredAddress.ApNumber);
            Assert.AreEqual(model.DeclaredStayAddressInfo.ApSuffix, declaredAddress.ApSuffix);
            Assert.AreEqual(model.DeclaredStayAddressInfo.FullAddress, declaredAddress.GetFullPersonAddress(true));

            var election = stayStatement.ElectionInstance;

            Assert.IsNotNull(election);
            Assert.IsNotNull(model.ElectionInfo);
            Assert.AreEqual(model.ElectionInfo.ElectionId, election.Id);
            Assert.AreEqual(model.ElectionInfo.ElectionDate, election.StatusDate);
            Assert.AreEqual(model.ElectionInfo.ElectionTypeName, election.NameRo);

            if (stayStatement.Created.HasValue)
            {
                Assert.AreEqual(model.CreationDate, stayStatement.Created.Value.LocalDateTime.ToShortDateString());
            }
        }

        private static void AssertUpdateAddressViewModel(ViewResultBase view, Person person, long personId)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(UpdateAddressModel));

            var model = view.Model as UpdateAddressModel;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PersonInfo);
            Assert.AreEqual(model.PersonInfo.PersonId, personId);
            Assert.AreEqual(model.PersonInfo.FullName, person.FullName);

            Assert.IsNotNull(model.BaseAddressInfo);
            Assert.AreEqual(model.BaseAddressInfo.PersonAddressId, person.EligibleAddress.Id);
            Assert.IsNotNull(person.EligibleAddress.Address);
            Assert.AreEqual(model.BaseAddressInfo.AddressId, person.EligibleAddress.Address.Id);
            Assert.AreEqual(model.BaseAddressInfo.FullAddress, person.EligibleAddress.GetFullPersonAddress(true));

            Assert.IsNotNull(model.DeclaredStayAddressInfo);
        }

        private static void AssertUpdateAddressViewModel(ViewResultBase view, UpdateAddressModel expectedModel)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(UpdateAddressModel));

            var model = view.Model as UpdateAddressModel;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PersonInfo);
            Assert.IsNotNull(expectedModel.PersonInfo);
            Assert.AreEqual(model.PersonInfo.PersonId, expectedModel.PersonInfo.PersonId);
            Assert.AreEqual(model.PersonInfo.FullName, expectedModel.PersonInfo.FullName);

            Assert.IsNotNull(model.BaseAddressInfo);
            Assert.IsNotNull(expectedModel.BaseAddressInfo);
            Assert.AreEqual(model.BaseAddressInfo.PersonAddressId, expectedModel.BaseAddressInfo.PersonAddressId);
            Assert.AreEqual(model.BaseAddressInfo.AddressId, expectedModel.BaseAddressInfo.AddressId);
            Assert.AreEqual(model.BaseAddressInfo.FullAddress, expectedModel.BaseAddressInfo.FullAddress);

            Assert.IsNotNull(model.DeclaredStayAddressInfo);
            Assert.IsNotNull(expectedModel.DeclaredStayAddressInfo);
        }

        private static void AssertChangePollingStationViewModel(ViewResultBase view, ChangePollingStationModel expectedModel)
        {
            Assert.IsNotNull(view);
            Assert.AreEqual("_ChangePollingStation", view.ViewName);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(ChangePollingStationModel));

            var model = view.Model as ChangePollingStationModel;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PeopleIds);
            Assert.IsNotNull(expectedModel.PeopleIds);
            AssertListsAreEqual(model.PeopleIds, expectedModel.PeopleIds);
            Assert.IsNotNull(model.NewPollingStation);
            Assert.IsNotNull(expectedModel.NewPollingStation);
            Assert.AreEqual(model.NewPollingStation.FullNumber, expectedModel.NewPollingStation.FullNumber);
            Assert.AreEqual(model.NewPollingStation.PStationId, expectedModel.NewPollingStation.PStationId);
        }

        private static void AssertListsAreEqual<T>(List<T> list1, List<T> list2)
        {
            Assert.AreEqual(list1.Count(), list2.Count());
            Assert.IsTrue(list1.All(item => list2.Exists(x => x.Equals(item))));
        }

        #endregion Assert Functions

        #endregion Set test methods

        #region Select test methods

        [TestMethod]
        public void SelectVoterStatus_returns_correct_view()
        {
            SelectTest(_controller.SelectVoterStatus, GetPersonStatusTypes, x => x.Name, x => x.Id.ToString());
        }

        [TestMethod]
        public void SelectVoterGender_returns_correct_view()
        {
            SelectTest(_controller.SelectVoterGender, GetGenders, x => x.Name, x => x.Name);
        }

        [TestMethod]
        public void SelectElectionInstance_returns_correct_view()
        {
            SelectTest(_controller.SelectElectionInstance, GetElections, x => x.NameRo, x => x.Id.ToString());
        }

        [TestMethod]
        public void SelectAddressType_returns_correct_view()
        {
            SelectTest(_controller.SelectAddressType, GetPersonAddressTypes, x => x.Name, x => x.Id.ToString());
        }

        private static void SelectTest<T>(Func<ActionResult> controllerFunc, Func<List<T>> modelFunc, Func<T, string> textFunc, Func<T, string> valueFunc) where T : AuditedEntity<IdentityUser>
        {
            // Arrange

            var model = ArrangeViewModel(modelFunc);

            // Act

            var result = controllerFunc() as PartialViewResult;

            // Assert

            AssertViewModel(result, textFunc, valueFunc, model);
        }

        private static List<T> ArrangeViewModel<T>(Func<List<T>> listFunc) where T : AuditedEntity<IdentityUser>
        {
            var list = listFunc();

            _bll.Setup(x => x.GetAll<T>()).Returns(list);

            return list;
        }

        private static void AssertViewModel<T>(PartialViewResult view, Func<T, string> textFunc, Func<T, string> valueFunc, List<T> expectedModel) where T : AuditedEntity<IdentityUser>
        {
            AssertViewModel(view, view.Model, textFunc, valueFunc, expectedModel);
        }

        private static void AssertViewData<T>(PartialViewResult view, string viewDataKey, Func<T, string> textFunc, Func<T, string> valueFunc, List<T> expectedViewData) where T : AuditedEntity<IdentityUser>
        {
            AssertViewModel(view, view.ViewData[viewDataKey], textFunc, valueFunc, expectedViewData);
        }

        private static void AssertViewModel<T>(PartialViewResult view, object viewModel, Func<T, string> textFunc, Func<T, string> valueFunc, List<T> expectedModel) where T : AuditedEntity<IdentityUser>
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(List<SelectListItem>));

            var model = viewModel as List<SelectListItem>;

            AssertListsAreEqual(model, expectedModel, textFunc, valueFunc);
        }

        private static void AssertListsAreEqual<T>(IEnumerable<SelectListItem> list1, List<T> list2, Func<T, string> textFunc, Func<T, string> valueFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.Text) && string.Equals(valueFunc(x), item.Value))));
        }

        #endregion Select test methods

        #region Ajax test methods

        [TestMethod]
        public void ListVotersAjaxNullRegionIdNullPollingStationId_returns_correct_format()
        {
            ListVotersAjaxTest(null, null);
        }

        [TestMethod]
        public void ListVotersAjaxNullRegionId_returns_correct_format()
        {
            ListVotersAjaxTest(null, 1);
        }

        [TestMethod]
        public void ListVotersAjaxNullPollingStationId_returns_correct_format()
        {
            ListVotersAjaxTest(1, null);
        }

        [TestMethod]
        public void ListVotersAjax_returns_correct_format()
        {
            ListVotersAjaxTest(1, 1);
        }

        [TestMethod]
        public void SearchVotersByIdnpAjax_returns_correct_format()
        {
            SearchVotersByIdnpAjaxTest("2003123456789");
        }

        [TestMethod]
        public void SearchVotersByNullIdnpAjax_returns_correct_format()
        {
            SearchVotersByIdnpAjaxTest(null);
        }

        [TestMethod]
        public void ListStayStatementsAjax_returns_correct_format()
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.GetStayStatements(It.IsAny<PageRequest>()))
                .Returns(new PageResponse<StayStatement> { Items = new List<StayStatement>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // Act

            var result = _controller.ListStayStatementsAjax(request);

            // Assert

            _bll.Verify(x => x.GetStayStatements(It.IsAny<PageRequest>()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        private static void ListVotersAjaxTest(long? regionId, long? pollingStationId)
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.GetByFilters(It.IsAny<PageRequest>(), regionId, pollingStationId))
                .Returns(new PageResponse<VoterRow> { Items = new List<VoterRow>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // Act

            var result = _controller.ListVotersAjax(request, regionId, pollingStationId);

            // Assert

            _bll.Verify(x => x.GetByFilters(It.IsAny<PageRequest>(), regionId, pollingStationId), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        private static void SearchVotersByIdnpAjaxTest(string idnp)
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.GetIdnp(It.IsAny<PageRequest>(), idnp))
                .Returns(new PageResponse<Person> { Items = new List<Person>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // Act

            var result = _controller.SearchVotersByIdnpAjax(request, idnp);

            // Assert

            _bll.Verify(x => x.GetIdnp(It.IsAny<PageRequest>(), idnp), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        #endregion Ajax test methods

        #region Bll actions test method

        [TestMethod]
        public void CancelStayStatement_has_correct_logic()
        {
            // Arrange

            const long stayStatementId = 1;

            _bll.Setup(x => x.CancelStayStatement(stayStatementId));

            // Act

            _controller.CancelStayStatement(stayStatementId);

            // Assert

            _bll.Verify(x => x.CancelStayStatement(stayStatementId), Times.Once());
        }

        #endregion Bll actions test method

        #region Export test methods

        [TestMethod]
        public void ExportVotersAllData_has_correct_logic()
        {
            ExportGridDataTest<VotersGridModel>(ExportType.AllData, "Alegatori");
        }

        [TestMethod]
        public void ExportVotersCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<VotersGridModel>(ExportType.CurrentPage, "Alegatori");
        }

        [TestMethod]
        public void ExportStayStatementsAllData_has_correct_logic()
        {
            ExportGridDataTest<StayStatementsGridModel>(ExportType.AllData, "DeclaratiiSedere");
        }

        [TestMethod]
        public void ExportStayStatementsCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<StayStatementsGridModel>(ExportType.CurrentPage, "DeclaratiiSedere");
        }

        #endregion Export test methods

        #region Initialization of mock objects

        #region Lists

        private static List<PersonStatusType> GetPersonStatusTypes()
        {
            return new List<PersonStatusType>
            {
                GetPersonStatusType0(),
                GetPersonStatusType1()
            };
        }

        private static List<Gender> GetGenders()
        {
            return new List<Gender>
            {
                GetGender0(),
                GetGender1()
            };
        }

        private static List<Election> GetElections()
        {
            var elections = new List<Election>
            {
                GetElection0(),
                GetElection1()
            };

            return elections;
        }

        private static List<PersonAddressType> GetPersonAddressTypes()
        {
            return new List<PersonAddressType>
            {
                GetPersonAddressType0(),
                GetPersonAddressType1()
            };
        }
        private static List<PollingStation> GetPollingStations()
        {
            var pollingStations = new List<PollingStation>
            {
                GetPollingStation0(),
                GetPollingStation1(),
                GetPollingStation2()
            };

            return pollingStations;
        }

        #endregion Lists

        #region Entities

        private static PersonStatusType GetPersonStatusType0()
        {
            return new PersonStatusType
            {
                Description = "Persoana ce are drept la vot",
                IsExcludable = false,
                Name = "Alegator"
            };
        }

        private static PersonStatusType GetPersonStatusType1()
        {
            return new PersonStatusType
            {
                Description = "Persoana ce a decedat",
                IsExcludable = true,
                Name = "Decedat"
            };
        }

        private static Gender GetGender0()
        {
            return new Gender
            {
                Description = "Masculin",
                Name = "M"
            };
        }

        private static Gender GetGender1()
        {
            return new Gender
            {
                Description = "Feminin",
                Name = "F"
            };
        }

        private static Election GetElection0()
        {
            return new Election
            {
                //AcceptAbroadDeclaration = true,
                //Comments = "Comentarii",
                //ElectionDate = new DateTime(2014, 11, 30),
                //ElectionType = new ElectionType
                //{
                //    Description = "Alegeri parlamentare in RM",
                //    Name = "Alegeri parlamentare"
                //},
                //SaiseId = null
            };
        }

        private static Election GetElection1()
        {
            return new Election
            {
                //AcceptAbroadDeclaration = false,
                //Comments = "Comentarii",
                //ElectionDate = new DateTime(2014, 12, 30),
                //ElectionType = new ElectionType
                //{
                //    Description = "Alegeri prezidentiale in RM",
                //    Name = "Alegeri prezidentiale"
                //},
                //SaiseId = null
            };
        }

        private static PersonAddressType GetPersonAddressType0()
        {
            return new PersonAddressType
            {
                Description = "locul unde isi are viza de resedinta persoana data",
                Name = "Viza de Resedinta"
            };
        }

        private static PersonAddressType GetPersonAddressType1()
        {
            return new PersonAddressType
            {
                Description = "locul unde isi are domiciliul persoana data",
                Name = "Domiciliu"
            };
        }

        private static Region GetRegion0()
        {
            return new Region(
                new RegionType
                {
                    Description = "oras mare care are in subordine alte orase",
                    Name = "municipiu",
                    Rank = 1
                })
            {
                Circumscription = 1,
                Description = "capitala RM",
                GeoLocation = new GeoLocation { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinau"
            };
        }

        private static Region GetRegion1()
        {
            return new Region(
                new RegionType
                {
                    Description = "oras mare care are in subordine alte orase",
                    Name = "municipiu",
                    Rank = 1
                })
            {
                Circumscription = 2,
                Description = "capitala de nord a RM",
                GeoLocation = new GeoLocation { Latitude = 30, Longitude = 10 },
                HasStreets = true,
                Name = "Balti"
            };
        }

        private static PollingStation GetPollingStation0()
        {
            var region = GetRegion0();

            var geoLocation = new GeoLocation { Latitude = 40, Longitude = 20 };

            return new PollingStation(region)
            {
                ContactInfo = "a@a.a",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, CENTRU",
                SaiseId = null,
                Number = "11",
                PollingStationAddress = GetAddress0(region, geoLocation),
                SubNumber = "1"
            };
        }

        private static PollingStation GetPollingStation1()
        {
            var region = GetRegion1();

            var geoLocation = new GeoLocation { Latitude = 20, Longitude = 40 };

            return new PollingStation(region)
            {
                ContactInfo = "b@b.b",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, BOTANICA",
                SaiseId = null,
                Number = "11",
                SubNumber = "2"
            };
        }

        private static PollingStation GetPollingStation2()
        {
            var region = GetRegion1();

            var geoLocation = new GeoLocation { Latitude = 20, Longitude = 40 };

            return new PollingStation(region)
            {
                ContactInfo = "b@b.b",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, BOTANICA",
                SaiseId = null,
                Number = "11",
                PollingStationAddress = GetAddress1(region, geoLocation),
                SubNumber = "2"
            };
        }

        private static Address GetAddress0()
        {
            var region = GetRegion0();

            var geoLocation = new GeoLocation { Latitude = 40, Longitude = 20 };

            return GetAddress0(region, geoLocation);
        }

        private static Address GetAddress1()
        {
            var region = GetRegion1();

            var geoLocation = new GeoLocation { Latitude = 20, Longitude = 40 };

            return GetAddress1(region, geoLocation);
        }

        private static Address GetAddress0(Region region, GeoLocation geoLocation)
        {
            return new Address
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = geoLocation,
                HouseNumber = 12,
                PollingStation = null,
                Street = new Street(
                    region,
                    new StreetType
                    {
                        Description = "strada noua, cu pavaj de ultima generatie",
                        Name = "alee"
                    },
                    "V. Alecsandri",
                    true)
                {
                    Description = "The best street",
                    RopId = null,
                    SaiseId = null
                }
            };
        }

        private static Address GetAddress1(Region region, GeoLocation geoLocation)
        {
            return new Address
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = geoLocation,
                HouseNumber = 12,
                PollingStation = null,
                Street = new Street(
                    region,
                    new StreetType
                    {
                        Description = "strada veche",
                        Name = "bulevard"
                    },
                    "Lermontov",
                    true)
                {
                    Description = "strada veche",
                    RopId = null,
                    SaiseId = null
                }
            };
        }

        private static Person GetPerson()
        {
            var person = new Person("2003123456789")
            {
                AlegatorId = 1,
                Comments = "Persoana cu IDNP 2003123456789",
                DateOfBirth = new DateTime(1974, 11, 15),
                Document = new PersonDocument
                {
                    Number = "12345678",
                    Seria = "A",
                    IssuedBy = "Of.11",
                    IssuedDate = new DateTime(1992, 11, 16),
                    Type = new DocumentType
                    {
                        Description = "Buletin de identitate al cetateanului RM",
                        IsPrimary = true,
                        Name = "Buletin de identitate"
                    },
                    ValidBy = new DateTime(2022, 11, 16)
                },
                FirstName = "Alexandru",
                Surname = "Grigore",
                MiddleName = "Ion",
                Gender = GetGender0()
            };

            person.SetEligibleAddress(GetPersonAddress(person));
            return person;
        }

        private static PersonAddress GetPersonAddress(Person person)
        {
            return new PersonAddress
            {
                Address = GetAddress0(),
                ApNumber = 12,
                ApSuffix = "A",
                IsEligible = true,
                Person = person,
                PersonAddressType = GetPersonAddressType0()
            };
        }

        private static PersonAddress GetDeclaredPersonAddress(Person person)
        {
            return new PersonAddress
            {
                Address = GetAddress1(),
                ApNumber = 10,
                ApSuffix = "B",
                IsEligible = true,
                Person = person,
                PersonAddressType = GetPersonAddressType1()
            };
        }

        private static StayStatement GetStayStatement()
        {
            var person = GetPerson();
            return new StayStatement(person, GetPersonAddress(person), GetDeclaredPersonAddress(person),
                GetElection0())
            {
                Created = new DateTimeOffset(new DateTime(2014, 8, 22))
            };
        }

        #endregion Entities

        #region Models

        private static UpdateVotersStatusModel GetVotersStatusModel()
        {
            return new UpdateVotersStatusModel
            {
                PersonInfo = new PersonInfo
                {
                    Address = "str. V. Alecsandri, 16",
                    FullName = "Andrei Gheorghe",
                    Idnp = "2003987654321"
                }
            };
        }

        private static StayStatementModel GetStayStatementModel()
        {
            return new StayStatementModel
            {
                PersonInfo = GetPersonModel(),
                BaseAddressInfo = GetPersonAddressModel0(),
                DeclaredStayAddressInfo = GetPersonAddressModel1(),
                ElectionInfo = new ElectionModel { ElectionDate = DateTime.Now, ElectionId = 1 },
                StayStatementRegionId = 1,
                HasStreets = true,
                RegionStreetsType = RegionStreetsType.Mixed,
                StayStatementPollingStationId = 1
            };
        }

        private static PersonAddressModel GetPersonAddressModel0()
        {
            return new PersonAddressModel
            {
                AddressId = 1,
                ApNumber = 12,
                ApSuffix = "A",
                FullAddress = "str. V. Alecsandri, 16",
                PersonAddressId = 1
            };
        }

        private static PersonAddressModel GetPersonAddressModel1()
        {
            return new PersonAddressModel
            {
                AddressId = 2,
                ApNumber = 23,
                ApSuffix = "B",
                FullAddress = "str. I. Creanga, 11",
                PersonAddressId = 1
            };
        }

        private static UpdateAddressModel GetUpdateAddressModel()
        {
            return new UpdateAddressModel
            {
                BaseAddressInfo = GetPersonAddressModel0(),
                DeclaredStayAddressInfo = GetPersonAddressModel1(),
                Id = 1,
                PersonInfo = GetPersonModel()
            };
        }

        private static PersonModel GetPersonModel()
        {
            return new PersonModel
            {
                DateOfBirth = new DateTime(1974, 11, 15),
                DocIssuedBy = "Of.11",
                DocIssuedDate = new DateTime(1992, 11, 16),
                DocNumber = "12345678",
                DocType = "Buletin de identitate",
                DocValidBy = new DateTime(2022, 11, 15),
                FullName = "Andrei Gheorghe",
                IDNP = "2003987654321",
                PersonId = 1
            };
        }

        private static ChangePollingStationModel GetChangePollingStationModel()
        {
            return new ChangePollingStationModel
            {
                NewPollingStation = GetPersonPollingStationModel(),
                PeopleIds = new List<long> { 1, 2, 3 }
            };
        }

        private static PersonPollingStationModel GetPersonPollingStationModel()
        {
            return new PersonPollingStationModel
            {
                FullNumber = "1/11",
                PStationId = 1
            };
        }

        #endregion Models

        #endregion Initialization of mock objects
    }
}

