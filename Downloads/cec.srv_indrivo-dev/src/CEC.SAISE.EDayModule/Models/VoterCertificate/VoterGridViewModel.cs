using CEC.SAISE.EDayModule.Infrastructure.Grids;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Models.VoterCertificate
{
    public class VoterGridViewModel: JqGridRow
    {

       

        [Display(Name = "IDNP")]
        [JqGridColumnLayout(Width = 50)]
        public string Idnp { get; set; }

        [Display(Name = "Numele Complet")]
        [JqGridColumnLayout(Width = 100)]       
        public string FullName { get; set; }

        [Display(Name = "Data Nașterii")]
        [JqGridColumnLayout(Width = 50)]
        public string DataOfBirth { get; set; }

        [Display(Name = "Adresa ")]
        public string Address { get; set; }

        //[Display(Name = "Tipul adresei")]
        //[JqGridColumnLayout(Width = 50)]
        //public string AddressType { get; set; }

        [Display(Name = "Seria Document")]
        [JqGridColumnLayout(Width = 50)]        
        public string Document { get; set; }

        [Display(Name = "Certificat")]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        
        public bool Certificat { get; set; }
        
    }
}