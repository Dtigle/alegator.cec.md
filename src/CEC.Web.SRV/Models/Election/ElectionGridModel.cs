using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Election
{
    public class ElectionGridModel : JqGridSoft
    {
        [Display(Name = "Election_ID", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnLayout(Width = 120)]
        [SearchData(DbName = "Id", Type = typeof(long?))]
        public long ElectionId { get; set; }

        [Display(Name = "Election_Description", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnLayout(Width = 180)]
        public string Description { get; set; }

        [Display(Name = "Election_Type", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, "SelectElectionTypes", "Elections", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnLayout(Width = 280)]
        [SearchData(DbName = "ElectionType.Id", Type = typeof(long?))]
        public string ElectionType { get; set; }

        [Display(Name = "Election_Status", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, "SelectElectionStatuses", "Elections", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnLayout(Width = 180)]
        [SearchData(DbName = "Status", Type = typeof(ElectionStatus))]
        public string ElectionStatus { get; set; }

        [Display(Name = "Election_Date", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.JQueryUIDatepicker, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnLayout(Width = 180)]
        [SearchData(DbName = "ElectionDate", Type = typeof(DateTime))]
        public string ElectionDate { get; set; }
    }
}