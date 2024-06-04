using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
    public class LinkRegionsModel
    {
        public long CurrentRegionId { get; set; }

        public IList<LinkedRegionsModel> LinkedRegions { get; set; }

        [Display(Name = "LinkedRegions_Regions", ResourceType = typeof (MUI))]
        [Select2RemoteConfig("", "GetLinkedRegions", "Lookup", "json", "linkedRegionsDataRequest",
            "linkedRegionsResults", PageLimit = 15)]
        [UIHint("Select2Multiple")]
        public long RegionId { get; set; }
    }
}