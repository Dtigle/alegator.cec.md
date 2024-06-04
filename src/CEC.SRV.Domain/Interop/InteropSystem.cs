
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.Interop
{
    public class InteropSystem:Lookup.Lookup
    {
        public virtual TransactionProcessingTypes TransactionProcessingType { get; set; }

        public virtual bool PersonStatusConsignment { get; set; }

        public virtual PersonStatusType PersonStatusType { get; set; }

        public virtual bool TemporaryAddressConsignment { get; set; }
    }
}
