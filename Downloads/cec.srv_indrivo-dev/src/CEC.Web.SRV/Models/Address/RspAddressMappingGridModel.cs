using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Address
{
    public class RspAddressMappingGridModel : JqGridRow
    {
        [Display(Name = "SrvRegion", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Cn | JqGridSearchOperators.Nc | JqGridSearchOperators.NullOperators)]
        [JqGridColumnEditable(false)]
        [SearchData(DbName = "SrvRegion.FullyQualifiedName", Type = typeof(string))]
        public string SrvRegion { get; set; }

        [Display(Name = "SrvAddress", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        [SearchData(DbName = "SrvFullAddress", Type = typeof(string))]
        public string SrvAddress { get; set; }

        [Display(Name = "RspRegionCode", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnEditable(false)]
        public long RspRegionCode { get; set; }

        [Display(Name = "RspRegion", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Cn | JqGridSearchOperators.Nc | JqGridSearchOperators.NullOperators)]
        [JqGridColumnEditable(false)]
        [SearchData(DbName = "RspRegion.FullyQualifiedName", Type = typeof(string))]
        public string RspRegion { get; set; }

        [Display(Name = "RspStreet", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        [SearchData(DbName = "RspStreetName", Type = typeof(string))]
        public string RspStreet { get; set; }

        [Display(Name = "RspStreetCode", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnEditable(false)]
        public long? RspStreetCode { get; set; }

        [Display(Name = "RspHouseNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [JqGridColumnEditable(false)]
        public int? RspHouseNumber { get; set; }

        [Display(Name = "RspHouseSuffix", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnEditable(false)]
        public string RspHouseSuffix { get; set; }
    }
}