using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.PoliticalParty
{
    public class DelimitationModel
    {
        public long ElectionId { get; set; }

        public bool ElectionIsLocal { get; set; }

        public bool IsMayorElection { get; set; }

        public long? CircumscriptionId { get; set; }

        public long? RegionId { get; set; }
    }
}