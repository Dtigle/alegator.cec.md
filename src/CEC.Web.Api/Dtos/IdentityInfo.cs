using CEC.SRV.Domain.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;

namespace CEC.Web.Api.Dtos
{
    [DataContract]
    public class IdentityInfo
    {        
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}