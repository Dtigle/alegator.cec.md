using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Quartz
{
    public enum JobAction
    {
        Pause = 1,
        Resume = 2,
        Interrupt = 3,
        RunImmediate = 4,
        ReSchedule = 5
    }
}
