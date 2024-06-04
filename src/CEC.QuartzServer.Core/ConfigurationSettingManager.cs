using System.Linq;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using NHibernate;
using NHibernate.Linq;

namespace CEC.QuartzServer.Core
{
    public class ConfigurationSettingManager : IConfigurationSettingManager
    {
        private readonly ISRVRepository _repository;
        private readonly ISessionFactory _sessionFactory;

        public ConfigurationSettingManager(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ConfigurationSetting Get(string key, string applicationName = null)
        {
            using (IStatelessSession session = _sessionFactory.OpenStatelessSession())
            {
                return session.Query<ConfigurationSetting>().FirstOrDefault(x => x.Name == key && x.ApplicationName == applicationName);
            }
        }

        public void Update(ConfigurationSetting item)
        {
            using (IStatelessSession session = _sessionFactory.OpenStatelessSession())
            {
                session.CreateSQLQuery(
                    "update SRV.ConfigurationSettings set [value] = :newValue where configurationSettingId = :id")
                    .SetString("newValue", item.Value)
                    .SetInt64("id", item.Id)
                    .ExecuteUpdate();
            }
        }
    }
}