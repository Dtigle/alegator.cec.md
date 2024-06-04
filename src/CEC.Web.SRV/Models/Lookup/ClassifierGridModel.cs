using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
    public class ClassifierGridModel : JqGridSoft
    {
        [Display(Name = "Lookups_Name", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Name { get; set; }

        [Display(Name = "Lookups_Namerus", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Namerus { get; set; }

        [Display(Name = "Lookups_Description", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Docprint { get; set; }

        [Display(Name = "Lookups_RspStreetTypeCodeId", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string RspStreetTypeCodeId { get; set; }

    }
}