using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto.Concurents
{
    public class UpdateCandidateOrderDto
    {
        public long CandidateId { get; set; }

        public long? CandidateRegionRelId { get; set; }

        public int CandidateOrder { get; set; }
    }
}
