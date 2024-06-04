using System;
using Amdaris.Domain;
using CEC.SRV.Domain.Importer;

namespace CEC.SRV.Domain.ViewItem
{
    public class ConflictViewItem : Entity
    {

        public virtual long RspModificationDataId { get; set; }
        public virtual long RspRegistrationDataId { get; set; }

        public virtual long RegionId { get; set; }

        public virtual long? SourceRegionId { get; set; }


        public virtual string Idnp { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string SecondName { get; set; }

        public virtual DateTime Birthdate { get; set; }
        public virtual string SexCode { get; set; }
        public virtual bool Dead { get; set; }
        public virtual bool CitizenRm { get; set; }
        public virtual int Doctypecode { get; set; }
        public virtual string Series { get; set; }
        public virtual string Number { get; set; }
        public virtual DateTime? Issuedate { get; set; }
        public virtual DateTime? Expirationdate { get; set; }
        public virtual bool Validity { get; set; }
        public virtual int? RegTypeCode { get; set; }
        public virtual string Region { get; set; }
        public virtual string Locality { get; set; }
        public virtual int AdministrativeCode { get; set; }
        public virtual int Streetcode { get; set; }
        public virtual int? HouseNr { get; set; }
        public virtual string HouseSuffix { get; set; }
        public virtual int? ApNr { get; set; }
        public virtual string ApSuffix { get; set; }
        public virtual DateTime DateOfRegistration { get; set; }
        public virtual DateTime? DateOfExpiration { get; set; }
        public virtual ConflictStatusCode StatusConflictCode { get; set; }
        public virtual ConflictStatusCode AcceptConflictCode { get; set; }
        public virtual ConflictStatusCode RejectConflictCode { get; set; }
        public virtual SourceEnum Source { get; set; }
        public virtual RawDataStatus Status { get; set; }
        public virtual string StatusMessage { get; set; }
        public virtual DateTimeOffset Created { get; set; }
        public virtual DateTimeOffset? StatusDate { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool IsInConflict { get; set; }
        public virtual string StreetName { get; set; }
    }
}