using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;


namespace CEC.Web.SRV.Models.Voters
{
	public class UpdateVotersStatusModel
    {
		public PersonInfo PersonInfo { get; set; }

		[Display(Name = "Voters_CurrentStatus", ResourceType = typeof(MUI))]
		public string CurrentStatus { get; set; }

		[Display(Name = "Voters_CurrentConfirmation", ResourceType = typeof(MUI))]
		public string CurrentConfirmation { get; set; }

		[Required(ErrorMessageResourceName = "Voters_RequireStatut", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Voters_StatutNew", ResourceType = typeof(MUI))]
		[UIHint("SelectList")]
		public long StatusId { get; set; }

		[Required(ErrorMessageResourceName = "Voters_RequireConfirmationNew", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Voters_ConfirmationNew", ResourceType = typeof(MUI))]
		public string ConfirmationNew { get; set; }
    }
}