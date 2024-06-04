using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class UserProfileModel
    {
		public string UserId { get; set; }
        
        [Display(Name = "Account_LoginName", ResourceType = typeof(MUI))]
        public string LoginName { get; set; }

        [Display(Name = "Account_LastLogin", ResourceType = typeof(MUI))]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "Account_LoginCreationDate", ResourceType = typeof(MUI))]
        public DateTime LoginCreationDate { get; set; }

        [Required(ErrorMessageResourceName = "AccountErrorRequired_FirstName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Account_FirstName", ResourceType = typeof(MUI))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "AccountErrorRequired_LastName", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Account_LastName", ResourceType = typeof(MUI))]
        public string LastName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "Account_DateOfBirth", ResourceType = typeof(MUI))]
        [AdditionalMetadata("data-input-mask", "99.99.9999")]
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(Const.EmailValidationExpression, ErrorMessageResourceName = "CreateUserErrorExpresion_Email", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Account_Email", ResourceType = typeof(MUI))]
        public string Email { get; set; }

        [RegularExpression(Const.TelephoneValidationExpression, ErrorMessageResourceName = "AccountErrorNum_LandlinePhoneNumber", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Account_LandlinePhoneNumber", Prompt = "TelephonePrompt", ResourceType = typeof(MUI))]
        [AdditionalMetadata("data-input-mask", "099999999")]
        public string LandlinePhone { get; set; }

        [RegularExpression(Const.TelephoneValidationExpression, ErrorMessageResourceName = "AccountErrorNum_MobilePhoneNumber", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Account_MobilePhoneNumber", Prompt = "TelephonePrompt", ResourceType = typeof(MUI))]
        [AdditionalMetadata("data-input-mask", "099999999")]
        public string MobilePhone { get; set; }

        [Display(Name = "Account_WorkAddressDetails", ResourceType = typeof(MUI))]
        public string WorkInfo { get; set; }
		
		public bool IsCurrentUser { get; set; }

		[Required(ErrorMessageResourceName = "CreateUserErrorRequired_GenderId", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "CreateUser_GenderId", ResourceType = typeof(MUI))]
		[UIHint("SelectList")]
		public long GenderId { get; set; }
    }
}