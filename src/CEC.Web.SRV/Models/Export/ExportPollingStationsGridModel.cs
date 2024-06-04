using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Export
{
	public class ExportPollingStationsGridModel : JqGridSoft
    {
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "PollingStationNumber", ResourceType = typeof(MUI))]
		[SearchData(DbName = "PollingStation.FullNumber", Type = typeof(string))]
		public string PollingStationName { get; set; }

		[Display(Name = "ExportPollingStations_StartDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "StartDate", Type = typeof(DateTimeOffset?))]
		public string StartDate { get; set; }

		[Display(Name = "ExportPollingStations_EndDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "EndDate", Type = typeof(DateTimeOffset?))]
		public string EndDate { get; set; }

		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "ExportPollingStations_ErrorMessage", ResourceType = typeof(MUI))]
		[SearchData(DbName = "StatusMessage", Type = typeof(string))]
		public string ErrorMessage { get; set; }

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, "SelectPrintStatus", "Export", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[Display(Name = "ExportPollingStations_Status", ResourceType = typeof(MUI))]
		[SearchData(DbName = "Status",Type = typeof(PrintStatus))]
		public string Status { get; set; }
    }
}