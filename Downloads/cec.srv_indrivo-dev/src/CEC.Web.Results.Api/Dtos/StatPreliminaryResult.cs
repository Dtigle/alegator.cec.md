using System;
using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{
    [DataContract]
    public class StatPreliminaryResult
    {
        [DataMember]
        public long CandidateId { get; set; }

        [DataMember]
        public string CandidateName { get; set; }

        [DataMember]
        public long CandidateResult { get; set; }

        [DataMember]
        public double CandidatePercentResult { get; set; }

        [DataMember]
        public string CandidatePercentResultStr
        {
            get { return string.Format("{0:P}", CandidatePercentResult); }
        }

        [DataMember]
        public string CandidateColor { get; set; }

        [DataMember]
        public int PartyType { get; set; }

        [DataMember]
        public int BallotOrder { get; set; }

    }
}