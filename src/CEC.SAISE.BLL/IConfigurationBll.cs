using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL
{
    public interface IConfigurationBll
    {
        TimeSpan GetPSOpenningTime();
        TimeSpan GetPSTurnoutsTime();
        TimeSpan GetPSElectionResultsTime();
        bool DebugModeEnabled();
    }
}
