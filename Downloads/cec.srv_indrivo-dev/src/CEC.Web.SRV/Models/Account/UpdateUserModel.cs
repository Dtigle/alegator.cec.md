using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
	public class UpdateUserModel
    {
        public string Id { get; set; }

		[Required(ErrorMessageResourceName = "Users_RequiredRol", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Users_Rol", ResourceType = typeof(MUI))]
		[UIHint("SelectList")]
		public long RoleId { get; set; }

		[Required(ErrorMessageResourceName = "Users_RequireStatut", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Users_Statut", ResourceType = typeof(MUI))]
		[UIHint("SelectList")]
		public long StatusId { get; set; }

		[Display(Name = "Users_Comentarii", ResourceType = typeof(MUI))]
		public string Comments { get; set; }
    }
}