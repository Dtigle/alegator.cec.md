using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CEC.SRV.Domain.Importer
{
    public abstract class PersonRaw : RawData
    {
        public virtual string Idnp { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string MiddleName { get; set; }

        public virtual string Surname { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual string Gender { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Idnp == null && FirstName == null)
            {
                yield return new ValidationResult("Idnp and First Name null");
            }
        }
    }
}