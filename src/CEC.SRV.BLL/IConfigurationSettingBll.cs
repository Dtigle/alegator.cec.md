using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.Domain;

namespace CEC.SRV.BLL
{
    public interface IConfigurationSettingBll : IBll
    {
        ConfigurationSetting GetSetting(string paramName, string appName = null);
        string GetValue(string paramName, string appName = null);
        T GetValue<T>(string paramName, string appName = null) where T: IConvertible;
        bool IsDuplicatedConfigurationSetting(long id, string name, string appName = null);
        void UpdateValue(long id, string value);
    }
}
