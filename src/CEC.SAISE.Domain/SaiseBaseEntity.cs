using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SAISE.Domain
{
    public abstract class SaiseBaseEntity : Entity 
    {
        public virtual SystemUser EditUser { get; set; }

        public virtual DateTime EditDate { get; set; }

        public virtual int Version { get; set; }
    }
}
