using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.AppConfiguration
{
    public class UpdateConfigurationSettingModel
    {
        public long Id { get; set; }

        [Display(Name = "ConfigurationSetting_Name", ResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Display(Name = "ConfigurationSetting_Value", ResourceType = typeof(MUI))]
        public string Value { get; set; }

        [Display(Name = "ConfigurationSetting_ApplicationName", ResourceType = typeof(MUI))]
        public string ApplicationName { get; set; }
    }
}