using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class ResolveAddressNoStreetConflictModel
    {
        public List<long> RspIds { get; set; }

        public long RegionId { get; set; }

        public PersonPollingStationModel NewPollingStation { get; set; }

        public long PollingStationId { get; set; }
    }
}