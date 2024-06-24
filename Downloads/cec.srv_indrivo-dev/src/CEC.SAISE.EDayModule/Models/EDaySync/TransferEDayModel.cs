using Amdaris.Domain;
using System.ComponentModel.DataAnnotations;

namespace CEC.SAISE.EDayModule.Models.EDaySync
{
    public class TransferEDayModel : IEntity
    {
        [Display(Name = "Server")]
        [Required(ErrorMessage = "Câmpul \"Server\" este câmp obligatoriu")]
        public string TargetHost { get; set; }

        [Display(Name = "Utilizator")]
        [Required(ErrorMessage = "Câmpul \"Utilizator\" este câmp obligatoriu")]
        public string TargetUserName { get; set; }

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Câmpul \"Parola\" este câmp obligatoriu")]
        [UIHint("Password")]
        public string TargetPassword { get; set; }
    }
}