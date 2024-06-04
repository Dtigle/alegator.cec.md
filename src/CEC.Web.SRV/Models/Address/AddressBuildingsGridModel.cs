using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Address
{
    public class AddressBuildingsGridModel : AddressGridModel
    {

        [Display(Name = "GeoLocation", ResourceType = typeof(MUI), Order = 6)]
		[JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
		[JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
		[JqGridColumnSearchable(false, SearchOperators = JqGridSearchOperators.NullOperators)]
		public bool GeoLocation { get; set; }

        [Display(Name = "NumberPollingStation", ResourceType = typeof(MUI), Order = 5)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true)]
		[JqGridColumnEditable(false)]
		[Required]
		public string PollingStation { get; set; }

        [Display(Name = "AddressPeopleNumber", ResourceType = typeof(MUI), Order = 4)]
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
		public int PeopleCount { get; set; }

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