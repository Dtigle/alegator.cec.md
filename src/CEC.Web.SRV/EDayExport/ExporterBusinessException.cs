using System;

namespace CEC.Web.SRV.EDayExport
{
    public class ExporterBusinessException : Exception
    {
        public ExporterBusinessException(string message) : base(message)
        {
        }

        public ExporterBusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}