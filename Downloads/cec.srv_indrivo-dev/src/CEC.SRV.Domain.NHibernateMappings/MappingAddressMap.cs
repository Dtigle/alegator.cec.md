using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class MappingAddressMap : IAutoMappingOverride<MappingAddress>
    {
        public void Override(AutoMapping<MappingAddress> mapping)
        {
            mapping.Schema(Schemas.Importer);
            mapping.Map(x => x.SrvAddressId).Not.Nullable();
            mapping.Map(x => x.RspAdministrativeCode).Not.Nullable();
            mapping.Map(x => x.RspStreetCode).Not.Nullable();
            mapping.Map(x => x.RspHouseNr);
            mapping.Map(x => x.RspHouseSuf);
        }
    }
}