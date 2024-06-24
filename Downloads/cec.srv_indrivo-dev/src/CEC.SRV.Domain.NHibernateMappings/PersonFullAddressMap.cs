using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PersonFullAddressMap : IAutoMappingOverride<PersonFullAddress>
    {
        public void Override(AutoMapping<PersonFullAddress> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "personAddressId");
            mapping.Table("PersonFullAddress");
            mapping.Schema(Schemas.RSA);

            mapping.References(x => x.AssignedUser).Column("regionId")
                .ReadOnly()
                .PropertyRef(x => x.Region);

        }
    }
}