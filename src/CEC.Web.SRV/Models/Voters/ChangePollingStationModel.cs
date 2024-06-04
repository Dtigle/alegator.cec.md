using System.Collections.Generic;

namespace CEC.Web.SRV.Models.Voters
{
    public class ChangePollingStationModel
    {
        public PersonPollingStationModel NewPollingStation { get; set; }

        public List<long> PeopleIds { get; set; }
    }
}