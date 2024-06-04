using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
    public class AdjustmentAddressesModel
    {
		[Display(Name = "AddressOfResidence", ResourceType = typeof(MUI))]
		public PersonAddressModel BaseAddressInfo { get; set; }
		
		[Display(Name = "Person_Address", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Select2RemoteConfig("", "GetAddresses", "Voters", "json", "addressDataRequest", "addressResults", "GetAddressName", "PollingStation", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "AdjustmentAddressesErrorRequired_Address", ErrorMessageResourceType = typeof(MUI))]
		public long AdjustmentAddressesId { get; set; }

		[Display(Name = "Region", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Select2RemoteConfig("", "GetRegions", "Voters", "json", "regionDataRequestStayStatement", "regionResultsStayStatement", "GetRegionName", "Voters", PageLimit = 10)]
		public long RegionId { get; set; }
    }
}