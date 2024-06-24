namespace CEC.SAISE.BLL.Dto
{
    public class BallotPaperHeaderDataDto
    {
        public string ElectionName { get; set; } //param1
        public string ElectionDate { get; set; } //param2
        public string ReferendumQuestion { get; set; } //param3
        public string CircumscriptionRegion { get; set; } //param4
        public string CircumscriptionName { get; set; } //param5
        public string PollingStationRegion { get; set; } //param6
        public string PollingStationNumber { get; set; } //param7
        public string DocumentHeaderElectionDetails { get; set; } //param8


    }
}
