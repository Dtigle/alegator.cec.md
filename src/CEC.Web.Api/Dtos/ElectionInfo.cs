using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos
{
    [DataContract]
    public class ElectionInfo
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name{ get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}