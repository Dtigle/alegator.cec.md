
using System;

namespace CEC.Web.Results.Api.Infrastructure
{
    public class BallotPapersCountData : IDbMap
    {
        public int TotalBallotPapers { get; set; }
        public int TotalProcessedBallotPapers { get; set; }
		
        public void Map(System.Data.IDataReader reader)
        {
            this.TotalBallotPapers = (int)reader.GetValue(0);
	        var value = reader.GetValue(1);
	        this.TotalProcessedBallotPapers = (value != System.DBNull.Value) ? (int)value: 0;
        }
    }
}