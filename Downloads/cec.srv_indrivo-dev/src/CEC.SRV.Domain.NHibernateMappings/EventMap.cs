
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class EventMap : IAutoMappingOverride<Event>
    {
        public void Override(AutoMapping<Event> mapping)
        {
            mapping.Map(x => x.EventType).Not.Nullable().Access.CamelCaseField(Prefix.Underscore)
                .CustomType<EnumType<EventTypes>>();
            mapping.Map(x => x.EntityType).Not.Nullable().Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
