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
	public class PrintSessionsGridModel : JqGridSoft
	{
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[Display(Name = "PrintSessionsGrid_Name", ResourceType = typeof(MUI))]
		[SearchData(DbName = "Election.ElectionType.Name", Type = typeof(string))]
		public string Name { get; set; }

		[Display(Name = "PrintSessionsGrid_StartDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "StartDate", Type = typeof(DateTimeOffset?))]
		public string StartDate { get; set; }

		[Display(Name = "PrintSessionsGrid_EndDate", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "EndDate", Type = typeof(DateTimeOffset?))]
		public string EndDate { get; set; }

		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, "SelectPrintStatus", "Export", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[Display(Name = "PrintSessionsGrid_Status", ResourceType = typeof(MUI))]
		[SearchData(DbName = "Status", Type = typeof(PrintStatus))]
		public string Status { get; set; }
	}
}