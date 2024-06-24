using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
    public class VoterCertificat : SaiseBaseEntity
    {
        public virtual AssignedVoter AssignedVoter { get; set; }
        public virtual DateTime?   ReleaseDate { get; set; }
        public virtual string    CertificatNr { get; set; }
        public virtual PollingStation PollingStation { get; set; }
        public virtual DateTime? DeletedDate { get; set; }
        




    }
}
