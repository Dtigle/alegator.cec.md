using CEC.SRV.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos
{
    [DataContract]
    public class AddressInfo : IdentityInfo
    {
        [DataMember]
        public PollingStationInfo PollingStation { get; set; }
    }
}