using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Helpers
{
    public class SaisePermissions
    {
        public const string VoterView = "VoterView";
        public const string ReceivedBallot = "ReceivedBallot";
        public const string ReceivedAbsenteeBallot = "ReceivedAbsenteeBallot";
        public const string ReceivedBallotMobile = "ReceivedBallotMobile";
        public const string VoterAddSupplementary = "VoterAddSupplementary";
        public const string ElectionResultView = "ElectionResultView";
        public const string AllowElectionResultsVerification = "AllowElectionResultsVerification";
        public const string PoliticalPartyView = "PoliticalPartyView";
	    public const string ManageFunctionality = "ManageFunctionality";
        public const string StatisticsView = "StatisticsView";
        public const string ResultsEdit = "ResultsEdit";
        public const string VoterCertificateCreate = "VoterCertificateCreate";
        public const string AdjustDbEday = "AdjustDbEday";
        public const string TransferDataToSSRS = "TransferDataToSSRS";
        public const string ReportElectionResults = "ReportElectionResults"; 
    }
}
