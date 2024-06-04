using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Interop
{
    public class UpdateInstitutionModel : UpdateLookupModel
    {
        [Display(Name = "Institution_InteropSystem", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetInteropSystems", "Interop", "json", "addressDataRequest", "addressResults", "GetInteropSystemName", "Interop", PageLimit = 10)]
        public long InstitutionTypeId { get; set; }

        [Display(Name = "Institution_Address", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetAddresses", "Voters", "json", "addressDataRequest", "addressResults", "GetAddressName", "PollingStation", PageLimit = 10)]
        public long AddressId { get; set; }


        [Display(Name = "Institution_LegacyId", ResourceType = typeof(MUI))]
        [RegularExpression(Const.OnlyNumbers, ErrorMessageResourceName = "InstitutionAddEditeErrorNum_LegacyId", ErrorMessageResourceType = typeof(MUI))]
        public long LegacyId { get; set; }
    }
}