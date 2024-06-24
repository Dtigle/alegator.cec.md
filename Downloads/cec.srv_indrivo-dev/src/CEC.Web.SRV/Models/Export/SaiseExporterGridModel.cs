using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Export
{
	public class SaiseExporterGridModel : JqGridSoft
	{
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "SaiseExporter_Description", ResourceType = typeof(MUI))]
		public string Description { get; set; }

		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "SaiseExporter_ErrorMessage", ResourceType = typeof(MUI))]
		public string ErrorMessage { get; set; }

		[Display(Name = "SaiseExporter_ExportAllVoters", ResourceType = typeof(MUI))]
		[JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
		[JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
		[JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
		[JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
		public bool ExportAllVoters { get; set; }

		[Display(Name = "SaiseExporter_StartDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "StartDate", Type = typeof(DateTimeOffset?))]
		public string StartDate { get; set; }

		[Display(Name = "SaiseExporter_EndDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "EndDate", Type = typeof(DateTimeOffset?))]
		public string EndDate { get; set; }

		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, "SelectExporterStatus", "Export", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[Display(Name = "SaiseExporter_Status", ResourceType = typeof(MUI))]
		[SearchData(DbName = "Status", Type = typeof(SaiseExporterStatus))]
		public string Status { get; set; }
	}
}