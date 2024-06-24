using System;

namespace CEC.QuartzServer.Jobs.Export
{
    public class SaiseExporterBusinessException : Exception
    {
        public SaiseExporterBusinessException(string message) : base(message)
        {
        }

        public SaiseExporterBusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}