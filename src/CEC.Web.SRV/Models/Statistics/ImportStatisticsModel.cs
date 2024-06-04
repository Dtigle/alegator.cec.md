using CEC.Web.SRV.Models.Grid;

namespace CEC.Web.SRV.Models.Statistics
{
    public class ImportStatisticsModel 
    {
        public long New { get; set; }
        public long Conflicted { get; set; }
        public long Updated { get; set; }
        public long Error { get; set; }
        public long Total { get; set; }
        public long ChangedStatus { get; set; }
        public long ResidenceChnaged { get; set; }
    }
}