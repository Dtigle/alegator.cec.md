
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class ResolveLocalityConflictModel
    {
        public long RspId { get; set; }

        [Display(Name = "Conflict_Region", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetUserRegions", "Conflict", "json", "regionDataRequest", "regionResults", PageLimit = 10)]
        public long RegionId { get; set; }
    }
}