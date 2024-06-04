using CEC.SRV.Domain;

namespace CEC.QuartzServer.Core
{
    public interface IConfigurationSettingManager
    {
        ConfigurationSetting Get(string id, string applicationName = null);
        void Update(ConfigurationSetting item);
    }
}