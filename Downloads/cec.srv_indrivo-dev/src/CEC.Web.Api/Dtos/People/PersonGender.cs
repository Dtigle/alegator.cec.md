using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos.People
{
    [DataContract]
    public enum PersonGender : long
    {
        [EnumMember]
        Unknown = 1,

        [EnumMember]
        Male = 2,

        [EnumMember]
        Female = 3
    }
}