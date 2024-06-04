using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Statistics
{
	public class ProblematicDataPollingStationGridModel : JqGridRow
    {
		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "Statistics_PollingStationRegion", ResourceType = typeof(MUI))]
		[SearchData(DbName = "FullRegionName", Type = typeof(string))]
		public string RegionName { get; set; }

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Statistics_PollingStationNumber", ResourceType = typeof(MUI))]
		[SearchData(DbName = "PollingStation", Type = typeof(string))]
        public string PollingStation { get; set; }

		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
		[Display(Name = "Statistics_PollingStationVoters", ResourceType = typeof(MUI))]
		public int VotersCount { get; set; }
    }
}