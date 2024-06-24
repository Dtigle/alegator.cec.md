using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Models.DocumentsGrid
{
    public class PollingStationDocumentStageGridModel : JqGridRow
    {
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Circumscripție")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string Circumscription { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Localitate")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string Locality { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Nr. circumscripție")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string CircumscriptionNumber { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Nr. secție de votare")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string PollingStation { get; set; }

        [Display(Name = "Statut Document")]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, "GetBPStatuses", "DocumentsGrid", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string DocumentStatus { get; set; }

        //[HiddenInput(DisplayValue = false)]
        //public long? BallotPaperId { get; set; }
        //[HiddenInput(DisplayValue = false)]
        //public long? Id { get; set; }
        //[HiddenInput(DisplayValue = false)]
        public string TemplateName { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int DocumentStatusId { get; set; }

    }
}
