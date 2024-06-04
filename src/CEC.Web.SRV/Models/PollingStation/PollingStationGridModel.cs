using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.SRV.Domain;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.PollingStation
{
    public class PollingStationGridModel : JqGridSoft
    {
		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(false)]
		[JqGridColumnSearchable(false)]
		[Display(Name = "Circumscription_Number", ResourceType = typeof(MUI))]
		public int? CircumscriptionNumber { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "PollingStationNumber", ResourceType = typeof(MUI))]
        public string Number { get; set; }

        [Display(Name = "PSType", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetPollingStationTypes", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "PollingStationType", Type = typeof(PollingStationTypes))]
        public string PollingStationType { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "PollingStationLocation", ResourceType = typeof(MUI))]
        public string Location { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "PollingStationAddress", ResourceType = typeof(MUI))]
		public string FullAddress { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "PollingStationContactInfo", ResourceType = typeof(MUI))]
        public string ContactInfo { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "VotersListOrderTypes", ResourceType = typeof(MUI))]
        public string VotersListOrderTypes { get; set; }

        [Display(Name = "PollingStationAddressCount", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
		public int TotalAddress { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [Display(Name = "PollingStationSaiseId", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        public long? SaiseId { get; set; }

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

        [JqGridColumnEditable(false)]
        [JqGridColumnSortable(false)]
        [SearchData(DbName = "VotersListOrderType.Id", Type = typeof(long?))]
        [Display(Name = "PollingStationVotersListOrderType", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(false, "SelectVotersListOrderTypes", "PollingStation", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        public string OrderType { get; set; }
    }
}