using CEC.SRV.Domain.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos
{
    [DataContract]
    public class StreetList
    {
        [DataMember]
        public IEnumerable<StreetInfo> Streets { get; set; }

        public StreetList()
        {

        }

        public StreetList(IEnumerable<Street> list)
        {
            Streets = list.Select(x => new StreetInfo(x));
        }
    }
}