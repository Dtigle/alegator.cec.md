using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
	public class UpdateLookupModel
    {
        public long? Id { get; set; }

		[Required(ErrorMessageResourceName = "Lookups_RequiredName", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Lookups_Name", ResourceType = typeof(MUI))]
		public string Name { get; set; }

		[Required(ErrorMessageResourceName = "Lookups_RequireDescription", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Lookups_Description", ResourceType = typeof(MUI))]
		public string Description { get; set; }
    }
}