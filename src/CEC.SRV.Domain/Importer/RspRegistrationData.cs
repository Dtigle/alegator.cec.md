using System;
using Amdaris.Domain;

namespace CEC.SRV.Domain.Importer
{
    public class RspRegistrationData : Entity
    {
        public virtual int? RegTypeCode { get; set; }
        public virtual string Region { get; set; }
        public virtual string Locality { get; set; }
        public virtual int Administrativecode { get; set; }
        public virtual string StreetName { get; set; }
        public virtual long StreetCode { get; set; }
        public virtual int? HouseNumber { get; set; }
        public virtual string HouseSuffix { get; set; }
        public virtual int? ApartmentNumber { get; set; }
        public virtual string ApartmentSuffix { get; set; }
        public virtual DateTime DateOfRegistration { get; set; }
        public virtual DateTime? DateOfExpiration { get; set; }
        public virtual bool IsInConflict { get; set; }
        public virtual RspModificationData RspModificationData { get; set; }

        public virtual string GetHouseSuffix()
        {
            if (HouseSuffix == "<" || HouseSuffix == ">") return null;
            return HouseSuffix;
        }

        public virtual string GetHouse()
        {
            if (HouseNumber == 0)
                return string.Empty;

            return string.Format("{0}{1}", HouseNumber,
                !String.IsNullOrWhiteSpace(HouseSuffix) ? string.Format("/{0}", HouseSuffix) : string.Empty);
        }

        public virtual string GetApartment()
        {
            if (ApartmentNumber == 0)
                return string.Empty;

            return string.Format(", ap. {0}{1}", ApartmentNumber,
                !String.IsNullOrWhiteSpace(ApartmentSuffix) ? string.Format("/{0}", ApartmentSuffix) : string.Empty);
        }

        public virtual int GetAddressType()
        {
            return RegTypeCode.Value;
        }

        public virtual void SetConflicted(bool state)
        {
            IsInConflict = state;
        }

        public virtual string GetRegistrationAddress()
        {
            return string.Format("{0} {1}{2} ", StreetName, GetHouse(), GetApartment());
        }
    }
}