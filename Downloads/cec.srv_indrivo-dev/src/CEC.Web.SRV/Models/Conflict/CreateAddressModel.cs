using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class CreateAddressModel
    {
        public long RspId { get; set; }

        public long RegionId { get; set; }

        public long? StreetId { get; set; }

        [Required(ErrorMessageResourceName = "AddressRequired_Street", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Address_StreetName", ResourceType = typeof(MUI))]
        public string Street { get; set; }

        [Required(ErrorMessageResourceName = "AddresseRequired_HouseNumber", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Address_HouseNumber", ResourceType = typeof(MUI))]
        [RegularExpression(Const.OnlyFiveNumbers, ErrorMessageResourceName = "Addresse_HouseNumberNotValid", ErrorMessageResourceType = typeof(MUI))]
        public int? HouseNumber { get; set; }

        [Display(Name = "Address_Suffix", ResourceType = typeof(MUI))]
        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string Suffix { get; set; }

        [Required(ErrorMessageResourceName = "Address_RequiredPollingStation", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Address_PollingStation", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        public long PollingStationId { get; set; }
    }
   
}