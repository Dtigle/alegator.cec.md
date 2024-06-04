using Amdaris.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
   public  class VoterCertificatDto : IEntity
    {
        public long Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
   
        public long Idnp { get; set; }

        public DateTime Election { get; set; }
        public DateTime BirthDate { get; set; }

        public string DocumentNr { get; set; }
        public DateTime? DocumentData { get; set; }
        public DateTime? DocumentExpireData { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string Adress { get; set; }
         
        public string CertificatNr { get; set; }

        public string  ElectionType { get; set; }
        public string PollingStationRegion { get; set; }


        public string PollingStation { get; set; }
        public long? PollingStationId { get; set; }

       


    }
}
