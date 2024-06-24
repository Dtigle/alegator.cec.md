using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class VotersListOrderTypeMap : IAutoMappingOverride<VotersListOrderType>
    {
        public void Override(AutoMapping<VotersListOrderType> mapping)
        {
            mapping.Schema(Schemas.Lookup);
            mapping.Id(x => x.Id, "votersListOrderTypeId");
        }
    }
}
