using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Notification;
using CEC.Web.SRV.Models.Voters;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Controllers
{
	[Authorize]
	public class NotificationController : BaseController
	{
		private readonly INotificationBll _bll;

		public NotificationController(INotificationBll bll)
		{
			_bll = bll;
		}

		[Authorize(Roles = Transactions.Notification)]
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult SelectNotificationType()
		{
			var notificationType = _bll.GetNotificationType();
			return PartialView("_Select", notificationType.ToSelectListUnencrypted(0, false, null, x => x, x => x));
		}

		public JqGridJsonResult ListNotificationAjax(JqGridRequest request)
		{
			var pageRequest = request.ToPageRequest<NotificationGridModel>();

			var data = _bll.GetNotificationReceiverList(pageRequest);

			return data.ToJqGridJsonResult<NotificationReceiver, NotificationGridModel>();
		}

        [HttpPost]
        public ActionResult ExportNotifications(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "Notifications", 
                typeof (NotificationGridModel), ListNotificationAjax);
        }

	    [HttpPost]
		public ActionResult GetCountNotification()
		{
			var notificationCount = _bll.CountUnReadNotifications();
			return Json(notificationCount);
		}

		[HttpPost]
		public void UpdateIsRead(long notificationId)
		{
			_bll.UpdateIsRead(notificationId);
		}
	}
}