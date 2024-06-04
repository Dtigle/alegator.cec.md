using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Models.Address;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Conflict
{
    public class AddressRSVGridModel : AddressGridModel
    {
        [JqGridColumnSortable(false)]
        [JqGridColumnSearchable(false)]
        [JqGridColumnEditable(false)]
        [HiddenInput(DisplayValue = false)]
        public string RegionId { get; set; }


        [Display(Name = "Region", ResourceType = typeof(MUI), Order = 4)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true)]
		[JqGridColumnEditable(false)]
		public string RegionName { get; set; }


		[Display(Name = "NumberPollingStation", ResourceType = typeof(MUI), Order = 5)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true)]
		[JqGridColumnEditable(false)]
		public string PollingStation { get; set; }
    }
}