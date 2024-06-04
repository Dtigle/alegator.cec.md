using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Prefix = FluentNHibernate.Mapping.Prefix;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RegionMap : IAutoMappingOverride<Region>
    {
        public void Override(AutoMapping<Region> mapping)
        {
            mapping.References(x => x.Parent).Nullable()
                .Not.Update()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.References(x => x.RegionType).Not.Nullable();
            mapping.HasMany(x => x.Children)
                .ExtraLazyLoad()
                .KeyColumn("parentId")
                .Access.CamelCaseField(Prefix.Underscore);
                
            mapping.Map(x => x.RegistruId).Nullable();
            mapping.Map(x => x.SaiseId).Nullable();

            mapping.Map(x => x.HasStreets).Not.Nullable().Default("0");
            
            mapping.Component(x => x.GeoLocation).ColumnPrefix("geo");

            mapping.IgnoreProperty(x => x.Level);

            mapping.References(x => x.PublicAdministration).Nullable();
        }
    }
}