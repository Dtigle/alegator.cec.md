using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
    public class ElectionTypeGridModel : JqGridSoft
    {
        [Display(Name = "ElectionTypes_Code", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Width = 60)]
        public int Code { get; set; }

        [Display(Name = "ElectionTypes_Name", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Width = 300)]
        [JqGridColumnFormatter("$.ElectionTypeShortNameFormatter")]
        public string Name { get; set; }

        [Display(Name = "ElectionTypes_ElectionCompetitorType", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Width = 130)]
        public string ElectionCompetitorType { get; set; }

        [Display(Name = "ElectionTypes_ElectionRoundsNo", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center, Width = 60)]
        public int ElectionRoundsNo { get; set; }

        [Display(Name = "ElectionTypes_ElectionArea", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Width = 150)]
        public string ElectionArea { get; set; }
        
        [Display(Name = "ElectionTypes_Grid_AcceptResidenceDoc", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center, Width = 150)]
        public bool AcceptResidenceDoc { get; set; }

        [Display(Name = "ElectionTypes_Grid_AcceptVotingCert", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center, Width = 170)]
        public bool AcceptVotingCert { get; set; }

        [Display(Name = "ElectionTypes_Grid_AcceptAbroadDeclaration", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center, Width = 170)]
        public bool AcceptAbroadDeclaration { get; set; }

        [Display(Name = "ElectionTypes_CircumscriptionList", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Width = 130)]
        public string CircumscriptionList { get; set; }
    }
}