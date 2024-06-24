using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Document
{
    public class ReportParameterValueModel
    {
        public long ParameterValueId { get; set; }
        public long DocumentId { get; set; }
        public long ReportParameterId { get; set; }
        public string ReportParameterName { get; set; }
        public string ValueContent { get; set; }
        public string ParameterCode { get; set; }
    }
}