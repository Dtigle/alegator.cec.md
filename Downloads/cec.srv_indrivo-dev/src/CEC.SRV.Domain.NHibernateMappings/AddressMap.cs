using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class AddressMap : IAutoMappingOverride<Address>
    {
        public void Override(AutoMapping<Address> mapping)
        {
            mapping.References(x => x.Street)
                .ForeignKey("FK_Addresses_Streets_Addresses").Not.Nullable();
			mapping.Component(x => x.GeoLocation).ColumnPrefix("geo");
            mapping.References(x => x.PollingStation).Nullable();
            mapping.Map(x => x.BuildingType).CustomType<EnumType<BuildingTypes>>();
        }

    }
}