using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
	public class CircumscriptionGridModel : JqGridRow
    {
        [Display(Name = "Circumscription_Number", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnLayout(Width = 75)]
        public string Number { get; set; }

        [Display(Name = "Circumscription_RegionName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnLayout(Width = 285)]
        [SearchData(DbName = "NameRo")]
        public string Name { get; set; }

        [Display(Name = "Circumscription_ListName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true, "SelectCircumscriptionList", "Lookup", EditType = JqGridColumnEditTypes.Select)]
        [JqGridColumnSearchable(true, "SelectCircumscriptionList", "Lookup", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [SearchData(DbName = "CircumscriptionList.Id", Type = typeof(long?))]
        public string CircumscriptionList { get; set; }
    }
}