using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class StatisticsDto
    {
        public StatisticsDto()
        {
            BaseListCounter = -1;
            SupplimentaryListCounter = -1;
            VotedCounter = -1;
        }

        public long BaseListCounter { get; set; }

        public long SupplimentaryListCounter { get; set; }

        public long VotedCounter { get; set; }

        public bool IsOpen { get; set; }
    }
}
