using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class VoterDocumentGridModel : JqGridSoft
    {
        [Display(Name = "Person_DocumentSeries", ResourceType = typeof(MUI), Order = 1)]
        public string Seria { get; set; }

        [Display(Name = "Person_DocumentNumber", ResourceType = typeof(MUI), Order = 2)]
        public string Number { get; set; }

        [Display(Name = "Person_DocIssuedDate", ResourceType = typeof(MUI), Order = 3)]
        public string IssuedDate { get; set; }

        [Display(Name = "Person_DocIssuedBy", ResourceType = typeof(MUI), Order = 4)]
        public string IssuedBy { get; set; }

        [Display(Name = "Person_DocValidBy", ResourceType = typeof(MUI), Order = 5)]
        public string ValidBy { get; set; }

 /*       [Display(Name = "Person_DocumentType", ResourceType = typeof(MUI), Order = 6)]
        public string Type { get; set; }  */
    }
}