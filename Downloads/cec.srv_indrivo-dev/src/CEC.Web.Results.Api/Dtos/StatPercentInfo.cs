using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{

    [DataContract]
    public class StatPercentInfo
    {
        [DataMember]
        public long TotalCount { get; set; }

        [DataMember]
        public long VotedCount { get; set; }

        [DataMember]
        public string VotedPercent { get; set; }

        [DataMember]
        public string NotVotedPercent { get; set; }

        [DataMember]
        public string LastTime { get; set; }
    }
}