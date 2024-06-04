using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class UpdateVoterResult
    {
        public bool Success { get; set; }

        public StatisticsDto Statistics { get; set; }
    }
}
