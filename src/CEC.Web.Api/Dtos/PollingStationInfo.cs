using CEC.SRV.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos
{

    [DataContract]
    public class PollingStationInfo
    {
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string LocationDescription { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string PhoneNumber;
        
        [DataMember]
        public double? LongY { get; set; }

        [DataMember]
        public double? LatX { get; set; }

        [DataMember]
        public string Circumscription { get; set; }
    }
}