using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Controllers
{
    public class VoterAdressModel
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

        [Display(Name = "Person_Region", ResourceType = typeof(MUI))]
        public string Region { get; set; }

        public long? RegionId { get; set; }
        
        [Display(Name = "Person_Locality", ResourceType = typeof(MUI))]
        public string Locality { get; set; }

        public long? LocalityId { get; set; }

        [Display(Name = "Person_Street", ResourceType = typeof(MUI))]
        public string Street { get; set; }

        public long? StreetId { get; set; }

        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        [StringLength(10, ErrorMessageResourceName = "PersonMaxNumCharacters_ApSuffix", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Person_ApSuffix", ResourceType = typeof(MUI))]
        public string ApSuffix { get; set; }

        [Display(Name = "Person_BlNumber", ResourceType = typeof(MUI))]
        public string BlNumber { get; internal set; }
    }
}