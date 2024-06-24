using System.Collections.Generic;

namespace CEC.SRV.BLL.Dto
{
    public class ElectionResultsPrintingParameters : SSRSPrintParameters
    {
        public IList<string> ExportFormats { get; set; }
    }
}
