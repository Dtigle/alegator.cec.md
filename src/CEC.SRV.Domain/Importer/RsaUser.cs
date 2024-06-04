using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.Importer
{
    public class RsaUser : RawData
    {
        public virtual Region Region { get; set; }

        public virtual string LoginName { get; set; }

        public virtual string Password { get; set; }


        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(LoginName))
            {
                result.Add(new ValidationResult("LoginName is null or empty", new [] {"LoginName"}));
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                result.Add(new ValidationResult("Password is null or empty", new [] {"Password"}));
            }

            if (Region == null)
            {
                result.Add(new ValidationResult("Region is null", new [] {"Region"}));
            }

            return result;
        }

		public virtual string GetObjectType()
		{
			return GetType().Name;
		}
    }
}