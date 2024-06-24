using System;
using Amdaris.Domain;

namespace CEC.SRV.BLL.Dto
{
    public class UserRow : BaseDto<string>, IEntity
    {
        public string LoginName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        public bool LogonDenied { get; set; }

        public string Comments { get; set; }

        public string GetFullName()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        }
    }
}
