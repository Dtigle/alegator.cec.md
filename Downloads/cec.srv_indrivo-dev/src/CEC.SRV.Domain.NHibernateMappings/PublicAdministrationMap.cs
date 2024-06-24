using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PublicAdministrationMap : IAutoMappingOverride<PublicAdministration>
    {
        public void Override(AutoMapping<PublicAdministration> mapping)
        {
            mapping.References(x => x.Region)
                .LazyLoad()
                .Not.Nullable()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.References(x => x.ManagerType).Not.Nullable();
            mapping.Map(x => x.Name).Nullable();
            mapping.Map(x => x.Surname).Nullable();
        }
    }
}