using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto.Concurents
{
    public class DelimitationDto
    {
        public long ElectionId { get; set; }

        public bool ElectionIsLocal { get; set; }

        public bool IsMayorElection { get; set; }

        public long? CircumscriptionId { get; set; }

        public long? RegionId { get; set; }

        public long GetElectionId()
        {
            return ElectionId;
        }

        public long GetCircumscriptionId()
        {
            return CircumscriptionId ?? -2;
        }

        public long GetRegionId()
        {
            return RegionId ?? -2;
        }

        public long GetCircumscriptionIdOrTBD()
        {
            return CircumscriptionId ?? -1;
        }

        public long GetRegionIdOrTBD()
        {
            return RegionId ?? -1;
        }
    }
}
