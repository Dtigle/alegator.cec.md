using System.Collections.Generic;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Controllers;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.PollingStation;
using CEC.Web.SRV.Profiles;
using FluentNHibernate.Conventions;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CEC.Web.SRV.Infrastructure.Export;

namespace CEC.Web.SRV.Tests.Controllers
{
	[TestClass]
	public class PollingStationControllerTests : BaseControllerTests
	{
        private static Mock<IPollingStationBll> _pollingStationBll;
        private static PollingStationController _controller;

        public PollingStationControllerTests()
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
            _pollingStationBll = new Mock<IPollingStationBll>();
            _controller = new PollingStationController(_pollingStationBll.Object);
		    BaseController = _controller;
		}

		[TestMethod]
		public void Index_returns_correct_view()
		{
			var result = _controller.Index();

			Assert.IsInstanceOfType(result, typeof (ViewResult));
			Assert.AreEqual(((ViewResult) result).ViewName, "");
		}

		[TestMethod]
        public void CreateUpdatePollingStation_returns_correct_view()
		{
		    const int regionId = 1;
            var region = GetRegion(regionId);
            var street = GetStreet(region);
		    _pollingStationBll.Setup(x => x.Get<Region>(regionId)).Returns(region);
            _pollingStationBll.Setup(x => x.GetPollingStationAddresses(regionId))
               .Returns(new List<Address>
               {
                    new Address {HouseNumber = 1, Street = street}
                });
            var result = _controller.CreateUpdatePollingStation(regionId, null);
            _pollingStationBll.Verify(x => x.Get<PollingStation>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewResult.ViewName, "_CreateUpdatePollingStationPartial");
			Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdatePollingStationModel));
		}

        [TestMethod]
        public void CreateUpdatePollingStation_has_correct_logic()
        {
            var pollingStationId = (long?)1;
            const long regionId = 1;
            var region = GetRegion(regionId);
            var street = GetStreet(region);
            _pollingStationBll.Setup(x => x.Get<Region>(regionId)).Returns(region);
            _pollingStationBll.Setup(x => x.Get<PollingStation>(pollingStationId.Value))
                .Returns(new PollingStation(region) { Number = pollingStationId.ToString() });
            _pollingStationBll.Setup(x => x.GetPollingStationAddresses(regionId))
                .Returns(new List<Address>
                {
                    new Address {HouseNumber = 1, Street = street}
                });

            var result = _controller.CreateUpdatePollingStation(regionId, pollingStationId);

            _pollingStationBll.Verify(x => x.Get<PollingStation>(pollingStationId.Value), Times.Once());
            var viewResult = result as PartialViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            var model = viewResult.Model as UpdatePollingStationModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Number, pollingStationId.ToString());
        }

        [TestMethod]
        public void CreateUpdatePollingStation_has_ValidModel()
        {
            var model = new UpdatePollingStationModel { Id = 1, Number = "1/11"};
            _pollingStationBll.Setup(x => x.CreateUpdatePollingStation(model.Id, model.RegionId, model.Number, model.Location,
                       model.AddressId.GetValueOrDefault(), model.ContactInfo, model.SaiseId, model.PollingStationType));

            var result = _controller.CreateUpdatePollingStation(model);
            _pollingStationBll.Verify(x => x.CreateUpdatePollingStation(model.Id, model.RegionId, It.IsAny<string>(), model.Location,
                        model.AddressId.GetValueOrDefault(), model.ContactInfo, model.SaiseId, model.PollingStationType), Times.Once);
            var viewResult = result as ContentResult;
            Assert.IsNotNull(viewResult);
            Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
        }

        [TestMethod]
        public void CreateUpdatePollingStation_has_NotValidModel()
        {
            //arrange
            const long regionId = 1;
            var region = GetRegion(regionId);
            var street = GetStreet(region);
            _pollingStationBll.Setup(x => x.GetPollingStationAddresses(regionId))
               .Returns(new List<Address>
               {
                    new Address {HouseNumber = 1, Street = street}
                });
            var model = new UpdatePollingStationModel {RegionId = regionId};
            var modelState = _controller.ModelState;
            modelState.AddModelError("","Error");
            _pollingStationBll.Setup(x => x.CreateUpdatePollingStation(model.Id, model.RegionId, model.Number, model.Location,
                       model.AddressId.GetValueOrDefault(), model.ContactInfo, model.SaiseId, model.PollingStationType));

            //act
            var result = _controller.CreateUpdatePollingStation(model);

            //assert
            _pollingStationBll.Verify(x => x.CreateUpdatePollingStation(model.Id, model.RegionId, model.Number, model.Location,
                        model.AddressId.GetValueOrDefault(), model.ContactInfo, model.SaiseId, model.PollingStationType), Times.Never);

            var viewResult = result as PartialViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
            Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
        }

        [TestMethod]
        public void DeletePollingStation_has_correct_logic()
        {
            //arrange
            const int pollingStationId = 1;
            _pollingStationBll.Setup(x => x.DeletePollingStation(pollingStationId));

            //act
            _controller.DeletePollingStation(pollingStationId);

            //assert
            _pollingStationBll.Verify(x => x.DeletePollingStation(pollingStationId), Times.Once());
        }

        [TestMethod]
        public void ListPollingStationAjax_return_correct_page_response()
        {
            const long regionId = 39;
            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
            
            _pollingStationBll.Setup(x => x.GetPollingStation(It.IsAny<PageRequest>(), regionId))
				.Returns(new PageResponse<PollingStationDto> { Items = new List<PollingStationDto>(), PageSize = 20, StartIndex = 1, Total = 2 });
            var result = _controller.ListPollingStationAjax(request, regionId);
            _pollingStationBll.Verify(x => x.GetPollingStation(It.IsAny<PageRequest>(), regionId), Times.Once());

            Assert.IsNotNull(result);
        }

	    [TestMethod]
	    public void GetAddressName_return_correct_view()
	    {
	        // arrange
            const long addressId = 1;
	        const int regionId = 1;
	        var adress = GetAddress(regionId, 11, "M");
	        _pollingStationBll.Setup(x => x.Get<Address>(addressId)).Returns(adress);

            // act
	        var result = _controller.GetAddressName(addressId);

	        // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.ToString().Contains("11M"));
	    }

        [TestMethod]
        public void GetAddressName_return_empty()
        {
            // arrange
            const long addressId = -1;
            
            // act
            var result = _controller.GetAddressName(addressId);

            // assert
            Assert.IsTrue(result.Data.ToString().IsEmpty());
        }

        [TestMethod]
        public void ExportPollingStationsAllData_has_correct_logic()
        {
            ExportGridDataTest<PollingStationGridModel>(ExportType.AllData, "PollingStations");
        }

        [TestMethod]
        public void ExportPollingStationsCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<PollingStationGridModel>(ExportType.CurrentPage, "PollingStations");
        }

	    private static Region GetRegion(long regionId)
	    { 
            var region = new Region(new RegionType());
            region.SetId(regionId);
            region.HasStreets = true;
	        return region;
	    }

	    private static Street GetStreet(Region region)
	    {
            var street = new Street(region, new StreetType(), "Name");
            street.SetId(region.Id);
            return street;
	    }

	    private static Address GetAddress(long regionId, int houseNumber, string suffix)
	    {
	        var address = new Address();
            var region = GetRegion(regionId);
            var street = GetStreet(region);
	        address.Street = street;
	        address.HouseNumber = houseNumber;
	        address.Suffix = suffix;
	        return address;
	    }
	}
}
