using System;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class PersonDocument
    {
        public static string MissingDocument = "<";

        private string _documentNumber;
        
        public virtual string Seria { get; set; }

        public virtual string Number { get; set; }

        public virtual DateTime? IssuedDate { get; set; }

        public virtual string IssuedBy { get; set; }

        public virtual DateTime? ValidBy { get; set; }

        public virtual DocumentType Type { get; set; }

        public virtual string DocumentNumber
        {
            get { return _documentNumber; }
        }
    }
}