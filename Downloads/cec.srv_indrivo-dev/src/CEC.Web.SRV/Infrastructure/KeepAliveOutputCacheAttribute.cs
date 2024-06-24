using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CEC.Web.SRV.Infrastructure
{
    public class KeepAliveOutputCacheAttribute : OutputCacheAttribute
    {
        public KeepAliveOutputCacheAttribute()
        {
            Duration = 7200;
            NoStore = true;
            Location = OutputCacheLocation.Server;
        }
    }
}