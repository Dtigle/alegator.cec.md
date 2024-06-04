
using System;
using Amdaris.Domain;

namespace CEC.SRV.Domain
{
    [Obsolete]
    public class PersonByConflict : Entity
    {
        public virtual Person Person { get; set; }
    }
}
