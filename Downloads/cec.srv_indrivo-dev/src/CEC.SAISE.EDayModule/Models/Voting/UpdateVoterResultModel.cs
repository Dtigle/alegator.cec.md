namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class UpdateVoterResultModel
    {
        public bool Success { get; set; }

        public PollingStationStatisticsModel Statistics { get; set; }

        public bool IsPollingStationSuspended { get; set; }

        public bool IsCapturingSignature { get; set; }

        public string Idnp { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}