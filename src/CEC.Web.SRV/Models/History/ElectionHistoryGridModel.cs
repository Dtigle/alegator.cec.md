using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.History
{
    public class ElectionHistoryGridModel : HistoryGridRow
    {
        [Display(Name = "ElectionGrid_ElectionType", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string ElectionType { get; set; }

        [Display(Name = "ElectionGrid_ElectionDate", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public string ElectionDate { get; set; }

        [Display(Name = "ElectionGrid_SaiseId", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public long? SaiseId { get; set; }

        [Display(Name = "ElectionGrid_Comments", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Comments { get; set; }
    }
}