using CEC.SRV.BLL;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ConfigurationSettingMap : IAutoMappingOverride<ConfigurationSetting>
    {
        public void Override(AutoMapping<ConfigurationSetting> mapping)
        {
            mapping.Schema(Schemas.RSA);
        }
    }
}