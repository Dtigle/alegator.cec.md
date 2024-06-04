using System;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Export
{
	public class ExportRsaToSaiseGridModel : JqGridSoft
    {
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "ExportRsaToSaise_Name", ResourceType = typeof(MUI))]
		public string Description { get; set; }

		[Display(Name = "ExportRsaToSaise_StartDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "StartDate", Type = typeof(DateTimeOffset?))]
		public string StartDate { get; set; }

		[Display(Name = "ExportRsaToSaise_EndDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "EndDate", Type = typeof(DateTimeOffset?))]
		public string EndDate { get; set; }

		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "ExportRsaToSaise_ErrorMessage", ResourceType = typeof(MUI))]
		public string ErrorMessage { get; set; }

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, "SelectStageStatus", "Export", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[Display(Name = "ExportPollingStations_Status", ResourceType = typeof(MUI))]
		[SearchData(DbName = "Status", Type = typeof(SaiseExporterStageStatus))]
		public string Status { get; set; }
    }
}