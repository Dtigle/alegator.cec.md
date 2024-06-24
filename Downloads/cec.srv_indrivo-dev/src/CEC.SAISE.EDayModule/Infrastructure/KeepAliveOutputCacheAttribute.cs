using System.Web.Mvc;
using System.Web.UI;

namespace CEC.SAISE.EDayModule.Infrastructure
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