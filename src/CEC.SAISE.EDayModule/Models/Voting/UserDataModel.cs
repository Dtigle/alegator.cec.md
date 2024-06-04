using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class UserDataModel
    {
        public ValueNameModel AssignedElection { get; set; }

        public ValueNameModel AssignedRegion { get; set; }

        public ValueNameModel AssignedCircumscription { get; set; }

        public ValueNameModel AssignedPollingStation { get; set; }

        public bool IsAdmin { get; set; }
    }
}