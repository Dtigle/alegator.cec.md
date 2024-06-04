using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Conflict
{
    public class AddressWithoutPollingStationGridModel : JqGridRow
    {

        [Display(Name = "AddressWithoutPS_ParentRegionId", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Eq)]
        [JqGridColumnEditable(false)]
        [HiddenInput(DisplayValue = false)]
        public long ParentRegionId { get; set; }

        [Display(Name = "AddressWithoutPS_ParentRegionName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        public string ParentRegionName { get; set; }

        [Display(Name = "AddressWithoutPS_RegionId", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Eq)]
        [JqGridColumnEditable(false)]
        [HiddenInput(DisplayValue = false)]
        public long RegionId { get; set; }

        [Display(Name = "AddressWithoutPS_RegionName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        public string Region { get; set; }

        [Display(Name = "AddressWithoutPS_RegionType", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        public string RegionType { get; set; }

        [Display(Name = "AddressWithoutPS_Street", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        public string Street { get; set; }

        [Display(Name = "AddressWithoutPS_HouseNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Eq)]
        [JqGridColumnEditable(false)]
        public int HouseNumber { get; set; }

        [Display(Name = "AddressWithoutPS_Suffix", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Eq)]
        [JqGridColumnEditable(false)]
        public string Suffix { get; set; }

        [Display(Name = "AddressWithoutPS_Persons", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnEditable(false)]
        public int Persons { get; set; }

    }
}