using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class CircumscriptionListMap : IAutoMappingOverride<CircumscriptionList>
    {
        public void Override(AutoMapping<CircumscriptionList> mapping)
        {
            mapping.Schema(Schemas.Lookup);
        }
    }
}
