using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class MultiSelectAccountEdit
    {
        public MultiSelectAccountEdit()
        {
            Accounts = new List<string>();
        }

        public List<string> Accounts { get; set; }

        [UIHint("SelectList")]
        [Required(ErrorMessageResourceType = typeof(MUI), ErrorMessageResourceName = "StatusRequired")]
        [Display(Name = "Account_Status", ResourceType = typeof(MUI))]
        public AccountStatus Status { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Account_Comments", ResourceType = typeof(MUI))]
        public string Comments { get; set; }
    }
}