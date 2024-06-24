using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class AdditionalUserInfo : SRVBaseEntity
    {
        private string _fullName;
        
        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string FullName {
            get { return _fullName; }
        }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual string Email { get; set; }

        public virtual string LandlinePhone { get; set; }

        public virtual string MobilePhone { get; set; }

        public virtual string WorkInfo { get; set; }

		public virtual Gender Gender { get; set; }
    }
}
