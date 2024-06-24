using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class FilterForVotersModel : JqGridSoft
    {
        [Display(Name = "FilterForVoters_Region", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
		[Select2RemoteConfig("", "GetRegions", "Voters", "json", "regionDataRequest", "regionResults", PageLimit = 10)]
        public long RegionId { get; set; }

        [Display(Name = "FilterForVoters_PollingStation", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "votersDataRequest", "votersResults", PageLimit = 10)]
        public long PollingStationId { get; set; }
    }
}