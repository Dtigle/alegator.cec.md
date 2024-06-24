using Amdaris.Domain;

namespace CEC.SRV.BLL.Dto
{
    public class ImportStatisticsDto : IEntity 
    {
        public long New { get; set; }

        public long Conflicted { get; set; }

        public long Updated { get; set; }

        public long Error { get; set; }

        public long Total { get; set; }

        public long ChangedStatus { get; set; }

        public long ResidenceChanged { get; set; }
    }
}
