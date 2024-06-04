using System;

namespace CEC.Web.Results.Api.Infrastructure
{
    public class VotersActivity : IDbMap
    {
        public int Hour { get; set; }

        public int Minutes { get; set; }

        public int VotersCount { get; set; }   


        public void Map(System.Data.IDataReader reader)
        {
            this.Hour = (int)reader.GetValue(0);
            this.Minutes = (int)reader.GetValue(1);
            this.VotersCount = (int)reader.GetValue(2);
        }
    }
}