using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ElectionRoundMap : IAutoMappingOverride<ElectionRound>
    {
        public void Override(AutoMapping<ElectionRound> mapping)
        {
            mapping.Schema(Schemas.RSA);
            mapping.References(x => x.Election).Cascade.SaveUpdate();
            mapping.Map(x => x.Status).CustomType<ElectionRoundStatus>().Not.Nullable();
        }
    }
}
