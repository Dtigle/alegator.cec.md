using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Interop
{
    public class InstitutionGridModel : JqGridSoft
    {

        [Display(Name = "Institution_InteropSystem", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, "SelectInteropSystems", "Interop", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "InteropSystem.Id", Type = typeof(long?))]
        public string InstitutionType { get; set; }


        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [Display(Name = "Institution_LegacyId", ResourceType = typeof(MUI))]
        [SearchData(DbName = "LegacyId", Type = typeof(long?))]
        public string LegacyId { get; set; }


        [Display(Name = "Lookups_Name", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Name { get; set; }

        [Display(Name = "Lookups_Description", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Description { get; set; }


        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Institution_Region", ResourceType = typeof(MUI))]
        public string Region { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Institution_Address", ResourceType = typeof(MUI))]
        public string FullAddress { get; set; }
    }
}