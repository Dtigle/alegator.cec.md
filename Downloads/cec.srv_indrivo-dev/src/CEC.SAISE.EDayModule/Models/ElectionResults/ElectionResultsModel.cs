using System;
using System.Linq;
using System.Web;
using CEC.SAISE.EDayModule.Models.Voting;

namespace CEC.SAISE.EDayModule.Models.ElectionResults
{
    public class ElectionResultsModel
    {
        public UserDataModel UserData { get; set; }

        public BallotPaperModel BallotPaper { get; set; }
    }
}