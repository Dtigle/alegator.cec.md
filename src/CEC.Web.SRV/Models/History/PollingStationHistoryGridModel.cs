using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class PollingStationHistoryGridModel : HistoryGridRow
    {
       
        [Display(Name = "PollingStationNumber", ResourceType = typeof(MUI))]
        public string Number { get; set; }

        [Display(Name = "PSType", ResourceType = typeof(MUI))]
        public string PollingStationType { get; set; }
        
        [Display(Name = "PollingStationLocation", ResourceType = typeof(MUI))]
        public string Location { get; set; }

        [Display(Name = "PollingStationAddress", ResourceType = typeof(MUI))]
        public string Address { get; set; }

        [Display(Name = "PollingStationContactInfo", ResourceType = typeof(MUI))]
        public string ContactInfo { get; set; }

        [Display(Name = "PollingStationSaiseId", ResourceType = typeof(MUI))]
        public long? SaiseId { get; set; }
    }
}