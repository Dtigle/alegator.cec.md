using CEC.SRV.Domain;

namespace CEC.Web.SRV.EDayExport
{
    public interface IConfigurationSettingManager
    {
        ConfigurationSetting Get(string id, string applicationName = null);
        void Update(ConfigurationSetting item);
    }
}