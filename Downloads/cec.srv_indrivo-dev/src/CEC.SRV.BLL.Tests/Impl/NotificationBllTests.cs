using System.Linq;
using CEC.SRV.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using CEC.SRV.BLL.Repositories;


namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class NotificationBllTests : BaseBllTests
    {
        private NotificationBll _bll;

        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<NotificationBll>();
        }

        [TestMethod]
        public void GetNotificationType_returns_correct_result()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, NotificationBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetNotificationReceiver);

            var expectedTypes = GetAllObjectsFromDbTable<NotificationReceiver>(
                                x => (x.User != null) && (x.Notification != null) && (x.Notification.Event != null) &&
                                (x.User.Id == SecurityHelper.GetLoggedUserId()))
                                .Select(x => x.Notification.Event.EntityType).ToList();
            
            // Act

            var types = SafeExecFunc(_bll.GetNotificationType);

            // Assert

            Assert.IsNotNull(types);
            AssertListsAreEqual(expectedTypes, types.ToList(), x => x, x => x);
        }

        [TestMethod]
        public void CountUnReadNotifications_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetNotificationReceiver);

            var expectedCount = GetDbTableCount<NotificationReceiver>(
                                x => (x.User != null) && (x.User.Id == SecurityHelper.GetLoggedUserId()) && (!x.NotificationIsRead));

            // Act

            var count = (long)SafeExecFunc(_bll.CountUnReadNotifications);

            // Assert

            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void GetNotificationReceiverList_returns_correct_page()
        {
            // Arrange

            _bll = InitializeRepositoryAndBll<SrvRepository, NotificationBll>();

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetNotificationReceiver);

            var expectedNotificationReceivers = GetAllIdsFromDbTable<NotificationReceiver>(
                                x => (x.User != null) && (x.Notification != null) && (x.Notification.Event != null) &&
                                (x.User.Id == SecurityHelper.GetLoggedUserId()));

            // Act & Assert

            ActAndAssertAllPages(_bll.GetNotificationReceiverList, expectedNotificationReceivers);
        }

        [TestMethod]
        public void UpdateIsRead_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var notificationReceiver = GetFirstObjectFromDbTable(GetNotificationReceiver);
            var notificationReceiverId = notificationReceiver.Id;

            // Act

            SafeExec(_bll.UpdateIsRead, notificationReceiverId);
            
            // Assert

            var newNotificationReceiver =
                GetFirstObjectFromDbTable<NotificationReceiver>(x => x.Id == notificationReceiverId);

            Assert.IsTrue(newNotificationReceiver.NotificationIsRead);
        }
    }
}
