
using System;
using FluentNHibernate.Conventions;

namespace CEC.Web.Results.Api.Infrastructure
{
    public class AssignedPollingStation : IDbMap
    {
        //[DataMember]
        public long AssignedPollingStationId { get; set; }
        public long ElectionId { get; set; }
        public long OpeningVoters { get; set; }
        public long PollingStationId { get; set; }
        public string PollingStationNumber { get; set; }
        public string Circumscription { get; set; }
        public string Locality { get; set; }
        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }
        public int VotersInSupplimentaryList { get; set; }
        public int VotersReceivedBallots { get; set; }

        public void Map(System.Data.IDataReader reader)
        {
            this.AssignedPollingStationId = (long)reader.GetValue(0);
            this.ElectionId = (long)reader.GetValue(1);
            this.OpeningVoters = (long)reader.GetValue(2);
            this.PollingStationId = (long)reader.GetValue(3);
            this.PollingStationNumber = string.Format("{0}/{1}",reader.GetValue(4).ToString().PadLeft(2,'0'),reader.GetValue(5).ToString().PadLeft(3,'0'));
            this.LocationLatitude = reader.IsDBNull(6) ? null : (double?)reader.GetValue(6);
            this.LocationLongitude = reader.IsDBNull(7) ? null : (double?)reader.GetValue(7);
            this.VotersReceivedBallots = (int)reader.GetValue(8);
            this.VotersInSupplimentaryList = (int)reader.GetValue(9);
            this.Circumscription = (string)reader.GetValue(10);
            this.Locality = (string)reader.GetValue(11);
        }
    }
}