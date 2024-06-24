using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Dto
{
    public class PersonPollingStationDto
    {
        public Person Person { get; set; }

        public IList<PollingStation> PollingStations { get; set; }

        public PollingStation CurrentPollingStation { get; set; }
    }
}
