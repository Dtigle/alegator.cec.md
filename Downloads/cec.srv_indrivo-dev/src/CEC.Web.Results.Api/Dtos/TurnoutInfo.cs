using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{
    [DataContract]
    public class TurnoutInfo
    {
        public TurnoutInfo()
        {
            PollingStationsTurnout = new List<PollingStationTurnout>();
        }
        [DataMember]
        public int TotalPollingStations { get; set; }

        [DataMember]
        public long TotalVotersOnBaseList { get; set; }

        [DataMember]
        public long TotalVotersOnSupplementaryList { get; set; }

        [DataMember]
        public int TotalVotersReceivedBallots { get; set; }

        [DataMember]
        public string TotalPercentCalculated { get; set; }

        [DataMember]
        public IList<PollingStationTurnout> PollingStationsTurnout { get; set; }

    }
}