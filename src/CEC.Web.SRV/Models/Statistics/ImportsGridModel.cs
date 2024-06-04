using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Statistics
{
    public class ImportsGridModel  :JqGridRow
    {
		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "Date", Type = typeof(DateTime))]
        [Display(Name = "Statistics_DataExecutionImport", ResourceType = typeof(MUI))]
    	public string DataExecutionImport { get; set; }

		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [Display(Name = "Statistics_RegionCount", ResourceType = typeof(MUI))]
    	public int RegionCount { get; set; }
    }
}