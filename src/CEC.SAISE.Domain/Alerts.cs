using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
   public  class Alerts : SaiseBaseEntity
    {
      

        public virtual   Voter Voter { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Patronymic { get; set; }

        public virtual long Idnp { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual string Adress { get; set; }

        public virtual string DocumentNumber { get; set; }

        public virtual PollingStation PollingStation { get; set; }

        public virtual string PollingStationAdress { get; set; }

        public virtual DateTime DateRegistration { get; set; }


    }
}
