using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Interop
{
    public class AssignPollingStationModel
    {
        public bool SameSystemAndInstitution { get; set; }

        [Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
        public ElectionModel ElectionInfo { get; set; }


        public PersonPollingStationModel NewPollingStation { get; set; }

        public List<long> TransactionIds { get; set; }

    }
}