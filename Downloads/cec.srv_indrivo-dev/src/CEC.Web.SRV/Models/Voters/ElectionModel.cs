using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class ElectionModel
    {
        [UIHint("Select2")]
		[Select2RemoteConfig("", "GetActiveElections", "Voters", "json", "electionsDataRequest", "electionsResults", "GetElectionsName", "Voters", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "StayStatementErrorRequired_Election", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
        public long ElectionId { get; set; }

        [Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
        public string ElectionTypeName { get; set; }

        [UIHint("Date")]
        [Display(Name = "ElectionGrid_ElectionDate", ResourceType = typeof(MUI))]
        [DataType(DataType.Date)]
        public DateTime? ElectionDate { get; set; }
    }

    public class ElectionModelStaytment
    {
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetActiveElectionsForStaytments", "Voters", "json", "electionsDataRequest", "electionsResults", "GetElectionsName", "Voters", PageLimit = 10)]
        [Required(ErrorMessageResourceName = "StayStatementErrorRequired_Election", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
        public long ElectionId { get; set; }

        [Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
        public string ElectionTypeName { get; set; }

        [UIHint("Date")]
        [Display(Name = "ElectionGrid_ElectionDate", ResourceType = typeof(MUI))]
        [DataType(DataType.Date)]
        public DateTime? ElectionDate { get; set; }
    }
}