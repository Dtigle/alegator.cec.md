using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Amdaris.Domain;

namespace CEC.SRV.Domain.Importer
{

    public enum SourceEnum
    {
        None = 0,
        Alegator = 1,
        Rsv = 2,
        Rsp = 3
    }

    public enum RawDataStatus
    {
        ToRemove = -1,
        New = 0,
        InProgress = 1,
        Error = 2,
        Retry = 3,
        End = 4,
        ForceImport = 5
    }


    public abstract class RawData : Entity, IValidatableObject
    {
        public virtual SourceEnum Source { get; set; }

        public virtual RawDataStatus Status { get; private set; }

        public virtual string StatusMessage { get; set; }

        public virtual DateTimeOffset Created { get; set; }

        public virtual DateTimeOffset? StatusDate { get; set; }

        public virtual string Comments { get; set; }

        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);

        public virtual void SetInProgress()
        {
            Status = RawDataStatus.InProgress;
            StatusMessage = null;
            StatusDate = DateTimeOffset.Now;
        }

        public virtual void SetError(string messsage)
        {
            Status = RawDataStatus.Error;
            StatusMessage = messsage;
            StatusDate = DateTime.Now;
        }

        public virtual void SetRetry(string message)
        {
            Status = RawDataStatus.Retry;
            StatusDate = DateTimeOffset.Now;
            StatusMessage = message;
        }

        public virtual void SetEnd()
        {
            Status = RawDataStatus.End;
            StatusMessage = null;
            StatusDate = DateTimeOffset.Now;
        }

        public virtual void SetToRemove()
        {
            Status = RawDataStatus.ToRemove;
            StatusMessage = null;
            StatusDate = DateTimeOffset.Now;
        }

        public virtual string GetValidationString()
        {
            var sb = new StringBuilder();

            foreach (var validationResult in ValidationResults())
            {
                sb.Append(string.Format("Field Members: {0} : Error: {1}",
                    string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage));
            }

            return sb.ToString();
        }

    }
}