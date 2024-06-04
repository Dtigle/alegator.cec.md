using System.Collections.Generic;
using System.Web.Mvc;
using CEC.SRV.BLL;
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

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class HistoryControllerTests : BaseControllerTests
    {
        private static Mock<IAuditerBll> _bll;
        private static HistoryController _controller;
        private static long _id;
        private static JqGridRequest _request;
  
        public HistoryControllerTests()
        {
            Mapper.Initialize(arg =>
            {
                arg.AddProfile<IdentityUserProfile>();
                arg.AddProfile<LookupProfile>();
                arg.AddProfile<SrvGridModelsProfile>();
                arg.AddProfile<HistoryProfile>();
            });
        }

        [TestInitialize]
        public void Startup()
        {
            _bll = new Mock<IAuditerBll>();
            _controller = new HistoryController(_bll.Object);
            BaseController = _controller;
            _id = 1;
        }

        [TestMethod]
        public void ManagerTypeHistory_returns_correct_view()
        {
            // Act

            var result = _controller.ManagerTypeHistory(_id) as PartialViewResult;

            // Assert

            AssertViewModel(result);
        }

        [TestMethod]
        public void ListManagerTypeHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<ManagerType>();

            // Act

            var result = _controller.ListManagerTypeHistoryAjax(_request, _id);

            // Assert

           AssertAjaxResponse<ManagerType>(result);
        }

        [TestMethod]
        public void GenderHistory_returns_correct_view()
        {
            // Act

            var result = _controller.GenderHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListGenderHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<Gender>();
            
            // Act

            var result = _controller.ListGenderHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<Gender>(result);
        }

        [TestMethod]
        public void ElectionTypeHistory_returns_correct_view()
        {
            // Act

            var result = _controller.ElectionTypeHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListElectionTypeHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<ElectionType>();

            // Act

            var result = _controller.ListElectionTypeHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<ElectionType>(result);
        }

        [TestMethod]
        public void ElectionHistory_returns_correct_view()
        {
            // Act

            var result = _controller.ElectionHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListElectionHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<Election>();

            // Act

            var result = _controller.ListElectionHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<Election>(result);
        }

        [TestMethod]
        public void StreetTypeHistory_returns_correct_view()
        {
            // Act

            var result = _controller.StreetTypeHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListStreetTypeHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<StreetType>();

            // Act

            var result = _controller.ListStreetTypeHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<StreetType>(result);
        }

        [TestMethod]
        public void PersonStatusHistory_returns_correct_view()
        {
            // Act

            var result = _controller.PersonStatusHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListPersonStatusHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<PersonStatusType>();

            // Act

            var result = _controller.ListPersonStatusHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<PersonStatusType>(result);
        }

        [TestMethod]
        public void DocumentTypeHistory_returns_correct_view()
        {
            // Act

            var result = _controller.DocumentTypeHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListDocumentTypeHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<DocumentType>();

            // Act

            var result = _controller.ListDocumentTypeHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<DocumentType>(result);
        }

        [TestMethod]
        public void RegionTypeHistory_returns_correct_view()
        {
            // Act

            var result = _controller.RegionTypeHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListRegionTypeHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<RegionType>();

            // Act

            var result = _controller.ListRegionTypeHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<RegionType>(result);
        }

        [TestMethod]
        public void RegionHistory_returns_correct_view()
        {
            // Act

            var result = _controller.RegionHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListRegionHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<Region>();

            // Act

            var result = _controller.ListRegionHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<Region>(result);
        }

        [TestMethod]
        public void StreetHistory_returns_correct_view()
        {
            // Act

            var result = _controller.StreetHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListStreetHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<Street>();

            // Act

            var result = _controller.ListStreetHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<Street>(result);
        }

        [TestMethod]
        public void PollingStationHistory_returns_correct_view()
        {
            // Act

            var result = _controller.PollingStationHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListPollingStationHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<PollingStation>();

            // Act

            var result = _controller.ListPollingStationHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<PollingStation>(result);
        }

        [TestMethod]
        public void AddressHistory_returns_correct_view()
        {
            // Act

            var result = _controller.AddressHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListAddressHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<Address>();

            // Act

            var result = _controller.ListAddressHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<Address>(result);
        }

        [TestMethod]
        public void PersonAddressTypeHistory_returns_correct_view()
        {
            // Act

            var result = _controller.PersonAddressTypeHistory(_id) as PartialViewResult;

            // Assert
            AssertViewModel(result);
        }

        [TestMethod]
        public void ListPersonAddressTypeHistoryAjax_returns_correct_format()
        {
            // Arrange

            ArrangeBllForAjaxCall<PersonAddressType>();

            // Act

            var result = _controller.ListPersonAddressTypeHistoryAjax(_request, _id);

            // Assert

            AssertAjaxResponse<PersonAddressType>(result);
        }

        private static void ArrangeBllForAjaxCall<T>() where T : AuditedEntity<IdentityUser>
        {
            _request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.Get<T>(It.IsAny<PageRequest>(), It.IsAny<long>()))
                .Returns(new PageResponse<T> { Items = new List<T>(), PageSize = 20, StartIndex = 1, Total = 2 });
        }

        private static void AssertAjaxResponse<T>(object result) where T : AuditedEntity<IdentityUser>
        {
            _bll.Verify(x => x.Get<T>(It.IsAny<PageRequest>(), _id), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        private static void AssertViewModel(ViewResultBase view)
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(long));
            Assert.AreEqual(_id, (long)view.Model);
        }
    }
}

