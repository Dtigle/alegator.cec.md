using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceName = "ChangePasswordErrorRequired_OldPassword", ErrorMessageResourceType = typeof(MUI))]
        [DataType(DataType.Password)]
        [Display(Name = "ChangePassword_OldPassword", ResourceType = typeof(MUI))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "ChangePasswordErrorRequired_NewPassword", ErrorMessageResourceType = typeof(MUI))]
        [RegularExpression(Const.PasswordValidationExpression, ErrorMessageResourceName = "ChangePasswordErrorRequired_NewPassword2", ErrorMessageResourceType = typeof(MUI))]
        [DataType(DataType.Password)]
        [Display(Name = "ChangePassword_NewPassword", ResourceType = typeof(MUI))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ChangePassword_ConfirmPassword", ResourceType = typeof(MUI))]
        [Compare("NewPassword", ErrorMessageResourceName = "ChangePasswordErrorCompare_ConfirmPassword", ErrorMessageResourceType = typeof(MUI))]
        public string ConfirmPassword { get; set; }
    }
}