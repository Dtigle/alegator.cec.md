using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.QuartzAdmin
{
    public class ProgressInfoModel
    {
        public string Id { get; set; }

        [Display(Name = "Info")]
        public string Comments { get; set; }

        public long Minimum { get; set; }

        public long Maximum { get; set; }

        public long Value { get; set; }

        [Display(Name = "Progres")]
        public double Ratio { get; set; }
    }
}