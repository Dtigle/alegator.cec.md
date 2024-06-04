using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class CreateUserViewModel
	{
        [Required(ErrorMessageResourceName = "CreateUserErrorRequired_UserName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "CreateUser_UserName", ResourceType = typeof(MUI), Prompt = "CreateUser_UserName")]
        [AdditionalMetadata("Tooltip", "CreateUserTooltip_UserName")]
		public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "CreateUserErrorRequired_Password", ErrorMessageResourceType = typeof(MUI))]
        [RegularExpression(Const.PasswordValidationExpression, ErrorMessageResourceName = "ChangePasswordErrorRequired_NewPassword2", ErrorMessageResourceType = typeof(MUI))]
		[DataType(DataType.Password)]
        [Display(Name = "CreateUser_Password", ResourceType = typeof(MUI))]
		public string Password { get; set; }

		[DataType(DataType.Password)]
        [Display(Name = "CreateUser_ConfirmPassword", ResourceType = typeof(MUI))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "CreateUserErrorCompare_ConfirmPassword", ErrorMessageResourceType = typeof(MUI))]
		public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "CreateUserErrorRequired_RoleId", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "CreateUser_RoleId", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
		public string RoleId { get; set; }

        [Required(ErrorMessageResourceName = "CreateUserErrorRequired_FirstName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "CreateUser_FirstName", ResourceType = typeof(MUI))]
		public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "CreateUserErrorRequired_LastName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "CreateUser_LastName", ResourceType = typeof(MUI))]
        public string LastName { get; set; }

		[Required(ErrorMessageResourceName = "CreateUserErrorRequired_Email", ErrorMessageResourceType = typeof(MUI))]
        [RegularExpression(Const.EmailValidationExpression, ErrorMessageResourceName = "CreateUserErrorExpresion_Email", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "CreateUser_Email", ResourceType = typeof(MUI))]
		public string Email { get; set; }

        [Display(Name = "CreateUser_Comments", ResourceType = typeof(MUI))]
        [UIHint("MultilineText")]
		public string Comments { get; set; }

		[Required(ErrorMessageResourceName = "CreateUserErrorRequired_GenderId", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "CreateUser_GenderId", ResourceType = typeof(MUI))]
		[UIHint("SelectList")]
		public long GenderId { get; set; }
	}
}
