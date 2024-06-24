using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ElectionTypeMap : IAutoMappingOverride<ElectionType>
    {
        public void Override(AutoMapping<ElectionType> mapping)
        {
            mapping.Schema(Schemas.Lookup);
            mapping.Map(x => x.ElectionCompetitorType).CustomType<ElectionCompetitorType>().Not.Nullable();
            mapping.Map(x => x.ElectionArea).CustomType<ElectionArea>().Not.Nullable();
            mapping.References(x => x.CircumscriptionList).Nullable();
        }
    }
}
