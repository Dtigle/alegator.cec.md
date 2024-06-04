using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Models.Grid;

namespace CEC.Web.SRV.Models.QuartzAdmin
{
    public class QuartzJobModel : JqGridRow
    {
        [Display(Name = "Grup")]
        public string GroupName { get; set; }

        [Display(Name = "Nume Job")]
        public string JobName { get; set; }

        [Display(Name = "Descriere")]
        public string JobDescription { get; set; }

        [Display(Name = "Nume Trigger")]
        public string TriggerName { get; set; }

        [Display(Name = "Grup Trigger")]
        public string TriggerGroup { get; set; }
    }
}