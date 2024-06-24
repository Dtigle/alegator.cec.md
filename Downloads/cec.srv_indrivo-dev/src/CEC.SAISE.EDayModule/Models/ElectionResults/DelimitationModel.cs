using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.ElectionResults
{
    public class DelimitationModel
    {
        public long ElectionId { get; set; }

        public long? DistrictId { get; set; }

        public long? VillageId { get; set; }

        public long? PollingStationId { get; set; }
    }
}