using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
    public class RegionTypesGridModel : LookupGridModel
    {
        [Display(Name = "RegionTypesLookup_Rank", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(Type = typeof(byte))]
        public int Rank { get; set; }
    }
}