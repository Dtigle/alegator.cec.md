
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using NHibernate.Criterion;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Transform;
using NHibernate.Type;

namespace CEC.SRV.BLL.Impl
{
    public class NotificationBll : Bll, INotificationBll
    {
        public NotificationBll(ISRVRepository repository): base(repository)
        {
        }

	    public IList<string> GetNotificationType()
	    {
			var userLoggedId = SecurityHelper.GetLoggedUserId();
		    Notification notification = null;
		    Event @event = null;
		    return Repository.QueryOver<NotificationReceiver>()
			    .JoinAlias(x => x.Notification, () => notification)
			    .JoinAlias(() => notification.Event, () => @event)
			    .Where(x => x.User.Id == userLoggedId)
				.SelectList(list => list
					.Select(Projections.Distinct(Projections.Property(() => @event.EntityType))))
			    .List<string>();
	    }

		public int CountUnReadNotifications()
		{
			var userLoggedId = SecurityHelper.GetLoggedUserId();
			return Repository.Query<NotificationReceiver>()
				.Where(x => x.User.Id == userLoggedId && !x.NotificationIsRead)
				.Count();
		}

		public void UpdateIsRead(long notificationId)
		{
			var entity = Get<NotificationReceiver>(notificationId);
			entity.NotificationIsRead = true;
			Repository.SaveOrUpdate(entity);
		}

		public PageResponse<NotificationReceiver> GetNotificationReceiverList(PageRequest pageRequest)
		{
			var userLoggedId = SecurityHelper.GetLoggedUserId();
			return Repository.QueryOver<NotificationReceiver>().Where(x=>x.User.Id==userLoggedId).OrderBy(c => c.Created).Desc
					.TransformUsing(Transformers.DistinctRootEntity)
					.RootCriteria
					.CreatePage<NotificationReceiver>(pageRequest);
		}
    }
}