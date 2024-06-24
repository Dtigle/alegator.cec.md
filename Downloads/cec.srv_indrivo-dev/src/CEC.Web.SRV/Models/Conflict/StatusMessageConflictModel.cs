
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class StatusMessageConflictModel
    {
        public long IdRSP { get; set; }

        [Display(Name = "Conflict_StatusMessage", ResourceType = typeof(MUI))]
        public string StatusMessage { get; set; }

        public IList<ConflictShareViewModel> ConflictShares { get; set; }
    }
}