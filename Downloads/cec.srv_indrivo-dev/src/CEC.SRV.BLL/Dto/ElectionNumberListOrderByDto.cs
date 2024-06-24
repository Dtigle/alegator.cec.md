using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Dto
{
    public class ElectionNumberListOrderByDto
    {
        public ElectionNumberListOrderByDto()
        {
            SelectedAPSIds = new List<long>();
        }

        public List<long> SelectedAPSIds { get; set; }

        
        public long first { get; set; }
    }
}
