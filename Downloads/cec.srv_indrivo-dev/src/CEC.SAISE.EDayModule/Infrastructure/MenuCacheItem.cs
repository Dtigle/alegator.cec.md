using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public class MenuCacheItem
    {
        public IEnumerable<MenuItem> Menu { get; set; }

        public string CultureName { get; set; }
    }
}