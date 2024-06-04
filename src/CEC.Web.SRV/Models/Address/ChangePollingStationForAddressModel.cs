using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
    public class ChangePollingStationForAddressModel
    {
        public ChangePollingStationForAddressModel()
        {
            Addresses = new List<long>();
        }

        public long RegionId { get; set; }

	    [Display(Name = "Address_PollingStation", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "votersDataRequest", "votersResults", PageLimit = 10)]
        public long PollingStationId { get; set; }

        public List<long> Addresses { get; set; }
    }
}