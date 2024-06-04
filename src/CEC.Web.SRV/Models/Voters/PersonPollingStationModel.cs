using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class PersonPollingStationModel
    {
        [Required(ErrorMessageResourceName = "VotersErrorRequired_PollingStation", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Person_PollingStation", ResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "votersDataRequest", "votersResults", PageLimit = 10)]
        [UIHint("Select2")]
        public long PStationId { get; set; }

        [Display(Name = "Person_PollingStation", ResourceType = typeof(MUI))]
        public string FullNumber { get; set; }
    }
}