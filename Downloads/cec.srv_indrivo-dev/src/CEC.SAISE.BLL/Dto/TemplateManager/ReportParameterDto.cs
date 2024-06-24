using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto.TemplateManager
{
    public class ReportParameterDto
    {
        public long ParameterId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterDescription { get; set; }
        public bool IsLookup { get; set; }
        public string ParameterCode { get; set; }
    }
}
