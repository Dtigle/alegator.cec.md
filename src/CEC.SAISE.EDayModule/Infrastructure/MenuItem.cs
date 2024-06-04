using System.Collections.Generic;
using System.Linq;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public class MenuItem
    {
        public MenuItem()
        {
            SubItems = new List<MenuItem>();
        }

        public string Title { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
        public string Css { get; set; }

        public bool IsActive { get; set; }
        public List<MenuItem> SubItems { get; set; }

        public bool IsCategory
        {
            get { return SubItems.Count > 0; }
        }

        public object RouteValues { get; set; }

    }
}