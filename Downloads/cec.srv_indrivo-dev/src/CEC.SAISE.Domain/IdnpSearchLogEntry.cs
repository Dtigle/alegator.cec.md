using System;
using Amdaris.Domain;

namespace CEC.SAISE.Domain
{
    public class IdnpSearchLogEntry : Entity
    {
        public IdnpSearchLogEntry()
        {
            //LogTime = DateTime.Now;
        }

        public virtual long SystemUserId { get; set; }

        public virtual long Idnp { get; set; }

        public virtual bool Found { get; set; }

        //public virtual DateTime LogTime { get; set; }

        public virtual int? VotingStatus { get; set; }

        public virtual long? VoterStatus { get; set; }

        public virtual long? AssignedVoterStatus { get; set; }
    }
}