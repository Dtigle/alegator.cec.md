using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.Account
{
    public class UpdateAccountModel
    {
        public string Id { get; set; }

        public string Role { get; set; }

        public AccountStatus Status { get; set; }

        public string Comments { get; set; }
    }
}