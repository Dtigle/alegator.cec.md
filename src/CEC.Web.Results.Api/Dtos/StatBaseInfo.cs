using System.Runtime.Serialization;

namespace CEC.Web.Results.Api.Dtos
{

    [DataContract]
    public class StatBaseInfo  
    {

        [DataMember]
        public string TimeOfData { get; set; }

        [DataMember]
        public long VotersByBaseList { get; set; }

        [DataMember]
        public long VotersByAddList { get; set; }

        [DataMember]
        public long VotersParticipateCount { get; set; }


    }
}