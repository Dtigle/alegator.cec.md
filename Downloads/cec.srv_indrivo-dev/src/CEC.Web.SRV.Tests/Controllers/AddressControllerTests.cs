using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Controllers;
using CEC.Web.SRV.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CEC.Web.SRV.Models.Address;
using CEC.Web.SRV.Infrastructure.Export;

namespace CEC.Web.SRV.Tests.Controllers
{
	[TestClass]
	public class AddressControllerTests : BaseControllerTests
	{
        private static Mock<IAddressBll> _addressBll;
        private static AddressController _controller;

        public AddressControllerTests()
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
            _addressBll = new Mock<IAddressBll>();
            _controller = new AddressController(_addressBll.Object);
		    BaseController = _controller;
		}
        
        [TestMethod]
        public void GetAddressesBy_return_correct_json()
        {
            const string term = "";
            long? regionId = 1;
            var region = GetRegion((long)regionId);
            var street = GetStreet(region);
            var streets = new List<Street> {street};
            _addressBll.Setup(x => x.GetStreetsByFilter(term))
                .Returns(streets);

            var result = _controller.GetAddressesBy(term);

            _addressBll.Verify(x => x.GetStreetsByFilter(term), Times.Once());

            var viewResult = result as JsonResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewResult.GetType(), typeof(JsonResult));
        }

        [TestMethod]
        public void GetPollingStationsByFilter_return_correct_json()
        {
            long? term = null;
            long? regionId = 1;
            var region = GetRegion((long)regionId);
            var pollingStation = GetPollingStation(region);
            var pollingStations = new List<PollingStation> { pollingStation };
			_addressBll.Setup(x => x.GetPollingStations(term))
                .Returns(pollingStations);

            var result = _controller.GetPollingStationsByFilter(term);

			_addressBll.Verify(x => x.GetPollingStations(term), Times.Once());

            var viewResult = result as JsonResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewResult.GetType(), typeof(JsonResult));
        }

        [TestMethod]
        public void SelectBuildingType_return_correct_view()
        {
            var result = _controller.SelectBuildingType();

            var viewResult = result as PartialViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(viewResult.ViewName, "_Select");
            Assert.AreEqual(viewResult.Model.GetType(), typeof(List<SelectListItem>));
        }

        [TestMethod]
        public void ExportAddressesAllData_has_correct_logic()
        {
            ExportGridDataTest<AddressBuildingsGridModel>(ExportType.AllData, "Addresses");
        }

        [TestMethod]
        public void ExportAddressesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<AddressBuildingsGridModel>(ExportType.CurrentPage, "Addresses");
        }
        
		//[TestMethod]
		//public void ListAddressApartmentAjax_return_correct_page_response()
		//{
		//	long? regionId = 39;
		//	var request = new JqGridRequest() { PageIndex = 0, RecordsCount = 20 };

		//	_addressBll.Setup(x => x.Get(It.IsAny<PageRequest>(), regionId, BuildingTypes.ApartmentBuilding))
		//		.Returns(new PageResponse<Address> { Items = new List<Address>(), PageSize = 20, StartIndex = 1, Total = 2 });
		//	var result = _controller.ListAddressesApartmentAjax(request, regionId);
		//	_addressBll.Verify(x => x.Get(It.IsAny<PageRequest>(), regionId, BuildingTypes.ApartmentBuilding), Times.Once());

		//	Assert.IsNotNull(result);
		//}

		//[TestMethod]
		//public void ListAddressesAdministrativeAjax_return_correct_page_response()
		//{
		//	long? regionId = 39;
		//	var request = new JqGridRequest() { PageIndex = 0, RecordsCount = 20 };

		//	_addressBll.Setup(x => x.Get(It.IsAny<PageRequest>(), regionId, BuildingTypes.AdministrativeBuilding))
		//		.Returns(new PageResponse<Address> { Items = new List<Address>(), PageSize = 20, StartIndex = 1, Total = 2 });
		//	var result = _controller.ListAddressesAdministrativeAjax(request, regionId);
		//	_addressBll.Verify(x => x.Get(It.IsAny<PageRequest>(), regionId, BuildingTypes.AdministrativeBuilding), Times.Once());

		//	Assert.IsNotNull(result);
		//}

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
        private static PollingStation GetPollingStation(Region region)
        {
            var pollingStation = new PollingStation(region);
            pollingStation.SetId(region.Id);
            return pollingStation;
        }
	}
}
