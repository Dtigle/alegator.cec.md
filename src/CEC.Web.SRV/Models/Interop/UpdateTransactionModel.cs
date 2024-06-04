using System;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Interop
{
    public class UpdateTransactionModel 
    {
        public long? Id { get; set; }

        [Display(Name = "Transaction_IDNP", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "Transaction_IDNP_Required", ErrorMessageResourceType = typeof(MUI))]
        public string Idnp { get; set; }

        [Display(Name = "Transaction_LastName", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "Transaction_LastName_Required", ErrorMessageResourceType = typeof(MUI))]
        public string LastName { get; set; }

        [Display(Name = "Transaction_FirstName", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "Transaction_FirstName_Required", ErrorMessageResourceType = typeof(MUI))]
        public string FirstName { get; set; }

        [Display(Name = "Transaction_DateOfBirth", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "Transaction_DateOfBirth_Required", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("Date")]
        public DateTime DateOfBirth { get; set; }
        
        [Display(Name = "Transaction_TipInstitutie", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetInstitutionTypesAjax", "Interop", "json", "institutionTypeDataRequest", "institutionTypeResults", "GetAddressName", "PollingStation", PageLimit = 10)]
        public long InstitutionTypeId { get; set; }

        [Display(Name = "Transaction_Institution", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetInstitutionsAjax", "Interop", "json", "institutionDataRequest", "institutionResults", "GetAddressName", "PollingStation", PageLimit = 10)]
        public long InstitutionId { get; set; }

    }
}