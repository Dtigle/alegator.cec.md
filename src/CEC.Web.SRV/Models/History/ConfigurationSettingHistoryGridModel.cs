using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class ConfigurationSettingHistoryGridModel : HistoryGridRow
    {
        [Display(Name = "ConfigurationSetting_Name", ResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Display(Name = "ConfigurationSetting_Value", ResourceType = typeof(MUI))]
        public string Value { get; set; }

        [Display(Name = "ConfigurationSetting_ApplicationName", ResourceType = typeof(MUI))]
        public string ApplicationName { get; set; }
    }
}