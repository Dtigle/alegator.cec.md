using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.AppConfiguration
{
    public class ConfigurationSettingsGridModel : JqGridSoft
    {
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "ConfigurationSetting_Name", ResourceType = typeof(MUI))]
        public string Name { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "ConfigurationSetting_Value", ResourceType = typeof(MUI))]
        public string Value { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "ConfigurationSetting_ApplicationName", ResourceType = typeof(MUI))]
        public string ApplicationName { get; set; }

    }
}