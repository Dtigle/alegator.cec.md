using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Grids;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.SAISE.EDayModule.Models.PermissionManage
{
    public class PollingStationStageEnablerGridModel : JqGridRow
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

        [JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
        [Display(Name = "Activare Deschidere")]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool EnableOpening { get; set; }

        [JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
        [Display(Name = "Activare Prezența la vot")]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool EnableTurnout { get; set; }

        [JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
        [Display(Name = "Activare Proces verbal")]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool EnabelElectionResult { get; set; }

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

        [Display(Name = "Statut PV")]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, "GetBPStatuses", "PermissionManage", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.NullOperators)]
        public string BallotPaperStatus { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long? BallotPaperId { get; set; }

        [Display(Name = "Ora începerii scrutinului")]//, ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.JQueryUIDatepicker, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [DataType(DataType.Time)]
        [AdditionalMetadata("data-input-mask", "99.99.9999")]
        [JqGridColumnLayout(Width = 180)]
        [SearchData(DbName = "ElectionStartTime", Type = typeof(TimeSpan))]
        public string ElectionStartTime { get; set; }

        [Display(Name = "Ora încheierii scrutinului")]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.JQueryUIDatepicker, SearchOperators = JqGridSearchOperators.Eq)]
        [DataType(DataType.Time)]
        [AdditionalMetadata("data-input-mask", "99.99.9999 99:99")]
        [JqGridColumnLayout(Width = 180)]
        [SearchData(DbName = "ElectionEndTime", Type = typeof(TimeSpan))]
        public string ElectionEndTime { get; set; }

        [Display(Name = "Diferența de ora Moldovei")]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int TimeDifferenceMoldova { get; set; }

        [Display(Name = "Minute prelungire prima zi")]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int ActivityTimeExtendedFirstDay { get; set; }

        [Display(Name = "Minute prelungire a doua zi")]
        [JqGridColumnSortable(true)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int ActivityTimeExtendedSecondDay { get; set; }

        [Display(Name = "Suspendată")]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool IsSuspended { get; set; }

        [Display(Name = "Captare")]
        [JqGridColumnSortable(true)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool IsCapturingSignature { get; set; }

        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Durata Scrutin")]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        public string ElectionDuration { get; set; }




    }
}