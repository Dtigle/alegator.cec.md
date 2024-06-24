using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Document
{
    public class BESVReportOnPollingStationPrep
    {
        public int ElectionOfficialsCount{ get; set; }
        public int OtherAuthorizedPersonsCount { get; set; }
        public int StationaryBalltoBoxesCount { get; set; }
        public int MobileBallotBoxesCount { get; set; }
        public int PlasticSealsCount { get; set; }
        public int VotingRequestsNumber { get; set; }
        public int CertsFromCECECount { get; set; }

    }
}