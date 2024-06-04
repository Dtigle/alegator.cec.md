using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.Synchronizer
{
    public class SyncExporterViewModel
    {
        public long Id { get; set; }

        public string Status { get; set; }

        public List<SyncExporterStageViewModel> Stages { get; set; }
    }

    public class SyncExporterStageViewModel
    {
        public string Description { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string Statistics { get; set; }
    }
}