using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CEC.SRV.Domain
{
    public class Notification : SRVBaseEntity
    {
        private readonly IList<NotificationReceiver> _receivers;
        private readonly Event _event;
        private readonly string _message;
        private readonly DateTime _createDate;

        public Notification()
        {
            _receivers = new List<NotificationReceiver>();
        }

        public Notification(Event @event, string message)
        {
            _receivers = new List<NotificationReceiver>();
            _event = @event;
            _message = message;
        }

        public virtual string Message
        {
            get { return _message; }
        }

        public virtual Event Event
        {
            get { return _event; }
        }

        public virtual IReadOnlyCollection<NotificationReceiver> Receivers
        {
            get
            {
                return new ReadOnlyCollection<NotificationReceiver>(_receivers);
            }
        }

        public virtual void AddReceiver(SRVIdentityUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (_receivers.Any(x => x.User.Id == user.Id))
            {
                return;
            }

            _receivers.Add(new NotificationReceiver(this, user));
        }

        public virtual void AddReceivers(IEnumerable<SRVIdentityUser> users)
        {
            foreach (var user in users)
            {
                AddReceiver(user);
            }
        }

		public virtual DateTime CreateDate
		{
			get { return _createDate; }
		}
    }
}
