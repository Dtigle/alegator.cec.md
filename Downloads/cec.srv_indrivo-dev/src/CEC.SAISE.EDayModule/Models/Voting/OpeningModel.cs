using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class OpeningModel
    {
        public UserDataModel UserData { get; set; }

        public PollingStationOpeningData OpeningData { get; set; }
    }
}