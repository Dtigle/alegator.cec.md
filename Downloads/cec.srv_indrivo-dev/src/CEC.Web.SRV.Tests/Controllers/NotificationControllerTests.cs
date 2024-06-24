using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CEC.SRV.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using AutoMapper;
using Moq;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.Notification;
using CEC.Web.SRV.Infrastructure.Export;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class NotificationControllerTests : BaseControllerTests
    {
        private static Mock<INotificationBll> _bll;
        private static NotificationController _controller;
        
        public NotificationControllerTests()
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
            _bll = new Mock<INotificationBll>();
            _controller = new NotificationController(_bll.Object);
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
        public void SelectNotificationType_returns_correct_view()
        {
            // Arrange

            const string viewName = "_Select";

            var notificationType = new List<string> { "NotificationType2", "NotificationType1"};

            _bll.Setup(x => x.GetNotificationType()).Returns(notificationType);

            // Act
            
            var result = _controller.SelectNotificationType() as PartialViewResult;

            // Assert
            
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(List<SelectListItem>));

            var model = result.Model as List<SelectListItem>;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.Count, notificationType.Count);
            Assert.IsTrue(model.All(item => notificationType.Exists(x => string.Equals(x, item.Text) && string.Equals(x, item.Value))));
        }
        
        [TestMethod]
        public void ListNotificationAjax_returns_correct_format()
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.GetNotificationReceiverList(It.IsAny<PageRequest>()))
                .Returns(new PageResponse<NotificationReceiver> { Items = new List<NotificationReceiver>(), PageSize = 20, StartIndex = 1, Total = 2 });
  
            // Act

            var result = _controller.ListNotificationAjax(request);

            // Assert

            _bll.Verify(x => x.GetNotificationReceiverList(It.IsAny<PageRequest>()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        [TestMethod]
        public void GetCountNotification_returns_correct_view()
        {
            // Arrange

            const int notificationCount = 100;

            _bll.Setup(x => x.CountUnReadNotifications()).Returns(notificationCount);

            // Act

            var result = _controller.GetCountNotification();

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.AreEqual(notificationCount, ((JsonResult)result).Data);
        }

        [TestMethod]
        public void UpdateIsRead_has_correct_logic()
        {
            // Arrange

            const long notificationId = 1;

            _bll.Setup(x => x.UpdateIsRead(notificationId));
            
            // Act

            _controller.UpdateIsRead(notificationId);

            // Assert

            _bll.Verify(x => x.UpdateIsRead(notificationId), Times.Once());
        }

        [TestMethod]
        public void ExportNotificationsAllData_has_correct_logic()
        {
            ExportGridDataTest<NotificationGridModel>(ExportType.AllData, "Notifications");
        }

        [TestMethod]
        public void ExportNotificationsCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<NotificationGridModel>(ExportType.CurrentPage, "Notifications");
        }
    }
}

