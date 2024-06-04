using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class NotificationMap : IAutoMappingOverride<Notification>
    {
        public void Override(AutoMapping<Notification> mapping)
        {
            mapping.Map(x => x.Message).Column("messageBody").Not.Nullable().Access.CamelCaseField(Prefix.Underscore);

            mapping.HasMany(x => x.Receivers)
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.All()
                .ExtraLazyLoad()
                .ForeignKeyConstraintName("FK_NotificationReceivers_Notifications_notificationId");

            mapping.References(x => x.Event).Not.Nullable().Access.CamelCaseField(Prefix.Underscore).Cascade.All();
			mapping.Map(x => x.CreateDate).Formula("CONVERT(datetime, Created)").Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
