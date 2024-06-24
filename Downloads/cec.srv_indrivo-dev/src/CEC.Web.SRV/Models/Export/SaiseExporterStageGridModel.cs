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
    public class SaiseExporterStageGridModel : JqGridSoft
    {
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		//[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "SaiseExporter_StageType", ResourceType = typeof(MUI))]
		//[SearchData(DbName = "PollingStation.FullNumber", Type = typeof(string))]
		public string StageType { get; set; }

        [Display(Name = "SaiseExporter_Description", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		//[SearchData(DbName = "StartDate", Type = typeof(DateTimeOffset?))]
		public string Description { get; set; }

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(true)]
		//[JqGridColumnSearchable(true, "SelectStatusType", "Reporting", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [Display(Name = "SaiseExporter_Status", ResourceType = typeof(MUI))]
		//[SearchData(DbName = "Status",Type = typeof(PrintStatus))]
		public string Status { get; set; }
    }
}