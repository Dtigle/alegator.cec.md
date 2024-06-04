
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class NotificationReceiverMap : IAutoMappingOverride<NotificationReceiver>
    {
        public void Override(AutoMapping<NotificationReceiver> mapping)
        {
            mapping.References(x => x.Notification).Not.Nullable().ForeignKey("FK_NotificationReceivers_Notifications_notificationId");
            mapping.References(x => x.User).Column("identityUserId").Not.Nullable().ForeignKey("FK_NotificationReceivers_SRVIdentityUsers_identityUserId");

            mapping.Map(x => x.NotificationIsRead).Not.Nullable().Default("0");
        }
    }
}
