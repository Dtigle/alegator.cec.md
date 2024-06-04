using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class StayStatementViewModel : StayStatementModel
    {
        [Display(Name = "StayStatement_CreateDate", ResourceType = typeof(MUI))]
        public string CreationDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}