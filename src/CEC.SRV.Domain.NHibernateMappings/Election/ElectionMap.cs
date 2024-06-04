using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ElectionMap : IAutoMappingOverride<Election>
    {
        public void Override(AutoMapping<Election> mapping)
        {
            mapping.Schema(Schemas.RSA);
            mapping.References(x => x.ElectionType).Not.Nullable();
            //mapping.References(x => x.ElectionRounds).Nullable();
            mapping.HasMany(x => x.ElectionRounds).Cascade.SaveUpdate().LazyLoad();
            mapping.Map(x => x.Status).CustomType<ElectionStatus>().Not.Nullable();
        }
    }
}
