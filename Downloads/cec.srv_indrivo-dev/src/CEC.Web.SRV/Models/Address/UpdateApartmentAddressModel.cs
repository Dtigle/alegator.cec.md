
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
	public class UpdateApartmentAddressModel : UpdateAddressModel
    {
        [Required(ErrorMessageResourceName = "Address_RequiredPollingStation", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Address_PollingStation", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        public long PollingStationId { get; set; }
    }
}