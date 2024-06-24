using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.SAISE.EDayModule.Models.VotingProcessStats
{
    public class VotingProcessStatsGridModel : JqGridRow
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
        public string Lacality { get; set; }

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

        [Display(Name = "Secție deschisă")]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool PSIsOpen { get; set; }

        [Display(Name = "Alegători LB")]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public long OpeningVoters { get; set; }

        [Display(Name = "Total Voturi")]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public long TotalVotes { get; set; }

        [Display(Name = "Voturi pe LS")]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public long SupplementaryVotes { get; set; }

        [Display(Name = "Statut PV")]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, "GetBPStatuses", "VotingProcessStats", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.NullOperators)]
        public string BallotPaperStatus { get; set; }
        


    }
}