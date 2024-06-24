using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos
{
    [DataContract]
    public class StatisticInfo
    {
        [DataMember(Name="lat")]
        public double Lat { get; set; }

        [DataMember(Name="lng")]
        public double Long { get; set; }

        [DataMember]
        public int Count { get; set; }
    }

    [DataContract]
    public class StatisticGrouped
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public int Count { get; set; }
    }

    [DataContract]
    public class StatisticDateGrouped
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public int Count { get; set; }
    }
}