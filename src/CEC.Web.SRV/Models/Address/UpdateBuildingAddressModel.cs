
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
	public class UpdateBuildingAddressModel : UpdateAddressModel
    {
		[UIHint("Select2")]
		[Display(Name = "Address_PollingStation", ResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "votersDataRequest", "votersResults", "GetPollingStationsName", "Voters", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "Address_RequiredPollingStation", ErrorMessageResourceType = typeof(MUI))]
		
        public long PollingStationId { get; set; }
    }
}