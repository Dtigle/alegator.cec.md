using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class PollingStationListModel
    {
        [Display(Name = "Conflict_PollingStation", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "pollingStationDataRequest", "pollingStationResults", PageLimit = 10)]
        public long PollingStationId { get; set; }
    }
}