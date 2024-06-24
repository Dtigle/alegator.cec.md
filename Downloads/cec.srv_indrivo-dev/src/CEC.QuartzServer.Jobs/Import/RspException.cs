using System;

namespace CEC.QuartzServer.Jobs.Import
{
    public class RspException : Exception
    {
        public RspException(int? resultCode, string errorText) : base(string.Format("Rsp Response Error {0}: {1}", resultCode, errorText))
        {}
    }
}