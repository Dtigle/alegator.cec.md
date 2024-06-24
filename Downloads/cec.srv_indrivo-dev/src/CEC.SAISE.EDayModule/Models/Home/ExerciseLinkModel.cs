using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Home
{
    public class ExerciseLinkModel
    {
        public ValueNameModel Election { get; set; }

        public ValueNameModel Region { get; set; }

        public ValueNameModel PollingStation { get; set; }

        public bool IsAvailable { get; set; }

        public string Path { get; set; }
    }
}