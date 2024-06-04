using System;

namespace SAISE.Domain
{
    public class Voter : SaiseEntity
    {
        public virtual string NameRo { get; set; }

        public virtual string LastNameRo { get; set; }

        public virtual string PatronymicRo { get; set; }

        public virtual DateTime DateOfBirth { get; set; }

        public virtual string PlaceOfBirth { get; set; }

        public virtual string PlaceOfResidence { get; set; }

        public virtual int Gender { get; set; }

        public virtual DateTime DateOfRegistration { get; set; }

        public virtual long Idnp { get; set; }

        public virtual string DocumentNumber { get; set; }

        public virtual DateTime? DateOfIssue { get; set; }

        public virtual DateTime? DateOfExpiry { get; set; }

        public virtual long Status { get; set; }

        public virtual long? StreetId { get; set; }

        public virtual string StreetName { get; set; }

        public virtual long? StreetNumber { get; set; }

        public virtual string StreetSubNumber { get; set; }

        public virtual long? BlockNumber { get; set; }

        public virtual string BlockSubNumber { get; set; }

        public virtual long? PollingStationId { get; set; }

        public virtual long? RegionId { get; set; }

        public virtual long? ElectionListNr { get; set; }
    }
}