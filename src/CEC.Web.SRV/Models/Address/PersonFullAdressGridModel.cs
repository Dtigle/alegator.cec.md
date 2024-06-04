using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
    public class PersonFullAdressGridModel : JqGridSoft
    {

        [Display(Name = "Lookups_DataCreated", ResourceType = typeof(MUI))]
        public string Created { get; set; }
        
        [Display(Name = "Lookups_DataModified", ResourceType = typeof(MUI))]
        public string Modified { get; set; }

        [Display(Name = "PersonAddress_Type", ResourceType = typeof(MUI))]
        public string AddressType { get; set; }

        [Display(Name = "FilterForVoters_Region", ResourceType = typeof(MUI))]
        public string Region { get; set; }

        [Display(Name = "Person_Locality", ResourceType = typeof(MUI))]
        public string Locality { get; set; }

        [Display(Name = "Address_StreetName", ResourceType = typeof(MUI))]
        public string Street { get; set; }

        [Display(Name = "Person_BlNumber", ResourceType = typeof(MUI))]
        public int? BlockNumber { get; set; }
        
        [Display(Name = "Person_ApNumber", ResourceType = typeof(MUI))]
        public int? ApNumber { get; set; }

        [Display(Name = "HouseSufix", ResourceType = typeof(MUI))]
        public string ApSuffix { get; set; }

        [Display(Name = "PersonAddress_DateOfExpiration", ResourceType = typeof(MUI))]
        public string DateOfExpiration { get; set; }
    }
}