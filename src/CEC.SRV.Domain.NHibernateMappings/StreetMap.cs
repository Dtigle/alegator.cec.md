using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class StreetMap : IAutoMappingOverride<Street>
    {
        public void Override(AutoMapping<Street> mapping)
        {
            mapping.References(x => x.Region)
                .LazyLoad()
                .Not.Nullable()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.References(x => x.StreetType).Not.Nullable();

            mapping.HasMany(x => x.Addresses).KeyColumn("addressId")
                .ExtraLazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.Map(x => x.RopId).Nullable();
            mapping.Map(x => x.SaiseId).Nullable();

        }
    }
}