using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.VoterCertificate
{
    public class CertificateDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var certificat = (CertificatModel)validationContext.ObjectInstance;
            if (certificat.ReleaseDate > DateTime.Now)
                return new ValidationResult("Data eliberarii certificatului nu trebuie sa fie mai mare ca data curentă . ");

            if(certificat.ReleaseDate > certificat.ElectionDate)
                return new ValidationResult("Data eliberarii certificatului nu trebuie sa fie mai mare ca data alegirilor. ");

            return ValidationResult.Success;
        }
    }
}