using CEC.SAISE.EDayModule.Infrastructure.Grids;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace CEC.SAISE.EDayModule.Models.EDaySync
{
    public class EDayStageGridModel : JqGridRow
    {
        [JqGridColumnSearchable(false)]
        [Display(Name = "Denumire")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string Description { get; set; }

        [JqGridColumnSearchable(false)]
        [Display(Name = "Statut")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string Status { get; set; }


        [JqGridColumnSearchable(false)]
        [Display(Name = "Progresul")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center, Width = 50)]
        public string Statistics { get; set; }

        [JqGridColumnSearchable(false)]
        [Display(Name = "Utilizator")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string User { get; set; }

        [JqGridColumnSearchable(false)]
        [Display(Name = "Data")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string Data { get; set; }


        [JqGridColumnSearchable(false)]
        [Display(Name = "Ip")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string IpAddress { get; set; }
    }
}