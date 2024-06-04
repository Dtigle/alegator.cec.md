using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.QuartzAdmin
{
    public class QuartzRunningJobsGridModel : QuartzJobModel
    {
        public JobProgressModel Progress { get; set; }

        [Display(Name = "Progres")]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnFormatter("progressFormatter", UnFormatter = "progressUnFormatter")]
        public double JobProgress
        {
            get { return Progress != null ? Progress.OverallProgress.Ratio : 0.0; }
        }

        [Display(Name = "Durata execuției")]
        public string RunTime { get; set; }

        [Display(Name = "Interuptibil")]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        public bool IsInterruptable { get; set; }
    }
}