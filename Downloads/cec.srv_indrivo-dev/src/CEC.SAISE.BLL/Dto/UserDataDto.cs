using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class UserDataDto
    {
        public ValueNamePair AssignedElection { get; set; }

        public ValueNamePair AssignedRegion { get; set; }

        public ValueNamePair AssignedPollingStation { get; set; }
        public ValueNamePair AssignedCircumscription { get; set; }

        public bool IsAdmin { get; set; }

        public bool CircumscriptionAcces { get; set; }
    }
}
