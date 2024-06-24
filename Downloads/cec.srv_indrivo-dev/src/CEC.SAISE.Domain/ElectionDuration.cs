using Amdaris.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
    public class ElectionDuration : EntityWithTypedId<long>
    {
        public virtual IEnumerable<AssignedPollingStation> AssignedPollingStations { get; set; }
        public virtual string Name { get; set; }
    }
}
