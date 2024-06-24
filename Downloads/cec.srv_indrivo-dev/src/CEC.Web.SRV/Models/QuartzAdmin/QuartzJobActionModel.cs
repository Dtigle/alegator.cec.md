using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEC.Web.SRV.Models.QuartzAdmin
{
    public class QuartzJobActionModel
    {
        public QuartzScheduledJobsGridModel JobDetails { get; set; }

        [Display(Name = "Acțiune")]
        [UIHint("SelectList")]
        public int SelectedAction { get; set; }

        //[Display(Name = "Reason")]
        //public string ReasonComment { get; set; }
    }
}