using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.PollingStation
{
    public class UpdatePollingStationModel : UpdatePollingStationBaseModel
    {
        //[Required(ErrorMessageResourceName = "PollingStationAddEditeErrorRequired_Address", ErrorMessageResourceType = typeof(MUI))]
       
		[Display(Name = "PollingStationAddress", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Select2RemoteConfig("", "GetAddresses", "Voters", "json", "addressDataRequest", "addressResults", "GetAddressName", "PollingStation", PageLimit = 10)]
        public long? AddressId { get; set; }

        [Display(Name = "Street", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetStreets", "Voters", "json", "streetDataRequest", "streetResults", "GetStreetName", "PollingStation", PageLimit = 10)]
        public long? StreetId { get; set; }
    }
}