using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.SAISE.EDayModule.WebServices.Modeldto
{
    [DataContract(Name ="VoterData")]
    public class ResultVotersdto
    {

        [DataMember(Order = 1)]
        public string VoterNumberList { get; set; }

        [DataMember(Order = 2)]
        public string VoterCertificatatNumber { get; set; }

        [DataMember(Order = 3)]
        public string PollingStationNumber { get; set; }

        [DataMember(Order = 4)]
        public string CircumscriptionName { get; set; }

        [DataMember(Order = 5)]
        public string CircumscriptionNumber { get; set; }
    }
}