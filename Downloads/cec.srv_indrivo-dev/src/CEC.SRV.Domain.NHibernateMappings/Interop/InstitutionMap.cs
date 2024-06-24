using CEC.SRV.Domain.Interop;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings.Interop
{
    public class InstitutionMap : IAutoMappingOverride<Institution>
    {
        public void Override(AutoMapping<Institution> mapping)
        {
            mapping.Schema(Schemas.Interop);
            mapping.References(x => x.InteropSystem);
            mapping.References(x => x.InstitutionAddress);
        }
    }
}
