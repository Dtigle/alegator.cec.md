using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
    public class StreetsGridModel : LookupGridModel
    {
        [Display(Name = "Lookups_StreetTypeName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true, "SelectStreetType", "Lookup", EditType = JqGridColumnEditTypes.Select)]
        [JqGridColumnSearchable(true, "SelectStreetType", "Lookup", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [SearchData(DbName = "StreetTypeId", Type = typeof(long?))]
        public string StreetType { get; set; }

        [Display(Name = "Lookups_StreetHousesNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int HousesCount { get; set; }

		[Display(Name = "Lookups_CreatedById", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "CreatedBy", Type = typeof(string))]
		public new string CreatedById { get; set; }

		[Display(Name = "Lookups_ModifiedById", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "ModifiedBy", Type = typeof(string))]
		public new string ModifiedById { get; set; }

		[Display(Name = "Lookups_DeletedById", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "DeletedBy", Type = typeof(string))]
		public new string DeletedById { get; set; }
    }
}