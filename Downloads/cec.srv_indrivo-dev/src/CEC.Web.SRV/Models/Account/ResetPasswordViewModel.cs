using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
	public class ResetPasswordViewModel
    {
		public string UserId { get; set; }

        [Required(ErrorMessageResourceName = "ResetPasswordErrorRequired_NewPassword", ErrorMessageResourceType = typeof(MUI))]
        [RegularExpression(Const.PasswordValidationExpression, ErrorMessageResourceName = "ChangePasswordErrorRequired_NewPassword2", ErrorMessageResourceType = typeof(MUI))]
        [DataType(DataType.Password)]
        [Display(Name = "ResetPassword_NewPassword", ResourceType = typeof(MUI))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ResetPassword_ConfirmPassword", ResourceType = typeof(MUI))]
        [Compare("NewPassword", ErrorMessageResourceName = "ResetPasswordErrorCompare_ConfirmPassword", ErrorMessageResourceType = typeof(MUI))]
        public string ConfirmPassword { get; set; }
    }
}