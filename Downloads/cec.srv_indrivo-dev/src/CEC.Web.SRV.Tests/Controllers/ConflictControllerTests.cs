using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using Moq;
using CEC.SRV.BLL;
using NHibernate.Linq;
using System.Security.Claims;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain;
using Amdaris.Domain;
using AutoMapper;
using CEC.Web.SRV.Resources;
using CEC.Web.SRV.Models.Conflict;
using CEC.Web.SRV.Infrastructure;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class ConflictControllerTests : BaseControllerTests
    {
        private readonly Mock<IConflictBll> _bll;
        private readonly Mock<IVotersBll> _voterBll;
        private readonly Mock<IAddressBll> _addressBll;
        private readonly Mock<IImportBll> _importBll;
        private readonly ConflictController _controller;

        public ConflictControllerTests()
        {
            _bll = new Mock<IConflictBll>();
            _voterBll = new Mock<IVotersBll>();
            _addressBll = new Mock<IAddressBll>();
            _importBll = new Mock<IImportBll>();

            _controller = new ConflictController(_bll.Object, _voterBll.Object, _addressBll.Object, _importBll.Object);
            BaseController = _controller;

            Mapper.Initialize(arg =>
            {
                arg.AddProfile<ConflictProfile>();
            });
        }

        [TestMethod]
        public void Index_returns_correct_view()
        {
            // Act

            var result = _controller.Index() as ViewResult;
        
            // Assert

            Assert.IsNotNull(result);
        }

        #region LoadGridList

        [TestMethod]
        public void LoadGridList_returns_correct_view()
        {
            var dictionary = new Dictionary<ConflictStatusCode, string>
            {
                {ConflictStatusCode.StatusConflict, "_StatusConflictList"},
                {ConflictStatusCode.AddressConflict, "_AddressConflictList"},
                {ConflictStatusCode.PollingStationConflict, "_AddressNoStreetConflictList"},
                {ConflictStatusCode.RegionConflict, "_RegionConflictList"},
                {ConflictStatusCode.LocalityConflict, "_LocalityConflictList"},
                {ConflictStatusCode.StreetZeroConflict, "_StreetConflictList"},
                {ConflictStatusCode.None, string.Empty}
            };

            dictionary.Keys.ForEach(x => LoadGridListTest(x, dictionary[x]));
        }

        private void LoadGridListTest(ConflictStatusCode statusCode, string viewName)
        {
            // Act

            var result = _controller.LoadGridList(statusCode) as PartialViewResult;

            // Assert

            if (statusCode == ConflictStatusCode.None)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(viewName, result.ViewName);
            }
        }

        #endregion LoadGridList

        #region StatusListConflictAjax

        [TestMethod]
        public void StatusListConflictAjax_ByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDataAdmins();

            _bll.Setup(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.StatusConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.StatusListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.StatusConflict), Times.Once);

        }

        [TestMethod]
        public void StatusListConflictAjax_ByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDatas();

            _bll.Setup(x => x.GetConflictList(It.IsAny<PageRequest>(), ConflictStatusCode.StatusConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.StatusListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictList(It.IsAny<PageRequest>(), ConflictStatusCode.StatusConflict), Times.Once);
        }

        #endregion StatusListConflictAjax

        #region ResolveStatusConflict

        [TestMethod]
        public void ResolveStatusConflict_ByFalseKeepRSVVersion_AndNotNullComments_has_correct_logic()
        {
            // Arrange

            const long idRsp = 1L;
            var rspModData = GetRspModificationData();
            var conflictRspRegistrationData = GetRspRegistrationData(rspModData);
            var oldComments = rspModData.Comments;

            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflictRspRegistrationData);
            _importBll.Setup(x => x.AcceptRspStatus(It.IsAny<RspModificationData>()));

            // Act

            var result = _controller.ResolveStatusConflict(idRsp) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);

            var comments = rspModData.Comments;

            Assert.AreEqual(string.Format("{0} {1}", oldComments, MUI.Conflict_AcceptRspStatus_comment), comments);
            _importBll.Verify(x => x.AcceptRspStatus(rspModData), Times.Once);
            _bll.Verify(x => x.Get<RspRegistrationData>(idRsp), Times.Once);
        }

        [TestMethod]
        public void ResolveStatusConflict_ByFalseKeepRSVVersion_AndNullComments_has_correct_logic()
        {
            // Arrange

            const long idRsp = 1L;
            var rspModData = GetRspModificationData();
            var conflictRspRegistrationData = GetRspRegistrationData(rspModData);
            rspModData.Comments = null;

            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflictRspRegistrationData);
            _importBll.Setup(x => x.AcceptRspStatus(It.IsAny<RspModificationData>()));

            // Act

            var result = _controller.ResolveStatusConflict(idRsp) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);

            var comments = rspModData.Comments;

            Assert.AreEqual(string.Format(MUI.Conflict_AcceptRspStatus_comment), comments);
            _importBll.Verify(x => x.AcceptRspStatus(rspModData), Times.Once);
            _bll.Verify(x => x.Get<RspRegistrationData>(idRsp), Times.Once);
        }

        [TestMethod]
        public void ResolveStatusConflict_ByTrueKeepRSVVersion_AndNotNullComments_has_correct_logic()
        {
            // Arrange

            const long idRsp = 1L;
            var rspModData = GetRspModificationData();
            var conflictRspRegistrationData = GetRspRegistrationData(rspModData);
            var oldComments = rspModData.Comments;

            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflictRspRegistrationData);
            _importBll.Setup(x => x.RejectRspStatus(It.IsAny<RspModificationData>()));
            _bll.Setup(x => x.WriteNotification(ConflictStatusCode.StatusConflict, rspModData, It.IsAny<string>()));

            // Act

            var result = _controller.ResolveStatusConflict(idRsp, true) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);

            var comments = rspModData.Comments;

            Assert.AreEqual(string.Format("{0} {1}", oldComments, MUI.Conflict_RejectRspStatus_comment), comments);
            _importBll.Verify(x => x.RejectRspStatus(rspModData), Times.Once);
            _bll.Verify(x => x.WriteNotification(ConflictStatusCode.StatusConflict, rspModData, It.IsAny<string>()), Times.Once);
            _bll.Verify(x => x.Get<RspRegistrationData>(idRsp), Times.Once);
        }

        [TestMethod]
        public void ResolveStatusConflict_ByTrueKeepRSVVersion_AndNullComments_has_correct_logic()
        {
            // Arrange

            const long idRsp = 1L;
            var rspModData = GetRspModificationData();
            var conflictRegistrationData = GetRspRegistrationData(rspModData);
            rspModData.Comments = null;

            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflictRegistrationData);
            _importBll.Setup(x => x.RejectRspStatus(It.IsAny<RspModificationData>()));
            _bll.Setup(x => x.WriteNotification(ConflictStatusCode.StatusConflict, rspModData, It.IsAny<string>()));

            // Act

            var result = _controller.ResolveStatusConflict(idRsp, true) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);

            var comments = rspModData.Comments;

            Assert.AreEqual(string.Format(MUI.Conflict_RejectRspStatus_comment), comments);
            _importBll.Verify(x => x.RejectRspStatus(rspModData), Times.Once);
            _bll.Verify(x => x.WriteNotification(ConflictStatusCode.StatusConflict, rspModData, It.IsAny<string>()), Times.Once);
            _bll.Verify(x => x.Get<RspRegistrationData>(idRsp), Times.Once);
        }

        #endregion ResolveStatusConflict

        #region AddressListConflictAjax

        [TestMethod]
        public void AddressListConflictAjax_ByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDataAdmins();

            //foreach (var conflict in conflicts)
            //{
            //    conflict.StatusConflictCode = ConflictStatusCode.AddressConflict;
            //}

            _bll.Setup(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(),
                new[] { ConflictStatusCode.AddressConflict, ConflictStatusCode.StreetConflict, ConflictStatusCode.AddressFatalConflict }))
                .Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.AddressListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(),
                new[] { ConflictStatusCode.AddressConflict, ConflictStatusCode.StreetConflict, ConflictStatusCode.AddressFatalConflict })
                , Times.Once);

        }

        [TestMethod]
        public void AddressListConflictAjax_ByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDatas();

            _bll.Setup(x => x.GetConflictList(It.IsAny<PageRequest>(), ConflictStatusCode.AddressConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.AddressListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictList(It.IsAny<PageRequest>(), ConflictStatusCode.AddressConflict), Times.Once);
        }

        #endregion AddressListConflictAjax

        #region ResolveAddressConflict

        [TestMethod]
        [ExpectedException(typeof(SrvException))]
        public void ResolveAddressConflict_ByNullRegion_throws_an_exception()
        {
            // Arrange

            const long idRsp = 1L;
            var conflict = GetRspRegistrationData(GetRspModificationData());
            
            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflict);
            _bll.Setup(x => x.GetConflictAddress(It.IsAny<long>())).Returns(GetRspRegistrationData());
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns((Region)null);

            // Act

            _controller.ResolveAddressConflict(idRsp);
        }

        [TestMethod]
        public void ResolveAddressConflict_ByNotNullRegion_AndNotNullStreetTypeCode_has_correct_logic()
        {
            // Arrange

            const long idRsp = 1L;
            var conflict = GetRspRegistrationData(GetRspModificationData());
            //var streetTypeCode = GetStreetTypeCode();
            //var conflictAddressData = GetRspRegistrationData();
            var region = GetRegion();

            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflict);
            //_bll.Setup(x => x.GetConflictAddress(It.IsAny<long>())).Returns(conflict);
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns(region);
            //_bll.Setup(x => x.Get<StreetTypeCode>(It.IsAny<long>())).Returns(streetTypeCode);

            // Act

            var result = _controller.ResolveAddressConflict(idRsp) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ResolveAddressConflict", result.ViewName);

            var model = result.Model as ResolveAddressConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.RspId);
            Assert.AreEqual(string.Format("{0} {1}{2} ", conflict.StreetName, conflict.GetHouse(), conflict.GetApartment()), model.RspAddress);
            Assert.AreEqual(region.Id, model.RegionId);

            _bll.Verify(x => x.Get<RspRegistrationData>(idRsp), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(It.IsAny<long>()), Times.Once);
            _bll.Verify(x => x.GetRegionByAdministrativeCode(It.IsAny<long>()), Times.Once);
            //_bll.Verify(x => x.Get<StreetTypeCode>(It.IsAny<long>()), Times.Once);

        }

        [TestMethod]
        public void ResolveAddressConflict_ByNotNullRegion_AndNullStreetTypeCode_has_correct_logic()
        {
            // Arrange

            const long idRsp = 1L;
            var conflict =GetRspRegistrationData(GetRspModificationData());
            var region = GetRegion();

            _bll.Setup(x => x.Get<RspRegistrationData>(idRsp)).Returns(conflict);
            //_bll.Setup(x => x.GetConflictAddress(It.IsAny<long>())).Returns(conflict);
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns(region);
            //_bll.Setup(x => x.Get<StreetTypeCode>(It.IsAny<long>())).Returns((StreetTypeCode)null);

            // Act

            var result = _controller.ResolveAddressConflict(idRsp) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ResolveAddressConflict", result.ViewName);

            var model = result.Model as ResolveAddressConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.RspId);
            //Assert.AreEqual(string.Empty, model.RspAddress);
            Assert.AreEqual(region.Id, model.RegionId);

            _bll.Verify(x => x.Get<RspRegistrationData>(idRsp), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(It.IsAny<long>()), Times.Once);
            _bll.Verify(x => x.GetRegionByAdministrativeCode(It.IsAny<long>()), Times.Once);
            //_bll.Verify(x => x.Get<StreetTypeCode>(It.IsAny<long>()), Times.Once);

        }

        #endregion ResolveAddressConflict

        [TestMethod]
        public void LoadAddressGridList_returns_correct_result()
        {
            // Arrange

            long? regionId = 1;

            // Act

            var result = _controller.LoadAddressGridList(regionId) as PartialViewResult;
            
            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_AddressRSVList", result.ViewName);

            var model = result.Model as long?;

            Assert.IsNotNull(model);
            Assert.AreEqual(regionId, model);
        }
        
        [TestMethod]
        public void ListAddressesAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            long? regionId = 1;
            var addresses = GetAddressBaseDtos();

            _bll.Setup(x => x.GetAddresses(It.IsAny<PageRequest>(), regionId)).Returns(GetPageResponse(addresses));

            // Act

            var result = _controller.ListAddressesAjax(request, regionId) as JqGridJsonResult;

            // Assert

            AssertDtoJqGridJsonResult(result, addresses);

            _bll.Verify(x => x.GetAddresses(It.IsAny<PageRequest>(), regionId), Times.Once);
        }

        #region MapAddressConflict

        [TestMethod]
        [Ignore]
        public void MapAddressConflict_ByNotNullComments_has_correct_logic()
        {
            // Arrange

            const long conflictId = 1;
            const long addressId = 1;

            var address = GetAddress();
            var conflictData = GetRspRegistrationData(GetRspModificationData());
            var oldComments = conflictData.RspModificationData.Comments;

            _bll.Setup(x => x.Get<RspRegistrationData>(conflictId)).Returns(conflictData);
            _bll.Setup(x => x.Get<Address>(addressId)).Returns(address);
            _importBll.Setup(x => x.MapAddress(conflictData.RspModificationData, address, It.IsAny<int>(), It.IsAny<string>()));

            // Act

            var result = _controller.MapAddressConflict(conflictId, addressId, true) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.AreEqual(string.Format("{0} {1}", oldComments, MUI.Conflict_MapAddress_comment), conflictData.RspModificationData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(conflictId), Times.Once);
            _bll.Verify(x => x.Get<Address>(addressId), Times.Once);
            _importBll.Verify(x => x.MapAddress(conflictData.RspModificationData, address, It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [Ignore]
        public void MapAddressConflict_ByNullComments_has_correct_logic()
        {
            // Arrange
            const long conflictId = 1;
            const long addressId = 1;

            var address = GetAddress();
            var conflictData = GetRspRegistrationData(GetRspModificationData());
            conflictData.RspModificationData.Comments = null;

            _bll.Setup(x => x.Get<RspRegistrationData>(conflictId)).Returns(conflictData);
            _bll.Setup(x => x.Get<Address>(addressId)).Returns(address);
            _importBll.Setup(x => x.MapAddress(conflictData.RspModificationData, address, It.IsAny<int>(), It.IsAny<string>()));
            _importBll.Setup(x => x.ResolveByMappingAddress(conflictId, addressId, true));

            // Act
            var result = _controller.MapAddressConflict(conflictId, addressId, true) as ContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.AreEqual(string.Format(MUI.Conflict_MapAddress_comment), conflictData.RspModificationData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(conflictId), Times.Once);
            _bll.Verify(x => x.Get<Address>(addressId), Times.Once);
            _importBll.Verify(x => x.MapAddress(conflictData.RspModificationData, address, It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion MapAddressConflict

        #region CreateNewAddress

        [TestMethod]
        public void CreateNewAddress_ByNotNullRsvStreet_has_correct_model()
        {
            // Arrange

            const long rspId = 1;
            const long regionId = 1;
            const long ropId = 11;

            var rspData = GetRspModificationData();
            var conflictAddressData = GetRspRegistrationData();
            var rspStreet = GetStreetTypeCode();
            var rsvStreet = GetStreet(ropId);
            var pollingStations = GetPollingStations();

            _bll.Setup(x => x.Get<RspRegistrationData>(rspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode)).Returns(rspStreet);
            _bll.Setup(x => x.GetStreetByRopId(rspStreet.Id)).Returns(rsvStreet);
            _addressBll.Setup(x => x.GetPollingStations(regionId)).Returns(pollingStations);

            //  Act

            var result = _controller.CreateNewAddress(rspId, regionId) as PartialViewResult;
            
            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateNewAddressPartial", result.ViewName);

            var model = result.Model as CreateAddressModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(rsvStreet.GetFullName(), model.Street);
            Assert.AreEqual(rsvStreet.Id, model.StreetId);
            
            Assert.AreEqual(rspId, model.RspId);
            Assert.AreEqual(regionId, model.RegionId);
            Assert.AreEqual(conflictAddressData.HouseNumber, model.HouseNumber);
            Assert.AreEqual(conflictAddressData.GetHouseSuffix(), model.Suffix);

            _bll.Verify(x => x.Get<RspRegistrationData>(rspId), Times.Once);
            _bll.Verify(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode), Times.Once);
            _bll.Verify(x => x.GetStreetByRopId(rspStreet.Id), Times.Once);
            _addressBll.Verify(x => x.GetPollingStations(regionId), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByNotNullRsvStreet_has_correct_view_data()
        {
            // Arrange

            const long rspId = 1;
            const long regionId = 1;
            const int ropId = 11;

            var rspData = GetRspModificationData();
            var conflictAddressData = GetRspRegistrationData();
            var rspStreet = GetStreetTypeCode();
            var rsvStreet = GetStreet(ropId);
            var pollingStations = GetPollingStations();

            _bll.Setup(x => x.Get<RspRegistrationData>(rspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode)).Returns(rspStreet);
            _bll.Setup(x => x.GetStreetByRopId(rspStreet.Id)).Returns(rsvStreet);
            _addressBll.Setup(x => x.GetPollingStations(regionId)).Returns(pollingStations);

            //  Act

            var result = _controller.CreateNewAddress(rspId, regionId) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateNewAddressPartial", result.ViewName);

            var viewData = result.ViewData["PollingStationId"] as IEnumerable<SelectListItem>;

            Assert.IsNotNull(viewData);
            AssertListsAreEqual(viewData, pollingStations, x => x.FullNumber, x => x.Id.ToString());
            
            _bll.Verify(x => x.Get<RspRegistrationData>(rspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode), Times.Once);
            _bll.Verify(x => x.GetStreetByRopId(rspStreet.Id), Times.Once);
            _addressBll.Verify(x => x.GetPollingStations(regionId), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByNullRsvStreet_has_correct_model()
        {
            // Arrange

            const long rspId = 1;
            const long regionId = 1;

            var rspData = GetRspModificationData();
            var conflictAddressData = GetRspRegistrationData();
            var rspStreet = GetStreetTypeCode();
            var pollingStations = GetPollingStations();

            _bll.Setup(x => x.Get<RspRegistrationData>(rspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode)).Returns(rspStreet);
            _bll.Setup(x => x.GetStreetByRopId(rspStreet.Id)).Returns((Street)null);
            _addressBll.Setup(x => x.GetPollingStations(regionId)).Returns(pollingStations);

            //  Act

            var result = _controller.CreateNewAddress(rspId, regionId) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateNewAddressPartial", result.ViewName);

            var model = result.Model as CreateAddressModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(rspStreet.Name, model.Street);

            Assert.AreEqual(rspId, model.RspId);
            Assert.AreEqual(regionId, model.RegionId);
            Assert.AreEqual(conflictAddressData.HouseNumber, model.HouseNumber);
            Assert.AreEqual(conflictAddressData.GetHouseSuffix(), model.Suffix);

            _bll.Verify(x => x.Get<RspRegistrationData>(rspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode), Times.Once);
            _bll.Verify(x => x.GetStreetByRopId(rspStreet.Id), Times.Once);
            _addressBll.Verify(x => x.GetPollingStations(regionId), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByNullRsvStreet_has_correct_view_data()
        {
            // Arrange

            const long rspId = 1;
            const long regionId = 1;

            var rspData = GetRspModificationData();
            var conflictAddressData = GetRspRegistrationData();
            var rspStreet = GetStreetTypeCode();
            var pollingStations = GetPollingStations();

            _bll.Setup(x => x.Get<RspRegistrationData>(rspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode)).Returns(rspStreet);
            _bll.Setup(x => x.GetStreetByRopId(rspStreet.Id)).Returns((Street)null);
            _addressBll.Setup(x => x.GetPollingStations(regionId)).Returns(pollingStations);

            //  Act

            var result = _controller.CreateNewAddress(rspId, regionId) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateNewAddressPartial", result.ViewName);

            var viewData = result.ViewData["PollingStationId"] as IEnumerable<SelectListItem>;

            Assert.IsNotNull(viewData);
            AssertListsAreEqual(viewData, pollingStations, x => x.FullNumber, x => x.Id.ToString());

            _bll.Verify(x => x.Get<RspRegistrationData>(rspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode), Times.Once);
            _bll.Verify(x => x.GetStreetByRopId(rspStreet.Id), Times.Once);
            _addressBll.Verify(x => x.GetPollingStations(regionId), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByNonValidModel_has_correct_model()
        {
            // Arrange

            var expModel = GetCreateAddressModel();
            var pollingStations = GetPollingStations();

            _addressBll.Setup(x => x.GetPollingStations(expModel.RegionId)).Returns(pollingStations);
            _controller.ModelState.AddModelError("", "error");

            // Act

            var result = _controller.CreateNewAddress(expModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateNewAddressPartial", result.ViewName);

            var model = result.Model as CreateAddressModel;

            Assert.IsNotNull(model);
            Assert.AreSame(expModel, model);

            _addressBll.Verify(x => x.GetPollingStations(expModel.RegionId), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByNonValidModel_has_correct_view_data()
        {
            // Arrange

            var expModel = GetCreateAddressModel();
            var pollingStations = GetPollingStations();

            _addressBll.Setup(x => x.GetPollingStations(expModel.RegionId)).Returns(pollingStations);
            _controller.ModelState.AddModelError("", "error");

            // Act

            var result = _controller.CreateNewAddress(expModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateNewAddressPartial", result.ViewName);

            var viewData = result.ViewData["PollingStationId"] as IEnumerable<SelectListItem>;

            Assert.IsNotNull(viewData);
            AssertListsAreEqual(viewData, pollingStations, x => x.FullNumber, x => x.Id.ToString());

            _addressBll.Verify(x => x.GetPollingStations(expModel.RegionId), Times.Once);
        }

        [TestMethod]
        [Ignore]
        public void CreateNewAddress_ByValidModel_AndNullStreetId_AndNullComments_has_correct_logic()
        {
            // Arrange

            var model = GetCreateAddressModel();
            model.StreetId = null;

            var rspData = GetRspModificationData();
            rspData.Comments = null;

            var conflictAddressData = GetRspRegistrationData();
            conflictAddressData.RspModificationData = rspData;
            var rspStreet = GetStreetTypeCode();
            const long streetId = 1;
            var address = GetAddress();

            _bll.Setup(x => x.Get<RspRegistrationData>(model.RspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode)).Returns(rspStreet);
            _bll.Setup(x => x.CreateStreet(rspStreet.Name, null, model.RegionId, 3, rspStreet.Id, null)).Returns(streetId);
            _bll.Setup(x => x.SaveAddress(streetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId)).Returns(address);
            _importBll.Setup(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()));

            // Act

            var result = _controller.CreateNewAddress(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);
            
            Assert.IsNotNull(model);

            // why streetId should be equal to 1 ?! , :( 
            //Assert.AreEqual(streetId, model.StreetId);

            Assert.AreEqual(string.Format(MUI.Conflict_AcceptRspAddress_comment), rspData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(model.RspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode), Times.Once);
            _bll.Verify(x => x.CreateStreet(rspStreet.Name, null, model.RegionId, 3, rspStreet.Id, null), Times.Once);
            _bll.Verify(x => x.SaveAddress((long)model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId), Times.Once);
            _importBll.Verify(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [Ignore]
        public void CreateNewAddress_ByValidModel_AndNullStreetId_AndNotNullComments_has_correct_logic()
        {
            // Arrange
            var model = GetCreateAddressModel();
            model.StreetId = null;

            var rspData = GetRspModificationData();
            var oldComments = rspData.Comments;

            var conflictAddressData = GetRspRegistrationData();
            conflictAddressData.RspModificationData = rspData;
            var rspStreet = GetStreetTypeCode();
            const long streetId = 1;
            var address = GetAddress();

            _bll.Setup(x => x.Get<RspRegistrationData>(model.RspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode)).Returns(rspStreet);
            _bll.Setup(x => x.CreateStreet(rspStreet.Name, null, model.RegionId, 3, rspStreet.Id, null)).Returns(streetId);
            _bll.Setup(x => x.SaveAddress(streetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId)).Returns(address);
            _importBll.Setup(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()));

            // Act

            var result = _controller.CreateNewAddress(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.IsNotNull(model);

            // why streetId should be equal to 1 ?! , :( 
            //Assert.AreEqual(streetId, model.StreetId);

            Assert.AreEqual(string.Format("{0} {1}", oldComments, MUI.Conflict_AcceptRspAddress_comment), rspData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(model.RspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.Get<StreetTypeCode>(conflictAddressData.StreetCode), Times.Once);
            _bll.Verify(x => x.CreateStreet(rspStreet.Name, null, model.RegionId, 3, rspStreet.Id, null), Times.Once);
            _bll.Verify(x => x.SaveAddress((long)model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId), Times.Once);
            _importBll.Verify(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByValidModel_AndNotNullStreetId_AndNullComments_has_correct_logic()
        {
            // Arrange

            var model = GetCreateAddressModel();

            var rspData = GetRspModificationData();
            rspData.Comments = null;

            var conflictAddressData = GetRspRegistrationData();
            conflictAddressData.RspModificationData = rspData;
            var address = GetAddress();

            _bll.Setup(x => x.Get<RspRegistrationData>(model.RspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.SaveAddress((long)model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId)).Returns(address);
            _importBll.Setup(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()));

            // Act

            var result = _controller.CreateNewAddress(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.IsNotNull(model);
            Assert.AreEqual(string.Format(MUI.Conflict_AcceptRspAddress_comment), rspData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(model.RspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.SaveAddress((long)model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId), Times.Once);
            _importBll.Verify(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CreateNewAddress_ByValidModel_AndNotNullStreetId_AndNotNullComments_has_correct_logic()
        {
            // Arrange

            var model = GetCreateAddressModel();

            var rspData = GetRspModificationData();
            var oldComments = rspData.Comments;
            
            var conflictAddressData = GetRspRegistrationData();
            conflictAddressData.RspModificationData = rspData;
            var address = GetAddress();

            _bll.Setup(x => x.Get<RspRegistrationData>(model.RspId)).Returns(conflictAddressData);
            _bll.Setup(x => x.GetConflictAddress(rspData.Id)).Returns(conflictAddressData);
            _bll.Setup(x => x.SaveAddress((long)model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId)).Returns(address);
            _importBll.Setup(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()));

            // Act

            var result = _controller.CreateNewAddress(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.IsNotNull(model);
            Assert.AreEqual(string.Format("{0} {1}", oldComments, MUI.Conflict_AcceptRspAddress_comment), rspData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(model.RspId), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(rspData.Id), Times.Once);
            _bll.Verify(x => x.SaveAddress((long)model.StreetId, model.HouseNumber, model.Suffix, BuildingTypes.Undefined, model.PollingStationId), Times.Once);
            _importBll.Verify(x => x.MapAddress(rspData, address, It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion CreateNewAddress

        [TestMethod]
        public void RejectAddressConflict_has_correct_model()
        {
            // Arrange

            var conflictIds = new List<long> {1, 2}; 
            var conflictStatus = ConflictStatusCode.LocalityConflict;

            // Act

            var result = _controller.RejectAddressConflict(conflictIds, conflictStatus) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as RejectConflict;

            Assert.IsNotNull(model);
            Assert.AreSame(conflictIds, model.ConflictIds);
            Assert.AreEqual(conflictStatus, model.ConflictStatus);
        }

        #region RejectAddressConflictSave

        [TestMethod]
        public void RejectAddressConflictSave_ByNonValidModel_has_correct_model()
        {
            // Arrange

            var expModel = GetRejectConflict();

            _controller.ModelState.AddModelError("", "error");

            // Act

            var result = _controller.RejectAddressConflictSave(expModel) as PartialViewResult;
            
            // Assert

            Assert.IsNotNull(result);

            var model = result.Model;

            Assert.IsNotNull(model);
            Assert.AreSame(expModel, model);
        }

        [TestMethod]
        public void RejectAddressConflictSave_ByValidModel_AndAddressConflict_AndNotNullComments_has_correct_model()
        {
            // Arrange

            var model = GetRejectConflict();
            
            var conflictData = GetRspRegistrationData(GetRspModificationData());
            var oldComments = conflictData.RspModificationData.Comments;
            
            const string userName = "userName";

            _bll.Setup(x => x.Get<RspRegistrationData>(It.IsAny<long>())).Returns(conflictData);
            _bll.Setup(x => x.GetUserName()).Returns(userName);
            _importBll.Setup(x => x.RejectRspAddress(conflictData.RspModificationData));
            _bll.Setup(x => x.WriteNotification(ConflictStatusCode.AddressConflict, conflictData.RspModificationData, It.IsAny<string>()));

            // Act

            var result = _controller.RejectAddressConflictSave(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);
            Assert.AreEqual(string.Format("{0} {1}", oldComments, string.Format(MUI.Conflict_RejectRspAddress_comment, userName, model.Comment)), conflictData.RspModificationData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(It.IsAny<long>()), Times.Once);
            _bll.Verify(x => x.GetUserName(), Times.Once);
            _importBll.Verify(x => x.RejectRspAddress(conflictData.RspModificationData), Times.Once);
            _bll.Verify(x => x.WriteNotification(ConflictStatusCode.AddressConflict, conflictData.RspModificationData, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void RejectAddressConflictSave_ByValidModel_AndAddressConflict_AndNullComments_has_correct_model()
        {
            // Arrange

            var model = GetRejectConflict();

            var conflictData = GetRspRegistrationData(GetRspModificationData());
            conflictData.RspModificationData.Comments = null;

            const string userName = "userName";

            _bll.Setup(x => x.Get<RspRegistrationData>(It.IsAny<long>())).Returns(conflictData);
            _bll.Setup(x => x.GetUserName()).Returns(userName);
            _importBll.Setup(x => x.RejectRspAddress(conflictData.RspModificationData));
            _bll.Setup(x => x.WriteNotification(ConflictStatusCode.AddressConflict, conflictData.RspModificationData, It.IsAny<string>()));

            // Act

            var result = _controller.RejectAddressConflictSave(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);
            Assert.AreEqual(string.Format(MUI.Conflict_RejectRspAddress_comment, userName, model.Comment), conflictData.RspModificationData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(It.IsAny<long>()), Times.Once);
            _bll.Verify(x => x.GetUserName(), Times.Once);
            _importBll.Verify(x => x.RejectRspAddress(conflictData.RspModificationData), Times.Once);
            _bll.Verify(x => x.WriteNotification(ConflictStatusCode.AddressConflict, conflictData.RspModificationData, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void RejectAddressConflictSave_ByValidModel_AndNotAddressConflict_has_correct_model()
        {
            // Arrange

            var model = GetRejectConflict();
            model.ConflictStatus = ConflictStatusCode.None;

            var conflictData1 = GetRspRegistrationData(GetRspModificationData());
            var conflictData2 = GetRspRegistrationData(GetRspModificationData());
            conflictData1.RspModificationData.Comments = null;
            var oldComments2 = conflictData2.RspModificationData.Comments;
            
            const string userName = "userName";

            _bll.Setup(x => x.Get<RspRegistrationData>(model.ConflictIds[0])).Returns(conflictData1);
            _bll.Setup(x => x.Get<RspRegistrationData>(model.ConflictIds[1])).Returns(conflictData2);
            _bll.Setup(x => x.GetUserName()).Returns(userName);
            _importBll.Setup(x => x.RejectRspPollingStation(It.IsAny<RspModificationData>()));
            _bll.Setup(x => x.WriteNotification(ConflictStatusCode.PollingStationConflict, It.IsAny<RspModificationData>(), It.IsAny<string>()));

            // Act

            var result = _controller.RejectAddressConflictSave(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.AreEqual(string.Format(MUI.Conflict_RejectRspAddress_comment, userName, model.Comment), conflictData1.RspModificationData.Comments);
            Assert.AreEqual(string.Format("{0} {1}", oldComments2, string.Format(MUI.Conflict_RejectRspAddress_comment, userName, model.Comment)), conflictData2.RspModificationData.Comments);

            _bll.Verify(x => x.Get<RspRegistrationData>(It.IsAny<long>()), Times.Exactly(2));
            _bll.Verify(x => x.GetUserName(), Times.Exactly(2));
            _importBll.Verify(x => x.RejectRspPollingStation(It.IsAny<RspModificationData>()), Times.Exactly(2));
            _bll.Verify(x => x.WriteNotification(ConflictStatusCode.PollingStationConflict, It.IsAny<RspModificationData>(), It.IsAny<string>()), Times.Exactly(2));
        }

        #endregion RejectAddressConflictSave

        #region AddressNoStreetListConflictAjax

        [TestMethod]
        public void AddressNoStreetListConflictAjax_ByAdmin_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDataAdmins();

            _bll.Setup(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.PollingStationConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.AddressNoStreetListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.PollingStationConflict), Times.Once);

        }

        [TestMethod]
        public void AddressNoStreetListConflictAjax_ByRegistrator_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDatas();

            _bll.Setup(x => x.GetConflictList(It.IsAny<PageRequest>(), ConflictStatusCode.PollingStationConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.AddressNoStreetListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictList(It.IsAny<PageRequest>(), ConflictStatusCode.PollingStationConflict), Times.Once);
        }

        #endregion AddressNoStreetListConflictAjax

        #region ResolveAddressNoStreetConflict

        [TestMethod]
        [ExpectedException(typeof(SrvException))]
        public void ResolveAddressNoStreetConflict_ByNullRegion_throws_an_exception()
        {
            // Arrange

            var rspIds = new List<long> {1};
            
            var conflict = GetRspRegistrationData(GetRspModificationData());

            _bll.Setup(x => x.Get<RspRegistrationData>(rspIds[0])).Returns(conflict);
            _bll.Setup(x => x.GetConflictAddress(It.IsAny<long>())).Returns(GetRspRegistrationData());
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns((Region)null);

            // Act

            _controller.ResolveAddressNoStreetConflict(rspIds);
        }

        [TestMethod]
        public void ResolveAddressNoStreetConflict_ByNotNullRegion_has_correct_model()
        {
            // Arrange
            var rspIds = new List<long> { 1 };
            var conflict = GetRspRegistrationData(GetRspModificationData());
            var region = GetRegion();
            var pollingStations = GetPollingStations();
            
            _bll.Setup(x => x.Get<RspRegistrationData>(rspIds[0])).Returns(conflict);
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns(region);
            _addressBll.Setup(x => x.GetPollingStations(region.Id)).Returns(pollingStations);

            // Act
            var result = _controller.ResolveAddressNoStreetConflict(rspIds) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_ResolveAddressNoStreetConflict", result.ViewName);

            var model = result.Model as ResolveAddressNoStreetConflictModel;

            Assert.IsNotNull(model);
            Assert.AreSame(rspIds, model.RspIds);
            Assert.AreEqual(region.Id, model.RegionId);

            _bll.Verify(x => x.Get<RspRegistrationData>(rspIds[0]), Times.Once);
            _bll.Verify(x => x.GetRegionByAdministrativeCode(It.IsAny<long>()), Times.Once);
            _addressBll.Verify(x => x.GetPollingStations(region.Id), Times.Once);
        }

        [TestMethod]
        public void ResolveAddressNoStreetConflict_ByNotNullRegion_has_correct_view_data()
        {
            // Arrange
            var rspIds = new List<long> { 1 };
            var conflict = GetRspRegistrationData(GetRspModificationData());
            var region = GetRegion();
            var pollingStations = GetPollingStations();

            _bll.Setup(x => x.Get<RspRegistrationData>(rspIds[0])).Returns(conflict);
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns(region);
            _addressBll.Setup(x => x.GetPollingStations(region.Id)).Returns(pollingStations);

            // Act

            var result = _controller.ResolveAddressNoStreetConflict(rspIds) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ResolveAddressNoStreetConflict", result.ViewName);

            var viewData = result.ViewData["PollingStationId"] as IEnumerable<SelectListItem>;

            Assert.IsNotNull(viewData);
            AssertListsAreEqual(viewData, pollingStations, x => x.FullNumber, x => x.Id.ToString());

            _bll.Verify(x => x.Get<RspRegistrationData>(rspIds[0]), Times.Once);
            _bll.Verify(x => x.GetRegionByAdministrativeCode(It.IsAny<long>()), Times.Once);
            _addressBll.Verify(x => x.GetPollingStations(region.Id), Times.Once);
        }
        
        #endregion ResolveAddressNoStreetConflict

        #region MapAddressNoStreetConflict

        [TestMethod]
        public void MapAddressNoStreetConflict_ByNonValidModel_has_correct_model()
        {
            // Arrange

            var expModel = GetResolveAddressNoStreetConflictModel();
            _controller.ModelState.AddModelError("", "error");

            var pollingStations = GetPollingStations();
            _addressBll.Setup(x => x.GetPollingStations(expModel.RegionId)).Returns(pollingStations);

            // Act

            var result = _controller.MapAddressNoStreetConflict(expModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as ResolveAddressNoStreetConflictModel;

            Assert.IsNotNull(model);
            Assert.AreSame(expModel, model);
            _addressBll.Verify(x => x.GetPollingStations(expModel.RegionId), Times.Once);
        }

        [TestMethod]
        public void MapAddressNoStreetConflict_ByNonValidModel_has_correct_view_data()
        {
            // Arrange

            var expModel = GetResolveAddressNoStreetConflictModel();
            _controller.ModelState.AddModelError("", "error");

            var pollingStations = GetPollingStations();
            _addressBll.Setup(x => x.GetPollingStations(expModel.RegionId)).Returns(pollingStations);

            // Act

            var result = _controller.MapAddressNoStreetConflict(expModel) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var viewData = result.ViewData["PollingStationId"] as IEnumerable<SelectListItem>;

            Assert.IsNotNull(viewData);
            AssertListsAreEqual(viewData, pollingStations, x => x.FullNumber, x => x.Id.ToString());

            _addressBll.Verify(x => x.GetPollingStations(expModel.RegionId), Times.Once);
        }
        
        [TestMethod]
        public void MapAddressNoStreetConflict_ByValidModel_has_correct_logic()
        {
            // Arrange

            var model = GetResolveAddressNoStreetConflictModel();
            var personIds = new List<long> {1, 2};
            var conflict1 = GetRspRegistrationData(GetRspModificationData());
            var conflict2 = GetRspRegistrationData(GetRspModificationData());
            conflict1.RspModificationData.Comments= null;
            var oldComments2 = conflict2.RspModificationData.Comments;
            
            _bll.Setup(x => x.GetPersonIdbyRspIds(model.RspIds)).Returns(personIds);
            _voterBll.Setup(x => x.ChangePollingStation(model.PollingStationId, personIds));
            _bll.Setup(x => x.Get<RspRegistrationData>(model.RspIds[0])).Returns(conflict1);
            _bll.Setup(x => x.Get<RspRegistrationData>(model.RspIds[1])).Returns(conflict2);
            _importBll.Setup(x => x.AssignRspPollingStation(It.IsAny<RspModificationData>()));

            // Act

            var result = _controller.MapAddressNoStreetConflict(model) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            Assert.AreEqual(string.Format(MUI.Conflict_MapAddress_comment), conflict1.RspModificationData.Comments);
            Assert.AreEqual(string.Format("{0} {1}", oldComments2, MUI.Conflict_MapAddress_comment), conflict2.RspModificationData.Comments);

            _bll.Verify(x => x.GetPersonIdbyRspIds(model.RspIds), Times.Once);
            _voterBll.Verify(x => x.ChangePollingStation(model.PollingStationId, personIds), Times.Once);
            _bll.Verify(x => x.Get<RspRegistrationData>(It.IsAny<long>()), Times.Exactly(2));
            _importBll.Verify(x => x.AssignRspPollingStation(It.IsAny<RspModificationData>()), Times.Exactly(2));
        }

        #endregion MapAddressNoStreetConflict

        [TestMethod]
        public void RegionListConflictAjax_returns_correct_result()
        {
            // Arrange
            
            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDataAdmins();

            _bll.Setup(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.RegionConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.RegionListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.RegionConflict), Times.Once);
        }

        [TestMethod]
        public void ResolveRegionConflict_returns_correct_result()
        {
            // Arrange

            const long conflictId = 1;
            var message = string.Format(MUI.Conflict_RegionConflict_RetryMessage);
            
            _bll.Setup(x => x.UpdateStatusToRetry(conflictId, message, ConflictStatusCode.RegionConflict));

            // Act

            var result = _controller.ResolveRegionConflict(conflictId) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);
            _bll.Verify(x => x.UpdateStatusToRetry(conflictId, message, ConflictStatusCode.RegionConflict), Times.Once);
        }
        
        [TestMethod]
        public void StreetListConflictAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDataAdmins();

            _bll.Setup(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.StreetZeroConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.StreetListConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictListForAdmin(It.IsAny<PageRequest>(), ConflictStatusCode.StreetZeroConflict), Times.Once);
        }

        [TestMethod]
        public void ResolveStreetConflict_returns_correct_result()
        {
            // Arrange

            const long conflictId = 1;
            const string message = "reprocesare";

            _bll.Setup(x => x.UpdateStatusToRetry(conflictId, message, ConflictStatusCode.StreetZeroConflict));

            // Act

            var result = _controller.ResolveStreetConflict(conflictId) as RedirectToRouteResult;

            // Assert

            Assert.IsNotNull(result);
            _bll.Verify(x => x.UpdateStatusToRetry(conflictId, message, ConflictStatusCode.StreetZeroConflict), Times.Once);
        }

        [TestMethod]
        public void LocalityConflictAjax_returns_correct_result()
        {
            // Arrange

            var request = GetJqGridRequest();
            var conflicts = GetRspConflictModificationDatas();

            _bll.Setup(x => x.GetConflictListForLinkedRegions(It.IsAny<PageRequest>(), ConflictStatusCode.LocalityConflict)).Returns(GetPageResponse(conflicts));

            // Act

            var result = _controller.LocalityConflictAjax(request) as JqGridJsonResult;

            // Assert

            AssertJqGridJsonResult(result, conflicts);

            _bll.Verify(x => x.GetConflictListForLinkedRegions(It.IsAny<PageRequest>(), ConflictStatusCode.LocalityConflict), Times.Once);
        }

        #region ResolveLocalityConflict

        [TestMethod]
        [ExpectedException(typeof(SrvException))]
        public void ResolveLocalityConflict_ByNullRegion_throws_an_exception()
        {
            // Arrange

            const long rspId = 1;

            var conflict = GetRspRegistrationData(GetRspModificationData());

            _bll.Setup(x => x.Get<RspRegistrationData>(rspId)).Returns(conflict);
            _bll.Setup(x => x.GetConflictAddress(It.IsAny<long>())).Returns(GetRspRegistrationData());
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns((Region)null);

            // Act

            _controller.ResolveLocalityConflict(rspId);
        }

        [TestMethod]
        public void ResolveLocalityConflict_ByNotNullRegion_has_correct_model()
        {
            // Arrange
            const long rspId = 1;
            var region = GetRegion();
            var conflict = GetRspRegistrationData(GetRspModificationData());

            _bll.Setup(x => x.Get<RspRegistrationData>(rspId)).Returns(conflict);
            _bll.Setup(x => x.GetRegionByAdministrativeCode(It.IsAny<long>())).Returns(region);

            // Act
            var result = _controller.ResolveLocalityConflict(rspId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_ResolveLocalityConflict", result.ViewName);

            var model = result.Model as ResolveLocalityConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(rspId, model.RspId);
            
            _bll.Verify(x => x.Get<RspRegistrationData>(rspId), Times.Once);
            _bll.Verify(x => x.GetRegionByAdministrativeCode(It.IsAny<long>()), Times.Once);
        }

        #endregion ResolveLocalityConflict

        [TestMethod]
        public void ChangeAddress_has_correct_logic()
        {
            // Arrange

            const long conflictId = 1;
            const long addressId = 1;
            const long personId = 1;
            
            _bll.Setup(x => x.GetPersonIdByConflictId(conflictId)).Returns(personId);
            _bll.Setup(x => x.ChangeAddress(addressId, personId));
            _importBll.Setup(x => x.AcceptRsvLocality(conflictId));
            
            // Act

            var result = _controller.ChangeAddress(conflictId, addressId) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            _bll.Verify(x => x.GetPersonIdByConflictId(conflictId), Times.Once);
            _bll.Verify(x => x.ChangeAddress(addressId, personId), Times.Once);
            _importBll.Verify(x => x.AcceptRsvLocality(conflictId), Times.Once);
        }

        [TestMethod]
        public void ChangePollingStation_has_correct_logic()
        {
            // Arrange

            const long conflictId = 1;
            const long pollingStationId = 1;
            const long personId = 1;

            _bll.Setup(x => x.GetPersonIdByConflictId(conflictId)).Returns(personId);
            _voterBll.Setup(x => x.UpdatePollingStation(personId, pollingStationId));
            _importBll.Setup(x => x.AcceptRsvLocality(conflictId));

            // Act

            var result = _controller.ChangePollingStation(conflictId, pollingStationId) as ContentResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(Const.CloseWindowContent, result.Content);

            _bll.Verify(x => x.GetPersonIdByConflictId(conflictId), Times.Once);
            _voterBll.Verify(x => x.UpdatePollingStation(personId, pollingStationId), Times.Once);
            _importBll.Verify(x => x.AcceptRsvLocality(conflictId), Times.Once);
        }

        [TestMethod]
        public void GetUserRegions_returns_correct_result()
        {
            // Arrange

            var request = GetSelect2Request();
            var expData = GetPageResponse(GetRegions());

            _bll.Setup(x => x.GetUserRegions(It.IsAny<PageRequest>())).Returns(expData);

            // Act

            var result = _controller.GetUserRegions(request);
            
            // Assert

            Assert.IsNotNull(result);

            var data = result.Data as Select2PagedResponse;

            AssertListsAreEqual(data.Items, expData.Items.ToList(), x => x.GetFullName(), x=> x.Id);

            _bll.Verify(x => x.GetUserRegions(It.IsAny<PageRequest>()), Times.Once);
        }

        [TestMethod]
        public void CheckRegionForStreets_returns_correct_result()
        {
            // Arrange

            const long regionId = 1;
            var region = GetRegion();
            _bll.Setup(x => x.Get<Region>(regionId)).Returns(region);

            // Act

            var result = _controller.CheckRegionForStreets(regionId);

            // Assert

            Assert.IsNotNull(result);

            var data = (bool)result.Data;
            
            Assert.IsNotNull(data);
            Assert.AreEqual(region.HasStreets, data);
        }

        [TestMethod]
        public void LoadPollingStationsList_returns_correct_view()
        {
            // Act

            var result = _controller.LoadPollingStationsList() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_PollingStationList", result.ViewName);
        }

        [TestMethod]
        public void SelectSource_has_correct_model()
        {
            // Arrange

            var expModel = Enum.GetValues(typeof (SourceEnum))
                .Cast<SourceEnum>().ToList();

            // Act

            var result = _controller.SelectSource() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as List<SelectListItem>;

            Assert.IsNotNull(model);
            Assert.AreEqual("_Select", result.ViewName);

            AssertListsAreEqual(model, expModel, x => x.GetEnumDescription(), x => x.GetFilterValue().ToString());
        }

        [TestMethod]
        public void SelectStatus_has_correct_model()
        {
            // Arrange

            var expModel = Enum.GetValues(typeof(RawDataStatus))
                .Cast<RawDataStatus>().ToList();

            // Act

            var result = _controller.SelectStatus() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);

            var model = result.Model as List<SelectListItem>;

            Assert.IsNotNull(model);
            Assert.AreEqual("_Select", result.ViewName);

            AssertListsAreEqual(model, expModel, x => x.GetEnumDescription(), x => x.GetFilterValue().ToString());
        }

        #region GetViewConflict

        [TestMethod]
        public void GetViewConflict_ByNullConflict_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.None;

            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns((RspRegistrationData)null);

            // Act

            var result = _controller.GetViewConflict(id, conflictType);

            // Assert

            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(SrvException))]
        public void GetViewConflict_ByStatusConflict_WithoutVoter_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.StatusConflict;

            var conflict = GetRspRegistrationData(GetRspModificationData());
            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);

            _bll.Setup(x => x.GetVoter(conflict.RspModificationData.Idnp)).Returns((VoterConflictDataDto)null);
            
            // Act

            var result = _controller.GetViewConflict(id, conflictType);
        }

        [TestMethod]
        public void GetViewConflict_ByStatusConflict_WithVoter_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.StatusConflict;

            var conflict = GetRspRegistrationData(GetRspModificationData());
            var voter = GetVoterConflictDataDto();
            
            var rspStreet = GetStreetTypeCode();

            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);
            _bll.Setup(x => x.GetVoter(conflict.RspModificationData.Idnp)).Returns(voter);
            //_bll.Setup(x => x.GetConflictAddress(conflict.Id)).Returns(conflict);
            //_bll.Setup(x => x.Get<StreetTypeCode>(Convert.ToInt64(conflict.GetStreetCode()))).Returns(rspStreet);

            // Act

            var result = _controller.GetViewConflict(id, conflictType) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ViewStatusConflict", result.ViewName);

            var model = result.Model as ViewStatusConflictModel;

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PeopleData);
            Assert.AreEqual(string.Format("{0} {1}{2} ", conflict.StreetName, conflict.GetHouse(), conflict.GetApartment()), model.PeopleData.Address);
            Assert.AreEqual(conflict.Region, model.PeopleData.Region);
            Assert.AreEqual(conflict.Locality, model.PeopleData.Locality);
            Assert.AreEqual(conflict.Administrativecode, model.PeopleData.AdministrativeCode);

            AssertAreEqual(voter, model.VoterData);

            _bll.Verify(x => x.Get<RspRegistrationData>(id), Times.Once);
            _bll.Verify(x => x.GetVoter(conflict.RspModificationData.Idnp), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(conflict.Id), Times.Once);
            //_bll.Verify(x => x.Get<StreetTypeCode>(Convert.ToInt64(conflict.GetStreetCode())), Times.Once);
        }

        [TestMethod]
        public void GetViewConflict_ByAddressConflict_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.AddressConflict;

            var conflict = GetRspRegistrationData( GetRspModificationData());
            //var conflictAddressData = GetRspRegistrationData();
            var rspStreet = GetStreetTypeCode();

            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);
            //_bll.Setup(x => x.GetConflictAddress(conflict.Id)).Returns(conflict);
            //_bll.Setup(x => x.Get<StreetTypeCode>(Convert.ToInt64(conflict.GetStreetCode()))).Returns(rspStreet);

            // Act

            var result = _controller.GetViewConflict(id, conflictType) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ViewAddressConflict", result.ViewName);

            var model = result.Model as ViewAddressConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.IdRSP);
            Assert.AreEqual(conflict.RspModificationData.StatusMessage, model.StatusMessage);
            Assert.AreEqual(string.Format("{0} {1}{2} ", conflict.StreetName, conflict.GetHouse(), conflict.GetApartment()), model.Address);

            _bll.Verify(x => x.Get<RspRegistrationData>(id), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(conflict.Id), Times.Once);
            //_bll.Verify(x => x.Get<StreetTypeCode>(Convert.ToInt64(conflict.GetStreetCode())), Times.Once);
        }

        [TestMethod]
        public void GetViewConflict_ByPollingStationConflictConflict_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.PollingStationConflict;

            var conflict = GetRspRegistrationData(GetRspModificationData());
            
            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);
            
            // Act

            var result = _controller.GetViewConflict(id, conflictType) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ViewAddressNoStreetConflict", result.ViewName);

            var model = result.Model as ViewAddressConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.IdRSP);
            //Assert.AreEqual(conflict.RspModificationData.StatusMessage, model.StatusMessage);

            _bll.Verify(x => x.Get<RspRegistrationData>(id), Times.Once);
        }
        
        [TestMethod]
        public void GetViewConflict_ByRegionConflict_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.RegionConflict;

            var conflict = GetRspRegistrationData(GetRspModificationData());
            
            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);
            //_bll.Setup(x => x.GetConflictAddress(conflict.Id)).Returns(conflict);
            
            // Act

            var result = _controller.GetViewConflict(id, conflictType) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ViewRegionConflict", result.ViewName);

            var model = result.Model as RegionConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.IdRSP);
            //Assert.AreEqual(conflict.RspModificationData.StatusMessage, model.StatusMessage);
            Assert.AreEqual(conflict.Administrativecode, model.RegistruId);

            _bll.Verify(x => x.Get<RspRegistrationData>(id), Times.Once);
            //_bll.Verify(x => x.GetConflictAddress(conflict.Id), Times.Once);
        }

        [TestMethod]
        public void GetViewConflict_ByStreetConflict_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.StreetZeroConflict;

            var conflict = GetRspRegistrationData(GetRspModificationData());

            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);
            
            // Act

            var result = _controller.GetViewConflict(id, conflictType) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ViewStreetConflict", result.ViewName);

            var model = result.Model as StatusMessageConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.IdRSP);
            //Assert.AreEqual(conflict.RspModificationData.StatusMessage, model.StatusMessage);
            
            _bll.Verify(x => x.Get<RspRegistrationData>(id), Times.Once);
        }

        [TestMethod]
        public void GetViewConflict_ByLocalityConflict_returns_correct_result()
        {
            // Arrange

            const long id = 1;
            const ConflictStatusCode conflictType = ConflictStatusCode.LocalityConflict;

            var conflict = GetRspRegistrationData(GetRspModificationData());

            _bll.Setup(x => x.Get<RspRegistrationData>(id)).Returns(conflict);

            // Act

            var result = _controller.GetViewConflict(id, conflictType) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("_ViewLocalityConflict", result.ViewName);

            var model = result.Model as ViewLocalityConflictModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(conflict.Id, model.IdRSP);
            //Assert.AreEqual(conflict.RspModificationData.StatusMessage, model.StatusMessage);

            _bll.Verify(x => x.Get<RspRegistrationData>(id), Times.Once);
        }

        #endregion GetViewConflict

        private static void AssertAreEqual(VoterConflictDataDto voter, VoterConflictModel model)
        {
            Assert.IsNotNull(model);
            Assert.AreEqual(voter.Id, model.Id);
            Assert.AreEqual(voter.Address, model.Address);
            Assert.AreEqual(voter.Comments, model.Comments);
            Assert.AreEqual(voter.DateOfBirth.Date.ToShortDateString(), model.DateOfBirth);
            Assert.AreEqual(voter.DocIssueBy, model.DocIssueBy);
            Assert.AreEqual(voter.DocIssueDate.Value.Date.ToShortDateString(), model.DocIssueDate);
            Assert.AreEqual(voter.DocNumber, model.DocNumber);
            Assert.AreEqual(voter.DocSeria, model.DocSeria);
            Assert.AreEqual(voter.DocType, model.DocType);
            Assert.AreEqual(voter.DocValidBy.Value.Date.ToShortDateString(), model.DocValidBy);
            Assert.AreEqual(voter.FirstName, model.FirstName);
            Assert.AreEqual(voter.Gender, model.Gender);
            Assert.AreEqual(voter.Idnp, model.Idnp);
            Assert.AreEqual(voter.MiddleName, model.MiddleName);
            Assert.AreEqual(voter.PersonStatus, model.PersonStatus);
            Assert.AreEqual(voter.Surname, model.Surname);
        }

        private static void AssertJqGridJsonResult<T>(JsonResult result, List<T> expItems) where T : Entity
        {
            Assert.IsNotNull(result);

            var data = result.Data as JqGridResponse;
            Assert.IsNotNull(data);
            AssertListsAreEqual(data.Records, expItems);
        }

        private static void AssertDtoJqGridJsonResult<T>(JsonResult result, List<T> expItems) where T : BaseDto<long>
        {
            Assert.IsNotNull(result);

            var data = result.Data as JqGridResponse;
            Assert.IsNotNull(data);
            AssertDtoListsAreEqual(data.Records, expItems);
        }

        private static void AssertListsAreEqual<T>(IEnumerable<JqGridRecord> list1, List<T> list2) where T : Entity
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(x.Id.ToString(), item.Id))));
        }

        private static void AssertListsAreEqual<T>(IEnumerable<SelectListItem> list1, List<T> list2, Func<T, string> textFunc, Func<T, string> valueFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.Text) && string.Equals(valueFunc(x), item.Value))));
        }

        private static void AssertListsAreEqual<T>(IEnumerable<Select2Item> list1, List<T> list2, Func<T, string> textFunc, Func<T, long> idFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.text) && string.Equals(idFunc(x), item.id))));
        }

        private static void AssertDtoListsAreEqual<T>(IEnumerable<JqGridRecord> list1, List<T> list2) where T : BaseDto<long>
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(x.Id.ToString(), item.Id))));
        }

        private static List<RspConflictDataAdmin> GetRspConflictModificationDataAdmins()
        {
            return new List<RspConflictDataAdmin>()
            {
                GetRspConflictModificationDataAdmin()
            };
        }

        private static RspConflictDataAdmin GetRspConflictModificationDataAdmin()
        {
            return new RspConflictDataAdmin
            {
                AcceptConflictCode = ConflictStatusCode.None,
                Administrativecode = 1,
                ApNr = 1,
                ApSuffix = "A",
                Birthdate = DateTime.Now,
                CitizenRm = true,
                Comments = "comments",
                Created = DateTime.Now,
                DateOfExpiration = DateTime.Now,
                DateOfRegistration = DateTime.Now,
                Dead = false,
                Doctypecode = 1,
                Expirationdate = DateTime.Now,
                FirstName = "first_name",
                HouseNr = 1,
                HouseSuffix = "A",
                Idnp = "1234567890123",
                Issuedate = DateTime.Now,
                LastName = "last_name",
                Locality = "locality",
                Number = "123",
                RegTypeCode = 1,
                Region = "Chisinau",
                RejectConflictCode = ConflictStatusCode.None,
                SecondName = "second_name",
                Series = "A",
                SexCode = "M",
                Source = SourceEnum.Rsp,
                SrvRegion = GetRegion(),
                Status = RawDataStatus.InProgress,
                StatusConflictCode = ConflictStatusCode.None,
                StatusDate = DateTime.Now,
                StatusMessage = "",
                StreetName = "street",
                Streetcode = 1,
                Validity = true,
                IsInConflict = true
            };
        }

        private static List<RspConflictData> GetRspConflictModificationDatas()
        {
            return new List<RspConflictData>()
            {
                GetRspConflictModificationData()
            };
        }

        private static RspConflictData GetRspConflictModificationData()
        {
            return new RspConflictData
            {
                AcceptConflictCode = ConflictStatusCode.None,
                Administrativecode = 1,
                ApNr = 1,
                ApSuffix = "A",
                Birthdate = DateTime.Now,
                CitizenRm = true,
                Comments = "comments",
                Created = DateTime.Now,
                DateOfExpiration = DateTime.Now,
                DateOfRegistration = DateTime.Now,
                Dead = false,
                Doctypecode = 1,
                Expirationdate = DateTime.Now,
                FirstName = "first_name",
                HouseNr = 1,
                HouseSuffix = "A",
                Idnp = "1234567890123",
                Issuedate = DateTime.Now,
                LastName = "last_name",
                Locality = "locality",
                Number = "123",
                RegTypeCode = 1,
                Region = "Chisinau",
                RejectConflictCode = ConflictStatusCode.None,
                SecondName = "second_name",
                Series = "A",
                SexCode = "M",
                Source = SourceEnum.Rsp,
                SrvRegion = GetRegion(),
                Status = RawDataStatus.InProgress,
                StatusConflictCode = ConflictStatusCode.None,
                StatusDate = DateTime.Now,
                StatusMessage = "",
                StreetName = "street",
                Streetcode = 1,
                Validity = true,
                IsInConflict = true
            };
        }

        private static RspModificationData GetRspModificationData()
        {
            return new RspModificationData
            {
                AcceptConflictCode = ConflictStatusCode.None,
                Birthdate = DateTime.Now,
                CitizenRm = true,
                Comments = "comments",
                Created = DateTime.Now,
                Dead = false,
                DocumentTypeCode = 1,
                ValidBydate = DateTime.Now,
                FirstName = "first_name",
                Idnp = "1234567890123",
                IssuedDate = DateTime.Now,
                LastName = "last_name",
                Number = "123",
                RejectConflictCode = ConflictStatusCode.None,
                SecondName = "second_name",
                Seria = "A",
                SexCode = "M",
                Source = SourceEnum.Rsp,
                StatusConflictCode = ConflictStatusCode.None,
                StatusDate = DateTime.Now,
                StatusMessage = "",
                Validity = true,
                Registrations = GetRspRegistrationDatas()
            };
        }

        private static List<RspRegistrationData> GetRspRegistrationDatas()
        {
            return new List<RspRegistrationData>()
            {
                GetRspRegistrationData()
            };
        }

        private static RspRegistrationData GetRspRegistrationData(RspModificationData data = null)
        {
            return new RspRegistrationData
            {
                Administrativecode = 1,
                ApartmentNumber = 1,
                ApartmentSuffix = "A",
                DateOfExpiration = DateTime.Now,
                DateOfRegistration = DateTime.Now,
                HouseNumber = 1,
                HouseSuffix = "A",
                IsInConflict = true,
                Locality = "Chisinau",
                RegTypeCode = 1,
                Region = "Chisinau",
                RspModificationData = data,
                StreetCode = 1
            };
        }

        private static List<Region> GetRegions()
        {
            return new List<Region>()
            {
                GetRegion()
            };
        }

        private static Region GetRegion()
        {
            var region = new Region(GetRegionType())
            {
                Circumscription = 1,
                Description = "description",
                GeoLocation = GetGeoLocation(),
                HasStreets = true,
                Name = "Chisinau",
                RegistruId = 123,
                SaiseId = 321,
                StatisticCode = 222,
                StatisticIdentifier = 111
            };

            region.PublicAdministration = GetPublicAdministration(region);

            return region;
        }

        private static RegionType GetRegionType()
        {
            return new RegionType
            {
                Description = "description",
                Name = "name",
                Rank = 1
            };
        }
        
        private static GeoLocation GetGeoLocation()
        {
            return new GeoLocation
            {
                Latitude = 20,
                Longitude = 20
            };
        }

        private static PublicAdministration GetPublicAdministration(Region region)
        {
            return new PublicAdministration(region, GetManagerType())
            {
                Name = "name",
                Surname = "surname"
            };
        }

        private static ManagerType GetManagerType()
        {
            return new ManagerType
            {
                Name = "manager",
                Description = "description"
            };
        }

        private static StreetTypeCode GetStreetTypeCode()
        {
            return new StreetTypeCode()
            {
                Docprint = "docprint",
                Name = "name",
                Namerus = "namerus",
                RspStreetTypeCodeId = 1
            };
        }

        private static List<AddressBaseDto> GetAddressBaseDtos()
        {
            return new List<AddressBaseDto>()
            {
                GetAddressBaseDto()
            };
        }

        private static AddressBaseDto GetAddressBaseDto()
        {
            return new AddressBaseDto()
            {
                HouseNumber = 1,
                PollingStation = "1/11",
                StreetName = "street",
                StreetTypeName = "street_type",
                Suffix = "A"
            };
        }

        private static Address GetAddress(PollingStation pollingStation = null)
        {
            return new Address
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = GetGeoLocation(),
                HouseNumber = 12,
                PollingStation = pollingStation,
                Street = GetStreet(11),
                Suffix = "A"
            };
        }

        private static Street GetStreet(long ropId)
        {
            return new Street(GetRegion(), GetStreetType(), "street")
            {
                RopId = ropId,
                SaiseId = 123
            };
        }

        private static StreetType GetStreetType()
        {
            return new StreetType
            {
                Name = "str",
                Description = "str"
            };
        }

        private static List<PollingStation> GetPollingStations()
        {
            return new List<PollingStation> { GetPollingStation() };
        }

        private static PollingStation GetPollingStation()
        {
            var pollingStation = new PollingStation(GetRegion())
            {
                ContactInfo = "info",
                GeoLocation = GetGeoLocation(),
                Location = "Chisinau",
                Number = "123",
                SaiseId = 123,
                SubNumber = "A"
            };

            pollingStation.PollingStationAddress = GetAddress(pollingStation);

            return pollingStation;
        }

        private static CreateAddressModel GetCreateAddressModel()
        {
            return new CreateAddressModel()
            {
                HouseNumber = 1,
                PollingStationId = 1,
                RegionId = 1,
                RspId = 1,
                Street = "street",
                StreetId = 1,
                Suffix = "A"
            };
        }

        private static RejectConflict GetRejectConflict()
        {
            return new RejectConflict()
            {
                Comment = "comment",
                ConflictIds = new List<long> {1, 2},
                ConflictStatus = ConflictStatusCode.AddressConflict
            };
        }

        private static ResolveAddressNoStreetConflictModel GetResolveAddressNoStreetConflictModel()
        {
            return new ResolveAddressNoStreetConflictModel()
            {
                PollingStationId = 1,
                RegionId = 1,
                RspIds = new List<long> {1, 2}
            };
        }

        private static VoterConflictDataDto GetVoterConflictDataDto()
        {
            return new VoterConflictDataDto()
            {
                Address = "address",
                Comments = "comments",
                DateOfBirth = DateTime.Now,
                DocIssueBy = "dc",
                DocIssueDate = DateTime.Now,
                DocNumber = "123",
                DocSeria = "A",
                DocType = "A",
                DocValidBy = DateTime.Now,
                FirstName = "firstName",
                Gender = "M",
                Idnp = "1234567890123",
                MiddleName = "middle",
                PersonStatus = "status",
                Surname = "surname"
            };
        }

        private static JqGridRequest GetJqGridRequest()
        {
            return new JqGridRequest
            {
                PageIndex = 1,
                PagesCount = 1,
                RecordsCount = 1,
                Searching = false
            };
        }

        private static Select2Request GetSelect2Request()
        {
            return new Select2Request()
            {
                page = 1,
                pageLimit = 1,
                q = ""
            };
        }

        private static PageResponse<T> GetPageResponse<T>(IList<T> items) where T : class
        {
            return new PageResponse<T>
            {
                Items = items,
                PageSize = 1,
                StartIndex = 1,
                Total = 1
            };
        }

        private void SetAdministratorRole()
        {
            SetRole("Administrator");
        }

        private void SetRegistratorRole()
        {
            SetRole("Registrator");
        }

        private void SetRole(string role)
        {
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, "1");
            var claimName = new Claim(ClaimTypes.Name, "test@user.com");
            var claimRole = new Claim(ClaimTypes.Role, role);
            var claimsIdentity = new ClaimsIdentity(new[] { claimNameIdentifier, claimName, claimRole }, "TestAuthentication");
            ClaimsPrincipal.Current.AddIdentity(claimsIdentity);
        }

    }
}

