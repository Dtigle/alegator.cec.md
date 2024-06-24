
using System;
using Amdaris.Domain;
using Amdaris.Domain.Identity;

namespace CEC.SRV.Domain.Interop
{
    public class Transaction : AuditedEntity<IdentityUser>
    {
        public virtual string Idnp { get; set; }

        public virtual string LastName { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string Patronymic { get; set; }

        public virtual DateTime DateOfBirth { get; set; }
        
        public virtual InteropSystem InteropSystem { get; set; }
        
        public virtual Institution Institution { get; set; }
        
        public virtual string Comments { get; set; }

        public virtual TransactionStatus TransactionStatus { get; set; }

        public virtual string StatusMessage { get; set; }

        public virtual PersonAddress OldPersonAddress { get; set; }

        public virtual PersonAddress NewPersonAddress { get; set; }
    }
}
