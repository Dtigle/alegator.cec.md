using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class LinkedRegionsFullNameMap : IAutoMappingOverride<LinkedRegionsFullName>
    {
        public void Override(AutoMapping<LinkedRegionsFullName> mapping)
        {
            mapping.ReadOnly();
            mapping.Id(x => x.Id, "regionId");
            mapping.Table("v_LinkedRegions");
            mapping.Schema(Schemas.RSA);
        }
    }
}