using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;

namespace CEC.SAISE.Domain
{
    public class CircumscriptionRegion : Entity
    {

        public virtual AssignedCircumscription AssignedCircumscription { get; set; }
        public virtual Region Region { get; set; }
        public virtual ElectionRound ElectionRound { get; set; }
    }
}
