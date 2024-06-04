using Amdaris.Domain;

namespace CEC.SRV.Domain.Lookup
{
    public abstract class Classifier : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Namerus { get; set; }
    }
}