namespace CEC.SRV.Domain.Dto
{
    public class ExportPollingStationDto
    {
        public long PollingStationId { get; set; }

        public long ElectionRoundId { get; set; }

        public long CircumscriptionId { get; set; }

        public string NumberPerElection { get; set; }
    }
}
