
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class RejectConflict
    {
        public List<long> ConflictIds { get; set; }

        public ConflictStatusCode ConflictStatus { get; set; }

        [Required(ErrorMessageResourceName = "Conflict_RequiredError_Message", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "Conflict_Comments", ResourceType = typeof(MUI))]
        [StringLength(150, ErrorMessageResourceName = "Conflict_StringLength_Message", ErrorMessageResourceType = typeof(MUI))]
        public string Comment { get; set; }
    }
}