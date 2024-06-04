using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "LoginErrorRequired_UserName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Login_UserName", ResourceType = typeof(MUI))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "LoginErrorRequired_Password", ErrorMessageResourceType = typeof(MUI))]
        [DataType(DataType.Password)]
        [Display(Name = "Login_Password", ResourceType = typeof(MUI))]
        public string Password { get; set; }
    }
}