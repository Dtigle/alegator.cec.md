using System.Collections.Generic;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL
{
    public interface INotificationBll : IBll
    {
		IList<string> GetNotificationType();
		PageResponse<NotificationReceiver> GetNotificationReceiverList(PageRequest pageRequest);
		int CountUnReadNotifications();
		void UpdateIsRead(long notificationId);
    }
}