using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;
using Microsoft.AspNet.Identity;

namespace CEC.Web.SRV.Models.Voters
{
    public class PersonAddressModel
    {
        public long PersonAddressId { get; set; }

        [Display(Name = "Person_Address", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
		[Select2RemoteConfig("", "GetAddresses", "Voters", "json", "addressDataRequest", "addressResults", "GetAddressName", "PollingStation", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "StayStatementErrorRequired_DeclaredStayAddress", ErrorMessageResourceType = typeof(MUI))]
        public long AddressId { get; set; }

        [Display(Name = "Person_Address", ResourceType = typeof(MUI))]
        public string FullAddress { get; set; }

        [Display(Name = "Person_ApNumber", ResourceType = typeof(MUI))]
        public int? ApNumber { get; set; }

        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        [StringLength(10, ErrorMessageResourceName = "PersonMaxNumCharacters_ApSuffix", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Person_ApSuffix", ResourceType = typeof(MUI))]
        public string ApSuffix { get; set; }
    }
}