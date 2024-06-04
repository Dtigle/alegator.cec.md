using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
	public class RegionGridViewModel : LookupGridModel
	{
		[Display(Name = "RegionLookup_HasStreets", ResourceType = typeof(MUI))]
		[JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
		[JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
		public bool HasStreets { get; set; }

		[Display(Name = "SAISE ID")]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
		public long? SaiseId { get; set; }

        [JqGridColumnEditable(false)]
        [Display(Name = "CUATM")]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public long? Cuatm { get; set; }

		[Display(Name = "RegionLookup_FullyQualifiedName", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		public string FullyQualifiedName { get; set; }

		[Display(Name = "RegionLookup_RegionType", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, "SelectRegionType", "Lookup", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[SearchData(DbName = "RegionType", Type = typeof(long?))]
		public string RegionType { get; set; }

        [Display(Name = "RegionLookup_HasLinkedRegions", ResourceType = typeof(MUI))]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        public bool HasLinkedRegions { get; set; }

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