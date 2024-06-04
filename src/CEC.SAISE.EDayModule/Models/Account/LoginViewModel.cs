using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Nume Utilizator")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Câmp obligatoriu")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Parolă")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Câmp obligatoriu")]
        public string Password { get; set; }
    }
}