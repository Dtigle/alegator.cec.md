using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
	public class UpdateRegionTypesModel : UpdateLookupModel
	{
		[Display(Name = "RegionTypesLookup_Rank", ResourceType = typeof(MUI))]
		[Required(ErrorMessageResourceName = "Lookups_RequiredRank", ErrorMessageResourceType = typeof(MUI))]
		[Range(0, 255, ErrorMessageResourceName = "Lookups_RankInvalidValue", ErrorMessageResourceType = typeof(MUI))]
		public int Rank { get; set; }
	}
}