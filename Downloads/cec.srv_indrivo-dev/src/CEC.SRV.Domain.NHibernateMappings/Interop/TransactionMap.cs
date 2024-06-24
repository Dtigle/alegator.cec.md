using CEC.SRV.Domain.Interop;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings.Interop
{
    public class TransactionMap : IAutoMappingOverride<CEC.SRV.Domain.Interop.Transaction>
    {
        public void Override(AutoMapping<CEC.SRV.Domain.Interop.Transaction> mapping)
        {
            mapping.Schema(Schemas.Interop);
            mapping.References(x => x.InteropSystem);
            mapping.References(x => x.Institution);
            mapping.Map(x => x.TransactionStatus).CustomType<EnumType<TransactionStatus>>().Not.Nullable();
            mapping.IgnoreProperty(x=>x.DateOfBirth);
        }
    }
}
 