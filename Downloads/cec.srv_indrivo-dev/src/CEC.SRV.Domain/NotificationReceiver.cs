
namespace CEC.SRV.Domain
{
    public class NotificationReceiver : SRVBaseEntity
    {
        private readonly Notification _notification;
        private readonly SRVIdentityUser _user;

        public NotificationReceiver()
        {
        }

        public NotificationReceiver(Notification notification, SRVIdentityUser user)
        {
            _notification = notification;
            _user = user;
        }

        public virtual Notification Notification
        {
            get { return _notification; }
        }

        public virtual SRVIdentityUser User
        {
            get { return _user; }
        }

        public virtual bool NotificationIsRead { get; set; }
    }
}