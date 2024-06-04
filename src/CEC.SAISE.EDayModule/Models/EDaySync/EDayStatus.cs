using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.EDaySync
{
    public enum EDaySyncStatus
    {
        [Description("Sincronizarea datelor este in process")]
        InProgress = 0,
        [Description("Sincronizarea datelor este in finisata")]
        Done = 1,
        [Description("Lipsa conexiune serviciu Web")]
        MissingConnection = 2
    }
}