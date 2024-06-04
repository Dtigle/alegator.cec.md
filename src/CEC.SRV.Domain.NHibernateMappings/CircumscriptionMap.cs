using CEC.SRV.Domain.Lookup;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class CircumscriptionMap : IAutoMappingOverride<Circumscription>
    {
        public void Override(AutoMapping<Circumscription> mapping)
        {
            mapping.Schema(Schemas.Lookup);

            mapping.References(x => x.Region).Not.Nullable();
            mapping.References(x => x.CircumscriptionList).Column("circumscriptionListId").Not.Nullable();
        }
    }
}
