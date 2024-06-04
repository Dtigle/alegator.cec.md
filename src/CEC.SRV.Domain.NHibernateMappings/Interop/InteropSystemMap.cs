using CEC.SRV.Domain.Interop;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings.Interop
{
    public class InteropSystemMap : IAutoMappingOverride<InteropSystem>
    {
        public void Override(AutoMapping<InteropSystem> mapping)
        {
            mapping.Schema(Schemas.Interop);
            mapping.Map(x => x.TransactionProcessingType).CustomType<EnumType<TransactionProcessingTypes>>().Not.Nullable();
        }
    }
}
