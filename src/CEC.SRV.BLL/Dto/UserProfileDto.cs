using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL.Dto
{
    public class UserProfileDto : BaseDto<string>, IEntity
    {
        public string LoginName { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        public DateTimeOffset LoginCreationDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }

        public string LandlinePhone { get; set; }

        public string MobilePhone { get; set; }

        public string WorkInfo { get; set; }

		public Gender Gender { get; set; }
    }
}
