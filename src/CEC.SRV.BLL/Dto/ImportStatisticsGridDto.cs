using System;
using Amdaris.Domain;

namespace CEC.SRV.BLL.Dto
{
    public class ImportStatisticsGridDto : BaseDto<long>, IEntity 
    {
        public DateTime ImportDateTime { get; set; }

        public int RegionCount { get; set; }
    }
}
