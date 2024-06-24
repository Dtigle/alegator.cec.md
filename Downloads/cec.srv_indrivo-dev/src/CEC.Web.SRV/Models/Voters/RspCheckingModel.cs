using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
	public class RspCheckingModel
    {
		[Display(Name = "Person_Idnp", ResourceType = typeof(MUI), Prompt = "Idnp_Placeholder")]
		[AdditionalMetadata("data-input-mask", "9999999999999")]
		[Required(ErrorMessageResourceName = "Voters_PersonIdnp", ErrorMessageResourceType = typeof(MUI))]
		public string Idnp { get; set; }
    }
}