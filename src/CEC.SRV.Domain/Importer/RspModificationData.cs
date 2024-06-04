using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.Importer
{
    public class RspModificationData : RawData, INotificationEntity
    {
        public RspModificationData()
        {
            Registrations = new List<RspRegistrationData>();
        }

        public virtual string Idnp { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string SecondName { get; set; }
        public virtual DateTime Birthdate { get; set; }
        public virtual string SexCode { get; set; }
        public virtual bool Dead { get; set; }
        public virtual bool CitizenRm { get; set; }
        public virtual long DocumentTypeCode { get; set; }        
        public virtual string Seria { get; set; }
        public virtual string Number { get; set; }
        public virtual DateTime? IssuedDate { get; set; }
        public virtual DateTime? ValidBydate { get; set; }
        public virtual bool Validity { get; set; }

        public virtual IList<RspRegistrationData> Registrations { get; set; }

        public virtual ConflictStatusCode StatusConflictCode { get; set; }
        public virtual ConflictStatusCode AcceptConflictCode { get; set; }
        public virtual ConflictStatusCode RejectConflictCode { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Idnp == null && FirstName == null && LastName == null)
            {
                yield return new ValidationResult("Idnp, Last Name and First Name null");
            }
        }

        public virtual string GetPersonStatus()
        {
            // Adaugat statut radiat
            if (Status == RawDataStatus.ToRemove)
                return "Radiat";
            if (Dead)
                return "Decedat";
            if (!CitizenRm)
                return "Altă cetățenie";            
            return "Alegător";

            //return (Dead) ? "Decedat" : (!CitizenRm) ? "Altă cetățenie" : "Alegător";
        }
        
        public virtual string GetSexCode()
        {
            return (SexCode == "2") ? "F" : "M";
        }

        public virtual void SetAddressConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.AddressConflict;
            SetError(message);
        }

        public virtual void SetRegionConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.RegionConflict;
            SetError(message);
        }

        public virtual void SetStatusConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.StatusConflict;
            SetError(message);
        }

        public virtual void SetPollingStationConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.PollingStationConflict;
            SetError(message);
        }

        public virtual void SetFatalAddressConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.AddressFatalConflict;
            SetError(message);
        }

        public virtual void SetStreetZeroConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.StreetZeroConflict;
            SetError(message);
        }

        public virtual void SetLocalityConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.LocalityConflict;
            SetError(message);
        }

        public virtual void SetStreetConflict(string message)
        {
            StatusConflictCode |= ConflictStatusCode.StreetConflict;
            SetError(message);
        }

        string INotificationEntity.GetNotificationType()
        {
            return "ResolveConflict_Notification";
        }

        public virtual void AcceptConflict(ConflictStatusCode conflictStatusCode)
        {
            AcceptConflictCode |= conflictStatusCode;
            SetEnd();
            StatusDate = DateTime.Now;
        }

        public virtual void RejectConflict(ConflictStatusCode conflictStatusCode)
        {
            RejectConflictCode |= conflictStatusCode;
            SetEnd();
            StatusDate = DateTime.Now;
        }

        public virtual bool IsResolved()
        {
            return StatusConflictCode == 0 || AcceptConflictCode == ConflictStatusCode.ManualSolved || StatusConflictCode.HasFlag(AcceptConflictCode | RejectConflictCode);
        }

        public override string ToString()
        {
            return Idnp;
        }
    }
}