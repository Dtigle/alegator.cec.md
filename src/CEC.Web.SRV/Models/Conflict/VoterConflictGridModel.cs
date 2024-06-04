using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class VoterConflictGridModel : JqGridSoft
    {
        [Display(Name = "Conflict_DataCreated", ResourceType = typeof(MUI), Order = 1)]
        public string ConflictDate { get; set; }

        [Display(Name = "Conflict_Type", ResourceType = typeof(MUI), Order = 2)]
        public string ConflictType { get; set; }

        [Display(Name = "Conflict_Status", ResourceType = typeof(MUI), Order = 3)]
        public string Status { get; set; }

        [Display(Name = "Conflict_StatusMessage", ResourceType = typeof(MUI), Order = 4)]
        public string StatusMessage { get; set; }

        public static string GetConflictTypeString(ConflictStatusCode code)
        {
             return ((code & ConflictStatusCode.AddressConflict) == ConflictStatusCode.AddressConflict ? "Conflict de adresa, " : "") +
                   ((code & ConflictStatusCode.AddressFatalConflict) == ConflictStatusCode.AddressFatalConflict ? "Conflict fatal de adresa, " : "") +
                   ((code & ConflictStatusCode.LocalityConflict) == ConflictStatusCode.LocalityConflict ? "Conflict de localitate, " : "") +
                   ((code & ConflictStatusCode.PollingStationConflict) == ConflictStatusCode.PollingStationConflict ? "Conflict de statie de votare, " : "") +
                   ((code & ConflictStatusCode.RegionConflict) == ConflictStatusCode.RegionConflict ? "Conflict de regiune, " : "") +
                   ((code & ConflictStatusCode.StatusConflict) == ConflictStatusCode.StatusConflict ? "Conflict de statut, " : "") +
                   ((code & ConflictStatusCode.StreetConflict) == ConflictStatusCode.StreetConflict ? "Conflict de strada, " : "") +
                   ((code & ConflictStatusCode.StreetZeroConflict) == ConflictStatusCode.StreetZeroConflict ? "Conflict de strada zero" : "");
        }
    }
}