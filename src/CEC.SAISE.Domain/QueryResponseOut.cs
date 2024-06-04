using Amdaris.Domain;

namespace CEC.SAISE.Domain
{
    public class QueryResponseOut : Entity
    {
        public virtual string ExecStatus { get; set; }

        public virtual string ExecMsg { get; set; }
    }
}
