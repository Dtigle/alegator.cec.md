using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Dto
{
    public class VotersListPrintingParameters : SSRSPrintParameters
    {
        public string AbroadListReportName { get; set; }
        public string LocalElectionsListReportName { get; set; }
        public string ExportFormat { get; set; }
        public string WebPageVotersListEnable { get; set; }
    }
}
