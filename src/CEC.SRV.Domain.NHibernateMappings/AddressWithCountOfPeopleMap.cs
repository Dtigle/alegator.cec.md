using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
	public class AddressWithCountOfPeopleMap : IAutoMappingOverride<AddressWithCountOfPeople>
    {
		public void Override(AutoMapping<AddressWithCountOfPeople> mapping)
        {
            mapping.ReadOnly();
			mapping.Id(x => x.Id, "addressId");
			mapping.Table("v_AddressWithCountOfPeople");
            mapping.Schema(Schemas.RSA);
        }
    }
}