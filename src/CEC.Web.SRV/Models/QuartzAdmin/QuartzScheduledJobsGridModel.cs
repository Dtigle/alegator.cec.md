using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.Web.SRV.Models.Grid;

namespace CEC.Web.SRV.Models.QuartzAdmin
{
    public class QuartzScheduledJobsGridModel : QuartzJobModel 
    {
        [Display(Name = "Starea")]
        public string TriggerState { get; set; }

        [HiddenInput(DisplayValue = false)]
        public bool HasCronTrigger { get; set; }

        [Display(Name = "Expresie Cron")]
        public string CronExpression { get; set; }

        [Display(Name = "Următoarea executare")]
        public string NextFireTime { get; set; }

        [Display(Name = "Precedenta executare")]
        public string PreviousFireTime { get; set; }
    }
}