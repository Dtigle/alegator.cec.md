using System;
using System.Runtime.Serialization;

namespace CEC.SRV.BLL.Exceptions
{
    [Serializable]
    public class SrvException : Exception
    {
        public SrvException()
        {
        }

        public SrvException(string localizationKey, string message)
            : base(message)
        {
            LocalizationKey = localizationKey;
        }

        public SrvException(string localizationKey, string message, Exception innerException)
            : base(message, innerException)
        {
            LocalizationKey = localizationKey;
        }

        protected SrvException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string LocalizationKey { get; set; }
    }
}