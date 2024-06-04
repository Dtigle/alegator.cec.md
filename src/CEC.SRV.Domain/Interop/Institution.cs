
namespace CEC.SRV.Domain.Interop
{
    public class Institution : Lookup.Lookup
    {
        public virtual long? LegacyId { get; set; }

        public virtual InteropSystem InteropSystem { get; set; }

        public virtual Address InstitutionAddress { get; set; }        
    }
}