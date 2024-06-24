using System;
using System.Linq;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class PersonAddress : SRVBaseEntity
    {
        public virtual Person Person { get; set; }

        public virtual Address Address { get; set; }

        public virtual PersonFullAddress PersonFullAddress { get; set; }

        public virtual DateTime DateOfRegistration { get; set; }

        public virtual DateTime? DateOfExpiration { get; set; }

        public virtual int? ApNumber { get; set; }

        public virtual string ApSuffix { get; set; }

        public virtual bool IsEligible { get; set; }

        public virtual PersonAddressType PersonAddressType { get; set; }

        public virtual string GetFullPersonAddress(bool includeRegion = false)
        {
            string address = Address.GetFullAddress(includeRegion);

            return string.Format("{0}{1}", address,
                ApNumber > 0 ? GetApNumber() : "");

        }

        public virtual bool IsNotExpired()
        {
            return DateOfExpiration == null || (DateOfExpiration != null && DateOfExpiration > DateTime.Now);
        }

        private string GetApNumber()
        {
            string result = string.Empty;
            if (ApNumber > 0)
            {
                if (!string.IsNullOrEmpty(ApSuffix) && char.IsDigit(ApSuffix.Trim().ToCharArray().First()))
                {
                    result = string.Format(", ap. {0}/{1}", ApNumber, ApSuffix);
                }
                else
                {
                    result = string.Format(", ap. {0} {1}", ApNumber, ApSuffix);
                }
            }

            return result;
        }
    }
}