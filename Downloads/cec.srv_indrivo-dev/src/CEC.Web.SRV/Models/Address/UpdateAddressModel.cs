using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
    public class UpdateAddressModel
    {
        public long Id { get; set; }

        public long RegionId { get; set; }
		
		[UIHint("Select2")]
		[Display(Name = "Address_StreetName", ResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetStreets", "Address", "json", "addressDataRequest", "addressResults", "GetStreetsName", "Address", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "AddressRequired_Street", ErrorMessageResourceType = typeof(MUI))]
        public long StreetId { get; set; }

        [Required(ErrorMessageResourceName = "AddresseRequired_HouseNumber", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Address_HouseNumber", ResourceType = typeof(MUI))]
        [RegularExpression(Const.OnlyFiveNumbers, ErrorMessageResourceName = "Addresse_HouseNumberNotValid", ErrorMessageResourceType = typeof(MUI))]
        public int? HouseNumber { get; set; }

        [Display(Name = "Address_Suffix", ResourceType = typeof(MUI))]
        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string Suffix { get; set; }
    }
   
}