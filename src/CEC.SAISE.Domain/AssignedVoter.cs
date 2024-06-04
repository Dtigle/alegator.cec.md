using System.Collections.Generic;

namespace CEC.SAISE.Domain
{
    public class AssignedVoter : SaiseBaseEntity
    {
        public virtual IList<VoterCertificat> VoterCertificats { get; set; }
        public AssignedVoter()
        {      
            VoterCertificats = new List<VoterCertificat>();
        }

        public virtual Region Region { get; set; }

        public virtual PollingStation RequestingPollingStation { get; set; }

        public virtual PollingStation PollingStation { get; set; }

        public virtual Voter Voter { get; set; }

        public virtual long Category { get; set; }

        public virtual AssignedVoterStatus Status { get; set; }

        public virtual string Comment { get; set; }
        public virtual long? ElectionListNr { get; set; }
    }
}