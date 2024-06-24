using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
    public class DocumentTypeGridModel : LookupGridModel
    {
        [Display(Name = "DocumentTypeLookup_IsPrimary", ResourceType = typeof(MUI))]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool IsPrimary { get; set; }
    }
}