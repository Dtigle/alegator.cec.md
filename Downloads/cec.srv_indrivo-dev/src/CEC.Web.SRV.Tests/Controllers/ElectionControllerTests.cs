using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Controllers;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Models.Election;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class ElectionControllerTests : BaseControllerTests
    {
        private static Mock<IElectionBll> _electionBll;
        private static Mock<ILookupBll> _lookupBll;
        private static ElectionController _controller;
        private static List<ElectionType> _electionTypes;
        private static UpdateElectionModel _model;
        private static Election _election;
        private static long _electionId;
        
        public ElectionControllerTests()
        {
            Mapper.Initialize(arg =>
            {
                arg.AddProfile<IdentityUserProfile>();
                arg.AddProfile<LookupProfile>();
                arg.AddProfile<SrvGridModelsProfile>();
                arg.AddProfile<HistoryProfile>();
                arg.AddProfile<ElectionProfile>();
            });
        }

        [TestInitialize]
        public void Startup()
        {
            _electionBll = new Mock<IElectionBll>();
            _lookupBll = new Mock<ILookupBll>();
            _controller = new ElectionController(_electionBll.Object, _lookupBll.Object);
            BaseController = _controller;
        }

        [TestMethod]
        public void Index_returns_correct_view()
        {
            // Act
            var result = _controller.Index() as ViewResult;
          
            // Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void ListElectionsAjax_returns_correct_format()
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _electionBll.Setup(x => x.Get<Election>(It.IsAny<PageRequest>()))
                .Returns(new PageResponse<Election> { Items = new List<Election>(), PageSize = 20, StartIndex = 1, Total = 2 });
  
            // Act

            var result = _controller.ListElectionsAjax(request);

            // Assert

            _electionBll.Verify(x => x.Get<Election>(It.IsAny<PageRequest>()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        [TestMethod]
        public void CreateUpdateWithNullId_returns_correct_view_model()
        {
            // Arrange

            ArrangeEmptyModel();
            ArrangeEmptyElection();
            ArrangeElectionTypes();

            // Act

            var result = _controller.CreateUpdate((long?)null);
            
            // Assert
            
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateUpdateModelViewModel((PartialViewResult)result);
        }

        [TestMethod]
        public void CreateUpdateWithNullId_returns_correct_view_data()
        {
            // Arrange

            ArrangeEmptyElection();
            ArrangeElectionTypes();

            // Act

            var result = _controller.CreateUpdate((long?)null);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateUpdateViewData((PartialViewResult)result);
        }

        //[TestMethod]
        //public void CreateUpdateWithId_returns_correct_view_model()
        //{
        //    // Arrange

        //    ArrangeElection();
        //    ArrangeElectionTypes();

        //    // Act

        //    var result = _controller.CreateUpdate(_electionId);

        //    // Assert

        //    Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        //    AssertCreateUpdateViewModel((PartialViewResult)result);
        //}

        //[TestMethod]
        //public void CreateUpdateWithId_returns_correct_view_data()
        //{
        //    // Arrange

        //    ArrangeElection();
        //    ArrangeElectionTypes();

        //    // Act

        //    var result = _controller.CreateUpdate(_electionId);

        //    // Assert

        //    Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        //    AssertCreateUpdateViewData((PartialViewResult)result);
        //}

        [TestMethod]
        public void CreateUpdateWithNotValidModel_returns_correct_view_model()
        {
            // Arrange
            
            ArrangeNotValidModel();
            ArrangeElectionTypes();

            // Act

            var result = _controller.CreateUpdate(_model);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateUpdateModelViewModel((PartialViewResult)result);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
        }

        [TestMethod]
        public void CreateUpdateWithNotValidModel_returns_correct_view_data()
        {
            // Arrange

            ArrangeNotValidModel();
            ArrangeElectionTypes();

            // Act

            var result = _controller.CreateUpdate(_model);

            // Assert

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            AssertCreateUpdateViewData((PartialViewResult)result);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
        }

        //[TestMethod]
        //public void CreateUpdateWithValidModel_returns_correct_content()
        //{
        //    // Arrange

        //    ArrangeValidModel();
        //    ArrangeElectionTypes();

        //    // Act

        //    var result = _controller.CreateUpdate(_model);

        //    // Assert

        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(ContentResult));
        //    Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
            
        //    _electionBll.Verify(x => x.SaveOrUpdate(_model.Id, _model.ElectionType, 
        //            _model.ElectionDate.Value, _model.SaiseId, _model.Comments, _model.AcceptAbroadDeclaration.Value), Times.Once);
            
        //    Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
        //}

        [TestMethod]
        public void Delete_has_correct_logic()
        {
            // Arrange

            const long id = 1;

            _electionBll.Setup(x => x.Delete<Election>(id));
            
            // Act

            _controller.Delete(id);

            // Assert

            _electionBll.Verify(x => x.Delete<Election>(id), Times.Once());
        }

        [TestMethod]
        public void UnDelete_has_correct_logic()
        {
            // Arrange

            const long id = 1;

            _electionBll.Setup(x => x.UnDelete<Election>(id));

            // Act

            _controller.UnDelete(id);

            // Assert

            _electionBll.Verify(x => x.UnDelete<Election>(id), Times.Once());
        }

        [TestMethod]
        public void ExportElectionsAllData_has_correct_logic()
        {
            ExportGridDataTest<ElectionGridModel>(ExportType.AllData, "Regions");
        }

        [TestMethod]
        public void ExportElectionsCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<ElectionGridModel>(ExportType.CurrentPage, "Regions");
        }

        private static void ArrangeElectionTypes()
        {
            _electionTypes = GetElectionTypes();
            _lookupBll.Setup(x => x.GetAll<ElectionType>()).Returns(_electionTypes);
        }

        private static void ArrangeEmptyElection()
        {
            _election = new Election();
        }

        //private static void ArrangeElection()
        //{
        //    _electionId = 1;
        //    _election = GetElection();
        //    _electionBll.Setup(x => x.Get<Election>(_electionId)).Returns(_election);
        //}

        private static void ArrangeEmptyModel()
        {
            _model = new UpdateElectionModel();
        }

        private static void ArrangeNotValidModel()
        {
            _model = GetModel();
            _controller.ModelState.AddModelError("", "Error");
        }

        //private static void ArrangeValidModel()
        //{
        //    _model = GetModel();
        //    _electionBll.Setup(x => x.SaveOrUpdate(_model.Id, _model.ElectionType,
        //        _model.ElectionDate.Value, _model.SaiseId, _model.Comments, _model.AcceptAbroadDeclaration.Value));
        //}

        //private static void AssertCreateUpdateViewModel(ViewResultBase view)
        //{
        //    Assert.IsNotNull(view);
        //    Assert.AreEqual("_UpdateElectionPartial", view.ViewName);
        //    Assert.IsNotNull(view.Model);
        //    Assert.IsInstanceOfType(view.Model, typeof(UpdateElectionModel));

        //    var model = view.Model as UpdateElectionModel;

        //    Assert.IsNotNull(model);
        //    Assert.AreEqual(model.Comments, _election.Comments);
        //    Assert.AreEqual(model.ElectionDate, _election.ElectionDate);
        //    Assert.AreEqual(model.ElectionType, _election.ElectionType.Id);
        //    Assert.AreEqual(model.Id, _election.Id);
        //    Assert.IsTrue(
        //        ((model.SaiseId == 0) && (_election.SaiseId == null)) ||
        //        (model.SaiseId == _election.SaiseId) );
        //}

        private static void AssertCreateUpdateModelViewModel(ViewResultBase view)
        {
            Assert.IsNotNull(view);
            Assert.AreEqual("_UpdateElectionPartial", view.ViewName);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(UpdateElectionModel));

            var model = view.Model as UpdateElectionModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.Comments, _model.Comments);
            Assert.AreEqual(model.ElectionDate, _model.ElectionDate);
            Assert.AreEqual(model.ElectionType, _model.ElectionType);
            Assert.AreEqual(model.Id, _model.Id);
            Assert.AreEqual(model.SaiseId, _model.SaiseId);
        }

        private static void AssertCreateUpdateViewData(ViewResultBase view)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.ViewData);
            Assert.IsTrue(view.ViewData.ContainsKey("ElectionType"));
            Assert.IsInstanceOfType(view.ViewData["ElectionType"], typeof(List<SelectListItem>));

            var viewData = view.ViewData["ElectionType"] as List<SelectListItem>;

            AssertListsAreEqual(viewData, _electionTypes.Where(x => x.Deleted == null).ToList(), x=> x.Name, x => x.Id.ToString());
        }
        
        private static void AssertListsAreEqual<T>(IReadOnlyCollection<SelectListItem> list1, List<T> list2, Func<T, string> textFunc, Func<T, string> valueFunc)
        {
            Assert.AreEqual(list1.Count, list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.Text) && string.Equals(valueFunc(x), item.Value))));
        }

        private static List<ElectionType> GetElectionTypes()
        {
            return new List<ElectionType>
            {
                GetElectionType0(),
                GeElectionType1(),
                GetElectionType2()
            };
        }

        private static ElectionType GetElectionType0()
        {
            return new ElectionType
            {
                Name = "Prezidentiale",
                Description = "Alegeri prezidentiale in RM"
            };
        }

        private static ElectionType GeElectionType1()
        {
            return new ElectionType
            {
                Deleted = DateTime.Now,
                Name = "Parlamentare",
                Description = "Alegeri parlamentare in RM"
            };
        }

        private static ElectionType GetElectionType2()
        {
            return new ElectionType
            {
                Name = "Referendum",
                Description = "Referendum Constitutional in RM"
            };
        }

        //private static Election GetElection()
        //{
        //    return new Election
        //    {
        //        AcceptAbroadDeclaration = true,
        //        Comments = "Comentarii",
        //        ElectionDate = new DateTime(2014, 11, 30),
        //        ElectionType = GeElectionType1()
        //    };
        //}

        private static UpdateElectionModel GetModel()
        {
            return new UpdateElectionModel
            {
                AcceptAbroadDeclaration = true,
                Comments = "Comentarii",
                ElectionDate = new DateTime(2014, 11, 30),
                ElectionType = 1
            };
        }
    }
}

