using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Address
{
    public class AddressGridModel : JqGridSoft
    {
        [Display(Name = "StreetName", ResourceType = typeof(MUI), Order = 1)]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string StreetName { get; set; }

        [Display(Name = "HouseNo", ResourceType = typeof(MUI), Order = 2)]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right, Width = 80)]
        public int? HouseNumber { get; set; }

        [Display(Name = "HouseSufix", ResourceType = typeof(MUI), Order = 3)]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnLayout(Width = 80)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Suffix { get; set; }
    }
}