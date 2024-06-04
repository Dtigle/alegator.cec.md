using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Reporting
{
    public class PollingStationBordersModel : ReportingModel
    {
        [Display(Name = "PSBordersModel_Region", ResourceType = typeof(MUI))]
        [UIHint("Select2")]
        [Select2RemoteConfig("", "GetRegions", "Voters", "json", "regionsDataRequest", "regionsResults", "GetRegionName", "Voters", PageLimit = 10)]
        [Required(ErrorMessageResourceName = "PSBordersModel_RegionRequired", ErrorMessageResourceType = typeof(MUI))]
        public long RegionId { get; set; }

        public bool DataReadyForReport
        {
            get { return RegionId != 0; }
        }
    }
}