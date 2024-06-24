using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL.Impl
{
    public class ConfigurationSettingBll : Bll, IConfigurationSettingBll
    {
        public ConfigurationSettingBll(ISRVRepository repository) 
            : base(repository)
        {
        }

        public ConfigurationSetting GetSetting(string paramName, string appName = null)
        {
            return Repository.Query<ConfigurationSetting>()
                .Where(x => x.Name == paramName && x.ApplicationName == appName)
                .FirstOrDefault();
        }

        public string GetValue(string paramName, string appName = null)
        {
            var configSetting = GetSetting(paramName, appName);
            return configSetting.Value;
        }

        public T GetValue<T>(string paramName, string appName = null) where T : IConvertible
        {
            var configSetting = GetSetting(paramName, appName);
            return configSetting.GetValue<T>();
        }

        public bool IsDuplicatedConfigurationSetting(long id, string name, string appName = null)
        {
            var configurationSetting =
                Repository.Query<ConfigurationSetting>()
                    .Where(x => x.Name == name && x.ApplicationName == appName);
            return configurationSetting.Any();
        }

        public void UpdateValue(long id, string value)
        {
            var entity = Get<ConfigurationSetting>(id);
            entity.Value = value;

            SaveOrUpdate(entity);
        }
    }
}
