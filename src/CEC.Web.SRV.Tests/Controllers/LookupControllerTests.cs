using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Amdaris.Domain;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Responses;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Controllers;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq.Expressions;

namespace CEC.Web.SRV.Tests.Controllers
{
	[TestClass]
	public class LookupControllerTests : BaseControllerTests
	{
		private static Mock<ILookupBll> _lookupBll;
		private static Mock<IService<ILookupBll>> _loockupService;
		private static LookupController _controller;

		public LookupControllerTests()
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
			_lookupBll = new Mock<ILookupBll>();
			_loockupService = new Mock<IService<ILookupBll>>();
			_controller = new LookupController(_lookupBll.Object, _loockupService.Object);
		    BaseController = _controller;
		}

		[TestMethod]
		public void StreetTypes_returns_correct_view()
		{
			var result = _controller.StreetTypes();

			Assert.IsInstanceOfType(result, typeof (ViewResult));
			Assert.AreEqual(((ViewResult) result).ViewName, "");
		}

		[TestMethod]
		public void CreateUpdateManagerType_returns_correct_view()
		{
			var result = _controller.CreateUpdateManagerType((long?) null);
			_lookupBll.Verify(x => x.Get<ManagerType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdateManagerTypePartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdateLookupModel));
		}

		[TestMethod]
		public void CreateUpdateManagerType_has_correct_logic()
		{
			var managerTypeId = (long?) 1;
			_lookupBll.Setup(x => x.Get<ManagerType>(managerTypeId.Value))
				.Returns(new ManagerType {Name = managerTypeId.ToString()});

			var result = _controller.CreateUpdateManagerType(managerTypeId);

			_lookupBll.Verify(x => x.Get<ManagerType>(managerTypeId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateLookupModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, managerTypeId.ToString());
		}

		[TestMethod]
		public void CreateUpdateManagerType_has_correct_logic_when_NotUnique()
		{
			var model = new UpdateLookupModel {Id = 1};
			_lookupBll.Setup(x => x.IsUnique<ManagerType>(model.Id, model.Name)).Returns(false);

			var result = _controller.CreateUpdateManagerType(model);

			_lookupBll.Verify(x => x.IsUnique<ManagerType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdate<ManagerType>(model.Id, model.Name, model.Description), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateManagerType_has_correct_logic_when_Unique()
		{
			var model = new UpdateLookupModel {Id = 1};
			_lookupBll.Setup(x => x.IsUnique<ManagerType>(model.Id, model.Name)).Returns(true);
			_lookupBll.Setup(x => x.SaveOrUpdate<ManagerType>(model.Id, model.Name, model.Description));

			var result = _controller.CreateUpdateManagerType(model);

			_lookupBll.Verify(x => x.IsUnique<ManagerType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdate<ManagerType>(model.Id, model.Name, model.Description), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeleteManagerType_has_correct_logic()
		{
			const int managerTypeId = 1;
			_lookupBll.Setup(x => x.Delete<ManagerType>(managerTypeId));
			_controller.DeleteManagerType(managerTypeId);
			_lookupBll.Verify(x => x.Delete<ManagerType>(managerTypeId), Times.Once());
		}

		[TestMethod]
		public void UnDeleteManagerType_has_correct_logic()
		{
			const int managerTypeId = 1;
			_lookupBll.Setup(x => x.UnDelete<ManagerType>(managerTypeId));
			_controller.UnDeleteManagerType(managerTypeId);
			_lookupBll.Verify(x => x.UnDelete<ManagerType>(managerTypeId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdateStreetType_returns_correct_view()
		{
			var result = _controller.CreateUpdateStreetType((long?)null);
			_lookupBll.Verify(x => x.Get<StreetType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdateStreetTypePartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdateLookupModel));
		}

		[TestMethod]
		public void CreateUpdateStreetType_has_correct_logic()
		{
			var streetTypeId = (long?)1;
			_lookupBll.Setup(x => x.Get<StreetType>(streetTypeId.Value))
				.Returns(new StreetType { Name = streetTypeId.ToString() });

			var result = _controller.CreateUpdateStreetType(streetTypeId);

			_lookupBll.Verify(x => x.Get<StreetType>(streetTypeId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateLookupModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, streetTypeId.ToString());
		}

		public void CreateUpdateStreetType_has_correct_logic_when_NotUnique()
		{
			var model = new UpdateLookupModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<StreetType>(model.Id, model.Name)).Returns(false);

			var result = _controller.CreateUpdateStreetType(model);

			_lookupBll.Verify(x => x.IsUnique<StreetType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdate<StreetType>(model.Id, model.Name, model.Description), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateStreetType_has_correct_logic_when_Unique()
		{
			var model = new UpdateLookupModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<StreetType>(model.Id, model.Name)).Returns(true);
			_lookupBll.Setup(x => x.SaveOrUpdate<StreetType>(model.Id, model.Name, model.Description));

			var result = _controller.CreateUpdateStreetType(model);

			_lookupBll.Verify(x => x.IsUnique<StreetType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdate<StreetType>(model.Id, model.Name, model.Description), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeleteStreetType_has_correct_logic()
		{
			const int streetTypeId = 1;
			_lookupBll.Setup(x => x.Delete<StreetType>(streetTypeId));
			_controller.DeleteStreetType(streetTypeId);
			_lookupBll.Verify(x => x.Delete<StreetType>(streetTypeId), Times.Once());
		}

		[TestMethod]
		public void UnDeleteStreetType_has_correct_logic()
		{
			const int streetTypeId = 1;
			_lookupBll.Setup(x => x.UnDelete<StreetType>(streetTypeId));
			_controller.UnDeleteStreetType(streetTypeId);
			_lookupBll.Verify(x => x.UnDelete<StreetType>(streetTypeId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdateRegionType_returns_correct_view()
		{
			var result = _controller.CreateUpdateRegionType((long?)null);
			_lookupBll.Verify(x => x.Get<RegionType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdateRegionTypePartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdateRegionTypesModel));
		}

		[TestMethod]
		public void CreateUpdateRegionType_has_correct_logic()
		{
			var regionTypeId = (long?)1;
			_lookupBll.Setup(x => x.Get<RegionType>(regionTypeId.Value))
				.Returns(new RegionType { Name = regionTypeId.ToString() });

			var result = _controller.CreateUpdateRegionType(regionTypeId);

			_lookupBll.Verify(x => x.Get<RegionType>(regionTypeId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateRegionTypesModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, regionTypeId.ToString());
		}

		[TestMethod]
		public void CreateUpdateRegionType_has_correct_logic_when_NotUnique()
		{
			var model = new UpdateRegionTypesModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<RegionType>(model.Id, model.Name)).Returns(false);

			var result = _controller.CreateUpdateRegionType(model);

			_lookupBll.Verify(x => x.IsUnique<RegionType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdateRegionType(model.Id, model.Name, model.Description, (byte)model.Rank), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateRegionType_has_correct_logic_when_Unique()
		{
			var model = new UpdateRegionTypesModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<RegionType>(model.Id, model.Name)).Returns(true);
			_lookupBll.Setup(x => x.SaveOrUpdateRegionType(model.Id, model.Name, model.Description, (byte)model.Rank));

			var result = _controller.CreateUpdateRegionType(model);

			_lookupBll.Verify(x => x.IsUnique<RegionType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdateRegionType(model.Id, model.Name, model.Description, (byte)model.Rank), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeleteRegionType_has_correct_logic()
		{
			const long regionTypeId = (long)1;
			_lookupBll.Setup(x => x.Delete<RegionType>(regionTypeId));
			_controller.DeleteRegionType(regionTypeId);
			_lookupBll.Verify(x => x.Delete<RegionType>(regionTypeId), Times.Once());
		}

		[TestMethod]
		public void UnDeleteRegionType_has_correct_logic()
		{
			const long regionTypeId = (long)1;
			_lookupBll.Setup(x => x.UnDelete<RegionType>(regionTypeId));
			_controller.UnDeleteRegionType(regionTypeId);
			_lookupBll.Verify(x => x.UnDelete<RegionType>(regionTypeId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdatePersonStatus_returns_correct_view()
		{
			var result = _controller.CreateUpdatePersonStatus((long?)null);
			_lookupBll.Verify(x => x.Get<PersonStatusType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdatePersonStatusPartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdatePersonStatusModel));
		}

		[TestMethod]
		public void CreateUpdatePersonStatus_has_correct_logic()
		{
			var personStatusId = (long?)1;
			_lookupBll.Setup(x => x.Get<PersonStatusType>(personStatusId.Value))
				.Returns(new PersonStatusType { Name = personStatusId.ToString() });

			var result = _controller.CreateUpdatePersonStatus(personStatusId);

			_lookupBll.Verify(x => x.Get<PersonStatusType>(personStatusId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdatePersonStatusModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, personStatusId.ToString());
		}

		[TestMethod]
		public void CreateUpdatePersonStatus_has_correct_logic_when_NotUnique()
		{
			var model = new UpdatePersonStatusModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<PersonStatusType>(model.Id, model.Name)).Returns(false);

			var result = _controller.CreateUpdatePersonStatus(model);

			_lookupBll.Verify(x => x.IsUnique<PersonStatusType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdatePersonStatus(model.Id, model.Name, model.Description, model.IsExcludable), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreatePersonStatus_has_correct_logic_when_Unique()
		{
			var model = new UpdatePersonStatusModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<PersonStatusType>(model.Id, model.Name)).Returns(true);

			var result = _controller.CreateUpdatePersonStatus(model);

			_lookupBll.Verify(x => x.IsUnique<PersonStatusType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdatePersonStatus(model.Id, model.Name, model.Description, model.IsExcludable), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeletePersonStatus_has_correct_logic()
		{
			const long personStatusId = 1;
			_lookupBll.Setup(x => x.Delete<PersonStatusType>(personStatusId));
			_controller.DeletePersonStatus(personStatusId);
			_lookupBll.Verify(x => x.Delete<PersonStatusType>(personStatusId), Times.Once());
		}

		[TestMethod]
		public void UnDeletePersonStatus_has_correct_logic()
		{
			const long personStatusId = 1;
			_lookupBll.Setup(x => x.UnDelete<PersonStatusType>(personStatusId));
			_controller.UnDeletePersonStatus(personStatusId);
			_lookupBll.Verify(x => x.UnDelete<PersonStatusType>(personStatusId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdateGender_returns_correct_view()
		{
			var result = _controller.CreateUpdateGender((long?)null);
			_lookupBll.Verify(x => x.Get<Gender>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdateGenderPartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdateLookupModel));
		}

		[TestMethod]
		public void CreateUpdateGender_has_correct_logic()
		{
			var genderId = (long?)1;
			_lookupBll.Setup(x => x.Get<Gender>(genderId.Value))
				.Returns(new Gender { Name = genderId.ToString() });

			var result = _controller.CreateUpdateGender(genderId);

			_lookupBll.Verify(x => x.Get<Gender>(genderId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateLookupModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, genderId.ToString());
		}

		[TestMethod]
		public void CreateUpdateGender_has_correct_logic_when_NotUnique()
		{
			var model = new UpdateLookupModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<Gender>(model.Id, model.Name)).Returns(false);

			var result = _controller.CreateUpdateGender(model);

			_lookupBll.Verify(x => x.IsUnique<Gender>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdate<Gender>(model.Id, model.Name, model.Description), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateGender_has_correct_logic_when_Unique()
		{
			var model = new UpdateLookupModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<Gender>(model.Id, model.Name)).Returns(true);
			_lookupBll.Setup(x => x.SaveOrUpdate<Gender>(model.Id, model.Name, model.Description));

			var result = _controller.CreateUpdateGender(model);

			_lookupBll.Verify(x => x.IsUnique<Gender>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdate<Gender>(model.Id, model.Name, model.Description), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeleteGender_has_correct_logic()
		{
			const long genderId = 1;
			_lookupBll.Setup(x => x.Delete<Gender>(genderId));
			_controller.DeleteGender(genderId);
			_lookupBll.Verify(x => x.Delete<Gender>(genderId), Times.Once());
		}

		[TestMethod]
		public void UnDeleteGender_has_correct_logic()
		{
			const long genderId = 1;
			_lookupBll.Setup(x => x.UnDelete<Gender>(genderId));
			_controller.UnDeleteGender(genderId);
			_lookupBll.Verify(x => x.UnDelete<Gender>(genderId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdateStreet_returns_correct_view()
		{
			const int regionId = 1;
			_lookupBll.Setup(x => x.GetAll<StreetType>()).Returns(new List<StreetType> { new StreetType {  Name = regionId.ToString()}});
			_lookupBll.Setup(x => x.Get<Region>(regionId))
				.Returns(new Region(new RegionType()) { HasStreets = true });

			var result = _controller.CreateUpdateStreet(regionId,null);

			_lookupBll.Verify(x => x.GetAll<StreetType>(), Times.Once());
			_lookupBll.Verify(x => x.Get<Street>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_CreateStreetPartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdateStreetModel));
			var viewData = viewResult.ViewData;
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["StreetTypeId"]).Any());
		}

		[TestMethod]
		public void CreateUpdateStreet_has_correct_logic()
		{
			var streetId = (long?)1;
			const int regionId = 1;
			_lookupBll.Setup(x => x.Get<Street>(streetId.Value))
				.Returns(new Street(new Region(new RegionType { Name = streetId.ToString() }) { HasStreets = true, Name = streetId.ToString() }, new StreetType(), streetId.ToString()) { Name = streetId.ToString() });
			_lookupBll.Setup(x => x.GetAll<StreetType>())
				.Returns(new List<StreetType> { new StreetType { Name = streetId.ToString() } });
			_lookupBll.Setup(x => x.Get<Region>(regionId))
				.Returns(new Region(new RegionType()) { HasStreets = true });

			var result = _controller.CreateUpdateStreet(regionId,streetId);

			_lookupBll.Verify(x => x.Get<Street>(streetId.Value), Times.Once());
			_lookupBll.Verify(x => x.GetAll<StreetType>(), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateStreetModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, streetId.ToString());
			var viewData = viewResult.ViewData;
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["StreetTypeId"]).Any());
		}

		[TestMethod]
		public void CreateUpdateStreet_has_correct_logic_when_NotUnique()
		{
			var streetId = (long?)1;
			var model = new UpdateStreetModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique(model.Id, model.RegionId, model.Name, model.StreetTypeId)).Returns(false);
			_lookupBll.Setup(x => x.GetAll<StreetType>())
				.Returns(new List<StreetType> { new StreetType { Name = streetId.ToString() } });

			var result = _controller.CreateUpdateStreet(model);

			_lookupBll.Verify(x => x.IsUnique(model.Id, model.RegionId, model.Name, model.StreetTypeId), Times.Once());
			_lookupBll.Verify(x => x.CreateUpdateStreet(model.Id, model.Name, model.Description, model.RegionId, model.StreetTypeId,
					   model.RopId, model.SaiseId), Times.Never);
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
			var viewData = viewResult.ViewData;
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["StreetTypeId"]).Any());
		}

		[TestMethod]
		public void CreateUpdateStreete_has_correct_logic_when_Unique()
		{
			var model = new UpdateStreetModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique(model.Id, model.RegionId, model.Name, model.StreetTypeId)).Returns(true);
			_lookupBll.Setup(x => x.CreateUpdateStreet(model.Id, model.Name, model.Description, model.RegionId, model.StreetTypeId,
                       model.RopId, model.SaiseId));

			var result = _controller.CreateUpdateStreet(model);

			_lookupBll.Verify(x => x.IsUnique(model.Id, model.RegionId, model.Name, model.StreetTypeId), Times.Once());
			_lookupBll.Verify(x => x.CreateUpdateStreet(model.Id, model.Name, model.Description, model.RegionId, model.StreetTypeId,
                       model.RopId, model.SaiseId), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeleteStreet_has_correct_logic()
		{
			const long streetId = 1;
            _lookupBll.Setup(x => x.DeleteStreet(streetId));
			_controller.DeleteStreet(streetId);
            _lookupBll.Verify(x => x.DeleteStreet(streetId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdateDocType_returns_correct_view()
		{
			var result = _controller.CreateUpdateDocType((long?)null);
			_lookupBll.Verify(x => x.Get<DocumentType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdateDocumentTypesPartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.AreEqual(viewResult.Model.GetType(), typeof(UpdateDocumentTypesModel));
		}

		[TestMethod]
		public void CreateUpdateDocType_has_correct_logic()
		{
			var documentTypeId = (long?)1;
			_lookupBll.Setup(x => x.Get<DocumentType>(documentTypeId.Value))
				.Returns(new DocumentType { Name = documentTypeId.ToString() });

			var result = _controller.CreateUpdateDocType(documentTypeId);

			_lookupBll.Verify(x => x.Get<DocumentType>(documentTypeId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateDocumentTypesModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, documentTypeId.ToString());
		}

		[TestMethod]
		public void CreateUpdateDocType_has_correct_logic_when_NotUnique()
		{
			var model = new UpdateDocumentTypesModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<DocumentType>(model.Id, model.Name)).Returns(false);

			var result = _controller.CreateUpdateDocType(model);

			_lookupBll.Verify(x => x.IsUnique<DocumentType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdateDocType(model.Id, model.Name, model.Description, model.IsPrimary), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateDocType_has_correct_logic_when_Unique()
		{
			var model = new UpdateDocumentTypesModel { Id = 1 };
			_lookupBll.Setup(x => x.IsUnique<DocumentType>(model.Id, model.Name)).Returns(true);
			_lookupBll.Setup(x => x.SaveOrUpdateDocType(model.Id, model.Name, model.Description, model.IsPrimary));

			var result = _controller.CreateUpdateDocType(model);

			_lookupBll.Verify(x => x.IsUnique<DocumentType>(model.Id, model.Name), Times.Once());
			_lookupBll.Verify(x => x.SaveOrUpdateDocType(model.Id, model.Name, model.Description, model.IsPrimary), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void DeleteDocType_has_correct_logic()
		{
			const long docTypeId = 1;
			_lookupBll.Setup(x => x.Delete<DocumentType>(docTypeId));
			_controller.DeleteDocType(docTypeId);
			_lookupBll.Verify(x => x.Delete<DocumentType>(docTypeId), Times.Once());
		}

		[TestMethod]
		public void UnDeleteDocType_has_correct_logic()
		{
			const long docTypeId = 1;
			_lookupBll.Setup(x => x.UnDelete<DocumentType>(docTypeId));
			_controller.UnDeleteDocType(docTypeId);
			_lookupBll.Verify(x => x.UnDelete<DocumentType>(docTypeId), Times.Once());
		}

		[TestMethod]
		public void CreateUpdateElectionType_returns_correct_view()
		{
			var result = _controller.CreateUpdateElectionType((long?)null);

			_lookupBll.Verify(x => x.Get<ElectionType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_UpdateElectionTypePartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.IsInstanceOfType(viewResult.Model, typeof(UpdateLookupModel));
		}

		[TestMethod]
		public void CreateUpdateElectionType_has_correct_logic()
		{
			var electionTypeId = (long?)1;
			_lookupBll.Setup(x => x.Get<ElectionType>(electionTypeId.Value))
				.Returns(new ElectionType { Name = electionTypeId.ToString() });

			var result = _controller.CreateUpdateElectionType(electionTypeId);

			_lookupBll.Verify(x => x.Get<ElectionType>(electionTypeId.Value), Times.Once());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateLookupModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, electionTypeId.ToString());
		}

		[TestMethod]
		public void CreateUpdateElectionType_has_correct_logic_when_NotUnique()
		{
			//var model = new UpdateLookupModel { Id = 1 };
			//_lookupBll.Setup(x => x.IsUnique<ElectionType>(model.Id, model.Name)).Returns(false);

			//var result = _controller.CreateUpdateElectionType(model);

			//_lookupBll.Verify(x => x.IsUnique<ElectionType>(model.Id, model.Name), Times.Once());
			//var viewResult = result as PartialViewResult;
			//Assert.IsNotNull(viewResult);
			//Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			//Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateElectionType_has_correct_logic_when_Unique()
		{
			//var model = new UpdateLookupModel { Id = 1 };
			//_lookupBll.Setup(x => x.IsUnique<ElectionType>(model.Id, model.Name)).Returns(true);
			//_lookupBll.Setup(x => x.SaveOrUpdate<ElectionType>(model.Id, model.Name, model.Description));

			//var result = _controller.CreateUpdateElectionType(model);

			//_lookupBll.Verify(x => x.IsUnique<ElectionType>(model.Id, model.Name), Times.Once());
			//_lookupBll.Verify(x => x.SaveOrUpdate<ElectionType>(model.Id, model.Name, model.Description), Times.Once());
			//var viewResult = result as ContentResult;
			//Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			//Assert.IsNotNull(viewResult);
			//Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void CreateUpdateRegion_returns_correct_view()
		{
			const int parentId = 1;

			_lookupBll.Setup(x => x.GetRegionTypesByFilter(parentId)).Returns(new List<RegionType> { new RegionType { Name = parentId.ToString() } });
			_lookupBll.Setup(x => x.GetAll<Region>()).Returns(new List<Region> { new Region(new RegionType()) { Name = parentId.ToString() } });

			var result = _controller.CreateUpdateRegion(parentId,(long?)null);

			_lookupBll.Verify(x => x.Get<Region>(It.IsAny<long>()), Times.Never());
			_lookupBll.Verify(x => x.GetRegionTypesByFilter(parentId), Times.Once());
			_lookupBll.Verify(x => x.GetAll<Region>(), Times.Once());

			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
            var viewData = viewResult.ViewData;

			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["Parent"]).Any());
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["RegionType"]).Any());
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_CreateRegionPartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.IsInstanceOfType(viewResult.Model, typeof(UpdateRegionModel));
		}

		[TestMethod]
		public void CreateUpdateRegion_has_correct_logic()
		{
			var regionId = (long?)1;
			const long parentId = 1;			
			_lookupBll.Setup(x => x.Get<Region>(regionId.Value))
				.Returns(new Region(new Region(new RegionType()).WithId(parentId),new RegionType()) { Name = regionId.ToString() });
			_lookupBll.Setup(x => x.GetRegionTypesByFilter(parentId))
				.Returns(new List<RegionType> { new RegionType { Name = parentId.ToString()} });
			_lookupBll.Setup(x => x.GetAll<Region>())
				.Returns(new List<Region> { new Region(new RegionType()) { Name = parentId.ToString() } });
			_lookupBll.Setup(x => x.GetAll<RegionType>())
				.Returns(new List<RegionType> { new RegionType { Name = parentId.ToString()}});

			var result = _controller.CreateUpdateRegion(parentId, regionId);

			_lookupBll.Verify(x => x.Get<Region>(regionId.Value), Times.Once());
			_lookupBll.Verify(x => x.GetRegionTypesByFilter(parentId), Times.Once());
			_lookupBll.Verify(x => x.GetAll<Region>(), Times.Once());
			_lookupBll.Verify(x => x.GetAll<RegionType>(), Times.Never());
			
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
            var viewData = viewResult.ViewData;
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["Parent"]).Any());
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["RegionType"]).Any());
			Assert.IsNotNull(viewResult);
			Assert.IsNotNull(viewResult.Model);
			var model = viewResult.Model as UpdateRegionModel;
			Assert.IsNotNull(model);
			Assert.AreEqual(model.Name, regionId.ToString());
		}

		[TestMethod]
		public void CreateUpdateRegion_has_correct_logic_when_NotUnique()
		{
			var model = new UpdateRegionModel { Id = 1, Parent =  1};
			const long parentId = 1;	

			_lookupBll.Setup(x => x.IsUnique(model.Id, model.Name, model.Parent, model.RegionType)).Returns(false);
			_lookupBll.Setup(x => x.GetRegionTypesByFilter(parentId))
				.Returns(new List<RegionType> { new RegionType { Name = parentId.ToString() } });
			_lookupBll.Setup(x => x.GetAll<Region>())
				.Returns(new List<Region> { new Region(new RegionType()) { Name = parentId.ToString() } });
			_lookupBll.Setup(x => x.GetAll<RegionType>())
				.Returns(new List<RegionType> { new RegionType { Name = parentId.ToString() } });

			var result = _controller.CreateUpdateRegion(model);

			_lookupBll.Verify(x => x.IsUnique(model.Id, model.Name, model.Parent, model.RegionType), Times.Once());

			_lookupBll.Verify(x => x.GetRegionTypesByFilter(parentId), Times.Once());
			_lookupBll.Verify(x => x.GetAll<Region>(), Times.Once());
			_lookupBll.Verify(x => x.GetAll<RegionType>(), Times.Never());

			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
            var viewData = viewResult.ViewData;
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["Parent"]).Any());
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["RegionType"]).Any());
			Assert.IsNotNull(viewResult);
			Assert.IsFalse(_controller.ViewData.ModelState.IsValid);
			Assert.IsTrue(_controller.ViewData.ModelState[""].Errors.Count > 0);
		}

		[TestMethod]
		public void CreateUpdateRegion_has_correct_logic_when_Unique()
		{
			var model = new UpdateRegionModel { Id = 1, Parent = 1 };
			const long parentId = 1;	
			_lookupBll.Setup(x => x.IsUnique(model.Id, model.Name, model.Parent, model.RegionType)).Returns(true);
			_lookupBll.Setup(x => x.CreateUpdateRegion(model.Id, model.Name, model.Description, model.Parent, model.RegionType,
					model.HasStreets, model.SaiseId, model.Cuatm));

			var result = _controller.CreateUpdateRegion(model);

			_lookupBll.Verify(x => x.IsUnique(model.Id, model.Name, model.Parent, model.RegionType), Times.Once());
			_lookupBll.Verify(x => x.CreateUpdateRegion(model.Id, model.Name, model.Description, model.Parent, model.RegionType,
					model.HasStreets, model.SaiseId, model.Cuatm), Times.Once());

			_lookupBll.Verify(x => x.GetRegionTypesByFilter(parentId), Times.Never());
			_lookupBll.Verify(x => x.GetAll<Region>(), Times.Never());
			_lookupBll.Verify(x => x.GetAll<RegionType>(), Times.Never());
			
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		[TestMethod]
		public void UpdateAdministrativeInfo_returns_correct_view()
		{
			const int regionId = 1;
			_lookupBll.Setup(x => x.GetPublicAdministration(regionId))
				.Returns(new PublicAdministration(new Region(new RegionType()), new ManagerType()) { Name = regionId.ToString() });
			_lookupBll.Setup(x => x.GetAll<ManagerType>())
				.Returns(new List<ManagerType> { new ManagerType { Name = regionId.ToString() } });

			var result = _controller.UpdateAdministrativeInfo(regionId);

			_lookupBll.Verify(x => x.Get<ManagerType>(It.IsAny<long>()), Times.Never());
			var viewResult = result as PartialViewResult;
			Assert.IsNotNull(viewResult);
            var viewData = viewResult.ViewData;
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_PublicAdministrativeInfoPartial");
			Assert.IsNotNull(viewResult.Model);
			Assert.IsInstanceOfType(viewResult.Model, typeof(UpdatePublicAdministrationModel));
			Assert.IsTrue(((IEnumerable<SelectListItem>)viewData["ManagerTypeId"]).Any());
		}

		[TestMethod]
		public void UpdateAdministrativeInfo_has_correct_logic()
		{
			const long administrativeInfoId = 1;
			var model = new UpdatePublicAdministrationModel { Id = 1 };

			_lookupBll.Setup(x => x.UpdateAdministrativeInfo(model.Id, model.Name, model.Surname, model.RegionId, model.ManagerTypeId));
			_lookupBll.Setup(x => x.GetAll<ManagerType>())
				.Returns(new List<ManagerType> { new ManagerType { Name = administrativeInfoId.ToString() } });

			var result = _controller.UpdateAdministrativeInfo(model);

			_lookupBll.Verify(x => x.UpdateAdministrativeInfo(model.Id, model.Name, model.Surname, model.RegionId, model.ManagerTypeId), Times.Once());
			_lookupBll.Verify(x => x.GetAll<ManagerType>(), Times.Once());
			var viewResult = result as ContentResult;
			Assert.IsTrue(_controller.ViewData.ModelState.IsValid);
			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.Content, Const.CloseWindowContent);
		}

		public void TestListAjax<T>(ActionResult result) where T : Entity
		{
			_lookupBll.Verify(x => x.Get<T>(It.IsAny<PageRequest>()), Times.Once());
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
		}

		[TestMethod]
		public void ListManagerTypesAjax_returns_correct_format()
		{
			var request = new JqGridRequest {PageIndex = 0, RecordsCount =  20};
			_lookupBll.Setup(x => x.Get<ManagerType>(It.IsAny<PageRequest>()))
				.Returns(new PageResponse<ManagerType> { Items = new List<ManagerType>(), PageSize = 20, StartIndex = 1, Total = 2 });
			
			var result = _controller.ListManagerTypesAjax(request);
			TestListAjax<ManagerType>(result);
		}

		[TestMethod]
		public void ListStreetTypeAjax_returns_correct_format()
		{
			var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
			_loockupService.Setup(x => x.Exec(It.IsAny<Expression<Func<ILookupBll, PageResponse<StreetType>>>>()))
				.Returns(new Responser<PageResponse<StreetType>> { Model = new PageResponse<StreetType> { Items = new List<StreetType>(), PageSize = 20, StartIndex = 1, Total = 2 } });

			var result = _controller.ListStreetTypeAjax(request);
			_loockupService.Verify(x => x.Exec(It.IsAny<Expression<Func<ILookupBll, PageResponse<StreetType>>>>()), Times.Once());
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void ListGendersAjax_returns_correct_format()
		{
			var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
			_lookupBll.Setup(x => x.Get<Gender>(It.IsAny<PageRequest>()))
				.Returns(new PageResponse<Gender> { Items = new List<Gender>(), PageSize = 20, StartIndex = 1, Total = 2 });

			var result = _controller.ListGendersAjax(request);
			TestListAjax<Gender>(result);
		}

		[TestMethod]
		public void ListRegionTypesAjax_returns_correct_format()
		{
			var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
			_lookupBll.Setup(x => x.Get<RegionType>(It.IsAny<PageRequest>()))
				.Returns(new PageResponse<RegionType> { Items = new List<RegionType>(), PageSize = 20, StartIndex = 1, Total = 2 });

			var result = _controller.ListRegionTypesAjax(request);
			TestListAjax<RegionType>(result);
		}

		[TestMethod]
		public void ListPersonStatusAjax_returns_correct_format()
		{
			var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
			_lookupBll.Setup(x => x.Get<PersonStatusType>(It.IsAny<PageRequest>()))
				.Returns(new PageResponse<PersonStatusType> { Items = new List<PersonStatusType>(), PageSize = 20, StartIndex = 1, Total = 2 });

			var result = _controller.ListPersonStatusAjax(request);
			TestListAjax<PersonStatusType>(result);
		}

		[TestMethod]
		public void ListDocTypesAjax_returns_correct_format()
		{
			var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
			_lookupBll.Setup(x => x.Get<DocumentType>(It.IsAny<PageRequest>()))
				.Returns(new PageResponse<DocumentType> { Items = new List<DocumentType>(), PageSize = 20, StartIndex = 1, Total = 2 });

			var result = _controller.ListDocTypesAjax(request);
			TestListAjax<DocumentType>(result);
		}

		[TestMethod]
		public void ListTreeViewRegionsAjax_returns_correct_format()
		{
			var nodeid = (int?) 1;
			_lookupBll.Setup(x => x.GetLocalities(nodeid)).Returns(new List<Region>());

			var result = _controller.ListTreeViewRegionsAjax(nodeid);
			_lookupBll.Verify(x => x.GetLocalities(nodeid), Times.Once());
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
		}

        [TestMethod]
        public void ListStreetsAjax_returns_correct_format()
        {
            var regionId = (long?)1;
            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
            _lookupBll.Setup(x => x.GetStreets(It.IsAny<PageRequest>(), regionId.GetValueOrDefault()))
                .Returns(new PageResponse<StreetDto> { Items = new List<StreetDto>(), PageSize = 20, StartIndex = 1, Total = 2 });

            var result = _controller.ListStreetsAjax(request, regionId);
            _lookupBll.Verify(x => x.GetStreets(It.IsAny<PageRequest>(), regionId.GetValueOrDefault()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

		[TestMethod]
		public void ListRegionsTreeAjax_returns_correct_format()
		{
			var parentId = (long?)1;
			var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };
			_lookupBll.Setup(x => x.GetRegions(It.IsAny<PageRequest>(), parentId.GetValueOrDefault()))
				.Returns(new PageResponse<RegionRow> { Items = new List<RegionRow>(), PageSize = 20, StartIndex = 1, Total = 2 });

			var result = _controller.ListRegionsTreeAjax(request, parentId);
			_lookupBll.Verify(x => x.GetRegions(It.IsAny<PageRequest>(), parentId.GetValueOrDefault()), Times.Once());
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
		}

		[TestMethod]
		public void SelectRegionType_returns_correct_format_with_null_param()
		{
			var regionId = (long?) 1;
			_lookupBll.Setup(x => x.GetAll<RegionType>())
				.Returns(new List<RegionType> { new RegionType { Name = regionId.ToString()} });

			var result = _controller.SelectRegionType((long?)null);
			_lookupBll.Verify(x => x.GetAll<RegionType>(), Times.Once());
			_lookupBll.Verify(x => x.GetRegionTypesByFilter((long)regionId), Times.Never());

			var viewResult = result as PartialViewResult;

			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_Select");
			Assert.IsNotNull(viewResult.Model);
		}

		[TestMethod]
		public void SelectRegionType_returns_correct_format_with_not_null_param()
		{
			var regionId = (long?)1;
			_lookupBll.Setup(x => x.GetRegionTypesByFilter((long)regionId))
				.Returns(new List<RegionType> { new RegionType { Name = regionId.ToString() } });

			var result = _controller.SelectRegionType(regionId);
			_lookupBll.Verify(x => x.GetAll<RegionType>(), Times.Never());
			_lookupBll.Verify(x => x.GetRegionTypesByFilter((long)regionId), Times.Once());

			var viewResult = result as PartialViewResult;

			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_Select");
			Assert.IsNotNull(viewResult.Model);
		}

		[TestMethod]
		public void SelectStreetType_returns_correct_format()
		{
			var streetTypesId = (long?)1;
			_lookupBll.Setup(x => x.GetAll<StreetType>())
				.Returns(new List<StreetType> { new StreetType { Name = streetTypesId.ToString() } });

			var result = _controller.SelectStreetType();
			_lookupBll.Verify(x => x.GetAll<StreetType>(), Times.Once());

			var viewResult = result as PartialViewResult;

			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_Select");
			Assert.IsNotNull(viewResult.Model);
		}

		[TestMethod]
		public void SelectYesNo_returns_correct_format()
		{
			var result = _controller.SelectYesNo();
			var viewResult = result as PartialViewResult;

			Assert.IsNotNull(viewResult);
			Assert.AreEqual(viewResult.ViewName, "_Select");
			Assert.IsNotNull(viewResult.Model);
		}

	    [TestMethod]
	    public void ExportElectionTypesAllData_has_correct_logic()
	    {
            ExportGridDataTest<LookupGridModel>(ExportType.AllData, "ElectionTypes");
	    }

        [TestMethod]
        public void ExportElectionTypesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.CurrentPage, "ElectionTypes");
        }

        [TestMethod]
        public void ExportPersonAddressTypesAllData_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.AllData, "PersonAddressTypes");
        }

        [TestMethod]
        public void ExportPersonAddressTypesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.CurrentPage, "PersonAddressTypes");
        }

        [TestMethod]
        public void ExportStreetTypesAllData_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.AllData, "StreetTypes");
        }

        [TestMethod]
        public void ExportStreetTypesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.CurrentPage, "StreetTypes");
        }

        [TestMethod]
        public void ExportManagerTypesAllData_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.AllData, "ManagerTypes");
        }

        [TestMethod]
        public void ExportManagerTypesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.CurrentPage, "ManagerTypes");
        }

        [TestMethod]
        public void ExportCircumscriptionsAllData_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.AllData, "Circumscriptions");
        }

        [TestMethod]
        public void ExportCircumscriptionsCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.CurrentPage, "Circumscriptions");
        }

        [TestMethod]
        public void ExportGendersAllData_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.AllData, "Genders");
        }

        [TestMethod]
        public void ExportGendersCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<LookupGridModel>(ExportType.CurrentPage, "Genders");
        }

        [TestMethod]
        public void ExportRegionTypesAllData_has_correct_logic()
        {
            ExportGridDataTest<RegionTypesGridModel>(ExportType.AllData, "RegionTypes");
        }

        [TestMethod]
        public void ExportRegionTypesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<RegionTypesGridModel>(ExportType.CurrentPage, "RegionTypes");
        }

        [TestMethod]
        public void ExportPersonStatusesAllData_has_correct_logic()
        {
            ExportGridDataTest<PersonStatusGridModel>(ExportType.AllData, "PersonStatuses");
        }

        [TestMethod]
        public void ExportPersonStatusesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<PersonStatusGridModel>(ExportType.CurrentPage, "PersonStatuses");
        }

        [TestMethod]
        public void ExportDocTypesAllData_has_correct_logic()
        {
            ExportGridDataTest<DocumentTypeGridModel>(ExportType.AllData, "DocTypes");
        }

        [TestMethod]
        public void ExportDocTypesCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<DocumentTypeGridModel>(ExportType.CurrentPage, "DocTypes");
        }

        [TestMethod]
        public void ExportStreetsAllData_has_correct_logic()
        {
            ExportGridDataTest<StreetsGridModel>(ExportType.AllData, "Streets");
        }

        [TestMethod]
        public void ExportStreetsCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<StreetsGridModel>(ExportType.CurrentPage, "Streets");
        }

        [TestMethod]
        public void ExportRegionsAllData_has_correct_logic()
        {
			ExportGridDataTest<RegionTreeViewModel>(ExportType.AllData, "Regions");
        }

        [TestMethod]
        public void ExportRegionsCurrentPage_has_correct_logic()
        {
			ExportGridDataTest<RegionTreeViewModel>(ExportType.CurrentPage, "Regions");
        }
	}
}
