using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class VoterPollingStationHistoryGridModel : JqGridSoft
    {
        [Display(Name = "PollingStationNumber", ResourceType = typeof(MUI), Order = 1)]
        public string FullNumber { get; set; }

        [Display(Name = "Person_Region", ResourceType = typeof(MUI), Order = 2)]
        public string Region { get; set; }

        [Display(Name = "PollingStationContactInfo", ResourceType = typeof(MUI), Order = 3)]
        public string ContactInfo { get; set; }

        [Display(Name = "PollingStationLocation", ResourceType = typeof(MUI), Order = 4)]
        public string Location { get; set; }

        [Display(Name = "PollingStationAddress", ResourceType = typeof(MUI), Order = 5)]
        public string PollingStationAddress { get; set; }

        [Display(Name = "Circumscription_Number", ResourceType = typeof(MUI), Order = 6)]
        public int OwingCircumscription { get; set; }

        [Display(Name = "PollingStation_Type", ResourceType = typeof(MUI), Order = 7)]
        public string PollingStationType { get; set; }
    }
}