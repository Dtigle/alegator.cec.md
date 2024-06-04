using System;
using Amdaris.Domain;

namespace CEC.SRV.Domain
{
    public class ConfigurationSetting : SRVBaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        public virtual string ApplicationName { get; set; }

        public virtual T GetValue<T>() where T : IConvertible
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }
    }
}