using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Statistics
{
	public class StatisticsPollingStationGridModel : JqGridRow
    {
        [HiddenInput(DisplayValue = false)]
        public long RegionId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long PollingStationId { get; set; }

        [JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "Statistics_PollingStationRegion", ResourceType = typeof(MUI))]
		public string RegionName { get; set; }

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Statistics_PollingStationNumber", ResourceType = typeof(MUI))]
        public string PollingStation { get; set; }

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [Display(Name = "Statistics_PollingStationVoters", ResourceType = typeof(MUI))]
        public int VotersCount { get; set; }

    }
}