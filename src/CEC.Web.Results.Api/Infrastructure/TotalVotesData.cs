

using System;

namespace CEC.Web.Results.Api.Infrastructure
{
    public class TotalVotesData :IDbMap
    {
        public long TotalVotes { get; set; }
        public long TotalValidVotes { get; set; }
        public long TotalSpoiledVotes { get; set; }

        public void Map(System.Data.IDataReader reader)
        {
            this.TotalVotes = (reader.GetValue(2) is DBNull)? 0 : (long) reader.GetValue(2);
            this.TotalValidVotes = (reader.GetValue(0) is DBNull) ? 0 :(long)reader.GetValue(0);
			this.TotalSpoiledVotes = (reader.GetValue(1) is DBNull) ? 0 : (long)reader.GetValue(1);
        }
    }
}