using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
    public class UpdateRegionModel
    {
        public long Id { get; set; }

        [Required(ErrorMessageResourceName = "RegionAddEditeErrorRequired_Name", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Lookups_Name", ResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Display(Name = "Lookups_Description", ResourceType = typeof(MUI))]
        public string Description { get; set; }
		
		[Display(Name = "RegionLookup_Parent", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Required(ErrorMessageResourceName = "RegionAddEditeErrorRequired_Parent", ErrorMessageResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetRegions", "Voters", "json", "regionDataRequestStayStatement", "regionResultsStayStatement", "GetRegionName", "Voters", PageLimit = 10)]
        public long Parent { get; set; }

        [Required(ErrorMessageResourceName = "RegionAddEditeErrorRequired_RegionType", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "RegionLookup_RegionType", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public long RegionType { get; set; }

        [Display(Name = "RegionLookup_HasStreets", ResourceType = typeof(MUI))]
        [UIHint("Checkbox")]
        public bool HasStreets { get; set; }

        [RegularExpression(Const.OnlyNumbers, ErrorMessageResourceName = "StreetAddEditeErrorNum_RopSaise", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "SAISE ID")]
        public long? SaiseId { get; set; }

        [RegularExpression(Const.OnlyNumbers, ErrorMessageResourceName = "StreetAddEditeErrorNum_RopSaise", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "CUTM")]
        public long? Cuatm { get; set; }
    }
}