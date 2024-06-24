
using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{
    [DataContract]
    public class PollingStationTurnout
    {
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Circumscription { get; set; }

        [DataMember]
        public string Locality { get; set; }

        [DataMember]
        public long VotersOnBaseList { get; set; }

        [DataMember]
        public int VotersReceivedBallots { get; set; }

        [DataMember]
        public int VotersOnSupplementaryList { get; set; }

        [DataMember]
        public string PercentCalculated { get; set; }

        [DataMember]
        public double? LocationLatitude { get; set; }

        [DataMember]
        public double? LocationLongitude { get; set; }
    }
}