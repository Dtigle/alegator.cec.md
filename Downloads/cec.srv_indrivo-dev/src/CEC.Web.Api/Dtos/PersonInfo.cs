using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos
{
    [DataContract]
    public class PersonInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DOB { get; set; }

        [DataMember]
        public string Residence { get; set; }

        [DataMember]
        public PollingStationInfo PollingStation { get; set; }

        [DataMember]
        public int VotersCount { get; set; }

        [DataMember]
        public string WarningMessage;

        [DataMember]
        public bool ReCaptchaError { get; set; }
    }

    [DataContract]
    public class PersonFullInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public int DOB { get; set; }

        [DataMember]
        public string Residence { get; set; }

        [DataMember]
        public string WarningMessage;
    }

    [DataContract]
    public class ReportModel
    {
        [DataMember]
        public string IDNP { get; set; }

        [DataMember]
        public string AbroadAddress { get; set; }

        [DataMember]
        public string AbroadAddressid { get; set; }

        [DataMember]
        public string AbroadAddresCountry { get; set; }

        [DataMember]
        public double AbroadAddressLat { get; set; }

        [DataMember]
        public double AbroadAddressLong { get; set; }

        [DataMember]
        public string ResidenceAddress { get; set; }

        [DataMember]
        public string Election { get; set; }

        [DataMember]
        public string Email { get; set; }
    }

    /*Person info for new Web Service*/
    [DataContract]
    public class PersonInfoWS : PersonInfo
    {
        [DataMember]
        public string IDNP { get; set; }

        [DataMember]
        public string Gender { get; set; }

        //[DataMember]
        //public string Residence { get; set; }

        //[DataMember]
        //public PollingStationInfo PollingStation { get; set; }

        //[DataMember]
        //public int VotersCount { get; set; }

        //[DataMember]
        //public string WarningMessage;

        //[DataMember]
        //public bool ReCaptchaError { get; set; }

        [DataMember]
        
        public string electionListNr { get; set; }

        [DataMember]
        public string Circumscription { get; set; }

        [DataMember]
        public bool VoterCertificate { get; set; }
    }

}