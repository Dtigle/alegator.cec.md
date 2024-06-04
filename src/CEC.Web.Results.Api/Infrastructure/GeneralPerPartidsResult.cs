using CEC.Web.Results.Api.Dtos;
using System;
using System.Data;

namespace CEC.Web.Results.Api.Infrastructure
{
    public class GeneralPerPartidsResult : StatPreliminaryResult, IDbMap
    {
        
        public void Map(IDataReader reader)
        {
            this.CandidateId = (long)reader.GetValue(0);
            this.CandidateName = reader.GetString(1);
            this.CandidateResult = (long)reader.GetValue(3);
            this.CandidatePercentResult = 0.0;

            object objColor = reader.GetValue(2);

            this.CandidateColor = objColor is DBNull ? "#888888" : (string)objColor;
            this.PartyType = (int) reader.GetValue(4);

            this.BallotOrder = (int) reader.GetValue(5);

        }
    }
}