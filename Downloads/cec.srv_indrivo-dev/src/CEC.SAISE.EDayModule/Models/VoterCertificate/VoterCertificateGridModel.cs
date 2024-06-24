using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.SAISE.EDayModule.Models.VoterCertificate
{
    public class VoterCertificateGridModel : JqGridRow
    {

        [HiddenInput(DisplayValue = false)]
        public long CerificatID { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Numele")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string FirstName { get; set; }


        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Prenumele")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string LastName { get; set; }


        public string Idnp { get; set; }


        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.JQueryUIDatepicker, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [Display(Name = "Ziua votării")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]       
        [SearchData(DbName = "Election", Type = typeof(DateTime?))]
        public string Election { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.JQueryUIDatepicker, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [Display(Name = "Data eliberarii")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]       
        [SearchData(DbName = "ReleaseDate", Type = typeof(DateTime?))]
        public string ReleaseDate { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Numărul Certificatului")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string CertificatNr { get; set; }

        [JqGridColumnSearchable(true, "SelectVoterStatus", "VoterCertificat", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [Display(Name = "Secția de votare")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [SearchData(DbName = "PollingStationId", Type = typeof(long?))]
        public string PollingStation { get; set; }

      

    }
}