using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class RegisterViewModel
    {
        [Display(Name = "Register_DisplayName", ResourceType = typeof(MUI))]
        public string DisplayName { get; set; }

        [Display(Name = "Register_PreferredEmail", ResourceType = typeof(MUI))]
        [RegularExpression(Const.EmailValidationExpression, ErrorMessageResourceName = "CreateUserErrorExpresion_Email", ErrorMessageResourceType = typeof(MUI))]
        public string PreferredEmail { get; set; }

        [Required(ErrorMessageResourceName = "RegisterErrorRequired_UserName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Register_UserName", ResourceType = typeof(MUI))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RegisterErrorRequired_Password", ErrorMessageResourceType = typeof(MUI))]
        [RegularExpression(Const.PasswordValidationExpression, ErrorMessageResourceName = "ChangePasswordErrorRequired_NewPassword2", ErrorMessageResourceType = typeof(MUI))]
        [DataType(DataType.Password)]
        [Display(Name = "Register_Password", ResourceType = typeof(MUI))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Register_ConfirmPassword", ResourceType = typeof(MUI))]
        [Compare("Password", ErrorMessageResourceName = "RegisterErrorCompare_ConfirmPassword", ErrorMessageResourceType = typeof(MUI))]
        public string ConfirmPassword { get; set; }
    }
}