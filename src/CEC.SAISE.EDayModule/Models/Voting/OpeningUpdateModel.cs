using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class OpeningUpdateModel
    {
        public PollingStationOpeningData OpeningData { get; set; }
        public OpenPollingStationResult Result { get; set; }
    }
}